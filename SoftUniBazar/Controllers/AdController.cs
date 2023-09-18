using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftUniBazar.Data;
using SoftUniBazar.Data.Model;
using SoftUniBazar.Models;
using System.Security.Claims;

namespace SoftUniBazar.Controllers
{
    public class AdController : Controller
    {
        private readonly BazarDbContext _data;
        private object ab;

        public AdController(BazarDbContext data)
        {
            _data = data;
        }

        public async Task<IActionResult> Add()
        {
            AdViewModel adModel = new AdViewModel()
            {
                Categories = GetCategories()
            };

            return View(adModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AdViewModel adModel)
        {
            if (!GetCategories().Any(a => a.Id == adModel.CategoryId))
            {
                ModelState.AddModelError(nameof(adModel.CategoryId), "Category does not exist!");
            }

            if (!ModelState.IsValid)
            {
                return View(adModel);
            }

            string userId = GetUserId();

            var adToAdd = new Ad()
            {
                Name = adModel.Name,
                Description = adModel.Description,
                ImageUrl = adModel.ImageUrl,
                Price = adModel.Price,
                CategoryId = adModel.CategoryId,
                OwnerId = GetUserId(),
            };

            await _data.Ads.AddAsync(adToAdd);
            await _data.SaveChangesAsync();

            return RedirectToAction("All", "Ad");
        }

        public async Task<IActionResult> All()
        {
            var adsAll = await _data
                .Ads
                .Select(a => new AdCartViewModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    ImageUrl = a.ImageUrl,
                    CreatedOn = a.CreatedOn.ToString("dd/MM/yyyy H:mm"),
                    Category = a.Category.Name,
                    Description= a.Description,
                    Price = a.Price.ToString("F2"),
                    Owner = a.Owner.UserName,
                })
                .ToListAsync();

            return View(adsAll);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            var userId = GetUserId();
            var ad = await _data.Ads
                .Where(a => a.Id == id)
                .Select(a => new AdCartViewModel() 
                {
                    Id = a.Id,
                    Name = a.Name,
                    ImageUrl = a.ImageUrl,
                    CreatedOn = a.CreatedOn.ToString("dd/MM/yyyy H:mm"),
                    Category = a.Category.Name,
                    Description = a.Description,
                    Price = a.Price.ToString("F2"),
                    Owner = a.Owner.UserName,
                }).FirstOrDefaultAsync();

            if (ad == null)
            {
                return RedirectToAction("All", "Ad");
            }

            bool alreadyAdded = await _data.AdBuyers
                .AnyAsync(ab => ab.BuyerId == userId && ab.AdId == ad.Id);

            if (alreadyAdded == false)
            {
                var adBuyer = new AdBuyer
                {
                    BuyerId = userId,
                    AdId = ad.Id
                };

                await _data.AdBuyers.AddAsync(adBuyer);
                await _data.SaveChangesAsync();
            }

            return RedirectToAction("All", "Ad");
        }
        
        public async Task<IActionResult> Cart()
        {
            var userId = GetUserId();
            var adModel = await _data.AdBuyers
                .Where(ab => ab.BuyerId == userId)
                .Select(a => new AdCartViewModel
                {
                    Id = a.Ad.Id,
                    Name = a.Ad.Name,
                    ImageUrl = a.Ad.ImageUrl,
                    CreatedOn = a.Ad.CreatedOn.ToString("dd/MM/yyyy H:mm"),
                    Category = a.Ad.Category.Name,
                    Description = a.Ad.Description,
                    Price = a.Ad.Price.ToString("F2"),
                    Owner = a.Ad.Owner.UserName,
                }).ToListAsync();

            return View(adModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var adToEdit = await _data.Ads.FindAsync(id);

            if (adToEdit == null)
            {
                return BadRequest();
            }

            string userId = GetUserId();
            if (userId != adToEdit.OwnerId)
            {
                return Unauthorized();
            }

            AdViewModel adModel = new AdViewModel()
            {
                Name = adToEdit.Name,
                Description = adToEdit.Description,
                ImageUrl = adToEdit.ImageUrl,
                Price = adToEdit.Price,
                CategoryId = adToEdit.CategoryId,
                Categories = GetCategories(),
            };

            return View(adModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, AdViewModel model)
        {
            var adToEdit = await _data.Ads.FindAsync(id);

            if (adToEdit == null)
            {
                return BadRequest();
            }

            string currentUser = GetUserId();
            if (currentUser != adToEdit.OwnerId)
            {
                return Unauthorized();
            }

            if (!GetCategories().Any(c => c.Id == model.CategoryId))
            {
                ModelState.AddModelError(nameof(model.CategoryId), "Category does not exist!");
            }

            adToEdit.Name = model.Name;
            adToEdit.Description = model.Description;
            adToEdit.ImageUrl = model.ImageUrl;
            adToEdit.Price = model.Price;
            adToEdit.CategoryId = model.CategoryId;

            await _data.SaveChangesAsync();
            return RedirectToAction("All", "Ad");
        }

        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var adId = id;
            var userId = GetUserId();

            var adToRemove = _data.Ads.FindAsync(adId);

            if (adToRemove == null)
            {
                return BadRequest();
            }

            var entry = await _data.AdBuyers.FirstOrDefaultAsync(ab => ab.BuyerId == userId && ab.AdId == adId);
            _data.AdBuyers.Remove(entry);
            await _data.SaveChangesAsync();

            return RedirectToAction("All", "Ad");
        }

        private IEnumerable<CategoryViewModel> GetCategories()
            => _data
                .Categories
                .Select(c => new CategoryViewModel()
                {
                    Id = c.Id,
                    Name = c.Name
                });

        private string GetUserId()
           => User.FindFirstValue(ClaimTypes.NameIdentifier);

    }
}

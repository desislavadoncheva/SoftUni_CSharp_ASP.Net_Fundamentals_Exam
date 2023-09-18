using System.ComponentModel.DataAnnotations;
using static SoftUniBazar.Data.DataConstants;

namespace SoftUniBazar.Models
{
    public class AdViewModel
    {
        [Required]
        [StringLength(AdMaxName, MinimumLength = AdMinName,
            ErrorMessage = "Ad name must be between 5 and 25 characters.")]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(AdMaxDescription, MinimumLength = AdMinDescription,
            ErrorMessage = "Description must be between 15 and 250 characters.")]
        public string Description { get; set; } = null!;

        [Required]
        public string ImageUrl { get; set; } = null!;

        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; }
            = new List<CategoryViewModel>();
    }
}

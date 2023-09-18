using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using static SoftUniBazar.Data.DataConstants;

namespace SoftUniBazar.Data.Model
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(CategoryMaxName)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Ad> Ads { get; set; } = new List<Ad>();
    }
}

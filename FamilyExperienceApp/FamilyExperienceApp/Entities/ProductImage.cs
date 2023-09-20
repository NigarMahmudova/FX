using System.ComponentModel.DataAnnotations;

namespace FamilyExperienceApp.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [Required]
        [MaxLength(100)]
        public string ImageName { get; set; }
        public bool? PosterStatus { get; set; }

        public Product Product { get; set; }
    }
}

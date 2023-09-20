using System.ComponentModel.DataAnnotations;

namespace FamilyExperienceApp.Entities
{
    public class ProductReview
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public int ProductId { get; set; }
        [Required]
        [Range(1, 5)]
        public byte Rate { get; set; }
        [MaxLength(500)]
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }

        public AppUser AppUser { get; set; }
        public Product Product { get; set; }
    }
}

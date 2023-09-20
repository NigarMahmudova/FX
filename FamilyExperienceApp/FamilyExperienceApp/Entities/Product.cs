using FamilyExperienceApp.Attributes.CustomValidationAttributes;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyExperienceApp.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int ColorId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(500)]
        public string Desc { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal SalePrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountedPrice { get; set; }
        public bool StockStatus { get; set; }
        public bool IsNew { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsDeleted { get; set; }
        public bool? Gender { get; set; }
        public bool? Season { get; set; }
        public byte Rate { get; set; }

        public Category Category { get; set; }
        public Color Color { get; set; }

        [NotMapped]
        [MaxFileLength(1073741824)]
        [AllowedContentTypes("image/png", "image/jpeg")]
        public IFormFile PosterFile { get; set; }
        [NotMapped]
        [MaxFileLength(1073741824)]
        [AllowedContentTypes("image/png", "image/jpeg")]
        public IFormFile HoverPosterFile { get; set; }
        [NotMapped]
        [MaxFileLength(1073741824)]
        [AllowedContentTypes("image/png", "image/jpeg")]
        public List<IFormFile> ImageFiles { get; set; } = new List<IFormFile>();
        [NotMapped]
        public List<int> SizeIds { get; set; } = new List<int>();
        [NotMapped]
        public List<int> ProductImageIds { get; set; } = new List<int>();

        public List<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public List<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
        public List<ProductReview> ProductReviews { get; set; }
        public List<BasketItem> BasketItems { get; set; }
        public List<WishlistItem> WishlistItems { get; set; }
    }
}

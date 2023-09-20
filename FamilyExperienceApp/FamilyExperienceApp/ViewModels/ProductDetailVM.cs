using FamilyExperienceApp.Entities;

namespace FamilyExperienceApp.ViewModels
{
    public class ProductDetailVM
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; }
        public ProductReview Review { get; set; }
    }
}

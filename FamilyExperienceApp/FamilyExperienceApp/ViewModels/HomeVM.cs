using FamilyExperienceApp.Entities;
using System.Reflection;

namespace FamilyExperienceApp.ViewModels
{
    public class HomeVM
    {
        public List<Slider> Sliders { get; set; }
        public List<Product> Products { get; set; }
        public List<Product> WinterProducts { get; set; }
        public List<Product> SummerProducts { get; set; }
        public List<Product> SpringAutumnProducts { get; set; }
        public List<Gender> Genders { get; set; }
        public List<Banner> Banners { get; set; }
        public List<ImageCarousel> ImageCarousels { get; set; }
    }
}

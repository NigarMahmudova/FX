using FamilyExperienceApp.Entities;
using FamilyExperienceApp.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing.Drawing2D;

namespace FamilyExperienceApp.ViewModels
{
    public class ShopVM
    {
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public List<Color> Colors { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal SelectedMinPrice { get; set; }
        public decimal SelectedMaxPrice { get; set; }

        public int? SelectedCategoryId { get; set; }
        public List<int> SelectedColorIds { get; set; }
        public List<SelectListItem> SortItems { get; set; }
        public List<SelectListItem> SeasonItems { get; set; }
        public List<SelectListItem> GenderItems { get; set; }

    }
}

using FamilyExperienceApp.DAL;
using FamilyExperienceApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FamilyExperienceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly FamilyExperienceDbContext _context;

        public HomeController(FamilyExperienceDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            HomeVM model = new HomeVM
            {
                Sliders = _context.Sliders.OrderBy(x => x.Order).ToList(),

                Genders = _context.Genders.Take(2).ToList(),

                Banners = _context.Banners.Take(3).ToList(),

                ImageCarousels = _context.ImageCarousels.Take(15).ToList(),

                WinterProducts = _context.Products
                                    .Include(x => x.ProductImages.Where(x => x.PosterStatus != null))
                                    .Where(x => x.Season == true).Take(8).ToList(),

                SummerProducts = _context.Products
                                    .Include(x => x.ProductImages.Where(x => x.PosterStatus != null))
                                    .Where(x => x.Season == false).Take(8).ToList(),

                SpringAutumnProducts = _context.Products
                                    .Include(x => x.ProductImages.Where(x => x.PosterStatus != null))
                                    .Where(x => x.Season == null).Take(8).ToList(),

                Products = _context.Products
                                    .Include(x => x.ProductImages.Where(x => x.PosterStatus != null))
                                    .Take(8).ToList()
            };

            return View(model);
        }
    }
}

using FamilyExperienceApp.Areas.Manage.ViewModels;
using FamilyExperienceApp.DAL;
using FamilyExperienceApp.Entities;
using FamilyExperienceApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FamilyExperienceApp.Areas.Manage.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Area("manage")]
    public class CarouselController : Controller
    {
        private readonly FamilyExperienceDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CarouselController(FamilyExperienceDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index(int page = 1)
        {
            var query = _context.ImageCarousels.AsQueryable();

            var vm = PaginatedList<ImageCarousel>.Create(query, page, 2);

            if (page > vm.TotalPages) return RedirectToAction("Index", new { page = vm.TotalPages });

            return View(vm);

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ImageCarousel carousel)
        {
            if (!ModelState.IsValid) return View();

            if (carousel.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "ImageFile is required");
                return View();
            }

            carousel.ImageName = FileManager.Save(carousel.ImageFile, _env.WebRootPath, "manage/uploads/carousels");

            _context.ImageCarousels.Add(carousel);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            ImageCarousel carousel = _context.ImageCarousels.Find(id);

            if (carousel == null) return View("Error");

            return View(carousel);
        }

        [HttpPost]
        public IActionResult Edit(ImageCarousel carousel)
        {
            if (!ModelState.IsValid) return View(carousel);

            ImageCarousel existCarousel = _context.ImageCarousels.Find(carousel.Id);

            if (existCarousel == null) return View("Error");

            string removableImageName = null;

            if (carousel.ImageFile != null)
            {
                removableImageName = existCarousel.ImageName;
                existCarousel.ImageName = FileManager.Save(carousel.ImageFile, _env.WebRootPath, "manage/uploads/carousels");
            }

            _context.SaveChanges();

            if (removableImageName != null)
            {
                FileManager.Delete(_env.WebRootPath, "manage/uploads/carousels", removableImageName);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            ImageCarousel carousel = _context.ImageCarousels.Find(id);

            if (carousel == null) return StatusCode(404);

            _context.ImageCarousels.Remove(carousel);
            _context.SaveChanges();

            return StatusCode(200);
        }
    }
}

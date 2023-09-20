using FamilyExperienceApp.Areas.Manage.ViewModels;
using FamilyExperienceApp.DAL;
using FamilyExperienceApp.Entities;
using FamilyExperienceApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FamilyExperienceApp.Areas.Manage.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Area("manage")]
    public class BannerController : Controller
    {
        private readonly FamilyExperienceDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BannerController(FamilyExperienceDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index(int page = 1)
        {
            var query = _context.Banners.AsQueryable();

            var vm = PaginatedList<Banner>.Create(query, page, 2);

            if (page > vm.TotalPages) return RedirectToAction("Index", new { page = vm.TotalPages });

            return View(vm);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Banner banner)
        {
            if (!ModelState.IsValid) return View();

            if (banner.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "ImageFile is required");
                return View();
            }

            banner.ImageName = FileManager.Save(banner.ImageFile, _env.WebRootPath, "manage/uploads/banners");

            _context.Banners.Add(banner);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Banner banner = _context.Banners.Find(id);

            if (banner == null) return View("Error");

            return View(banner);
        }

        [HttpPost]
        public IActionResult Edit(Banner banner)
        {
            if (!ModelState.IsValid) return View(banner);

            Banner existBanner = _context.Banners.Find(banner.Id);

            if (existBanner == null) return View("Error");

            string removableImageName = null;

            if (banner.ImageFile != null)
            {
                removableImageName = existBanner.ImageName;
                existBanner.ImageName = FileManager.Save(banner.ImageFile, _env.WebRootPath, "manage/uploads/banners");
            }

            existBanner.Title = banner.Title;
            existBanner.SubTitle1 = banner.SubTitle1;
            existBanner.SubTitle2 = banner.SubTitle2;
            existBanner.BtnUrl = banner.BtnUrl;
            existBanner.BtnText = banner.BtnText;
            _context.SaveChanges();

            if (removableImageName != null)
            {
                FileManager.Delete(_env.WebRootPath, "manage/uploads/banners", removableImageName);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Banner banner = _context.Banners.Find(id);

            if (banner == null) return StatusCode(404);

            _context.Banners.Remove(banner);
            _context.SaveChanges();

            return StatusCode(200);
        }
    }
}

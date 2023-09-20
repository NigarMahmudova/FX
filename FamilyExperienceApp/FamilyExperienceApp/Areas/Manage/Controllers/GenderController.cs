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
    public class GenderController : Controller
    {
        private readonly FamilyExperienceDbContext _context;
        private readonly IWebHostEnvironment _env;

        public GenderController(FamilyExperienceDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index(int page = 1)
        {
            var query = _context.Genders.AsQueryable();

            var vm = PaginatedList<Gender>.Create(query, page, 2);

            if (page > vm.TotalPages) return RedirectToAction("Index", new { page = vm.TotalPages });

            return View(vm);

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Gender gender)
        {
            if (!ModelState.IsValid) return View();

            if (gender.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "ImageFile is required");
                return View();
            }

            gender.ImageName = FileManager.Save(gender.ImageFile, _env.WebRootPath, "manage/uploads/genders");

            _context.Genders.Add(gender);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Gender gender = _context.Genders.Find(id);

            if (gender == null) return View("Error");

            return View(gender);
        }

        [HttpPost]
        public IActionResult Edit(Gender gender)
        {
            if (!ModelState.IsValid) return View(gender);

            Gender existGender = _context.Genders.Find(gender.Id);

            if (existGender == null) return View("Error");

            string removableImageName = null;

            if (gender.ImageFile != null)
            {
                removableImageName = existGender.ImageName;
                existGender.ImageName = FileManager.Save(gender.ImageFile, _env.WebRootPath, "manage/uploads/genders");
            }

            existGender.Title = gender.Title;
            existGender.BtnText = gender.BtnText;
            existGender.BtnUrl = gender.BtnUrl;
            _context.SaveChanges();

            if (removableImageName != null)
            {
                FileManager.Delete(_env.WebRootPath, "manage/uploads/genders", removableImageName);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Gender gender = _context.Genders.Find(id);

            if (gender == null) return StatusCode(404);

            _context.Genders.Remove(gender);
            _context.SaveChanges();

            return StatusCode(200);
        }
    }
}

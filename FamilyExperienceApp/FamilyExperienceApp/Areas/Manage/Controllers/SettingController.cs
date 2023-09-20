using FamilyExperienceApp.Areas.Manage.ViewModels;
using FamilyExperienceApp.DAL;
using FamilyExperienceApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FamilyExperienceApp.Areas.Manage.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Area("manage")]
    public class SettingController : Controller
    {
        private readonly FamilyExperienceDbContext _context;

        public SettingController(FamilyExperienceDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            var query = _context.Settings.AsQueryable();

            return View(PaginatedList<Setting>.Create(query, page, 2));
        }

        public IActionResult Edit(string id)
        {
            Setting setting = _context.Settings.FirstOrDefault(x => x.Key == id);

            if (setting == null) return View("Error");

            return View(setting);
        }

        [HttpPost]
        public IActionResult Edit(Setting setting)
        {
            if (!ModelState.IsValid) return View();

            Setting existSetting = _context.Settings.FirstOrDefault(x => x.Key == setting.Key);

            if (existSetting == null) return View("Error");

            if (setting.Key != existSetting.Key && _context.Settings.Any(x => x.Value == setting.Value))
            {
                ModelState.AddModelError("Value", "Value is alredy taken");
                return View();
            }

            existSetting.Value = setting.Value;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}

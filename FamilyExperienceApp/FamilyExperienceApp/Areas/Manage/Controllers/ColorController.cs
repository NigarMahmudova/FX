using FamilyExperienceApp.Areas.Manage.ViewModels;
using FamilyExperienceApp.DAL;
using FamilyExperienceApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FamilyExperienceApp.Areas.Manage.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Area("manage")]
    public class ColorController : Controller
    {
        private readonly FamilyExperienceDbContext _context;

        public ColorController(FamilyExperienceDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            var query = _context.Colors.Include(x => x.Products).AsQueryable();

            var vm = PaginatedList<Color>.Create(query, page, 2);

            if (page > vm.TotalPages) return RedirectToAction("Index", new { page = vm.TotalPages });

            return View(vm);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Color color)
        {
            if (!ModelState.IsValid) return View();

            if (_context.Colors.Any(x => x.Name == color.Name))
            {
                ModelState.AddModelError("Name", "Name is already taken.");
            }

            _context.Colors.Add(color);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Color color = _context.Colors.FirstOrDefault(x => x.Id == id);

            if (color == null) return View("Error");

            return View(color);
        }

        [HttpPost]
        public IActionResult Edit(Color color)
        {
            if (!ModelState.IsValid) return View();

            Color existColor = _context.Colors.FirstOrDefault(x => x.Id == color.Id);

            if (existColor == null) return View("Error");

            if (color.Name != existColor.Name && _context.Colors.Any(x => x.Name == color.Name))
            {
                ModelState.AddModelError("Name", "Name is already taken.");
                return View();
            }

            existColor.Name = color.Name;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Color color = _context.Colors.Find(id);

            if (color == null) return StatusCode(404);

            if (_context.Products.Any(x => x.ColorId == id)) return StatusCode(400);

            _context.Colors.Remove(color);
            _context.SaveChanges();

            return StatusCode(200);
        }
    }
}

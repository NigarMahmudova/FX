using FamilyExperienceApp.Entities;
using FamilyExperienceApp.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FamilyExperienceApp.Areas.Manage.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace FamilyExperienceApp.Areas.Manage.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Area("manage")]
    public class CategoryController : Controller
    {
        private readonly FamilyExperienceDbContext _context;

        public CategoryController(FamilyExperienceDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1, string search = null)
        {
            ViewBag.Search = search;

            var query = _context.Categories.Include(x => x.Products).AsQueryable();

            if (search != null) query = query.Where(x => x.Name.Contains(search));

            var vm = PaginatedList<Category>.Create(query, page, 2);

            if (page > vm.TotalPages) return RedirectToAction("Index", new { page = vm.TotalPages, search = search });

            return View(vm);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid) return View();

            if (_context.Categories.Any(x => x.Name == category.Name))
            {
                ModelState.AddModelError("Name", "Name is already taken.");
                return View();
            }

            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Category category = _context.Categories.FirstOrDefault(x => x.Id == id);

            if (category == null) return View("Error");

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid) return View();

            Category existCategory = _context.Categories.FirstOrDefault(x => x.Id == category.Id);

            if (existCategory == null) return View("Error");

            if (category.Name != existCategory.Name && _context.Categories.Any(x => x.Name == category.Name))
            {
                ModelState.AddModelError("Name", "Name is already taken.");
                return View();
            }

            existCategory.Name = category.Name;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Category category = _context.Categories.Find(id);

            if (category == null) return StatusCode(404);

            if (_context.Products.Any(x => x.CategoryId == id)) return StatusCode(400);

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return StatusCode(200);
        }
    }
}

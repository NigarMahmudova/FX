using FamilyExperienceApp.Areas.Manage.ViewModels;
using FamilyExperienceApp.DAL;
using FamilyExperienceApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FamilyExperienceApp.MVC.Areas.Manage.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Area("manage")]
    public class DashboardController : Controller
    {
        private readonly FamilyExperienceDbContext _context;

        public DashboardController(FamilyExperienceDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}

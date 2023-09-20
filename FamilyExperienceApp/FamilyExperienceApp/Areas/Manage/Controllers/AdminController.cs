using FamilyExperienceApp.Areas.Manage.ViewModels;
using FamilyExperienceApp.DAL;
using FamilyExperienceApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FamilyExperienceApp.Areas.Manage.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Area("manage")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly FamilyExperienceDbContext _context;

        public AdminController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, FamilyExperienceDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {

            var data = _userManager.Users.Where(x => x.IsAdmin).ToList();

            List<AdminCreateVM> list = new List<AdminCreateVM>();

            foreach (var item in data)
            {
                AdminCreateVM vm = new AdminCreateVM()
                {
                    UserName = item.UserName,
                    FullName = item.FullName,
                    Email = item.Email,
                    UserId = item.Id,
                    Phone = item.PhoneNumber
                };

                list.Add(vm);
            }

            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminCreateVM adminVM)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = new AppUser
            {
                FullName = adminVM.FullName,
                Email = adminVM.Email,
                UserName = adminVM.UserName,
                PhoneNumber = adminVM.Phone,
                IsAdmin = true
            };

            var result = await _userManager.CreateAsync(user, adminVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                    ModelState.AddModelError("", item.Description);

                return View();
            }

            await _userManager.AddToRoleAsync(user, "Admin");

            return RedirectToAction("index");
        }


        public async Task<IActionResult> Delete(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);

            if (user == null) return StatusCode(404);

            await _userManager.DeleteAsync(user);

            return StatusCode(200);
        }
    }
}

using FamilyExperienceApp.Areas.Manage.ViewModels;
using FamilyExperienceApp.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FamilyExperienceApp.Areas.Manage.Controllers
{
    [Area("manage")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        //public async Task<IActionResult> CreateRoles()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Admin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Member"));

        //    return Content("created");
        //}

        //public async Task<IActionResult> CreateAdmin()
        //{
        //    AppUser admin = new AppUser
        //    {
        //        FullName = "Gunel Atlikhanova",
        //        UserName = "GunelAdmin",
        //    };

        //    var result = await _userManager.CreateAsync(admin, "Admin123");

        //    await _userManager.AddToRoleAsync(admin, "Admin");


        //    return Content("created");
        //}

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginVM adminVM, string returnUrl = null)
        {
            AppUser admin = await _userManager.FindByNameAsync(adminVM.UserName);

            if (admin == null || !admin.IsAdmin)
            {
                ModelState.AddModelError("", "UserName or Password is incorrect");
                return View();
            }


            var result = await _signInManager.PasswordSignInAsync(admin, adminVM.Password, adminVM.RememberMe, true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "User is locked out for 15 seconds");
                return View();
            }
            else if (!result.Succeeded)
            {
                ModelState.AddModelError("", "UserName or Password is incorrect");
                return View();
            }


            return returnUrl == null ? RedirectToAction("index", "dashboard") : Redirect(returnUrl);
        }

        public async Task<IActionResult> AdminLogout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("login", "account");
        }
    }
}

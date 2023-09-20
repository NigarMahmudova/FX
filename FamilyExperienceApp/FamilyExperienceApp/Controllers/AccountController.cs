using FamilyExperienceApp.DAL;
using FamilyExperienceApp.Entities;
using FamilyExperienceApp.Mail;
using FamilyExperienceApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FamilyExperienceApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly FamilyExperienceDbContext _context;
        private readonly IMailService _mailService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, FamilyExperienceDbContext context, IMailService mailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _mailService = mailService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(MemberRegisterVM memberVM)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = new AppUser
            {
                FullName = memberVM.FullName,
                Email = memberVM.Email,
                UserName = memberVM.UserName,
                PhoneNumber = memberVM.Phone,
                IsAdmin = false
            };

            var result = await _userManager.CreateAsync(user, memberVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                    ModelState.AddModelError("", item.Description);

                return View();
            }

            await _userManager.AddToRoleAsync(user, "Member");

            //var code = new Random().Next(1000,9999);
            //user.MailConfirmCode=code;
            //await _mailService.SendEmailAsync(new MailRequest()
            //{
            //    ToEmail= memberVM.Email,    
            //    Subject= "Mail Confirmation",
            //    Body=$"<h1>Salam hormetli istifadeci,<h1/><p>{code}<p/>"
            //});
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(MemberLoginVM memberVM, string returnUrl = null)
        {
            if (!ModelState.IsValid) return View();

            AppUser member = await _userManager.FindByNameAsync(memberVM.UserName);

            if (member == null || member.IsAdmin)
            {
                ModelState.AddModelError("", "UserName or Password is incorrect!");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(member, memberVM.Password, memberVM.RememberMe, true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "User is locked out for 15 seconds");
                return View();
            }
            else if (!result.Succeeded)
            {
                ModelState.AddModelError("", "UserName or Password is incorrect!");
                return View();
            }

            return returnUrl == null ? RedirectToAction("index", "home") : Redirect(returnUrl);
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Profile(string tab = "Profile")
        {
            ViewBag.Tab = tab;
            AppUser member = await _userManager.FindByNameAsync(User.Identity.Name);

            ProfileVM vm = new ProfileVM
            {
                Member = new MemberUpdateVM
                {
                    FullName = member.FullName,
                    Email = member.Email,
                    Phone = member.PhoneNumber,
                    UserName = member.UserName,
                },
                Orders = _context.Orders.Include(x => x.OrderItems).Where(x => x.AppUserId == member.Id).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> MemberUpdate(MemberUpdateVM memberVM)
        {
            if (!ModelState.IsValid)
            {
                ProfileVM vm = new ProfileVM { Member = memberVM };
                return View("Profile", vm);
            }

            AppUser member = await _userManager.FindByNameAsync(User.Identity.Name);

            member.FullName = memberVM.FullName;
            member.Email = memberVM.Email;
            member.PhoneNumber = memberVM.Phone;
            member.UserName = memberVM.UserName;

            var result = await _userManager.UpdateAsync(member);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors) ModelState.AddModelError("", err.Description);
                ProfileVM vm = new ProfileVM { Member = memberVM };
                return View("Profile", vm);
            }

            if (memberVM.NewPassword != null)
            {
                if (memberVM.CurrentPassword == null)
                {
                    ModelState.AddModelError("CurrentPassword", "CurrentPassword must be required!");
                    ProfileVM model = new ProfileVM { Member = memberVM };
                    return View("Profile", model);
                }

                await _userManager.ChangePasswordAsync(member, memberVM.CurrentPassword, memberVM.NewPassword);
                ProfileVM vm = new ProfileVM { Member = memberVM };
            }

            await _signInManager.SignInAsync(member, false);

            return RedirectToAction("profile");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }






        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);

            if (user == null) return View("Error");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var url = Url.Action("verifytoken", "account", new { email = email, token = token }, Request.Scheme);

            return Json(new
            {
                url = url
            });
        }

        public async Task<IActionResult> VerifyToken(string email, string token)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);

            if (await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token))
            {
                TempData["Email"] = email;
                TempData["Token"] = token;
                return RedirectToAction("ResetPassword");
            }

            return View("Error");
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPassword)
        {
            AppUser user = await _userManager.FindByEmailAsync(resetPassword.Email);

            var result = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);

            if (!result.Succeeded)
                return View("Error");

            return RedirectToAction("login");
        }
    }
}

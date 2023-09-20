using FamilyExperienceApp.DAL;
using FamilyExperienceApp.Entities;
using FamilyExperienceApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FamilyExperienceApp.Controllers
{
    public class ContactController : Controller
    {
        private readonly FamilyExperienceDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public ContactController(FamilyExperienceDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            ContactVM vm = new ContactVM();

            string userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;

            if (userId != null)
            {
                AppUser user = _userManager.FindByIdAsync(userId).Result;

                vm.FullName = user.FullName;
                vm.Phone = user.PhoneNumber;
                vm.Email = user.Email;
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContactVM contactVM)
        {
            string userId = (User.Identity.IsAuthenticated && User.IsInRole("Member")) ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
            AppUser user = (User.Identity.IsAuthenticated && User.IsInRole("Member")) ? await _userManager.FindByIdAsync(userId) : null;

            if (!ModelState.IsValid)
            {
                return View(contactVM);
            }

            Contact contact = new Contact
            {
                FullName = user == null ? contactVM.FullName : user.FullName,
                Email = user == null ? contactVM.Email : user.Email,
                Phone = user == null ? contactVM.Phone : user.PhoneNumber,
                Subject = contactVM.Subject,
                Message = contactVM.Message,
                CreatedAt = DateTime.UtcNow.AddHours(4),
                AppUserId = userId,
            };

            _context.Contacts.Add(contact);
            _context.SaveChanges();

            return RedirectToAction("index", "ContactUs");
        }
    }
}

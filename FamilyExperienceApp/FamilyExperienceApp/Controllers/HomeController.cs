using DocumentFormat.OpenXml.Spreadsheet;
using FamilyExperienceApp.DAL;
using FamilyExperienceApp.Entities;
using FamilyExperienceApp.Mail;
using FamilyExperienceApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Policy;

namespace FamilyExperienceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly FamilyExperienceDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMailService _mailService;

        public HomeController(FamilyExperienceDbContext context, UserManager<AppUser> userManager, IMailService mailService)
        {
            _context = context;
            _userManager = userManager;
            _mailService = mailService;
        }
        public IActionResult Index()
        {
            HomeVM model = new HomeVM
            {
                Sliders = _context.Sliders.OrderBy(x => x.Order).ToList(),

                Genders = _context.Genders.Take(2).ToList(),

                Banners = _context.Banners.Take(3).ToList(),

                ImageCarousels = _context.ImageCarousels.Take(15).ToList(),

                NewProducts = _context.Products
                                    .Include(x => x.ProductImages.Where(x => x.PosterStatus != null))
                                    .Where(x => x.IsNew).Take(8).ToList(),


                DiscountedProducts = _context.Products
                                    .Include(x => x.ProductImages.Where(x => x.PosterStatus != null))
                                    .Where(x => x.DiscountedPrice>0).Take(8).ToList(),

                //SpringAutumnProducts = _context.Products
                //                    .Include(x => x.ProductImages.Where(x => x.PosterStatus != null))
                //                    .Where(x => x.Season == null).Take(8).ToList(),

                FavoriteProducts = _context.Products
                                    .Include(x => x.ProductImages.Where(x => x.PosterStatus != null))
                                    .Where(x=>x.IsFavorite).Take(8).ToList()

            };

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

				foreach (Product pro in model.NewProducts)
                {
                    if (_context.WishlistItems.Where(x=>x.AppUserId==userId && x.ProductId==pro.Id).Count() > 0)
                    {
                        pro.IsAdded= true;
                    }
                }
				foreach (Product pro in model.DiscountedProducts)
				{
					if (_context.WishlistItems.Where(x => x.AppUserId == userId && x.ProductId == pro.Id).Count() > 0)
					{
						pro.IsAdded = true;
					}
				}
				//foreach (Product pro in model.SpringAutumnProducts)
				//{
				//	if (_context.WishlistItems.Where(x => x.AppUserId == userId && x.ProductId == pro.Id).Count() > 0)
				//	{
				//		pro.IsAdded = true;
				//	}
				//}
				foreach (Product pro in model.FavoriteProducts)
				{
					if (_context.WishlistItems.Where(x => x.AppUserId == userId && x.ProductId == pro.Id).Count() > 0)
					{
						pro.IsAdded = true;
					}
				}
			}

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EmailPost(EmailVM emailVM)
        {
            //string userId = (User.Identity.IsAuthenticated && User.IsInRole("Member")) ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
            //AppUser user = (User.Identity.IsAuthenticated && User.IsInRole("Member")) ? await _userManager.FindByIdAsync(userId) : null;

            if (!ModelState.IsValid)
            {
                return View(emailVM);
            }


            var url = Url.Action("subscribe", "home", new { email = emailVM.Email }, Request.Scheme);

            _mailService.SendEmailAsync(new MailRequest()
            {
                ToEmail = emailVM.Email,
                Subject = "Subscribe",
                Body = $"<a href={url}>Click here</a>"
            });
            TempData["Success"] = "Please, check your email!";
            return RedirectToAction("index", "Home");
        }
        public IActionResult Subscribe(string email)
        {
			EmailAddress emailAddress = new EmailAddress
			{
				Email = email,
			};

			_context.EmailAddresses.Add(emailAddress);
			_context.SaveChanges();

			return View();
        }
    }
}

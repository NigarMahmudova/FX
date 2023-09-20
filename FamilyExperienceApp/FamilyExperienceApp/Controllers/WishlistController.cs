using FamilyExperienceApp.DAL;
using FamilyExperienceApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace FamilyExperienceApp.Controllers
{
    [Authorize(Roles = "Member")]
    public class WishlistController : Controller
    {
        private readonly FamilyExperienceDbContext _context;

        public WishlistController(FamilyExperienceDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var wishlist = _context.WishlistItems.Include(x=>x.Product).ThenInclude(x=>x.ProductImages).ToList();
            return View(wishlist);
        }

        public IActionResult RemoveFromWishlist(int id)
        {
            WishlistVM wishlistVM = new WishlistVM();

            var userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
            if (userId != null)
            {
                var wishlistItem = _context.WishlistItems.FirstOrDefault(x => x.ProductId == id && x.AppUserId == userId);
                if (wishlistItem == null) return View("Error");
                else
                {
                    _context.WishlistItems.Remove(wishlistItem);
                    _context.SaveChanges();
                }

                var items = _context.WishlistItems.Include(x => x.Product).ThenInclude(x => x.ProductImages.Where(x => x.PosterStatus == true)).Where(x => x.AppUserId == userId).ToList();

                foreach (var wi in items)
                {
                    WishlistItemVM item = new WishlistItemVM
                    {
                        Count = wi.Count,
                        Product = wi.Product,
                    };
                    wishlistVM.Items.Add(item);
                }

            }

            return RedirectToAction("index", "wishlist");
        }
    }
}

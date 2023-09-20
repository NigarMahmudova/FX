using FamilyExperienceApp.DAL;
using FamilyExperienceApp.Entities;
using FamilyExperienceApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace FamilyExperienceApp.Controllers
{
    public class BasketController : Controller
    {
        private readonly FamilyExperienceDbContext _context;

        public BasketController(FamilyExperienceDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int id, int sizeId)
        {
            BasketVM basketVM = new BasketVM();

            if (User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


                var items = _context.BasketItems
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages.Where(x => x.PosterStatus == true))
                    .Include(x => x.Size)
                    .Where(x => x.AppUserId == userId).ToList();

                foreach (var bi in items)
                {
                    BasketItemVM item = new BasketItemVM
                    {
                        Count = bi.Count,
                        Product = bi.Product,
                        Size = bi.Size,
                    };
                    basketVM.Items.Add(item);
                    basketVM.TotalAmount += (item.Product.DiscountedPrice > 0 ? item.Product.DiscountedPrice : item.Product.SalePrice) * item.Count;
                }
            }
            else
            {
                var basketStr = Request.Cookies["basket"];

                List<BasketCookieItemVM> cookieItems = null;

                if (basketStr == null)
                {
                    cookieItems = new List<BasketCookieItemVM>();
                }
                else
                {
                    cookieItems = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(basketStr);
                }


                foreach (var ci in cookieItems)
                {
                    BasketItemVM item = new BasketItemVM
                    {
                        Count = ci.Count,
                        Product = _context.Products.Include(x => x.ProductImages.Where(x => x.PosterStatus == true))
                        .FirstOrDefault(x => x.Id == ci.ProductId),
                        Size = _context.Sizes.FirstOrDefault(x => x.Id == ci.SizeId),
                    };
                    basketVM.Items.Add(item);
                    basketVM.TotalAmount += (item.Product.DiscountedPrice > 0 ? item.Product.DiscountedPrice : item.Product.SalePrice) * item.Count;
                }
            }

            return View(basketVM);
        }

        public IActionResult RemoveFromBasket(int id)
        {
            BasketVM basketVM = new BasketVM();

            var userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
            if (userId != null)
            {
                var basketItem = _context.BasketItems.FirstOrDefault(x => x.ProductId == id && x.AppUserId == userId);
                if (basketItem == null) return View("Error");
                else
                {
                    _context.BasketItems.Remove(basketItem);
                    _context.SaveChanges();
                }

                var items = _context.BasketItems.Include(x => x.Product).ThenInclude(x => x.ProductImages.Where(x => x.PosterStatus == true)).Where(x => x.AppUserId == userId).ToList();

                foreach (var bi in items)
                {
                    BasketItemVM item = new BasketItemVM
                    {
                        Count = bi.Count,
                        Product = bi.Product,
                    };
                    basketVM.Items.Add(item);
                    basketVM.TotalAmount += (item.Product.DiscountedPrice > 0 ? item.Product.DiscountedPrice : item.Product.SalePrice) * item.Count;
                }

            }
            else
            {
                var basketStr = Request.Cookies["basket"];

                List<BasketCookieItemVM> cookieItems = null;

                if (basketStr == null)
                    cookieItems = new List<BasketCookieItemVM>();
                else
                    cookieItems = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(basketStr);


                var wantedItem = cookieItems.FirstOrDefault(x => x.ProductId == id);

                if (wantedItem == null) return View("Error");
                else cookieItems.Remove(wantedItem);

                Response.Cookies.Append("basket", JsonConvert.SerializeObject(cookieItems));

                foreach (var ci in cookieItems)
                {
                    BasketItemVM item = new BasketItemVM
                    {
                        Count = ci.Count,
                        Product = _context.Products.Include(x => x.ProductImages.Where(x => x.PosterStatus == true)).FirstOrDefault(x => x.Id == ci.ProductId)
                    };
                    basketVM.Items.Add(item);
                    basketVM.TotalAmount += (item.Product.DiscountedPrice > 0 ? item.Product.DiscountedPrice : item.Product.SalePrice) * item.Count;
                }
            }

            return RedirectToAction("index", "basket", basketVM);
        }

    }
}

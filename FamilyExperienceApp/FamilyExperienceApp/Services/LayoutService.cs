using FamilyExperienceApp.DAL;
using FamilyExperienceApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Configuration;
using System.Security.Claims;

namespace FamilyExperienceApp.Services
{
    public class LayoutService
    {
        private readonly FamilyExperienceDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutService(FamilyExperienceDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public Dictionary<string, string> GetSettings()
        {
            return _context.Settings.ToDictionary(x => x.Key, x => x.Value);
        }

        public BasketVM GetBasket()
        {
            var basketVM = new BasketVM();

            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                string userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var dbItems = _context.BasketItems.Include(x=>x.Size).Include(x => x.Product).ThenInclude(x => x.ProductImages.Where(x => x.PosterStatus == true)).Where(x => x.AppUserId == userId).ToList();
                foreach (var dbItem in dbItems)
                {
                    BasketItemVM item = new BasketItemVM
                    {
                        Count = dbItem.Count,
                        Product = _context.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == dbItem.ProductId),
                        Size = _context.Sizes.FirstOrDefault(x => x.Id == dbItem.SizeId),
                    };
                    basketVM.Items.Add(item);
                    basketVM.TotalAmount += (item.Product.DiscountedPrice > 0 ? item.Product.DiscountedPrice : item.Product.SalePrice) * item.Count;
                }
            }
            else
            {
                var basketStr = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

                if (basketStr != null)
                {
                    List<BasketCookieItemVM> cookieItems = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(basketStr);

                    foreach (var cookieItem in cookieItems)
                    {
                        BasketItemVM item = new BasketItemVM
                        {
                            Count = cookieItem.Count,
                            Product = _context.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == cookieItem.ProductId),
                            Size = _context.Sizes.FirstOrDefault(x => x.Id == cookieItem.SizeId),
                        };
                        basketVM.Items.Add(item);
                        basketVM.TotalAmount += (item.Product.DiscountedPrice > 0 ? item.Product.DiscountedPrice : item.Product.SalePrice) * item.Count;
                    }
                }
            }
            return basketVM;
        }


        public WishlistVM GetWishlist()
        {
            var wishlistVM = new WishlistVM();

            string userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var dbItems = _context.WishlistItems.Include(x => x.Product).ThenInclude(x => x.ProductImages.Where(x => x.PosterStatus == true)).Where(x => x.AppUserId == userId).ToList();
            foreach (var dbItem in dbItems)
            {
                WishlistItemVM item = new WishlistItemVM
                {
                    Count = dbItem.Count,
                    Product = _context.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == dbItem.ProductId),
                };
                wishlistVM.Items.Add(item);
            }

            return wishlistVM;
        }
    }
}

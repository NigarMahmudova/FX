using FamilyExperienceApp.DAL;
using FamilyExperienceApp.Entities;
using FamilyExperienceApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;

namespace FamilyExperienceApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly FamilyExperienceDbContext _context;

        public ProductController(FamilyExperienceDbContext context)
        {
            _context = context;
        }

        public IActionResult GetDetail(int id)
        {
            Product product = _context.Products.Include(x => x.ProductImages).Include(x=>x.ProductSizes)
                .ThenInclude(x=>x.Size).Include(x=>x.Color).FirstOrDefault(x => x.Id == id);
            return PartialView("_BasketModalPartial", product);
        }

        
        [HttpPost]
        public ActionResult AddToBasket(int id, int sizeId)
        {

            BasketVM basketVM = new BasketVM();
            var dataCount = 0;

            if (User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var basketItems = _context.BasketItems.Where(x => x.AppUserId == userId).ToList();

                var basketItem = basketItems.FirstOrDefault(x => x.ProductId == id && x.SizeId == sizeId);

                if (basketItem == null)
                {
                    basketItem = new BasketItem()
                    {
                        ProductId = id,
                        SizeId = sizeId,
                        AppUserId = userId,
                        Count = 1
                    };
                    _context.BasketItems.Add(basketItem);
                }
                else
                {
                    basketItem.Count++;
                }
                _context.SaveChanges();

                
                dataCount = _context.BasketItems.Count();
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


                BasketCookieItemVM cookieItem = cookieItems.FirstOrDefault(x => x.ProductId == id && x.SizeId == sizeId);
                if (cookieItem == null)
                {
                    cookieItem = new BasketCookieItemVM
                    {
                        ProductId = id,
                        SizeId = sizeId,
                        Count = 1
                    };
                    cookieItems.Add(cookieItem);
                }
                else
                    cookieItem.Count++;

                HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(cookieItems));
                dataCount = basketStr.Count();

                
            }
            return Content("Okey");
            //return Json(new { data = dataCount});
        }


        

        [Authorize(Roles = "Member")]
        public IActionResult AddToWishlist(int id)
        {

            WishlistVM wishlistVM = new WishlistVM();

            if (User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var wishlistItems = _context.WishlistItems.Where(x => x.AppUserId == userId).ToList();

                var wishlistItem = wishlistItems.FirstOrDefault(x => x.ProductId == id);

                if (wishlistItem == null)
                {
                    wishlistItem = new WishlistItem()
                    {
                        ProductId = id,
                        AppUserId = userId,
                        Count = 1
                    };
                    _context.WishlistItems.Add(wishlistItem);
                }
                else
                {
                    wishlistItem.Count++;
                }

                _context.SaveChanges();

                var items = _context.WishlistItems
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages.Where(x => x.PosterStatus == true))
                    .Where(x => x.AppUserId == userId).ToList();

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

        

        public IActionResult Detail(int id)
        {
            var vm = _getProductDetailVM(id);

            if (vm.Product == null) return View("error");
            return View(vm);
        }

        [Authorize(Roles = "Member")]
        [HttpPost]
        public IActionResult Review(ProductReview review)
        {
            if (!ModelState.IsValid)
            {
                var vm = _getProductDetailVM(review.ProductId);
                vm.Review = review;
                return View("Detail", vm);
            }

            Product product = _context.Products.Include(x => x.ProductReviews).FirstOrDefault(x => x.Id == review.ProductId);
            if (product == null) return View("error");

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            review.AppUserId = userId;
            review.CreatedAt = DateTime.UtcNow.AddHours(4);
            product.ProductReviews.Add(review);
            product.Rate = (byte)Math.Ceiling(product.ProductReviews.Average(x => x.Rate));

            _context.SaveChanges();

            return RedirectToAction("detail", new { id = review.ProductId });
        }

        private ProductDetailVM _getProductDetailVM(int id)
        {
            var product = _context.Products.Include(x => x.ProductImages).Include(x => x.ProductSizes)
                .ThenInclude(x => x.Size).Include(x => x.Category).Include(x => x.Color)
                .Include(x => x.ProductReviews).ThenInclude(x => x.AppUser).FirstOrDefault(x => x.Id == id);

            ProductDetailVM vm = new ProductDetailVM()
            {
                Product = product,
                RelatedProducts = product != null ? _context.Products.Include(x => x.Category)
                .Include(x => x.ProductImages.Where(x => x.PosterStatus == true)).Take(4).ToList() : null,
                Review = new ProductReview { ProductId = id },
            };
            return vm;
        }
    }
}

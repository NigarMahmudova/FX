using FamilyExperienceApp.DAL;
using FamilyExperienceApp.Entities;
using FamilyExperienceApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace FamilyExperienceApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly FamilyExperienceDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(FamilyExperienceDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Checkout()
        {
            CheckoutVM vm = new CheckoutVM();
            vm.Order = new OrderCreateVM();

            string userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;

            vm.Items = _generateCheckoutItem(userId);
            vm.TotalAmount = vm.Items.Sum(x => x.Price);

            if (userId != null)
            {
                AppUser user = _userManager.FindByIdAsync(userId).Result;

                vm.Order.FullName = user.FullName;
                vm.Order.Phone = user.PhoneNumber;
                vm.Order.Email = user.Email;
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateVM orderVM)
        {
            string userId = (User.Identity.IsAuthenticated && User.IsInRole("Member")) ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
            AppUser user = (User.Identity.IsAuthenticated && User.IsInRole("Member")) ? await _userManager.FindByIdAsync(userId) : null;

            if (!ModelState.IsValid)
            {
                CheckoutVM vm = new CheckoutVM();
                vm.Order = orderVM;
                vm.Items = _generateCheckoutItem(userId);
                vm.TotalAmount = vm.Items.Sum(x => x.Price);
                return View("checkout", vm);
            }

            Order order = new Order
            {
                FullName = user == null ? orderVM.FullName : user.FullName,
                Email = user == null ? orderVM.Email : user.Email,
                Phone = user == null ? orderVM.Phone : user.PhoneNumber,
                Address = orderVM.Address,
                City = orderVM.City,
                PostCode = orderVM.PostCode,
                Note = orderVM.Note,
                Status = Enums.OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow.AddHours(4),
                AppUserId = userId,
                OrderItems = _generateOrderItems(userId),
            };

            order.TotalAmount = order.OrderItems.Sum(x => x.Count * (x.DiscountedPrice > 0 ? x.DiscountedPrice : x.UnitSalePrice));
            _context.Orders.Add(order);
            _context.SaveChanges();

            _clearBasket(userId);

            if (userId != null)
            {
                return RedirectToAction("profile", "account", new { tab = "Orders" });
            }

            TempData["Success"] = "Order created successfully!";
            return RedirectToAction("index", "home");

        }

        private List<OrderItem> _generateOrderItems(string userId = null)
        {
            List<OrderItem> orderItems = new List<OrderItem>();
            if (userId != null)
            {
                var basketItems = _context.BasketItems.Include(x => x.Product).Include(x=>x.Size).Where(x => x.AppUserId == userId).ToList();

                orderItems = basketItems.Select(x => new OrderItem
                {
                    ProductId = x.ProductId,
                    SizeId = x.SizeId,
                    Count = x.Count,
                    UnitCostPrice = x.Product.CostPrice,
                    UnitSalePrice = x.Product.SalePrice,
                    DiscountedPrice = x.Product.DiscountedPrice,
                }).ToList();
            }
            else
            {
                var basketStr = Request.Cookies["Basket"];
                if (basketStr != null)
                {
                    var basketItems = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(basketStr);

                    foreach (var item in basketItems)
                    {
                        var product = _context.Products.Find(item.ProductId);
                        OrderItem orderItem = new OrderItem
                        {
                            ProductId = item.ProductId,
                            SizeId = item.SizeId,
                            Count = item.Count,
                            UnitCostPrice = product.CostPrice,
                            UnitSalePrice = product.SalePrice,
                            DiscountedPrice = product.DiscountedPrice,
                        };
                        orderItems.Add(orderItem);
                    }
                }
            }
            return orderItems;
        }

        private List<CheckoutItemVM> _generateCheckoutItem(string userId = null)
        {
            List<CheckoutItemVM> items = new List<CheckoutItemVM>();
            if (userId != null)
            {
                var basketItems = _context.BasketItems.Include(x => x.Product).Include(x=>x.Size).Where(x => x.AppUserId == userId).ToList();
                items = basketItems.Select(x => new CheckoutItemVM
                {
                    Count = x.Count,
                    ProductName = x.Product.Name,
                    SizeName = x.Size.Name,
                    Price = x.Count * (x.Product.DiscountedPrice > 0 ? x.Product.DiscountedPrice : x.Product.SalePrice)
                }).ToList();
            }
            else
            {
                string basketStr = Request.Cookies["basket"];

                if (basketStr != null)
                {
                    List<BasketCookieItemVM> basketItems = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(basketStr);

                    foreach (var item in basketItems)
                    {
                        var product = _context.Products.Find(item.ProductId);
                        var size = _context.Sizes.Find(item.SizeId);
                        var checkoutItem = new CheckoutItemVM
                        {
                            Count = item.Count,
                            ProductName = product.Name,
                            SizeName = size.Name,
                            Price = item.Count * (product.DiscountedPrice > 0 ? product.DiscountedPrice : product.SalePrice)
                        };

                        items.Add(checkoutItem);
                    }
                }
            }

            return items;
        }

        private void _clearBasket(string userId = null)
        {
            if (userId == null)
                Response.Cookies.Delete("basket");
            else
            {
                _context.RemoveRange(_context.BasketItems.Where(x => x.AppUserId == userId).ToList());
                _context.SaveChanges();
            }
        }
    }
}

using FamilyExperienceApp.Areas.Manage.ViewModels;
using FamilyExperienceApp.DAL;
using FamilyExperienceApp.Entities;
using FamilyExperienceApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FamilyExperienceApp.Areas.Manage.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Area("manage")]
    public class ProductController : Controller
    {
        private readonly FamilyExperienceDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(FamilyExperienceDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1, string search = null)
        {
            ViewBag.Search = search;

            var query = _context.Products.Include(x => x.Category).Include(x => x.Color)
                .Include(x => x.ProductImages.Where(x => x.PosterStatus == true)).Where(x => !x.IsDeleted).AsQueryable();

            if (search != null) query = query.Where(x => x.Name.Contains(search));

            var vm = PaginatedList<Product>.Create(query, page, 4);

            if (page > vm.TotalPages) return RedirectToAction("Index", new { page = vm.TotalPages, search = search });

            return View(vm);

        }

        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Colors = _context.Colors.ToList();
            ViewBag.Sizes = _context.Sizes.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.ToList();
                ViewBag.Colors = _context.Colors.ToList();
                ViewBag.Sizes = _context.Sizes.ToList();
                return View();
            }

            if (product.PosterFile == null)
            {
                ModelState.AddModelError("PosterFile", "PosterFile is required");
                ViewBag.Categories = _context.Categories.ToList();
                ViewBag.Colors = _context.Colors.ToList();
                ViewBag.Sizes = _context.Sizes.ToList();
                return View();
            }

            if (product.HoverPosterFile == null)
            {
                ModelState.AddModelError("HoverPosterFile", "HoverPosterFile is required");
                ViewBag.Categories = _context.Categories.ToList();
                ViewBag.Colors = _context.Colors.ToList();
                ViewBag.Sizes = _context.Sizes.ToList();
                return View();
            }

            if (!_context.Categories.Any(x => x.Id == product.CategoryId)) return View("Error");
            if (!_context.Colors.Any(x => x.Id == product.ColorId)) return View("Error");

            ProductImage poster = new ProductImage
            {
                PosterStatus = true,
                ImageName = FileManager.Save(product.PosterFile, _env.WebRootPath, "manage/uploads/products")
            };
            product.ProductImages.Add(poster);

            ProductImage hoverPoster = new ProductImage
            {
                PosterStatus = false,
                ImageName = FileManager.Save(product.HoverPosterFile, _env.WebRootPath, "manage/uploads/products")
            };
            product.ProductImages.Add(hoverPoster);

            foreach (var file in product.ImageFiles)
            {
                ProductImage productImage = new ProductImage
                {
                    PosterStatus = null,
                    ImageName = FileManager.Save(file, _env.WebRootPath, "manage/uploads/products"),
                };
                product.ProductImages.Add(productImage);
            }

            foreach (var sizeId in product.SizeIds)
            {
                if (!_context.Sizes.Any(x => x.Id == sizeId))
                    return View("error");

                ProductSize size = new ProductSize
                {
                    SizeId = sizeId
                };

                product.ProductSizes.Add(size);
            }

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Product product = _context.Products.Include(x => x.ProductImages).Include(x => x.ProductSizes).FirstOrDefault(x => x.Id == id);

            if (product == null) return View("error");

            product.SizeIds = product.ProductSizes.Select(x => x.SizeId).ToList();

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Colors = _context.Colors.ToList();
            ViewBag.Sizes = _context.Sizes.ToList();

            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            Product existProduct = _context.Products.Include(x => x.ProductImages).Include(x => x.ProductSizes).FirstOrDefault(x => x.Id == product.Id);

            if (existProduct == null) return View("Error");

            if (!_context.Categories.Any(x => x.Id == product.CategoryId)) return View("Error");
            if (!_context.Colors.Any(x => x.Id == product.ColorId)) return View("Error");

            existProduct.ProductSizes = new List<ProductSize>();

            foreach (var sizeId in product.SizeIds)
            {
                if (!_context.Sizes.Any(x => x.Id == sizeId)) return View("Error");

                existProduct.ProductSizes.Add(new ProductSize { SizeId = sizeId });
            }

            existProduct.Name = product.Name;
            existProduct.Desc = product.Desc;
            existProduct.SalePrice = product.SalePrice;
            existProduct.CostPrice = product.CostPrice;
            existProduct.DiscountedPrice = product.DiscountedPrice;
            existProduct.StockStatus = product.StockStatus;
            existProduct.IsNew = product.IsNew;
            existProduct.IsFavorite = product.IsFavorite;
            existProduct.Gender = product.Gender;
            existProduct.Season = product.Season;
            existProduct.CategoryId = product.CategoryId;
            existProduct.ColorId = product.ColorId;

            List<string> removableImageNames = new List<string>();

            if (product.PosterFile != null)
            {
                ProductImage poster = existProduct.ProductImages.FirstOrDefault(x => x.PosterStatus == true);
                removableImageNames.Add(poster.ImageName);
                poster.ImageName = FileManager.Save(product.PosterFile, _env.WebRootPath, "manage/uploads/products");
            }

            if (product.HoverPosterFile != null)
            {
                ProductImage hoverPoster = existProduct.ProductImages.FirstOrDefault(x => x.PosterStatus == false);
                removableImageNames.Add(hoverPoster.ImageName);
                hoverPoster.ImageName = FileManager.Save(product.PosterFile, _env.WebRootPath, "manage/uploads/products");
            }

            var removableImages = existProduct.ProductImages.FindAll(x => x.PosterStatus == false && !product.ProductImageIds.Contains(x.Id));
            _context.ProductImages.RemoveRange(removableImages);

            removableImageNames.AddRange(removableImages.Select(x => x.ImageName).ToList());

            foreach (var file in product.ImageFiles)
            {
                ProductImage image = new ProductImage
                {
                    PosterStatus = null,
                    ImageName = FileManager.Save(file, _env.WebRootPath, "manage/uploads/products")
                };

                existProduct.ProductImages.Add(image);
            }

            _context.SaveChanges();
            FileManager.DeleteAll(_env.WebRootPath, "manage/uploads/products", removableImageNames);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Product product = _context.Products.Find(id);

            if (product == null) return NotFound();

            product.IsDeleted = true;
            _context.SaveChanges();

            return Ok();
        }
    }
}

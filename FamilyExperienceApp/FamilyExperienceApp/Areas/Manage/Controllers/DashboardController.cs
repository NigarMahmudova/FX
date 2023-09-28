using DocumentFormat.OpenXml.InkML;
using FamilyExperienceApp.Areas.Manage.ViewModels;
using FamilyExperienceApp.DAL;
using FamilyExperienceApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FamilyExperienceApp.MVC.Areas.Manage.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Area("manage")]
    public class DashboardController : Controller
    {
        private readonly FamilyExperienceDbContext _context;

        public DashboardController(FamilyExperienceDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            DateTime currentDate = DateTime.Now;
            int currentYear = currentDate.Year;
            int currentMonth = currentDate.Month;

            ChartVM vm = new ChartVM()
            {
                TotalAmount = _context.Orders.Where(x => x.Status == Enums.OrderStatus.Accepted).Sum(x => x.TotalAmount),
                TotalOrders = _context.Orders.Count(),
                NewOrderCount = ((_context.Orders.Where(x=>x.CreatedAt.Month == DateTime.Now.Month).Count())*100)/ _context.Orders.Count(),
                OldOrderCount = ((_context.Orders.Where(x=>x.CreatedAt.Month < DateTime.Now.Month).Count())*100)/ _context.Orders.Count(),
                Orders = _context.Orders.Include(x=>x.OrderItems).OrderByDescending(x=>x.CreatedAt).Take(15).ToList(),
                
                AprilCount = _context.Orders.Where(x=>x.CreatedAt.Month == 4).Count(),
                MayCount = _context.Orders.Where(x => x.CreatedAt.Month == 5).Count(),
                JuneCount = _context.Orders.Where(x => x.CreatedAt.Month == 6).Count(),
                JulyCount = _context.Orders.Where(x => x.CreatedAt.Month == 7).Count(),
                AugustCount = _context.Orders.Where(x => x.CreatedAt.Month == 8).Count(),
                SeptemberCount = _context.Orders.Where(x => x.CreatedAt.Month == 9).Count(),
            };
            return View(vm);
        }
    }
}

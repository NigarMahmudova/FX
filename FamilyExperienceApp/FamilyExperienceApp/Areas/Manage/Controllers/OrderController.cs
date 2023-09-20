using ClosedXML.Excel;
using FamilyExperienceApp.Areas.Manage.ViewModels;
using FamilyExperienceApp.DAL;
using FamilyExperienceApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FamilyExperienceApp.Areas.Manage.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    [Area("manage")]
    public class OrderController : Controller
    {
        private readonly FamilyExperienceDbContext _context;

        public OrderController(FamilyExperienceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<FileResult> ExportOrderInExcel()
        {
            var order = _context.Orders.Include(x => x.OrderItems).AsEnumerable();
            var fileName = "orders.xlsx";
            return _generateExcel(fileName,order);
        }

        private FileResult _generateExcel(string fileName, IEnumerable<Order> orders)
        {
            DataTable dataTable = new DataTable("Orders");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Id"),
                new DataColumn("FullName"),
                new DataColumn("Email"),
                new DataColumn("Status"),
                new DataColumn("Date"),
                new DataColumn("TotalItems"),
                new DataColumn("TotalAmounts"),
            });

            foreach(var order in orders)
            {
                dataTable.Rows.Add(order.Id, order.FullName, order.Email, order.Status, order.CreatedAt, order.OrderItems.Count, order.TotalAmount);
            }

            using(XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);
                using(MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        public IActionResult Index(int page = 1)
        {
            var query = _context.Orders.Include(x => x.OrderItems).AsQueryable();

            var vm = PaginatedList<Order>.Create(query, page, 4);

            if (page > vm.TotalPages) return RedirectToAction("Index", new { page = vm.TotalPages });

            return View(vm);
        }

        public IActionResult Edit(int id)
        {
            Order order = _context.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Product).FirstOrDefault(x => x.Id == id);

            if (order == null) return View("Error");

            return View(order);
        }

        public IActionResult Detail(int id)
        {
            Order order = _context.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Product).FirstOrDefault(x => x.Id == id);

            if (order == null) return View("Error");

            return View(order);
        }

        public IActionResult Accept(int id)
        {
            Order order = _context.Orders.Include(x => x.AppUser).FirstOrDefault(x => x.Id == id);

            if (order == null || order.Status != Enums.OrderStatus.Pending) return View("Error");

            order.Status = Enums.OrderStatus.Accepted;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Reject(int id)
        {
            Order order = _context.Orders.Include(x => x.AppUser).FirstOrDefault(x => x.Id == id);

            if (order == null || order.Status != Enums.OrderStatus.Pending) return View("Error");

            order.Status = Enums.OrderStatus.Rejected;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}

using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc;
using ClothesWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace ClothesWeb.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Statistics()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Statistics(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                ModelState.AddModelError("", "Дата начала не может быть позже даты конца.");
                return View();
            }

            var sales = _context.Sells
                .Include(s => s.SellItem)
                    .ThenInclude(si => si.Product)
                .Where(s => s.Date >= startDate && s.Date <= endDate)
                .ToList();

            decimal totalRevenue = sales
                .SelectMany(s => s.SellItem)
                .Sum(si => si.Price * si.Quantity);

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");
            ViewBag.Sales = sales;

            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using ClothesWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        [Authorize(Roles = "Manager")]
        public IActionResult Statistics()
        {
            ViewBag.Employee = _context.Users.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = u.UserName
            }).ToList();
            ViewBag.SelectedEmployeeId = "";
            return View();
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public IActionResult Statistics(DateTime startDate, DateTime endDate, DateTime clientDate, String? employeeId)
        {
            if (startDate > endDate)
            {
                ViewData["Error"] =  "Дата начала не может быть позже даты конца.";
                return View();
            }

            if (clientDate < startDate)
            {
                ViewData["Error"] = "Статистика для будущего времени недоступна.";
                return View();
            }
            var query = _context.Sells
        .       Include(s => s.SellItem)
                .ThenInclude(si => si.Product)
                .Where(s => s.Date >= startDate && s.Date <= endDate)
                .AsQueryable();

            if (!string.IsNullOrEmpty(employeeId))
            {
                query = query.Where(s => s.EmployeeId == employeeId);
            }

            var sales = query.ToList();
 

            decimal totalRevenue = sales
                .SelectMany(s => s.SellItem)
                .Sum(si => si.Price * si.Quantity);

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "StatisticsFiles");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"statistics_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            var filePath = Path.Combine(folderPath, fileName);

            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"Статистика с {startDate:yyyy-MM-dd} по {endDate:yyyy-MM-dd}");
                if (!string.IsNullOrEmpty(employeeId))
                {
                    string employeeName = _context.Users
                        .Where(u => u.Id == employeeId) 
                       .Select(u => u.UserName)        
                       .FirstOrDefault();
                    writer.WriteLine($"Отчет по сотруднику: {employeeName}");
                }
                else { writer.WriteLine("Отчет по всем сотрудникам"); }
                writer.WriteLine("Дата продажи | Товар | Количество | Цена | Сумма");

                foreach (var sale in sales)
                {
                    foreach (var item in sale.SellItem)
                    {
                        writer.WriteLine($"{sale.Date:yyyy-MM-dd} , {item.Product.Name} , {item.Quantity} , {item.Price} ,{item.Price * item.Quantity}");
                    }
                }

                writer.WriteLine($"Общая сумма за период: {totalRevenue}");
                writer.WriteLine(new string('-', 50));
            }
            ViewBag.Employee = _context.Users.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = u.UserName
            }).ToList();
            ViewBag.SelectedEmployeeId = employeeId ?? ""; ;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");
            ViewBag.Sales = sales;
            ViewBag.FileName = fileName; 

            return View();
        }


        public IActionResult DownloadStatistics(string fileName)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "StatisticsFiles");
            var filePath = Path.Combine(folderPath, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "text/plain", fileName);
        }
    }
}

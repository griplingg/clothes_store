using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClothesWeb.Models;

using System.Linq;
using System.Security.Claims;

namespace ClothesWeb.Controllers
{
    public class ReturnsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReturnsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var returns = _context.ReturnProducts
                .Include(r => r.SellItem)
                    .ThenInclude(si => si.Product)
                .Include(r => r.SellItem)
                    .ThenInclude(si => si.Size)
                .Include(r => r.Status)
                .ToList();

            var userIds = returns.Select(r => r.EmployeeId).Distinct().ToList();
            var users = _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToList();

            ViewBag.EmployeeEmails = users.ToDictionary(u => u.Id, u => u.Email);

            return View(returns);
        }


        public IActionResult CreateSelect(string? searchString)
        {
            var dateFrom = DateTime.Now.AddDays(-14);

            var query = _context.Sells
                .Include(s => s.SellItem)
                    .ThenInclude(si => si.Product)
                .Include(s => s.SellItem)
                    .ThenInclude(si => si.Size)
                .Include(s => s.SellItem)
                    .ThenInclude(si => si.Returns)
                .Where(s => s.Date >= dateFrom);

        
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                
                query = query.Where(s => s.SellItem.Any(si => si.Product.Name.Contains(searchString)));
            }


            var sells = query.OrderByDescending(s => s.Date).ToList();

            ViewBag.SearchString = searchString;

            return View(sells);
        }



        public IActionResult Create(int sellItemId)
        {
            var sellItem = _context.SellItems
                .Include(si => si.Product)
                .Include(si => si.Size)
                .FirstOrDefault(si => si.Id == sellItemId);

            if (sellItem == null)
                return NotFound();

            return View(new ReturnProduct
            {
                SellItemId = sellItem.Id,
                SellId = sellItem.SellId,
                Date = DateTime.Now
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ReturnProduct model, DateTime clientDate)
        {
 
            model.Date =clientDate;
            model.StatusId = 1; 

            model.EmployeeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _context.ReturnProducts.Add(model);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


      
        [Authorize(Roles = "Manager")]
        public IActionResult ChangeStatus(int id, string actionType)
        {
            var returnProduct = _context.ReturnProducts.FirstOrDefault(r => r.Id == id);
            if (returnProduct == null)
                return NotFound();


            switch (actionType.ToLower())
            {
                case "approve":
                    returnProduct.StatusId = 3; 
                    break;
                case "reject":
                    returnProduct.StatusId = 4; 
                    break;
                default:
                    return BadRequest("Неверное действие");
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

    }

}




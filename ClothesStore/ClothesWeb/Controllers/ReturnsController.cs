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



        public IActionResult CreateSelect()
        {
            var sellItems = _context.SellItems
                .Include(si => si.Product)
                .Include(si => si.Size)
                .Include(si => si.Returns)
                .ToList();

            var availableItems = sellItems
                .Where(si => si.Quantity > si.Returns.Count)
                .Select(si => new ReturnSelectViewModel
                {
                    SellItemId = si.Id,
                    ProductName = si.Product.Name,
                    SizeName = si.Size.Name,
                    AvailableCount = si.Quantity - si.Returns.Count
                })
                .ToList();

            return View(availableItems);
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
        public IActionResult Create(ReturnProduct model)
        {
 
            model.Date = DateTime.Now;
            model.StatusId = 1; 


            model.EmployeeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _context.ReturnProducts.Add(model);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


       /* [HttpPost]
        [Authorize(Roles = "manager")] 
        public IActionResult ChangeStatus(int id, int newStatusId)
        {
            var returnProduct = _context.ReturnProducts
                .FirstOrDefault(r => r.Id == id);

            if (returnProduct == null)
                return NotFound();

            returnProduct.StatusId = newStatusId; 
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }*/

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




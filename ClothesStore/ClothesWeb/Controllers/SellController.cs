
using ClothesWeb;
using ClothesWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class SellController : Controller
{
    private readonly ApplicationDbContext _context; 

    public SellController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Manager,Salesman")]
    public async Task<IActionResult> AddPurchase()
    {
        var products = await _context.Products.Where(p => p.IsDeleted == false).ToListAsync();

        var model = new AddPurchaseViewModel
        {
            AllProducts = products
        };

        return View(model);
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Manager,Salesman")]
    public async Task<IActionResult> AddPurchase(AddPurchaseViewModel model, DateTime clientDate)
    {
        if (model.Items == null || model.Items.Count == 0)
        {
            ModelState.AddModelError("Items", "Необходимо добавить хотя бы одну позицию товара.");
            model.AllProducts = await _context.Products.Where(p => !p.IsDeleted).ToListAsync();
            return View(model);
        }

        if (!ModelState.IsValid)
        {
            model.AllProducts = await _context.Products.Where(p => !p.IsDeleted).ToListAsync();
            return View(model);
        }

        var newSell = new Sell
        {
            Date = clientDate,
            PaymentMethod = model.PaymentMethod,
            EmployeeId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
        };
        _context.Sells.Add(newSell);
        await _context.SaveChangesAsync(); 

        foreach (var itemVm in model.Items)
        {
            var productSize = await _context.ProductSizes
                .FirstOrDefaultAsync(ps => ps.ProductId == itemVm.ProductId && ps.SizeId == itemVm.SizeId);

            if (productSize == null || productSize.Quantity < itemVm.Quantity)
            {
                ModelState.AddModelError("", $"Недостаточно товара. Доступно: {productSize?.Quantity ?? 0}");
                model.AllProducts = await _context.Products.Where(p => !p.IsDeleted).ToListAsync();
                return View(model);
            }

            var product = await _context.Products.FindAsync(itemVm.ProductId);

            var sellItem = new SellItem
            {
                SellId = newSell.Id,
                ProductId = itemVm.ProductId,
                SizeId = itemVm.SizeId,
                Quantity = itemVm.Quantity,
                Price = product.Price,
                Color = product.Color
            };

            _context.SellItems.Add(sellItem);
            productSize.Quantity -= itemVm.Quantity;
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Продажа проведена успешно";
        return RedirectToAction("AddPurchase");
    }


    [HttpGet]
    public async Task<JsonResult> GetSizesByProduct(int productId)
    {
        var productSizes = await _context.ProductSizes
            .Where(ps => ps.ProductId == productId && ps.Quantity > 0)
            .Include(ps => ps.Size)
            .Select(ps => new
            {
                sizeId = ps.SizeId,
                sizeName = ps.Size.Name,
                quantity = ps.Quantity
            })
            .ToListAsync();

        return Json(productSizes);
    }
}
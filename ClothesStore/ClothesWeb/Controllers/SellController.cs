
using ClothesWeb;
using ClothesWeb.Models;
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

    public async Task<IActionResult> AddPurchase()
    {
        var products = await _context.Products.Where(p => p.IsDeleted == false).ToListAsync();

        var model = new AddPurchaseViewModel
        {
            AllProducts = products
        };

        return View(model);
    }

    /*[HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPurchase(AddPurchaseViewModel model, DateTime clientDate)
    {
        if (model.Items == null || model.Items.Count == 0)
        {
            ModelState.AddModelError("","Необходимо добавить хотя бы одну позицию товара.");
        }

        if (ModelState.IsValid)
        {
            var newSell = new Sell
            {
                Date = clientDate,
                PaymentMethod = model.PaymentMethod,
                EmployeeId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            };
            _context.Sells.Add(newSell);


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Ошибка сохранения. Попробуйте снова.");
                model.AllProducts = await _context.Products.ToListAsync();
                return View(model);
            }


            foreach (var itemVm in model.Items)
            {
                var productSize = await _context.ProductSizes
                    .FirstOrDefaultAsync(ps => ps.ProductId == itemVm.ProductId && ps.SizeId == itemVm.SizeId);

                var product = await _context.Products.FindAsync(itemVm.ProductId);
                var size = await _context.Sizes.FindAsync(itemVm.SizeId);


                if (productSize == null || productSize.Quantity < itemVm.Quantity)
                {
                    ModelState.AddModelError("", $"Ошибка: Недостаточно товара '{product?.Name} ({size?.Name})'. Доступно: {productSize?.Quantity ?? 0}.");
                    model.AllProducts = await _context.Products.ToListAsync();
                    return View(model);
                }

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

        model.AllProducts = await _context.Products.ToListAsync();
        return View(model);
    }*/
    [HttpPost]
    [ValidateAntiForgeryToken]
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
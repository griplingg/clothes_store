using System.Diagnostics;
using System.Security.Claims;
using ClothesWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClothesWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet]
        public IActionResult SupplierCatalog(string searchString)
        {
            var suppliers = _context.Supplier.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                suppliers = suppliers.Where(p => p.OrganizationName.Contains(searchString));
            }

            return View(suppliers.ToList());
        }

        [HttpGet]
        public IActionResult EditSupplierCard(int id, string searchString)
        {
            var supplier = _context.Supplier.AsQueryable().FirstOrDefault(p => p.Id == id);
            if (supplier == null)
                return NotFound();

            ViewBag.SearchString = searchString;
            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult EditSupplierCard(Supplier supplier, string? searchString)
        {
            
            var productToUpdate = _context.Supplier
        .FirstOrDefault(p => p.Id == supplier.Id);

            if (productToUpdate == null)
            {
                return NotFound();
            }


            productToUpdate.OrganizationName = supplier.OrganizationName;
            productToUpdate.ContactMail = supplier.ContactMail;
            productToUpdate.ContactName = supplier.ContactName;
            productToUpdate.ContactPhone = supplier.ContactPhone;



            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, "ќшибка сохранени€ изменений: " + ex.Message);

            }

            return RedirectToAction("SupplierCatalog", new { searchString = searchString });


        }

        [HttpGet]
        public IActionResult AddSupplier()
        {
                ViewBag.Supplier = _context.Supplier.Select(s => new SelectListItem
                {
                
                }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddSupplier(Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                foreach (var err in ModelState)
                {
                    foreach (var e in err.Value.Errors)
                    { Console.WriteLine($"ошибка: {err.Key} Ч {e.ErrorMessage}"); }
                }

                return View(supplier);
            }



            _context.Supplier.Add(supplier);
            _context.SaveChanges();
            return RedirectToAction("SupplierCatalog");
        }

        [HttpGet]
        public IActionResult Catalog(string searchString)
        {
            var products = _context.Products
                .Where(p => p.IsDeleted == false)
                .Include(p => p.Category)
                .Include(p => p.ProductSizes)
                .ThenInclude(ps => ps.Size)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString));
            }
        
            var model = products
        .ToList()
        .GroupBy(p => p.Category)
        .Select(g => new CategoryProductsViewModel
        {
            Category = g.Key,
            Products = g.ToList()
        })
        .ToList();

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult EditCard(int id, string searchString)
        {
            var product = _context.Products.Include(p => p.Category).Include(p => p.Supplier).Include(p => p.ProductSizes)
        .ThenInclude(ps => ps.Size)
        .AsQueryable().FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            ViewBag.SearchString = searchString;
            ViewBag.Categories = new SelectList(_context.Category, "Id", "Name", product.CategoryId);
            ViewBag.Suppliers = new SelectList(_context.Supplier, "Id", "OrganizationName", product.SupplierId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public IActionResult EditCard(Product product, string? searchString)
        {
            ViewBag.Categories = new SelectList(_context.Category, "Id", "Name", product.CategoryId);
            ViewBag.Suppliers = new SelectList(_context.Supplier, "Id", "OrganizationName", product.SupplierId);
            ViewBag.SearchString = searchString;

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            var productToUpdate = _context.Products.FirstOrDefault(p => p.Id == product.Id);
           

            if (productToUpdate == null)
            {
                return NotFound();
            }


            productToUpdate.Name = product.Name;
            productToUpdate.Price = product.Price;
            productToUpdate.Color = product.Color; 
            productToUpdate.SupplierId = product.SupplierId;
            productToUpdate.CategoryId = product.CategoryId;
            productToUpdate.IsDeleted = false;



            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "ќшибка сохранени€ изменений: " + ex.Message);
                return View(productToUpdate); 
            }

            return RedirectToAction("Catalog", new { searchString = searchString }); ;


        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult DeleteCard(int Id, string? searchString)
        {
           
            ViewBag.SearchString = searchString;
            var productToUpdate = _context.Products.FirstOrDefault(p => p.Id == Id);


            if (productToUpdate == null)
            {
                return NotFound();
            }


            productToUpdate.IsDeleted = true;



            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "ќшибка удалени€: " + ex.Message);
                return RedirectToAction("Catalog", new { searchString = searchString });
            
        }
            return RedirectToAction("Catalog", new { searchString = searchString }); ;


        }
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewData["UserId"] = userId;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Supplier = _context.Supplier.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.OrganizationName
             }).ToList();

            ViewBag.Sizes = _context.Sizes.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product, int[] sizeIds, int[] quantities)
        { 
            if (!ModelState.IsValid)
            {
                foreach (var err in ModelState)
                {
                    foreach (var e in err.Value.Errors)
                    { Console.WriteLine($"ошибка: {err.Key} Ч {e.ErrorMessage}"); }
                }

                ViewBag.Sizes = _context.Sizes.ToList();
                return View(product);
            }



            _context.Products.Add(product);
            _context.SaveChanges();
            if (sizeIds != null && quantities != null && sizeIds.Length == quantities.Length)
            {
                for (int i = 0; i < sizeIds.Length; i++)
                {
                    if (quantities[i] > 0)
                    {
                        var ps = new ProductSizes
                        {
                            ProductId = product.Id,
                            SizeId = sizeIds[i],
                            Quantity = quantities[i]
                        };
                        _context.ProductSizes.Add(ps);
                    }
                }
                _context.SaveChanges();
            }
            return RedirectToAction("Catalog");
        }

        public IActionResult Supply(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) 
                return NotFound();

            ViewBag.Sizes = _context.Sizes.ToList();

            return View(product);
        }

        [HttpPost]
        public IActionResult Supply(int productId, int sizeId, int quantity)
        {

            var existing = _context.ProductSizes
                .FirstOrDefault(ps => ps.ProductId == productId && ps.SizeId == sizeId);

            if (existing != null)
            {

                existing.Quantity += quantity;
            }
            else
            {
                var newRecord = new ProductSizes
                {
                    ProductId = productId,
                    SizeId = sizeId,
                    Quantity = quantity
                };
                _context.ProductSizes.Add(newRecord);
            }

            _context.SaveChanges();
            return RedirectToAction("EditCard", new { id = productId });
        }
    }
}

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
        private readonly IWebHostEnvironment _env;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment env)
        {
            _logger = logger;
            _context = context;
            _env = env;
        }
        [HttpGet]
        public IActionResult SupplierCatalog(string searchString)
        {
            var suppliers = _context.Supplier.Where(p => p.IsDeleted == false).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                suppliers = suppliers.Where(p => p.OrganizationName.Contains(searchString));
            }

            return View(suppliers.ToList());
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
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
        [Authorize(Roles = "Manager")]
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

                ModelState.AddModelError(string.Empty, "Ошибка сохранения изменений: " + ex.Message);

            }

            return RedirectToAction("SupplierCatalog", new { searchString = searchString });


        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public IActionResult AddSupplier()
        {
                ViewBag.Supplier = _context.Supplier.Select(s => new SelectListItem
                {
                
                }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public IActionResult AddSupplier(Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                foreach (var err in ModelState)
                {
                    foreach (var e in err.Value.Errors)
                    { Console.WriteLine($"ошибка: {err.Key} — {e.ErrorMessage}"); }
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
        [Authorize(Roles = "Manager")]
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
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> EditCard(Product product, string? searchString, IFormFile? imageFile)
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
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadPath = Path.Combine(_env.WebRootPath, "images", "products");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

             //удаление старой картинки из папки (подумать убрать или нет)
                if (!string.IsNullOrEmpty(productToUpdate.ImagePath))
                {
                    var oldPath = Path.Combine(
                        _env.WebRootPath,
                        productToUpdate.ImagePath.TrimStart('/'));

                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                productToUpdate.ImagePath = "/images/products/" + fileName;
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
                ModelState.AddModelError(string.Empty, "Ошибка сохранения изменений: " + ex.Message);
                return View(productToUpdate); 
            }

            return RedirectToAction("Catalog", new { searchString = searchString }); ;


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
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
                ModelState.AddModelError(string.Empty, "Ошибка удаления: " + ex.Message);
                return RedirectToAction("Catalog", new { searchString = searchString });
            
        }
            return RedirectToAction("Catalog", new { searchString = searchString }); ;


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public IActionResult DeleteSupplier(int Id, string? searchString)
        {

            ViewBag.SearchString = searchString;
            var supplierToUpdate = _context.Supplier.FirstOrDefault(p => p.Id == Id);


            if (supplierToUpdate == null)
            {
                return NotFound();
            }


            supplierToUpdate.IsDeleted = true;



            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Ошибка удаления: " + ex.Message);
                return RedirectToAction("SupplierCatalog", new { searchString = searchString });

            }
            return RedirectToAction("SupplierCatalog", new { searchString = searchString }); ;


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
        [Authorize(Roles = "Manager")]
        public IActionResult Create()
        {
            ViewBag.Supplier = _context.Supplier.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.OrganizationName
             }).ToList();

            ViewBag.Sizes = _context.Sizes.ToList();
            ViewBag.Categories = _context.Category.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult>  Create(Product product, int[] sizeIds, int[] quantities, IFormFile imageFile)
        { 
            if (!ModelState.IsValid)
            {
                foreach (var err in ModelState)
                {
                    foreach (var e in err.Value.Errors)
                    { Console.WriteLine($"ошибка: {err.Key} — {e.ErrorMessage}"); }
                }
                ViewBag.Supplier = _context.Supplier.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.OrganizationName
                }).ToList();
                ViewBag.Sizes = _context.Sizes.ToList();

                ViewBag.Categories = new SelectList(_context.Category, "Id", "Name", product?.CategoryId);
           
                return View(product);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadPath = Path.Combine(_env.WebRootPath, "images", "products");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                product.ImagePath = "/images/products/" + fileName;
            }

            try { 
            _context.Products.Add(product);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Ошибка при добавлении нового товара: одно из полей введено некорректно/одного из значений не существует");
                ViewBag.Sizes = _context.Sizes.ToList();
                ViewBag.Supplier = _context.Supplier.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.OrganizationName
                }).ToList();
                ViewBag.Categories= _context.Category.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList();
                return View(product);

            }
      
            
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
            TempData["SuccessMessage"] = "Товар успешно добавлен в каталог!";
            return RedirectToAction("Catalog");
        }
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public IActionResult Supply(int id, string? searchString)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) 
                return NotFound();

            ViewBag.Sizes = _context.Sizes.ToList();

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public IActionResult Supply(int productId, int sizeId, int quantity, string? searchString)
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
            return RedirectToAction("Catalog", new { searchString });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public IActionResult AddCategory(string name, string? searchString)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Название категории не может быть пустым";
                return RedirectToAction("Catalog", new { searchString });
            }

            if (_context.Category.Any(c => c.Name == name))
            {
                TempData["Error"] = "Категория с таким названием уже существует.";
                return RedirectToAction("Catalog", new { searchString });
            }

            var category = new Category { Name = name };
            _context.Category.Add(category);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ошибка при сохранении категории: " + ex.Message;
                return RedirectToAction("Catalog", new { searchString });
            }

            return RedirectToAction("Catalog", new { searchString });
        }

    }
}

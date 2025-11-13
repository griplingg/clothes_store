using System.Diagnostics;
using ClothesWeb.Models;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Catalog(string searchString)
        {
            var products = from p in _context.Products
                           select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString));
            }

            return View(products.ToList());
        }

        [HttpGet]
        public IActionResult EditCard(int id, string searchString)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            ViewBag.SearchString = searchString;
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCard(Product product, string searchString)
        {
            if (ModelState.IsValid)
            {
                var productToUpdate = _context.Products.FirstOrDefault(p => p.Id == product.Id);
                if (productToUpdate == null)
                    return NotFound();

                productToUpdate.Name = product.Name;
                productToUpdate.Price = product.Price;
                productToUpdate.Color = product.Color;
                productToUpdate.Sizes = product.Sizes;
                productToUpdate.SupplierId = product.SupplierId;

                _context.SaveChanges();
             
                if (!string.IsNullOrEmpty(searchString))
                    return RedirectToAction("Catalog", new { searchString });

                return RedirectToAction("Catalog");
            }

            ViewBag.SearchString = searchString;
            return View(product);
        }
        public IActionResult Index()
        {
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
    }
}

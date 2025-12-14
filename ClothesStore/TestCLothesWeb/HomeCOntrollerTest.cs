using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClothesWeb.Controllers;
using ClothesWeb.Models;
using ClothesWeb;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;

namespace TestCLothesWeb
{
    public class HomeControllerTest
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            context.Category.Add(new Category { Id = 1, Name = "Одежда" });

            context.Supplier.Add(new Supplier
            {
                Id = 1,
                OrganizationName = "TestSupplier",
                ContactName = "Ivan",
                ContactMail = "test@mail.ru",
                ContactPhone = "123",
                IsDeleted = false
            });

            context.Products.Add(new Product
            {
                Id = 1,
                Name = "Футболка",
                Price = 100,
                Color = "Белый",
                ArticleNumber = "A001",
                CategoryId = 1,
                SupplierId = 1,
                IsDeleted = false
            });

            context.SaveChanges();
            return context;
        }

        private HomeController GetController(ApplicationDbContext context)
        {
            var logger = new Mock<ILogger<HomeController>>();

            var env = new Mock<IWebHostEnvironment>();
            env.Setup(e => e.WebRootPath).Returns("wwwroot");

            var controller = new HomeController(logger.Object, context, env.Object);

     
            controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()
            );

            return controller;
        }

        [Fact]
        public void Catalog_ReturnsViewWithModel()
        {
            var context = GetDbContext();
            var controller = GetController(context);

            var result = controller.Catalog(null) as ViewResult;

            Assert.NotNull(result);
            Assert.NotNull(result.Model);
        }

        [Fact]
        public void SupplierCatalog_FilterSarch()
        {
            var context = GetDbContext();
            var controller = GetController(context);

            var result = controller.SupplierCatalog("Test") as ViewResult;
            var model = Assert.IsAssignableFrom<List<Supplier>>(result.Model);

            Assert.Single(model);
        }

        [Fact]
        public void EditCard_Get_ProductExists()
        {
            var context = GetDbContext();
            var controller = GetController(context);

            var result = controller.EditCard(1, null);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void EditCard_Get_ProductNotFound()
        {
            var context = GetDbContext();
            var controller = GetController(context);

            var result = controller.EditCard(999, null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteCard_DeleteProduct()
        {
            var context = GetDbContext();
            var controller = GetController(context);

            var result = controller.DeleteCard(1, null);

            var product = context.Products.First();
            Assert.True(product.IsDeleted);
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void AddCategory_EmptyNamer()
        {
            var context = GetDbContext();
            var controller = GetController(context);

            var result = controller.AddCategory("", null) as RedirectToActionResult;

            Assert.Equal("Catalog", result.ActionName);
            Assert.True(controller.TempData.ContainsKey("Error"));
        }

        [Fact]
        public void AddCategory_NewCategory()
        {
            var context = GetDbContext();
            var controller = GetController(context);

            var result = controller.AddCategory("Обувь", null);

            Assert.True(context.Category.Any(c => c.Name == "Обувь"));
            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}

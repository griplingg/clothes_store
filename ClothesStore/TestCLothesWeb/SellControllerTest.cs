using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClothesWeb;
using ClothesWeb.Controllers;
using ClothesWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace TestCLothesWeb
{
    public class SellControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            var category = new Category { Id = 1, Name = "Одежда" };
            context.Category.Add(category);

            context.Supplier.Add(new Supplier
            {
                Id = 1,
                OrganizationName = "TestSupplier",
                ContactName = "Ivan",
                ContactMail = "test@test.com",
                ContactPhone = "123"
            });


            var size = new Size { Id = 1, Name = "M" };
            context.Sizes.Add(size);

 
            var product = new Product
            {
                Id = 1,
                Name = "Футболка",
                Price = 100,
                Color = "Черный",
                ArticleNumber = "A001",
                SupplierId = 1,
                CategoryId = 1,
                IsDeleted = false
            };
            context.Products.Add(product);

            context.ProductSizes.Add(new ProductSizes
            {
                ProductId = 1,
                SizeId = 1,
                Quantity = 5
            });

            context.SaveChanges();
            return context;
        }

        [Fact]
        public async Task AddPurchase_Get_ReturnsView()
        {
            var context = GetInMemoryDbContext();
            var controller = new SellController(context);

            var result = await controller.AddPurchase() as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsType<AddPurchaseViewModel>(result.Model);
            Assert.Single(model.AllProducts);
        }

        [Fact]
        public async Task AddPurchase_Post_NoItems()
        {
            var context = GetInMemoryDbContext();
            var controller = new SellController(context);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(
                        new System.Security.Claims.ClaimsIdentity(
                            new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "test-user") },
                            "TestAuthType"))
                }
            };

            var model = new AddPurchaseViewModel
            {
                PaymentMethod = "Наличные",
                Items = new List<AddPurchaseItemViewModel>() 
            };

            var result = await controller.AddPurchase(model, DateTime.Today) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task AddPurchase_Post_ValidData()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new ApplicationDbContext(options);

            var category = new Category { Id = 1, Name = "Одежда" };
            context.Category.Add(category);

            var supplier = new Supplier
            {
                Id = 1,
                OrganizationName = "TestSupplier",
                ContactName = "Иван",
                ContactMail = "test@mail.com",
                ContactPhone = "123"
            };
            context.Supplier.Add(supplier);

            var size = new Size { Id = 1, Name = "M" };
            context.Sizes.Add(size);

            var product = new Product
            {
                Id = 1,
                Name = "Футболка",
                Price = 100,
                Color = "Красный",
                ArticleNumber = "A001",
                SupplierId = supplier.Id,
                CategoryId = category.Id,
                IsDeleted = false
            };
            context.Products.Add(product);

            context.ProductSizes.Add(new ProductSizes
            {
                ProductId = product.Id,
                SizeId = size.Id,
                Quantity = 5
            });

            await context.SaveChangesAsync();

            var controller = new SellController(context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(
                        new System.Security.Claims.ClaimsIdentity(
                            new[]
                            {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "test-user")
                            }, "TestAuthType"))
                }
            };

            var model = new AddPurchaseViewModel
            {
                PaymentMethod = "Наличные",
                Items = new List<AddPurchaseItemViewModel>
        {
            new AddPurchaseItemViewModel
            {
                ProductId = product.Id,
                SizeId = size.Id,
                Quantity = 2
            }
        }
            };

            var result = await controller.AddPurchase(model, DateTime.Today);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddPurchase", redirectResult.ActionName);

            var sell = context.Sells.Include(s => s.SellItem).FirstOrDefault();
            Assert.NotNull(sell);
            Assert.Equal("Наличные", sell.PaymentMethod);
            Assert.Equal("test-user", sell.EmployeeId);
            Assert.Single(sell.SellItem);
            Assert.Equal(2, sell.SellItem.First().Quantity);

            var updatedProductSize = context.ProductSizes.First();
            Assert.Equal(3, updatedProductSize.Quantity); // 5 - 2 = 3
        }




        [Fact]
        public async Task AddPurchase_Post_NotEnoughQuantity()
        {
            var context = GetInMemoryDbContext();
            var controller = new SellController(context);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(
                        new System.Security.Claims.ClaimsIdentity(
                            new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "test-user") },
                            "TestAuthType"))
                }
            };

            var model = new AddPurchaseViewModel
            {
                PaymentMethod = "Карта",
                Items = new List<AddPurchaseItemViewModel>
                {
                    new AddPurchaseItemViewModel
                    {
                        ProductId = 1,
                        SizeId = 1,
                        Quantity = 100 
                    }
                }
            };

            var result = await controller.AddPurchase(model, DateTime.Today) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
        }

  
        [Fact]
        public async Task GetSizesByProduc()
        {
            var context = GetInMemoryDbContext();
            var controller = new SellController(context);

            var result = await controller.GetSizesByProduct(1) as JsonResult;

            Assert.NotNull(result);
            var data = Assert.IsAssignableFrom<IEnumerable<object>>(result.Value);
            Assert.Single(data);
        }
    }
}

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
using System.Security.Claims;

namespace TestCLothesWeb
{
    public class ReturnsControllerTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private ReturnsController GetController(ApplicationDbContext context, string? userId = null)
        {
            var controller = new ReturnsController(context);

            if (userId != null)
            {
                var user = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        new[] { new Claim(ClaimTypes.NameIdentifier, userId) },
                        "TestAuth"
                    )
                );

                controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = user
                    }
                };
            }

            return controller;
        }


        [Fact]
        public void Index_ReturnsViewResult()
        {
            var context = GetDbContext();
            var controller = GetController(context);

            var result = controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void CreateSelect_ReturnsView()
        {
            var context = GetDbContext();
            var controller = GetController(context);

            var result = controller.CreateSelect(null);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Create_Get_InvalidId()
        {
            var context = GetDbContext();
            var controller = GetController(context);

            var result = controller.Create(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_Get_ValidId()
        {
            var context = GetDbContext();

            var product = new Product
            {
                Id = 1,
                Name = "футболка",
                Color = "красный",
                ArticleNumber = "A1"
            };

            var size = new Size
            {
                Id = 1,
                Name = "M"
            };

            var sellItem = new SellItem
            {
                Id = 1,
                SellId = 1,
                Product = product,
                Size = size,
                Color = "красный"
            };

            context.Products.Add(product);
            context.Sizes.Add(size);
            context.SellItems.Add(sellItem);
            context.SaveChanges();

            var controller = GetController(context);

            var result = controller.Create(1);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Create_Post_AddsReturnProduct()
        {
            var context = GetDbContext();
            var userId = "123";

            var controller = GetController(context, userId);

            var model = new ReturnProduct
            {
                SellItemId = 1,
                SellId = 1,
                Reason = "-"
            };

            var result = controller.Create(model, DateTime.Now);

            Assert.Single(context.ReturnProducts);
            Assert.Equal(userId, context.ReturnProducts.First().EmployeeId);
            Assert.Equal(1, context.ReturnProducts.First().StatusId);
            Assert.IsType<RedirectToActionResult>(result);
        }

  
        [Fact]
        public void ChangeStatus_Approve()
        {
            var context = GetDbContext();

            var returnProduct = new ReturnProduct
            {
                Id = 1,
                StatusId = 1,
                Reason = "-",
                EmployeeId = "123"
            };

            context.ReturnProducts.Add(returnProduct);
            context.SaveChanges();

            var controller = GetController(context);

            var result = controller.ChangeStatus(1, "approve");

            Assert.Equal(3, context.ReturnProducts.First().StatusId);
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void ChangeStatus_Reject()
        {
            var context = GetDbContext();

            var returnProduct = new ReturnProduct
            {
                Id = 1,
                StatusId = 1,
                Reason = "-",
                EmployeeId = "1"
            };

            context.ReturnProducts.Add(returnProduct);
            context.SaveChanges();

            var controller = GetController(context);

            var result = controller.ChangeStatus(1, "reject");

            Assert.Equal(4, context.ReturnProducts.First().StatusId);
            Assert.IsType<RedirectToActionResult>(result);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClothesWeb.Controllers;
using ClothesWeb.Models;
using ClothesWeb;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TestCLothesWeb
{
    public class ReportsControllerTest
    {
        public class ReportsControllerTests
        {
            private ApplicationDbContext GetInMemoryDbContext()
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                var context = new ApplicationDbContext(options);

 
                context.Users.AddRange(
                    new IdentityUser { Id = "1", UserName = "user1" },
                    new IdentityUser { Id = "2", UserName = "user2" }
                );

                var product1 = new Product
                {
                    Id = 1,
                    Name = "футболка",
                    Price = 1000,
                    Color = "красный",
                    ArticleNumber = "A001",
                    SupplierId = 1,
                    CategoryId = 1
                };

                var product2 = new Product
                {
                    Id = 2,
                    Name = "брюки",
                    Price = 2550,
                    Color = "черный",
                    ArticleNumber = "A002",
                    SupplierId = 1,
                    CategoryId = 1
                };
                context.Products.AddRange(product1, product2);
                context.Category.Add(new Category { Id = 1, Name = "категория1" });

             
                context.Sells.Add(new Sell
                {
                    Id = 1,
                    Date = DateTime.Today,
                    EmployeeId = "1",
                    PaymentMethod = "Cash",
                    SellItem = new List<SellItem>

                    {
                        new SellItem { Id = 1, Product = product1, Quantity = 1, Price = 1000, Color = "красный"},
                        new SellItem { Id = 2, Product = product2, Quantity = 1, Price = 2550 , Color = "черный"}
                    }
                });
                context.Supplier.Add(new Supplier
                {
                    Id = 1,
                    OrganizationName = "поставщик1",
                    ContactName = "Ольга",
                    ContactMail = "test@mail.com",
                    ContactPhone = "+79004562123"
                });

                context.SaveChanges();
                return context;
            }

            [Fact]
            public void Statistics_Get_ReturnsView()
            {
                var context = GetInMemoryDbContext();
                var controller = new ReportsController(context);

                var result = controller.Statistics() as ViewResult;

                Assert.NotNull(result);
                var employees = result.ViewData["Employee"] as List<SelectListItem>;
                Assert.Equal(2, employees.Count);
            }

            [Fact]
            public void Statistics_Post_StartMoreThanEnd()
            {
                var context = GetInMemoryDbContext();
                var controller = new ReportsController(context);

                var result = controller.Statistics(
                    startDate: DateTime.Today.AddDays(1),
                    endDate: DateTime.Today,
                    clientDate: DateTime.Today,
                    employeeId: null
                ) as ViewResult;

                Assert.NotNull(result);
                Assert.Equal("Дата начала не может быть позже даты конца.", controller.ViewData["Error"]);
            }

            [Fact]
            public void Statistics_Post_DateBeforeStart()
            {
                var context = GetInMemoryDbContext();
                var controller = new ReportsController(context);

                var result = controller.Statistics(
                    startDate: DateTime.Today,
                    endDate: DateTime.Today.AddDays(1),
                    clientDate: DateTime.Today.AddDays(-1),
                    employeeId: null
                ) as ViewResult;

                Assert.NotNull(result);
                Assert.Equal("Статистика для будущего времени недоступна.", controller.ViewData["Error"]);
            }

            [Fact]
            public void Statistics_Post()
            {
                var context = GetInMemoryDbContext();
                var controller = new ReportsController(context);

                var result = controller.Statistics(
                    startDate: DateTime.Today.AddDays(-1),
                    endDate: DateTime.Today.AddDays(1),
                    clientDate: DateTime.Today,
                    employeeId: null
                ) as ViewResult;

                Assert.NotNull(result);
                Assert.True(result.ViewData.ContainsKey("TotalRevenue"));
                var totalRevenue = (decimal)controller.ViewBag.TotalRevenue;
                Assert.Equal(3550, totalRevenue); 
            }

            [Fact]
            public void Statistics_Post_EmployeeFilter()
            {
                var context = GetInMemoryDbContext();
                var controller = new ReportsController(context);

                var result = controller.Statistics(
                    startDate: DateTime.Today.AddDays(-1),
                    endDate: DateTime.Today.AddDays(1),
                    clientDate: DateTime.Today,
                    employeeId: "1"
                ) as ViewResult;

                Assert.NotNull(result);
                var totalRevenue = (decimal)controller.ViewBag.TotalRevenue;
                Assert.Equal(3550, totalRevenue);
            }
        }
    }
}

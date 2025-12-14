using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ClothesWeb.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClothesWeb.Models;

namespace TestCLothesWeb
{
public class AdminControllerTest
{
    private static Mock<UserManager<IdentityUser>> MockUserManager(List<IdentityUser> users)
    {
        var store = new Mock<IUserStore<IdentityUser>>();

        var userManager = new Mock<UserManager<IdentityUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        userManager.Setup(u => u.Users)
            .Returns(users.AsQueryable());

        userManager.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string id) => users.FirstOrDefault(u => u.Id == id));

        userManager.Setup(u => u.GetRolesAsync(It.IsAny<IdentityUser>()))
            .ReturnsAsync(new List<string> { "User" });

        userManager.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        userManager.Setup(u => u.DeleteAsync(It.IsAny<IdentityUser>()))
            .ReturnsAsync(IdentityResult.Success);

        userManager.Setup(u => u.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        userManager.Setup(u => u.RemoveFromRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        return userManager;
    }

    private static Mock<RoleManager<IdentityRole>> MockRoleManager(List<IdentityRole> roles)
    {
        var store = new Mock<IRoleStore<IdentityRole>>();

        var roleManager = new Mock<RoleManager<IdentityRole>>(
            store.Object, null, null, null, null);

        roleManager.Setup(r => r.Roles)
            .Returns(roles.AsQueryable());

        roleManager.Setup(r => r.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync((string roleName) => roles.Any(r => r.Name == roleName));

        return roleManager;
    }

    private AdminController GetController()
    {
        var users = new List<IdentityUser>
        {
            new IdentityUser { Id = "1", UserName = "admin@test.com" },
            new IdentityUser { Id = "2", UserName = "manager@test.com" }
        };

        var roles = new List<IdentityRole>
        {
            new IdentityRole("Admin"),
            new IdentityRole("Manager")
        };

        var userManager = MockUserManager(users);
        var roleManager = MockRoleManager(roles);

        return new AdminController(userManager.Object, roleManager.Object);
    }


    [Fact]
    public async Task Manage_Get_ReturnView()
    {
        var controller = GetController();

        var result = await controller.Manage();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task CreateUser_ValidModel()
    {
        var controller = GetController();

        var model = new CreateUserView
        {
            Email = "test@mail.com",
            Password = "123"
        };

        var result = await controller.CreateUser(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Manage", redirect.ActionName);
    }

    [Fact]
    public async Task AddUserToRole_ValidData()
    {
        var controller = GetController();

        var result = await controller.AddUserToRole("1", "Admin");

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Manage", redirect.ActionName);
    }

    [Fact]
    public async Task RemoveUserFromRole()
    {
        var controller = GetController();

        var result = await controller.RemoveUserFromRole("2", "Manager");

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Manage", redirect.ActionName);
    }

    [Fact]
    public async Task DeleteUser_ValidUser_RedirectsToManage()
    {
        var controller = GetController();

        var result = await controller.DeleteUser("2");

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Manage", redirect.ActionName);
    }
}
}
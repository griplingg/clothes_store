using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ClothesWeb.Controllers;
using ClothesWeb.Models;
using System.Threading.Tasks;
namespace TestCLothesWeb
{
    public class AccountControllerTest
    {

        private static Mock<UserManager<IdentityUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            return new Mock<UserManager<IdentityUser>>(
                store.Object,
                null, null, null, null, null, null, null, null
            );
        }

        [Fact]
        public void Login_Get_ReturnView()
        {
            var userManager = MockUserManager();
            var signInManager = new Mock<SignInManager<IdentityUser>>(
                userManager.Object,
                new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
                null, null, null, null
            );

            var controller = new AccountController(
                signInManager.Object,
                userManager.Object
            );

            var result = controller.Login();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Login_Post_UncorrectModel()
        {
            var userManager = MockUserManager();
            var signInManager = new Mock<SignInManager<IdentityUser>>(
                userManager.Object,
                new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
                null, null, null, null
            );

            var controller = new AccountController(
                signInManager.Object,
                userManager.Object
            );

            controller.ModelState.AddModelError("Email", "Required");

            var model = new LoginViewModel();

            var result = await controller.Login(model);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Login_Post_UserNotFound()
        {
            var userManager = MockUserManager();
            userManager
                .Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((IdentityUser)null);

            var signInManager = new Mock<SignInManager<IdentityUser>>(
                userManager.Object,
                new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
                null, null, null, null
            );

            var controller = new AccountController(
                signInManager.Object,
                userManager.Object
            );

            var model = new LoginViewModel
            {
                Email = "test@mail.com",
                Password = "123"
            };

            var result = await controller.Login(model);

            var view = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }
        [Fact]
        public async Task Login_Post_Success()
        {
            var user = new IdentityUser { Email = "test@mail.com" };

            var userManager = MockUserManager();
            userManager
                .Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            var signInManager = new Mock<SignInManager<IdentityUser>>(
                userManager.Object,
                new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
                null, null, null, null
            );

            signInManager
                .Setup(s => s.PasswordSignInAsync(
                    user,
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var controller = new AccountController(
                signInManager.Object,
                userManager.Object
            );

            var model = new LoginViewModel
            {
                Email = "test@mail.com",
                Password = "123"
            };

            var result = await controller.Login(model);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }


    }
}
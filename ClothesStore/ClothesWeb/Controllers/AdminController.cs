using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using ClothesWeb.Models;
using System.Collections.Generic;

namespace ClothesWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [HttpGet]
        public async Task<IActionResult> Manage()
        {

            var users = _userManager.Users.ToList();


            var usersRoles = new List<UserRolesView>();
            foreach (var user in users)
            {
                var roleNames = await _userManager.GetRolesAsync(user);
                usersRoles.Add(new UserRolesView
                {
                    User = user,
                    RoleNames = roleNames
                });
            }

            var roles = _roleManager.Roles.ToList();

       
            ViewBag.UsersRoles = usersRoles;
            ViewBag.AllRoles = roles;
            ViewBag.CreateUser = new CreateUserView();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserView model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Manage");

            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join(", ",
                    result.Errors.Select(e => e.Description));
            }

            return RedirectToAction("Manage");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserToRole(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
                return RedirectToAction("Manage");

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && await _roleManager.RoleExistsAsync(roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }

            return RedirectToAction("Manage");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUserFromRole(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
            return RedirectToAction("Manage");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !await _roleManager.RoleExistsAsync(roleName))
                    return RedirectToAction("Manage");

            if (roleName == "Admin" && user.Id == _userManager.GetUserId(User))
                {
                    TempData["Error"] = "Вы не можете удалить роль Admin у себя!";
                    return RedirectToAction("Manage");
                }


             await _userManager.RemoveFromRoleAsync(user, roleName);

             return RedirectToAction("Manage");
            
        }



    

        
    }



    
}

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

        // GET: Управление ролями
        [HttpGet]
        public async Task<IActionResult> Manage()
        {

            var users = _userManager.Users.ToList();


            var usersRoles = new List<UserRolesViewModel>();
            foreach (var user in users)
            {
                var roleNames = await _userManager.GetRolesAsync(user);
                usersRoles.Add(new UserRolesViewModel
                {
                    User = user,
                    RoleNames = roleNames
                });
            }

            var roles = _roleManager.Roles.ToList();

       
            ViewBag.UsersRoles = usersRoles;
            ViewBag.AllRoles = roles;

            return View();
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


    public class UserRolesViewModel
    {
        public IdentityUser User { get; set; }
        public IList<string> RoleNames { get; set; }
    }
}

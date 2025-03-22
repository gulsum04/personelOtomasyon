using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personelOtomasyon.Models;
using personelOtomasyon.Data.ViewModels;

namespace personelOtomasyon.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Kullanıcıları listele
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserWithRoleVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRolesViewModel.Add(new UserWithRoleVM
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    TcKimlikNo = user.TcKimlikNo,
                    CurrentRole = roles.FirstOrDefault() ?? "Rol Yok"
                });
            }

            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            return View(userRolesViewModel);
        }

        // Rol atama işlemi
        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string selectedRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            await _userManager.AddToRoleAsync(user, selectedRole);

            return RedirectToAction("Users");
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personelOtomasyon.Data;
using personelOtomasyon.Data.Static;
using personelOtomasyon.Data.ViewModels;
using personelOtomasyon.Models;

namespace personelOtomasyon.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // ---------- PROFİL ----------
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");
            return View(user);
        }

        // ---------- ŞİFRE DEĞİŞTİR ----------
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["Success"] = "Şifre başarıyla güncellendi.";
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // ---------- KULLANICILARI LİSTELE ----------
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        // ---------- LOGIN GET ----------
        public IActionResult Login(string role)
        {
            return View(new LoginVM { RequestedRole = role });
        }

        // ---------- LOGIN POST ----------
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.TcKimlikNo == loginVM.TcKimlikNo);
            if (user == null)
            {
                TempData["Error"] = "TC Kimlik No bulunamadı.";
                return View(loginVM);
            }

            // ❗ Rol kontrolü
            var isInRole = await _userManager.IsInRoleAsync(user, loginVM.RequestedRole);
            if (!isInRole)
            {
                TempData["Error"] = $"Bu giriş ekranı sadece '{loginVM.RequestedRole}' rolüne sahip kullanıcılar içindir.";
                return View(loginVM);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
            if (!result.Succeeded)
            {
                TempData["Error"] = "Şifre hatalı!";
                return View(loginVM);
            }

            // Giriş başarılıysa yönlendir
            if (loginVM.RequestedRole == UserRoles.Admin)
                return RedirectToAction("Dashboard", "Admin");

            if (loginVM.RequestedRole == UserRoles.Yonetici)
                return RedirectToAction("Dashboard", "Yonetici");

            if (loginVM.RequestedRole == UserRoles.Juri)
                return RedirectToAction("GelenBasvurular", "Juri");

            return RedirectToAction("Index", "Aday"); // Default: Aday
        }

        // ---------- REGISTER GET ----------
        public IActionResult Register() => View(new RegisterVM());

        // ---------- REGISTER POST ----------
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(registerVM);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.TcKimlikNo == registerVM.TcKimlikNo);
            if (user != null)
            {
                TempData["Error"] = "Bu TC Kimlik No zaten kayıtlı!";
                return View(registerVM);
            }

            var newUser = new ApplicationUser
            {
                FullName = registerVM.FullName,
                TcKimlikNo = registerVM.TcKimlikNo,
                UserName = registerVM.TcKimlikNo,
                Email = $"{registerVM.TcKimlikNo}@fake.tc",
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newUser, registerVM.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.User); // Default: Aday
                return View("RegisterSuccess");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(registerVM);
        }

        // ---------- LOGOUT ----------
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}

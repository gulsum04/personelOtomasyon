using KpsService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personelOtomasyon.Connected_Services.KpsService;
using personelOtomasyon.Data;
using personelOtomasyon.Data.Static;
using personelOtomasyon.Data.ViewModels;
using personelOtomasyon.Models;
using personelOtomasyon.Services;

namespace personelOtomasyon.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IKpsService _kpsService;
        private readonly MailService _mailService;


        public AccountController(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      ApplicationDbContext context,
      IKpsService kpsService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _kpsService = kpsService;
            _mailService = new MailService();

        }
     
       
        // ---------- PROFİL ----------
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");
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
            if (user == null)
                return RedirectToAction("Login");

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["Success"] = "Şifre başarıyla değiştirildi.";
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // ---------- TÜM KULLANICILAR (Admin) ----------
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }


        // ✅ KPS Web Servisi ile T.C. Kimlik Doğrulama
        /*try
        {
            var client = new KPSPublicSoapClient(KPSPublicSoapClient.EndpointConfiguration.KPSPublicSoap);

            // Ad ve Soyad'ı ayır
            var ad = registerVM.FullName.Split(" ")[0].ToUpper();
            var soyad = registerVM.FullName.Split(" ")[^1].ToUpper();

            var kpsResult = await client.TCKimlikNoDogrulaAsync(
                long.Parse(registerVM.TcKimlikNo),
                ad,
                soyad,
                registerVM.DogumYili
            );

            if (!kpsResult.Body.TCKimlikNoDogrulaResult)
            {
                TempData["Error"] = "TC Kimlik bilgileri doğrulanamadı. Lütfen tekrar kontrol edin.";
                return View(registerVM);
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Kimlik doğrulama servisinde hata oluştu: " + ex.Message;
            return View(registerVM);
        }*/


        // ---------- LOGIN GET ----------
        [HttpGet]
        public IActionResult Login(string role)
        {
            TempData.Clear(); // Önceki TempData mesajlarını temizle

            if (string.IsNullOrEmpty(role))
                role = UserRoles.User;

            return View(new LoginVM { RequestedRole = role });
        }

        // ---------- LOGIN POST ----------
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.TcKimlikNo == loginVM.TcKimlikNo);
            if (user == null)
            {
                TempData["Error"] = "TC Kimlik No bulunamadı.";
                return View(loginVM);
            }

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

            if (loginVM.RequestedRole == UserRoles.Admin)
                return RedirectToAction("Dashboard", "Admin");

            if (loginVM.RequestedRole == UserRoles.Yonetici)
                return RedirectToAction("Dashboard", "Yonetici");

            if (loginVM.RequestedRole == UserRoles.Juri)
                return RedirectToAction("Index", "Juri");

            return RedirectToAction("Index", "Aday");
        }

        // ---------- REGISTER GET ----------
        [HttpGet]
        public IActionResult Register()
        {
            TempData.Clear();
            return View(new RegisterVM());
        }

        // ---------- REGISTER POST ----------
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
                return View(registerVM);

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.TcKimlikNo == registerVM.TcKimlikNo);
            if (existingUser != null)
            {
                TempData["Error"] = "Bu TC Kimlik No ile zaten kayıt olunmuş.";
                return View(registerVM);
            }

            //service doğrulama 
            try
            {
                var ad = registerVM.FullName.Split(" ")[0].ToUpper();
                var soyad = registerVM.FullName.Split(" ")[^1].ToUpper();

                var dogrulandiMi = await _kpsService.TcKimlikDogrulaAsync(
                    long.Parse(registerVM.TcKimlikNo),
                    ad,
                    soyad,
                    registerVM.DogumYili
                );

                if (!dogrulandiMi)
                {
                    TempData["Error"] = "T.C. Kimlik bilgileri doğrulanamadı.";
                    return View(registerVM);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kimlik doğrulama sırasında hata oluştu: " + ex.Message;
                return View(registerVM);
            }

            var newUser = new ApplicationUser
            {
                FullName = registerVM.FullName,
                TcKimlikNo = registerVM.TcKimlikNo,
                UserName = registerVM.TcKimlikNo,
                Email = registerVM.Email,
                EmailConfirmed = true,
                KayitTarihi = DateTime.Now
            };

            var createResult = await _userManager.CreateAsync(newUser, registerVM.Password);
            if (createResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);

                // ✅ Kayıt başarılı olunca ADMIN'e bildirim maili gönder
                await _mailService.SendEmailAsync(
                    "admin@gmail.com", // Adminin mail adresi buraya
                    "Yeni Kullanıcı Kaydı",
                    $"Yeni kullanıcı kaydoldu:\n\nAd Soyad: {newUser.FullName}\nTC Kimlik No: {newUser.TcKimlikNo}\nE-posta: {newUser.Email}\nKayıt Tarihi: {newUser.KayitTarihi}"
                );

                // ✅ Kayıt başarılı olunca KULLANICIYA hoşgeldin maili gönder
                await _mailService.SendEmailAsync(
                    newUser.Email,
                    "Personel Otomasyon Sistemine Hoşgeldiniz!",
                    $"Sayın {newUser.FullName},\n\nSistemimize başarıyla kayıt oldunuz. Giriş yapabilirsiniz."
                );

                return View("RegisterSuccess");
            }

            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(registerVM);
        }

        // ---------- LOGOUT ----------
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); //  Sadece çıkış yapılıyor
            TempData.Clear(); // Kullanıcıya ait mesajları temizle
            return RedirectToAction("Index", "Home");
        }
    }
}

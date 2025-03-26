using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personelOtomasyon.Models;
using personelOtomasyon.Data;
using personelOtomasyon.Data.ViewModels;

namespace personelOtomasyon.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager,
                               RoleManager<IdentityRole> roleManager,
                               ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // 🔹 Kullanıcıları listele
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

        // 🔹 Rol atama işlemi
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

        // 🔹 Admin panel ana sayfa
        public IActionResult Dashboard()
        {
            return View();
        }

        // 🔹 Tüm başvuruları listele (yönlendirme için)
        public async Task<IActionResult> Basvurular()
        {
            var basvurular = await _context.Basvurular
                .Include(b => b.Ilan)
                .Include(b => b.Aday)
                .ToListAsync();

            return View(basvurular);
        }

        // 🔹 Yönlendirme formu (GET)
        public async Task<IActionResult> JuriyeYonet(int basvuruId)
        {
            var basvuru = await _context.Basvurular
                .Include(b => b.Ilan)
                .Include(b => b.Aday)
                .FirstOrDefaultAsync(b => b.BasvuruId == basvuruId);

            if (basvuru == null) return NotFound();

            var juriUyeleri = await _context.JuriUyeleri
                .Include(j => j.Juri)
                .Where(j => j.IlanId == basvuru.IlanId)
                .ToListAsync();

            ViewBag.JuriList = juriUyeleri;
            ViewBag.Basvuru = basvuru;

            return View();
        }

        // 🔹 Yönlendirme işlemi (POST)
        [HttpPost]
        public async Task<IActionResult> JuriyeYonet(int basvuruId, List<string> seciliJuriIdler)
        {
            if (seciliJuriIdler == null || !seciliJuriIdler.Any())
            {
                TempData["Error"] = "En az bir jüri üyesi seçmelisiniz.";
                return RedirectToAction("JuriyeYonet", new { basvuruId });
            }

            foreach (var juriId in seciliJuriIdler)
            {
                var atama = new BasvuruJuri
                {
                    BasvuruId = basvuruId,
                    JuriId = juriId
                };
                _context.BasvuruJuriAtamalari.Add(atama);
            }

            var basvuru = await _context.Basvurular.FindAsync(basvuruId);
            if (basvuru != null)
            {
                basvuru.Durum = "Jüriye Yönlendirildi";
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Başvuru jüri üyelerine yönlendirildi.";
            return RedirectToAction("Basvurular");
        }


        // 🔹 İlan için jüri atama formu (GET)
        public async Task<IActionResult> IlanaJuriAta(int ilanId)
        {
            var ilan = await _context.AkademikIlanlar.FindAsync(ilanId);
            if (ilan == null) return NotFound();

            // Tüm jüri kullanıcılarını al
            var tumJuriUyeleri = await _userManager.GetUsersInRoleAsync("Juri");

            // Bu ilana daha önce atanmış jürilerin Id'lerini al
            var atananJuriIdler = await _context.JuriUyeleri
                .Where(j => j.IlanId == ilanId)
                .Select(j => j.KullaniciJuriId)
                .ToListAsync();

            // Sadece henüz atanmamış jürileri getir
            var uygunJuriListesi = tumJuriUyeleri
                .Where(j => !atananJuriIdler.Contains(j.Id))
                .ToList();

            ViewBag.Ilan = ilan;
            ViewBag.JuriList = uygunJuriListesi;

            return View();
        }


        // 🔹 İlan için jüri atama işlemi (POST)
        [HttpPost]
        public async Task<IActionResult> IlanaJuriAta(int ilanId, List<string> seciliJuriIdler)
        {
            var ilan = await _context.AkademikIlanlar.FindAsync(ilanId);
            if (ilan == null) return NotFound();

            foreach (var juriId in seciliJuriIdler)
            {
                // Aynı jüri bu ilana daha önce atanmışsa tekrar ekleme
                var varMi = await _context.JuriUyeleri
                    .AnyAsync(j => j.IlanId == ilanId && j.KullaniciJuriId == juriId);
                if (!varMi)
                {
                    _context.JuriUyeleri.Add(new JuriUyesi
                    {
                        IlanId = ilanId,
                        KullaniciJuriId = juriId
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Jüri üyeleri ilana başarıyla atandı.";
            return RedirectToAction("Dashboard");
        }

    }
}

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

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var model = new List<UserWithRoleVM>();

            foreach (var user in users)
            {
                var role = await _userManager.GetRolesAsync(user);
                model.Add(new UserWithRoleVM
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    TcKimlikNo = user.TcKimlikNo,
                    CurrentRole = role.FirstOrDefault() ?? "Rol Yok"
                });
            }

            ViewBag.AllRoles = _roleManager.Roles
                .Where(r => new[] { "Admin", "User", "Juri", "Yonetici" }.Contains(r.Name))
                .Select(r => r.Name)
                .ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string selectedRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, selectedRole);

            _context.Bildirimler.Add(new Bildirim
            {
                KullaniciId = userId,
                Mesaj = $"Rolünüz {selectedRole} olarak değiştirildi.",
                OkunduMu = false
            });

            await _context.SaveChangesAsync();
            return RedirectToAction("Users");
        }

        public IActionResult Dashboard() => View();

        public async Task<IActionResult> Basvurular()
        {
            var basvurular = await _context.Basvurular
                .Include(b => b.Ilan)
                .Include(b => b.Aday)
                .Where(b => b.Ilan != null)
                .ToListAsync();

            var basvuruIdler = basvurular.Select(b => b.BasvuruId).ToList();

            var juriAtamaDict = _context.BasvuruJuriAtamalari
                .Where(j => basvuruIdler.Contains(j.BasvuruId))
                .Select(j => j.BasvuruId)
                .Distinct()
                .ToDictionary(id => id, id => true);

            ViewBag.JuriAtamaDurumlari = juriAtamaDict;

            return RedirectToAction("Index", "Basvuru");
        }

        public async Task<IActionResult> IlanaJuriAta(int ilanId)
        {
            var ilan = await _context.AkademikIlanlar.FindAsync(ilanId);
            if (ilan == null) return NotFound();

            var tumJuriUyeleri = await _userManager.GetUsersInRoleAsync("Juri");
            var atananJuriIdler = await _context.JuriUyeleri
                .Where(j => j.IlanId == ilanId)
                .Select(j => j.KullaniciJuriId)
                .ToListAsync();

            ViewBag.Ilan = ilan;
            ViewBag.AtanabilecekJuriListesi = tumJuriUyeleri.Where(j => !atananJuriIdler.Contains(j.Id)).ToList();
            ViewBag.AtanmisJuriListesi = tumJuriUyeleri.Where(j => atananJuriIdler.Contains(j.Id)).ToList();

            return View();
        }

        [HttpPost]
        public JsonResult IlanaJuriAtaAjax(int ilanId, List<string> seciliJuriIdler)
        {
            var atananlar = new List<object>();

            foreach (var juriId in seciliJuriIdler)
            {
                if (!_context.JuriUyeleri.Any(j => j.IlanId == ilanId && j.KullaniciJuriId == juriId))
                {
                    _context.JuriUyeleri.Add(new JuriUyesi
                    {
                        JuriUyesiId = Guid.NewGuid().ToString(), // 🔑 BU SATIR ZORUNLU
                        IlanId = ilanId,
                        KullaniciJuriId = juriId
                    });

                    var user = _context.Users.FirstOrDefault(u => u.Id == juriId);
                    if (user != null)
                    {
                        atananlar.Add(new
                        {
                            id = user.Id,
                            adSoyad = $"{user.FullName} ({user.TcKimlikNo})"
                        });
                    }
                }
            }

            _context.SaveChanges();
            return Json(new { success = true, atananlar });
        }
        [HttpPost]
        public JsonResult JuriSil(string juriId, int ilanId)
        {
            // 1. Bu jüriye ait başvuru atamaları varsa önce onları kaldır
            var basvuruAtamalari = _context.BasvuruJuriAtamalari
                .Where(x => x.JuriId == juriId)
                .ToList();

            if (basvuruAtamalari.Any())
            {
                _context.BasvuruJuriAtamalari.RemoveRange(basvuruAtamalari);
            }

            // 2. İlan-jüri ilişkisini kaldır
            var juri = _context.JuriUyeleri.FirstOrDefault(j => j.KullaniciJuriId == juriId && j.IlanId == ilanId);
            if (juri != null)
            {
                _context.JuriUyeleri.Remove(juri);
            }

            // 3. Kayıtları kaydet
            _context.SaveChanges();

            // 4. Kullanıcı bilgisi döndür
            var user = _context.Users.FirstOrDefault(u => u.Id == juriId);
            if (user != null)
            {
                return Json(new { success = true, juriAdi = $"{user.FullName} ({user.TcKimlikNo})" });
            }

            return Json(new { success = false });
        }


        public async Task<IActionResult> JuriyeYonet(int basvuruId)
        {
            var basvuru = await _context.Basvurular
                .Include(b => b.Ilan)
                .Include(b => b.Aday)
                .FirstOrDefaultAsync(b => b.BasvuruId == basvuruId);

            if (basvuru == null || basvuru.Ilan == null)
            {
                TempData["Error"] = "Geçersiz başvuru.";
                return RedirectToAction("Basvurular");
            }

            var ilanJuriIdleri = await _context.JuriUyeleri
                .Where(j => j.IlanId == basvuru.IlanId)
                .Select(j => j.KullaniciJuriId)
                .ToListAsync();

            var atanmisJuriIdleri = await _context.BasvuruJuriAtamalari
                .Where(x => x.BasvuruId == basvuruId)
                .Select(x => x.JuriId)
                .ToListAsync();

            var juriUyeleri = await _userManager.Users
                .Where(u => ilanJuriIdleri.Contains(u.Id))
                .ToListAsync();

            ViewBag.Basvuru = basvuru;
            ViewBag.AtanabilirJuriListesi = juriUyeleri.Where(x => !atanmisJuriIdleri.Contains(x.Id)).ToList();
            ViewBag.AtanmisJuriListesi = juriUyeleri.Where(x => atanmisJuriIdleri.Contains(x.Id)).ToList();

            return View();
        }

        [HttpPost]
        public JsonResult JuriyeAtaAjax(int basvuruId, List<string> seciliJuriIdler)
        {
            var atananlar = new List<object>();

            foreach (var juriId in seciliJuriIdler)
            {
                if (!_context.BasvuruJuriAtamalari.Any(x => x.BasvuruId == basvuruId && x.JuriId == juriId))
                {
                    _context.BasvuruJuriAtamalari.Add(new BasvuruJuri { BasvuruId = basvuruId, JuriId = juriId });

                    var user = _context.Users.FirstOrDefault(u => u.Id == juriId);
                    if (user != null)
                    {
                        atananlar.Add(new { id = user.Id, adSoyad = $"{user.FullName} ({user.TcKimlikNo})" });
                    }
                }
            }

            _context.SaveChanges();
            return Json(new { success = true, atananlar });
        }

        [HttpPost]
        public JsonResult BasvuruJuriSil(int basvuruId, string juriId)
        {
            var kayit = _context.BasvuruJuriAtamalari
                .FirstOrDefault(x => x.BasvuruId == basvuruId && x.JuriId == juriId);

            if (kayit != null)
            {
                _context.BasvuruJuriAtamalari.Remove(kayit);
                _context.SaveChanges();

                var user = _context.Users.FirstOrDefault(u => u.Id == juriId);
                return Json(new { success = true, juriAdi = $"{user.FullName} ({user.TcKimlikNo})" });
            }

            return Json(new { success = false });
        }

        [HttpGet]
        public JsonResult GetBasvuruyaAtanmisJuriler(int basvuruId)
        {
            var juriIdler = _context.BasvuruJuriAtamalari
                .Where(x => x.BasvuruId == basvuruId)
                .Select(x => x.JuriId)
                .ToList();

            var juriler = _context.Users
                .Where(u => juriIdler.Contains(u.Id))
                .Select(u => new
                {
                    id = u.Id,
                    adSoyad = $"{u.FullName} ({u.TcKimlikNo})"
                })
                .ToList();

            return Json(juriler);
        }
    }
}
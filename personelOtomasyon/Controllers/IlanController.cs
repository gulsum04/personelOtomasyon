using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personelOtomasyon.Data;
using personelOtomasyon.Models;
using Microsoft.AspNetCore.Identity;

namespace personelOtomasyon.Controllers
{
    [Authorize(Roles = "Admin")]
    public class IlanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IlanController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // İlanları Listele
        public async Task<IActionResult> Index()
        {
            var ilanlar = await _context.AkademikIlanlar.ToListAsync();
            return View(ilanlar);
        }

        // Yeni İlan Formu (GET)
        public IActionResult Create()
        {
            return View();
        }

        // Yeni İlan Ekle (POST)
        [HttpPost]
        public async Task<IActionResult> Create(AkademikIlan ilan)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Tüm alanları doldurmalısınız.";
                return View(ilan);
            }

            var adminId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(adminId))
            {
                TempData["Error"] = "Giriş yapmış olmanız gerekiyor.";
                return View(ilan);
            }

            ilan.KullaniciAdminId = adminId;

            try
            {
                _context.Add(ilan);
                await _context.SaveChangesAsync();

                // 🔔 Aday ve jüriye bildirim gönder
                var hedefRoller = new[] { "Juri" };
                foreach (var rol in hedefRoller)
                {
                    var users = await _userManager.GetUsersInRoleAsync(rol);
                    foreach (var user in users)
                    {
                        _context.Bildirimler.Add(new Bildirim
                        {
                            KullaniciId = user.Id,
                            Mesaj = $"Yeni bir ilan yayınlandı: {ilan.Baslik}",
                            OkunduMu = false
                        });
                    }
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "İlan başarıyla eklendi ve bildirim gönderildi.";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Error"] = "Kayıt sırasında bir hata oluştu.";
                return View(ilan);
            }
        }

        // Güncelle (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var ilan = await _context.AkademikIlanlar.FindAsync(id);
            if (ilan == null) return NotFound();
            return View(ilan);
        }

        // Güncelle (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(int id, AkademikIlan ilan)
        {
            if (id != ilan.IlanId) return NotFound();
            if (!ModelState.IsValid) return View(ilan);

            ilan.KullaniciAdminId = _userManager.GetUserId(User);
            _context.Update(ilan);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Sil (GET)
        public async Task<IActionResult> Delete(int id)
        {
            var ilan = await _context.AkademikIlanlar.FindAsync(id);
            if (ilan == null) return NotFound();
            return View(ilan);
        }

        // Sil (POST)
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ilan = await _context.AkademikIlanlar
                .Include(i => i.Basvurular) // Bu satır ilişkili veriler için gerekli olabilir (eğer tanımlandıysa)
                .FirstOrDefaultAsync(i => i.IlanId == id);

            if (ilan == null)
            {
                TempData["Error"] = "İlan bulunamadı.";
                return RedirectToAction("Index");
            }

            try
            {
                _context.AkademikIlanlar.Remove(ilan);
                await _context.SaveChangesAsync();

                TempData["Success"] = "İlan ve ilgili başvurular başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Silme işlemi sırasında bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }

    }
}

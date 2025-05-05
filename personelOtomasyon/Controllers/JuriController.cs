using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personelOtomasyon.Data;
using personelOtomasyon.Data.ViewModels;
using personelOtomasyon.Models;

namespace personelOtomasyon.Controllers
{
    [Authorize(Roles = "Juri")]
    [Route("Juri")]
    public class JuriController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public JuriController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("")]
        public IActionResult Index() => View();

        [HttpGet("GelenBasvurular")]
        public IActionResult GelenBasvurular()
        {
            var userId = _userManager.GetUserId(User);
            var basvuruIdler = _context.BasvuruJuriAtamalari
                .Where(j => j.JuriId == userId)
                .Select(j => j.BasvuruId)
                .ToList();

            var basvurular = _context.Basvurular
                .Include(b => b.Ilan)
                .Include(b => b.Belgeler)
                .Include(b => b.Aday)
                .Where(b => basvuruIdler.Contains(b.BasvuruId))
                .Where(b => !_context.DegerlendirmeRaporlari
                    .Any(r => r.BasvuruId == b.BasvuruId && r.KullaniciJuriId == userId))
                .ToList();

            return View(basvurular);
        }

        [HttpGet("DegerlendirilenBasvurular")]
        public IActionResult DegerlendirilenBasvurular()
        {
            var userId = _userManager.GetUserId(User);
            var basvurular = _context.DegerlendirmeRaporlari
                .Include(r => r.Basvuru)
                    .ThenInclude(b => b.Ilan)
                .Include(r => r.Basvuru)
                    .ThenInclude(b => b.Aday)
                .Where(r => r.KullaniciJuriId == userId)
                .Select(r => r.Basvuru)
                .Distinct()
                .ToList();

            return View(basvurular);
        }

        [HttpGet("BasvuruDetay/{id}")]
        public IActionResult BasvuruDetay(int id, string kaynak = "")
        {
            var userId = _userManager.GetUserId(User);
            var yetkiliMi = _context.BasvuruJuriAtamalari.Any(j => j.BasvuruId == id && j.JuriId == userId);
            if (!yetkiliMi) return Forbid();

            var basvuru = _context.Basvurular
                .Include(b => b.Belgeler)
                .Include(b => b.Ilan)
                .Include(b => b.Aday)
                .FirstOrDefault(b => b.BasvuruId == id);

            if (basvuru == null) return NotFound();

            // İlgili jüriye ait değerlendirme raporunu çek
            var rapor = _context.DegerlendirmeRaporlari
                .FirstOrDefault(r => r.BasvuruId == id && r.KullaniciJuriId == userId);

            ViewBag.Kaynak = kaynak;
            ViewBag.Rapor = rapor;

            return View(basvuru);
        }
        [HttpGet("Degerlendir/{id}")]
        public IActionResult Degerlendir(int id)
        {
            var userId = _userManager.GetUserId(User);
            var yetkiliMi = _context.BasvuruJuriAtamalari.Any(j => j.BasvuruId == id && j.JuriId == userId);
            if (!yetkiliMi) return Forbid();

            var dahaOnceDegerlendirilmisMi = _context.DegerlendirmeRaporlari
                .Any(r => r.BasvuruId == id && r.KullaniciJuriId == userId);
            if (dahaOnceDegerlendirilmisMi)
            {
                TempData["Uyari"] = "Bu başvuru zaten değerlendirildi.";
                return RedirectToAction("DegerlendirilenBasvurular");
            }

            return View(new DegerlendirmeRaporuVM { BasvuruId = id });
        }

        [HttpPost("Degerlendir")]
        [ValidateAntiForgeryToken]
        public IActionResult DegerlendirPost(DegerlendirmeRaporuVM model)
        {
            var userId = _userManager.GetUserId(User);

            if (!ModelState.IsValid)
            {
                TempData["Sonuc"] = "Formda eksik alanlar var.";
                return View("Degerlendir", model);
            }

            var yetkiliMi = _context.BasvuruJuriAtamalari
                .Any(j => j.BasvuruId == model.BasvuruId && j.JuriId == userId);
            if (!yetkiliMi) return Forbid();

            var yeniRapor = new DegerlendirmeRaporu
            {
                BasvuruId = model.BasvuruId,
                RaporDosyasi = model.RaporDosyasi,
                Sonuc = model.Sonuc,
                KullaniciJuriId = userId
            };

            _context.DegerlendirmeRaporlari.Add(yeniRapor);
            _context.SaveChanges();

            return RedirectToAction("GelenBasvurular");
        }

        [HttpGet("DegerlendirmeGuncelle")]
        public IActionResult DegerlendirmeGuncelle(int id)
        {
            var userId = _userManager.GetUserId(User);
            var rapor = _context.DegerlendirmeRaporlari
                .FirstOrDefault(r => r.RaporId == id && r.KullaniciJuriId == userId);

            if (rapor == null) return Forbid();

            return View(new DegerlendirmeRaporuVM
            {
                RaporId = rapor.RaporId,
                BasvuruId = rapor.BasvuruId,
                Sonuc = rapor.Sonuc,
                RaporDosyasi = rapor.RaporDosyasi
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DegerlendirmeGuncelle")]
        public IActionResult DegerlendirmeGuncelle(DegerlendirmeRaporuVM guncelRapor)
        {
            var userId = _userManager.GetUserId(User);

            if (!ModelState.IsValid)
            {
                return View(guncelRapor);
            }

            var mevcutRapor = _context.DegerlendirmeRaporlari
                .FirstOrDefault(r => r.RaporId == guncelRapor.RaporId && r.KullaniciJuriId == userId);

            if (mevcutRapor == null)
                return Forbid();

            // 🔍 Değişiklik kontrolü
            if (mevcutRapor.Sonuc == guncelRapor.Sonuc && mevcutRapor.RaporDosyasi == guncelRapor.RaporDosyasi)
            {
                TempData["Uyari"] = "❗ Herhangi bir değişiklik yapılmadı.";
                return View(guncelRapor);
            }

            // Güncelleme işlemi
            mevcutRapor.Sonuc = guncelRapor.Sonuc;
            mevcutRapor.RaporDosyasi = guncelRapor.RaporDosyasi;
            _context.SaveChanges();

            TempData["GuncellemeMesaji"] = "✔ Değerlendirme başarıyla güncellendi.";
            return RedirectToAction("DegerlendirilenBasvurular");
        }
    }
}

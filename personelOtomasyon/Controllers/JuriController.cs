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

            // 1️⃣ Değerlendirme Raporu Kaydet
            var yeniRapor = new DegerlendirmeRaporu
            {
                BasvuruId = model.BasvuruId,
                RaporDosyasi = model.RaporDosyasi,
                Sonuc = model.Sonuc,
                KullaniciJuriId = userId
            };
            _context.DegerlendirmeRaporlari.Add(yeniRapor);

            // 2️⃣ Başvuruya Jüri sonucu kaydet
            var basvuru = _context.Basvurular.FirstOrDefault(b => b.BasvuruId == model.BasvuruId);
            if (basvuru != null)
            {
                basvuru.JuriSonucu = model.Sonuc;
                basvuru.JuriRaporu = model.RaporDosyasi;
                basvuru.DegerlendirmeTamamlandiMi = true;

                // Eğer jüri sonucu olumsuz ise, başvuru durumunu da değiştir
                if (model.Sonuc == "Olumsuz")
                {
                    basvuru.Durum = "Reddedildi";
                }

                _context.Basvurular.Update(basvuru);
            }

            _context.SaveChanges();

            TempData["Sonuc"] = "Başvuru değerlendirme işlemi tamamlandı.";
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

        [HttpPost("DegerlendirmeGuncelle")]
        [ValidateAntiForgeryToken]
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

            mevcutRapor.Sonuc = guncelRapor.Sonuc;
            mevcutRapor.RaporDosyasi = guncelRapor.RaporDosyasi;

            _context.SaveChanges();

            TempData["GuncellemeMesaji"] = "✔ Değerlendirme başarıyla güncellendi.";
            return RedirectToAction("DegerlendirilenBasvurular");
        }
    }
}

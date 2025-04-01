using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personelOtomasyon.Data;
using personelOtomasyon.Models;

namespace personelOtomasyon.Controllers
{
    [Authorize(Roles = "Juri")]
    public class JuriController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public JuriController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 🎓 Jüri Ana Paneli
        public IActionResult Index()
        {
            return View();
        }

        // 📥 Gelen Başvurular (Sadece jüriye atanmış ve henüz değerlendirilmemiş başvurular)
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

        // ✅ Daha Önce Değerlendirilmiş Başvurular
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

        // 📌 Detay Sayfası (Sadece yetkili jüri erişebilir)
        public IActionResult BasvuruDetay(int id)
        {
            var userId = _userManager.GetUserId(User);
            var yetkiliMi = _context.BasvuruJuriAtamalari.Any(j => j.BasvuruId == id && j.JuriId == userId);

            if (!yetkiliMi)
                return Forbid();

            var basvuru = _context.Basvurular
                .Include(b => b.Belgeler)
                .Include(b => b.Ilan)
                .Include(b => b.Aday)
                .FirstOrDefault(b => b.BasvuruId == id);

            if (basvuru == null)
                return NotFound();

            return View(basvuru);
        }

        // 📝 Değerlendirme Formu (GET)
        [HttpGet]
        public IActionResult Degerlendir(int id)
        {
            var userId = _userManager.GetUserId(User);

            var yetkiliMi = _context.BasvuruJuriAtamalari.Any(j => j.BasvuruId == id && j.JuriId == userId);
            if (!yetkiliMi)
                return Forbid();

            var dahaOnceDegerlendirilmisMi = _context.DegerlendirmeRaporlari.Any(r => r.BasvuruId == id && r.KullaniciJuriId == userId);
            if (dahaOnceDegerlendirilmisMi)
            {
                TempData["Uyari"] = "Bu başvuru zaten değerlendirildi. Güncellemek için 'Değerlendirilen Başvurular' sayfasına gidin.";
                return RedirectToAction("DegerlendirilenBasvurular");
            }

            var rapor = new DegerlendirmeRaporu { BasvuruId = id };
            return View(rapor);
        }

        // 📨 Değerlendirme Gönderme (POST)
        [HttpPost]
        public IActionResult Degerlendir(DegerlendirmeRaporu rapor)
        {
            var userId = _userManager.GetUserId(User);

            var yetkiliMi = _context.BasvuruJuriAtamalari.Any(j => j.BasvuruId == rapor.BasvuruId && j.JuriId == userId);
            if (!yetkiliMi)
                return Forbid();

            if (ModelState.IsValid)
            {
                rapor.KullaniciJuriId = userId;
                rapor.RaporDosyasi = ""; // Dosya özelliği eklenecekse burada set edilir
                _context.DegerlendirmeRaporlari.Add(rapor);
                _context.SaveChanges();

                return RedirectToAction("GelenBasvurular");
            }

            return View(rapor);
        }

        // 🛠️ Değerlendirme Güncelle (GET)
        [HttpGet]
        public IActionResult DegerlendirmeGuncelle(int id)
        {
            var userId = _userManager.GetUserId(User);
            var rapor = _context.DegerlendirmeRaporlari
                .FirstOrDefault(r => r.BasvuruId == id && r.KullaniciJuriId == userId);

            if (rapor == null)
                return Forbid();

            return View(rapor);
        }

        // 🔄 Güncellenmiş Değerlendirmeyi Kaydet (POST)
        [HttpPost]
        public IActionResult DegerlendirmeGuncelle(DegerlendirmeRaporu guncelRapor)
        {
            var userId = _userManager.GetUserId(User);

            var mevcutRapor = _context.DegerlendirmeRaporlari
                .FirstOrDefault(r => r.RaporId == guncelRapor.RaporId && r.KullaniciJuriId == userId);

            if (mevcutRapor == null)
                return Forbid();

            if (!ModelState.IsValid)
                return View(guncelRapor);

            mevcutRapor.Sonuc = guncelRapor.Sonuc;
            mevcutRapor.RaporDosyasi = guncelRapor.RaporDosyasi;

            _context.SaveChanges();

            return RedirectToAction("DegerlendirilenBasvurular");
        }
    }
}

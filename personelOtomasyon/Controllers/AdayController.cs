using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personelOtomasyon.Data;
using personelOtomasyon.Models;
using Microsoft.AspNetCore.Identity;

namespace personelOtomasyon.Controllers
{
    [Authorize(Roles = "User,Yonetici")]
    public class AdayController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdayController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Yayındaki ilanlar
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user?.Id;

            var ilanlar = await _context.AkademikIlanlar
                .Where(i => i.Yayinda)
                .Include(i => i.KadroKriterleri)
                .ToListAsync();

            foreach (var ilan in ilanlar)
            {
                var basvuru = await _context.Basvurular
                    .FirstOrDefaultAsync(b => b.IlanId == ilan.IlanId && b.KullaniciAdayId == userId);

                ViewData["BasvuruDurumu" + ilan.IlanId] = basvuru != null ? "Başvuruldu" : "Başvurulmadı";
            }

            return View(ilanlar);
        }

        // İlan detay
        public async Task<IActionResult> IlanDetay(int id)
        {
            var ilan = await _context.AkademikIlanlar
                .Include(i => i.KadroKriterleri)
                    .ThenInclude(k => k.AltBelgeTurleri)
                .FirstOrDefaultAsync(i => i.IlanId == id);

            if (ilan == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            bool basvurduMu = false;

            if (user != null)
            {
                basvurduMu = await _context.Basvurular
                    .AnyAsync(b => b.IlanId == id && b.KullaniciAdayId == user.Id);
            }

            ViewBag.BasvurduMu = basvurduMu;

            return View(ilan);
        }

        // Başvuru yap sayfası
        public async Task<IActionResult> Basvur(int id)
        {
            var ilan = await _context.AkademikIlanlar
                .Include(i => i.KadroKriterleri)
                    .ThenInclude(k => k.AltBelgeTurleri)
                .FirstOrDefaultAsync(i => i.IlanId == id && i.Yayinda);

            if (ilan == null || ilan.KadroKriterleri == null || !ilan.KadroKriterleri.Any())
            {
                TempData["Error"] = "Bu ilana başvuru yapılamaz.";
                return RedirectToAction("Index");
            }

            return View(ilan);
        }

        // Başvuru yap post
        [HttpPost]
        public async Task<IActionResult> BasvuruYap(int ilanId, List<IFormFile> belgeler, List<string> belgeTurleri)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var ilan = await _context.AkademikIlanlar
                .Include(i => i.KadroKriterleri)
                    .ThenInclude(k => k.AltBelgeTurleri)
                .FirstOrDefaultAsync(i => i.IlanId == ilanId);

            if (ilan == null)
            {
                TempData["Error"] = "İlan bulunamadı.";
                return RedirectToAction("Index");
            }

            int toplamGerekliBelge = 0;
            foreach (var kriter in ilan.KadroKriterleri)
            {
                if (kriter.BelgeYuklenecekMi || kriter.ZorunluMu)
                {
                    if (kriter.AltBelgeTurleri != null && kriter.AltBelgeTurleri.Any())
                        toplamGerekliBelge += kriter.AltBelgeTurleri.Sum(a => a.BelgeSayisi);
                    else
                        toplamGerekliBelge += 1;
                }
            }

            if (belgeler.Count < toplamGerekliBelge)
            {
                TempData["Error"] = $"Eksik belge yüklemesi. Yüklemeniz gereken belge sayısı: {toplamGerekliBelge}.";
                return RedirectToAction("Basvur", new { id = ilanId });
            }

            var basvuru = new Basvuru
            {
                IlanId = ilanId,
                KullaniciAdayId = user.Id,
                BasvuruTarihi = DateTime.Now,
                Durum = "Beklemede"
            };

            _context.Basvurular.Add(basvuru);
            await _context.SaveChangesAsync();

            for (int i = 0; i < belgeler.Count; i++)
            {
                var belge = belgeler[i];
                var belgeTuru = belgeTurleri[i];

                if (belge != null && belge.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = Path.GetFileName(belge.FileName);
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await belge.CopyToAsync(stream);
                    }

                    _context.BasvuruBelgeleri.Add(new BasvuruBelge
                    {
                        BasvuruId = basvuru.BasvuruId,
                        BelgeTuru = belgeTuru,
                        DosyaYolu = "/uploads/" + uniqueFileName
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Başvurunuz başarıyla alınmıştır!";
            return RedirectToAction("Basvurularim");
        }

        // Başvuru Detay - düzenlendi
        public async Task<IActionResult> BasvuruDetay(int id)
        {
            var basvuru = await _context.Basvurular
                .Include(b => b.Ilan)
                    .ThenInclude(i => i.KadroKriterleri)
                .Include(b => b.Belgeler)
                .Include(b => b.Aday) //OLMUYOR
                .Include(b => b.DegerlendirmeRaporlari)
                    .ThenInclude(r => r.Juri)
                .FirstOrDefaultAsync(b => b.BasvuruId == id);

            if (basvuru == null)
            {
                TempData["Error"] = "Başvuru bulunamadı.";
                return RedirectToAction("Index");
            }

            return View(basvuru);
        }


        // Adayın yaptığı başvurular
        public async Task<IActionResult> Basvurularim()
        {
            var user = await _userManager.GetUserAsync(User);

            var basvurular = await _context.Basvurular
                .Include(b => b.Ilan)
                .Where(b => b.KullaniciAdayId == user.Id)
                .ToListAsync();

            return View(basvurular);
        }

        // Başvuru Sil
        public async Task<IActionResult> BasvuruSil(int id)
        {
            var basvuru = await _context.Basvurular
                .Include(b => b.Ilan)
                .FirstOrDefaultAsync(b => b.BasvuruId == id);

            if (basvuru == null)
            {
                TempData["Error"] = "Başvuru bulunamadı.";
                return RedirectToAction("Basvurularim");
            }

            _context.Basvurular.Remove(basvuru);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Başvuru başarıyla silindi.";
            return RedirectToAction("Basvurularim");
        }
    }
}

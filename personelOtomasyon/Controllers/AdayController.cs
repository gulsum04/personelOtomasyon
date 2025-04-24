using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personelOtomasyon.Data;
using personelOtomasyon.Models;
using Microsoft.AspNetCore.Identity;

namespace personelOtomasyon.Controllers
{
    [Authorize(Roles = "User")]
    public class AdayController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdayController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Başvuru Detay Sayfası
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User); // Kullanıcı bilgilerini alıyoruz
            var userId = user?.Id; // Kullanıcı ID'sini alıyoruz

            // Yayında olan ilanları alıyoruz
            var ilanlar = await _context.AkademikIlanlar
                .Where(i => i.Yayinda) // Yayında olan ilanları filtreliyoruz
                .Include(i => i.KadroKriterleri) // Kadro kriterlerini dahil ediyoruz
                .ToListAsync();

            // Başvuru durumu ekliyoruz
            foreach (var ilan in ilanlar)
            {
                var basvuru = await _context.Basvurular
                    .FirstOrDefaultAsync(b => b.IlanId == ilan.IlanId && b.KullaniciAdayId == userId);

                // Başvuru yapılmışsa "Başvuruldu", yapılmamışsa "Başvurulmadı"
                ViewData["BasvuruDurumu" + ilan.IlanId] = basvuru != null ? "Başvuruldu" : "Başvurulmadı";
            }

            return View(ilanlar); // Veriyi view'a gönderiyoruz
        }



        public async Task<IActionResult> Basvur(int id)
        {
            var ilan = await _context.AkademikIlanlar
                .Include(i => i.KadroKriterleri)
                    .ThenInclude(k => k.AltBelgeTurleri) // ✅ Alt belge türleri de dahil ediliyor
                .FirstOrDefaultAsync(i => i.IlanId == id && i.Yayinda);

            if (ilan == null || ilan.KadroKriterleri == null || !ilan.KadroKriterleri.Any())
            {
                TempData["Error"] = "Bu ilana başvuru yapılamaz.";
                return RedirectToAction("Index");
            }

            return View(ilan);
        }


        // Başvuru işlemi
        [HttpPost]
        public async Task<IActionResult> BasvuruYap(int ilanId, List<IFormFile> belgeler, List<string> belgeTurleri)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

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
                    var dosyaAdi = Path.GetFileName(belge.FileName);
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    if (!Directory.Exists(uploads))
                        Directory.CreateDirectory(uploads);

                    var filePath = Path.Combine(uploads, dosyaAdi);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await belge.CopyToAsync(stream);
                    }

                    _context.BasvuruBelgeleri.Add(new BasvuruBelge
                    {
                        BasvuruId = basvuru.BasvuruId,
                        BelgeTuru = belgeTuru,
                        DosyaYolu = "/uploads/" + dosyaAdi
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Başvurunuz alındı.";
            return RedirectToAction("Basvurularim");
        }


        public async Task<IActionResult> BasvuruDetay(int id)
        {
            var basvuru = await _context.Basvurular
                .Include(b => b.Ilan)
                .ThenInclude(i => i.KadroKriterleri)
                .Include(b => b.Belgeler)
                .FirstOrDefaultAsync(b => b.BasvuruId == id);

            if (basvuru == null)
            {
                TempData["Error"] = "Başvuru bulunamadı.";
                return RedirectToAction("Index"); // Geri yönlendirme
            }

            return View(basvuru);
        }



        // Adayın kendi başvurularını listelemesi
        public async Task<IActionResult> Basvurularim()
        {
            var user = await _userManager.GetUserAsync(User);
            var basvurular = await _context.Basvurular
                .Include(b => b.Ilan)
                .Where(b => b.KullaniciAdayId == user.Id)
                .ToListAsync();

            return View(basvurular);
        }

        public async Task<IActionResult> BasvuruSil(int id)
        {
            // Başvuruyu veritabanından al
            var basvuru = await _context.Basvurular
                .Include(b => b.Ilan)
                .FirstOrDefaultAsync(b => b.BasvuruId == id);

            // Başvuru bulunamazsa hata mesajı döndür
            if (basvuru == null)
            {
                TempData["Error"] = "Başvuru bulunamadı.";
                return RedirectToAction("Basvurularim");
            }

            // Başvuruyu veritabanından sil
            _context.Basvurular.Remove(basvuru);
            await _context.SaveChangesAsync();

            // Başarı mesajı ekle ve başvurularım sayfasına yönlendir
            TempData["Success"] = "Başvuru başarıyla silindi.";
            return RedirectToAction("Basvurular");
         }

    }
    }

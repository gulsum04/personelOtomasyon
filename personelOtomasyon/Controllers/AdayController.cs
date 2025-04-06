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
        public async Task<IActionResult> BasvuruDetay(int id)
        {
            var ilan = await _context.AkademikIlanlar.FirstOrDefaultAsync(i => i.IlanId == id);
            if (ilan == null) return NotFound();

            return View(ilan);
        }

        // Tüm ilanları listele
        public async Task<IActionResult> Index()
        {
            var ilanlar = await _context.AkademikIlanlar.ToListAsync();
            return View(ilanlar);
        }

        // Basvur sayfası
        public async Task<IActionResult> Basvur(int id)
        {
            var ilan = await _context.AkademikIlanlar.FirstOrDefaultAsync(i => i.IlanId == id);
            if (ilan == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var dahaOnceBasvurmusMu = await _context.Basvurular.AnyAsync(b => b.IlanId == id && b.KullaniciAdayId == user.Id);

            if (dahaOnceBasvurmusMu)
            {
                TempData["Uyari"] = "❗ Bu ilana zaten başvurdunuz.";
                return RedirectToAction("Index");
            }

            return View(ilan);
        }
        // Başvuru işlemi
        [HttpPost]
        public async Task<IActionResult> BasvuruYap(int ilanId, string belgeTuru, IFormFile belge)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Başvuru oluştur
            var basvuru = new Basvuru
            {
                IlanId = ilanId,
                KullaniciAdayId = user.Id,
                BasvuruTarihi = DateTime.Now,
                Durum = "Beklemede"
            };

            _context.Basvurular.Add(basvuru);
            await _context.SaveChangesAsync();

            // Belgeyi yükle
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

                var basvuruBelge = new BasvuruBelge
                {
                    BasvuruId = basvuru.BasvuruId,
                    BelgeTuru = belgeTuru,
                    DosyaYolu = "/uploads/" + dosyaAdi
                };

                _context.BasvuruBelgeleri.Add(basvuruBelge);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Basvurularim");
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
    }
}

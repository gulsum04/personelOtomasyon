using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personelOtomasyon.Data;
using personelOtomasyon.Models;
using System.Text.Json;
using System.Linq;

namespace personelOtomasyon.Controllers
{
    [Authorize(Roles = "Yonetici")]
    public class YoneticiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public YoneticiController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 📋 Yönetici Paneli Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var ilanlar = await _context.AkademikIlanlar
                .Include(i => i.KadroKriterleri)
                .ThenInclude(k => k.AltBelgeTurleri)
                .ToListAsync();

            return View(ilanlar);
        }

        // 📜 Tüm İlan Listesi
        public async Task<IActionResult> IlanListesi()
        {
            var ilanlar = await _context.AkademikIlanlar
                .Include(i => i.KadroKriterleri)
                .ThenInclude(k => k.AltBelgeTurleri)
                .ToListAsync();

            return View(ilanlar);
        }

        // 📢 Yayınlanan İlanlar
        public async Task<IActionResult> YayinlananIlanlar()
        {
            var yayinlanmisIlanlar = await _context.AkademikIlanlar
                .Where(i => i.Yayinda == true)
                .ToListAsync();

            return View(yayinlanmisIlanlar);
        }

        // 🛑 İlan Yayından Kaldır
        public async Task<IActionResult> IlanYayindanKaldir(int id)
        {
            var ilan = await _context.AkademikIlanlar.FindAsync(id);
            if (ilan == null)
            {
                TempData["Error"] = "İlan bulunamadı.";
                return RedirectToAction("YayinlananIlanlar");
            }

            ilan.Yayinda = false;
            await _context.SaveChangesAsync();

            TempData["Success"] = "İlan yayından kaldırıldı.";
            return RedirectToAction("YayinlananIlanlar");
        }

        // ✅ İlanı Yayına Al
        public async Task<IActionResult> IlanYayinla(int id)
        {
            var ilan = await _context.AkademikIlanlar.FindAsync(id);
            if (ilan == null)
            {
                TempData["Error"] = "İlan bulunamadı.";
                return RedirectToAction("IlanListesi");
            }

            ilan.Yayinda = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = "İlan yayına alındı.";
            return RedirectToAction("IlanListesi");
        }

        // 🧩 Kriter Belirle (GET)
        [HttpGet]
        public async Task<IActionResult> KriterBelirle(int id)
        {
            var ilan = await _context.AkademikIlanlar
                .Include(i => i.KadroKriterleri)
                .ThenInclude(k => k.AltBelgeTurleri)
                .FirstOrDefaultAsync(i => i.IlanId == id);

            if (ilan == null)
            {
                TempData["Error"] = "İlan bulunamadı.";
                return RedirectToAction("IlanListesi");
            }

            return View(ilan);
        }

        // 🧩 Kriter Belirle (POST)
        [HttpPost]
        public async Task<IActionResult> KriterBelirle(
            int ilanId,
            string TemelAlan,
            string Unvan,
            List<string> KriterAdlari,
            List<string> Aciklamalar,
            List<string> Zorunluluklar,
            List<string> BelgeYuklenebilirlik,
            List<string> Secilenler,
            List<int> BelgeSayilari,
            string AltBelgeJson)
        {
            var user = await _userManager.GetUserAsync(User);

            var ilan = await _context.AkademikIlanlar
                .Include(i => i.KadroKriterleri)
                .ThenInclude(k => k.AltBelgeTurleri)
                .FirstOrDefaultAsync(i => i.IlanId == ilanId);

            if (ilan == null)
            {
                TempData["Error"] = "İlan bulunamadı.";
                return RedirectToAction("IlanListesi");
            }

            _context.KadroKriterAltlar.RemoveRange(ilan.KadroKriterleri.SelectMany(k => k.AltBelgeTurleri));
            _context.KadroKriterleri.RemoveRange(ilan.KadroKriterleri);
            await _context.SaveChangesAsync();

            var altBelgeListesi = new List<KadroKriterAltDTO>();
            if (!string.IsNullOrEmpty(AltBelgeJson))
            {
                altBelgeListesi = JsonSerializer.Deserialize<List<KadroKriterAltDTO>>(AltBelgeJson);
            }

            var yeniKriterler = new List<KadroKriteri>();

            for (int i = 0; i < KriterAdlari.Count; i++)
            {
                if (!Secilenler.Contains(i.ToString()))
                    continue;

                bool zorunluMu = Zorunluluklar.Contains(i.ToString());
                bool belgeYuklenecekMi = BelgeYuklenebilirlik.Contains(i.ToString());

                if (zorunluMu)
                    belgeYuklenecekMi = true;

                var kriter = new KadroKriteri
                {
                    IlanId = ilanId,
                    KriterAdi = KriterAdlari[i],
                    Aciklama = Aciklamalar.Count > i ? Aciklamalar[i] : "Açıklama belirtilmedi",
                    ZorunluMu = zorunluMu,
                    BelgeYuklenecekMi = belgeYuklenecekMi,
                    BelgeSayisi = BelgeSayilari.Count > i ? BelgeSayilari[i] : 0,
                    TemelAlan = TemelAlan,
                    Unvan = Unvan,
                    KullaniciYoneticiId = user.Id,
                    AltBelgeTurleri = new List<KadroKriterAlt>()
                };

                var altlar = altBelgeListesi.Where(x => x.KriterIndex == i).ToList();
                foreach (var alt in altlar)
                {
                    kriter.AltBelgeTurleri.Add(new KadroKriterAlt
                    {
                        BelgeTuru = alt.BelgeTuru,
                        BelgeSayisi = alt.BelgeSayisi,
                        Kriter = kriter
                    });
                }

                yeniKriterler.Add(kriter);
            }

            if (yeniKriterler.Any())
            {
                await _context.KadroKriterleri.AddRangeAsync(yeniKriterler);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Kriterler başarıyla kaydedildi.";
            }
            else
            {
                TempData["Error"] = "Herhangi bir kriter seçilmedi.";
            }

            return RedirectToAction("KriterBelirle", new { id = ilanId });
        }

        // 📄 Başvuruları Listele
        public IActionResult Basvurular()
        {
            var basvurular = _context.Basvurular
                .Include(b => b.Ilan)
                .Include(b => b.Aday)
                .Include(b => b.DegerlendirmeRaporlari)
                    .ThenInclude(r => r.Juri)
                .Where(b => b.DegerlendirmeRaporlari.Count == 1) // Sadece 5 jüri değerlendirmişse
                .ToList();

            return View(basvurular);
        }

        // 🔍 Jüri Değerlendirmelerini İncele
        [HttpGet]
        public IActionResult JuriDegerlendirmeIncele(int id)
        {
            var basvuru = _context.Basvurular
                .Include(b => b.DegerlendirmeRaporlari)
                    .ThenInclude(r => r.Juri)
                .Include(b => b.Ilan)
                .FirstOrDefault(b => b.BasvuruId == id);

            if (basvuru == null)
                return NotFound();

            return View(basvuru);
        }


        // ✅ Final Karar Ver
        [HttpPost]
        public IActionResult FinalKararVer(int id, string karar)
        {
            var basvuru = _context.Basvurular
                .FirstOrDefault(b => b.BasvuruId == id);

            if (basvuru == null)
            {
                TempData["Error"] = "Başvuru bulunamadı.";
                return RedirectToAction("Basvurular");
            }

            if (karar == "Olumlu")
            {
                basvuru.JuriSonucu = "Olumlu";
                basvuru.Durum = "Onaylandı";
            }
            else if (karar == "Olumsuz")
            {
                basvuru.JuriSonucu = "Olumsuz";
                basvuru.Durum = "Reddedildi";
            }

            basvuru.DegerlendirmeTamamlandiMi = true;

            _context.Update(basvuru);
            _context.SaveChanges();

            TempData["Success"] = "Nihai karar başarıyla verildi.";
            return RedirectToAction("Basvurular");
        }

        // 📜 Belge Puanla (GET)
        [HttpGet]
        public IActionResult BelgePuanla(int basvuruId)
        {
            var basvuru = _context.Basvurular
                .Include(b => b.Ilan)
                    .ThenInclude(i => i.KadroKriterleri)
                        .ThenInclude(k => k.AltBelgeTurleri)
                .FirstOrDefault(b => b.BasvuruId == basvuruId);

            if (basvuru == null)
                return NotFound();

            ViewBag.BasvuruId = basvuruId;
            ViewBag.IlanBasligi = basvuru.Ilan.Baslik;
            ViewBag.JuriSonucu = basvuru.JuriSonucu; 

            var altKriterler = basvuru.Ilan.KadroKriterleri
                .SelectMany(k => k.AltBelgeTurleri)
                .ToList();

            return View(altKriterler);
        }




        // 📥 Belge Puanla Kaydet (POST)
        [HttpPost]
        public async Task<IActionResult> BelgePuanlaKaydet(int basvuruId, List<string> kriterAdlari, List<string> aciklamalar, List<int> puanlar)
        {
            var user = await _userManager.GetUserAsync(User);

            for (int i = 0; i < kriterAdlari.Count; i++)
            {
                var puanKaydi = new BasvuruPuan
                {
                    BasvuruId = basvuruId,
                    BelgeTuru = kriterAdlari[i],
                    FaaliyetAdi = aciklamalar[i],
                    Puan = puanlar[i],
                    YoneticiId = user.Id
                };

                _context.BasvuruPuanlar.Add(puanKaydi);
            }

            await _context.SaveChangesAsync();

            // Toplam puanı güncelle
            var toplamPuan = _context.BasvuruPuanlar
                .Where(x => x.BasvuruId == basvuruId)
                .Sum(x => x.Puan);

            var basvuru = await _context.Basvurular.FindAsync(basvuruId);
            if (basvuru != null)
            {
                basvuru.ToplamPuan = toplamPuan;
                _context.Basvurular.Update(basvuru);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Puanlar başarıyla kaydedildi.";
            return RedirectToAction("Basvurular");
        }


        // DTO (Alt Belge için)
        private class KadroKriterAltDTO
        {
            public int KriterIndex { get; set; }
            public string BelgeTuru { get; set; }
            public int BelgeSayisi { get; set; }
        }
    }
}

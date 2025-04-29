using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personelOtomasyon.Data;
using personelOtomasyon.Models;
using System.Text.Json;

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
                .Where(i => i.Yayinda)
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
                return RedirectToAction(nameof(YayinlananIlanlar));
            }

            ilan.Yayinda = false;
            await _context.SaveChangesAsync();

            TempData["Success"] = "İlan yayından kaldırıldı.";
            return RedirectToAction(nameof(YayinlananIlanlar));
        }

        // ✅ İlanı Yayına Al
        public async Task<IActionResult> IlanYayinla(int id)
        {
            var ilan = await _context.AkademikIlanlar.FindAsync(id);
            if (ilan == null)
            {
                TempData["Error"] = "İlan bulunamadı.";
                return RedirectToAction(nameof(IlanListesi));
            }

            ilan.Yayinda = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = "İlan yayına alındı.";
            return RedirectToAction(nameof(IlanListesi));
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
                return RedirectToAction(nameof(IlanListesi));
            }

            return View(ilan);
        }

        // 🧩 Kriter Belirle (POST)
        [HttpPost]
        public async Task<IActionResult> KriterBelirle(int ilanId, string TemelAlan, string Unvan,
            List<string> KriterAdlari, List<string> Aciklamalar, List<string> Zorunluluklar,
            List<string> BelgeYuklenebilirlik, List<string> Secilenler, List<int> BelgeSayilari, string AltBelgeJson)
        {
            var user = await _userManager.GetUserAsync(User);

            var ilan = await _context.AkademikIlanlar
                .Include(i => i.KadroKriterleri)
                .ThenInclude(k => k.AltBelgeTurleri)
                .FirstOrDefaultAsync(i => i.IlanId == ilanId);

            if (ilan == null)
            {
                TempData["Error"] = "İlan bulunamadı.";
                return RedirectToAction(nameof(IlanListesi));
            }

            _context.KadroKriterAltlar.RemoveRange(ilan.KadroKriterleri.SelectMany(k => k.AltBelgeTurleri));
            _context.KadroKriterleri.RemoveRange(ilan.KadroKriterleri);
            await _context.SaveChangesAsync();

            var altBelgeListesi = string.IsNullOrEmpty(AltBelgeJson)
                ? new List<KadroKriterAltDTO>()
                : JsonSerializer.Deserialize<List<KadroKriterAltDTO>>(AltBelgeJson);

            var yeniKriterler = new List<KadroKriteri>();

            for (int i = 0; i < KriterAdlari.Count; i++)
            {
                if (!Secilenler.Contains(i.ToString()))
                    continue;

                var kriter = new KadroKriteri
                {
                    IlanId = ilanId,
                    KriterAdi = KriterAdlari[i],
                    Aciklama = Aciklamalar.Count > i ? Aciklamalar[i] : "Açıklama belirtilmedi",
                    ZorunluMu = Zorunluluklar.Contains(i.ToString()),
                    BelgeYuklenecekMi = BelgeYuklenebilirlik.Contains(i.ToString()),
                    BelgeSayisi = BelgeSayilari.Count > i ? BelgeSayilari[i] : 0,
                    TemelAlan = TemelAlan,
                    Unvan = Unvan,
                    KullaniciYoneticiId = user.Id,
                    AltBelgeTurleri = altBelgeListesi.Where(x => x.KriterIndex == i)
                        .Select(x => new KadroKriterAlt { BelgeTuru = x.BelgeTuru, BelgeSayisi = x.BelgeSayisi })
                        .ToList()
                };

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

            return RedirectToAction(nameof(KriterBelirle), new { id = ilanId });
        }

        // 📄 Başvuruları Listele
        public IActionResult Basvurular()
        {
            var basvurular = _context.Basvurular
                .Include(b => b.Ilan)
                .Include(b => b.Aday)
                .Include(b => b.DegerlendirmeRaporlari)
                    .ThenInclude(r => r.Juri)
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

        // ✅ Yönetici Final Kararı
        [HttpPost]
        public IActionResult FinalKararVer(int id, string karar)
        {
            var basvuru = _context.Basvurular.FirstOrDefault(b => b.BasvuruId == id);

            if (basvuru == null)
            {
                TempData["Error"] = "Başvuru bulunamadı.";
                return RedirectToAction(nameof(Basvurular));
            }

            if (karar == "Olumlu")
            {
                basvuru.YoneticiSonucu = "Olumlu";
                basvuru.Durum = "Onaylandı";
            }
            else if (karar == "Olumsuz")
            {
                basvuru.YoneticiSonucu = "Olumsuz";
                basvuru.Durum = "Reddedildi";
            }

            basvuru.DegerlendirmeTamamlandiMi = true;

            _context.Update(basvuru);
            _context.SaveChanges();

            TempData["Success"] = "Nihai karar başarıyla verildi.";
            return RedirectToAction(nameof(Basvurular));
        }

        // 📥 Belge Puanla (GET)
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

        [HttpPost]
        public async Task<IActionResult> BelgePuanlaKaydet(int basvuruId, List<string> kriterAdlari, List<string> aciklamalar, List<int> puanlar, string karar)
        {
            var user = await _userManager.GetUserAsync(User);

            // Eğer kriter listesi boş değilse belge puanlarını kaydet
            if (kriterAdlari != null && kriterAdlari.Any())
            {
                for (int i = 0; i < kriterAdlari.Count; i++)
                {
                    var puanKaydi = new BasvuruPuan
                    {
                        BasvuruId = basvuruId,
                        BelgeTuru = kriterAdlari[i],
                        FaaliyetAdi = (aciklamalar.Count > i) ? aciklamalar[i] : "", // Güvenli okuma ✅
                        Puan = (puanlar.Count > i) ? puanlar[i] : 0,
                        YoneticiId = user.Id
                    };

                    _context.BasvuruPuanlar.Add(puanKaydi);
                }

                await _context.SaveChangesAsync();
            }

            // ➡️ Toplam puanı hesapla (puan kaydedildiyse)
            var toplamPuan = _context.BasvuruPuanlar
                .Where(x => x.BasvuruId == basvuruId)
                .Sum(x => x.Puan);

            var basvuru = await _context.Basvurular.FindAsync(basvuruId);
            if (basvuru != null)
            {
                basvuru.ToplamPuan = toplamPuan;

                if (karar == "Onayla")
                {
                    basvuru.YoneticiSonucu = "Olumlu";
                    basvuru.Durum = "Onaylandı";
                }
                else if (karar == "Reddet")
                {
                    basvuru.YoneticiSonucu = "Olumsuz";
                    basvuru.Durum = "Reddedildi";
                }

                basvuru.DegerlendirmeTamamlandiMi = true;

                _context.Basvurular.Update(basvuru);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Puanlar ve karar başarıyla kaydedildi.";
            return RedirectToAction(nameof(Basvurular));
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

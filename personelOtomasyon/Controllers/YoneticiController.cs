using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personelOtomasyon.Data;
using personelOtomasyon.Models;
using System.Security.Claims;
using System.Text.Json;

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

    public async Task<IActionResult> IlanListesi()
    {
        var ilanlar = await _context.AkademikIlanlar
            .Include(i => i.KadroKriterleri)
            .ThenInclude(k => k.AltBelgeTurleri)
            .ToListAsync();

        return View(ilanlar);
    }

    public async Task<IActionResult> Dashboard()
    {
        var ilanlar = await _context.AkademikIlanlar
            .Include(i => i.KadroKriterleri)
            .ThenInclude(k => k.AltBelgeTurleri)
            .ToListAsync();

        return View(ilanlar);
    }

    // ✅ YAYINLANAN ILANLAR SAYFASI EKLENDİ
    public async Task<IActionResult> YayinlananIlanlar()
    {
        var yayinlanmisIlanlar = await _context.AkademikIlanlar
            .Where(i => i.Yayinda == true)
            .ToListAsync();

        return View(yayinlanmisIlanlar);
    }

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
    string AltBelgeJson
)
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

        // Önce eskileri sil
        _context.KadroKriterAltlar.RemoveRange(ilan.KadroKriterleri.SelectMany(k => k.AltBelgeTurleri));
        _context.KadroKriterleri.RemoveRange(ilan.KadroKriterleri);
        await _context.SaveChangesAsync();

        // Alt belge türlerini deserialize et
        var altBelgeListesi = new List<KadroKriterAltDTO>();
        if (!string.IsNullOrEmpty(AltBelgeJson))
        {
            altBelgeListesi = JsonSerializer.Deserialize<List<KadroKriterAltDTO>>(AltBelgeJson);
        }

        var yeniKriterler = new List<KadroKriteri>();

        for (int i = 0; i < KriterAdlari.Count; i++)
        {
            // Checkbox işaretlenmişse devam et
            //if (Secilenler.Count <= i || Secilenler[i]?.ToLower() != "true")
            //continue;

            if (!Secilenler.Contains(i.ToString()))
                continue;

            bool zorunluMu = Zorunluluklar.Contains(i.ToString());
            bool belgeYuklenecekMi = BelgeYuklenebilirlik.Contains(i.ToString());
            // Eğer zorunluysa, belge yüklenmesi de gerekir
            if (zorunluMu)
                belgeYuklenecekMi = true;

            var kriter = new KadroKriteri
            {
                IlanId = ilanId,
                KriterAdi = KriterAdlari[i],
                Aciklama = Aciklamalar.Count > i ? Aciklamalar[i] : "",
                ZorunluMu = zorunluMu,
                BelgeYuklenecekMi = belgeYuklenecekMi,
                BelgeSayisi = BelgeSayilari.Count > i ? BelgeSayilari[i] : 0,
                TemelAlan = TemelAlan,
                Unvan = Unvan,
                KullaniciYoneticiId = user.Id,
                AltBelgeTurleri = new List<KadroKriterAlt>()
            };

            // Alt belge türlerini ekle
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

    // DTO sınıfı
    private class KadroKriterAltDTO
    {
        public int KriterIndex { get; set; }
        public string BelgeTuru { get; set; }
        public int BelgeSayisi { get; set; }
    }
}
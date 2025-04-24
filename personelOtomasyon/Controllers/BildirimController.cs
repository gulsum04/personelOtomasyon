using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personelOtomasyon.Models;
using personelOtomasyon.Data;

namespace personelOtomasyon.Controllers
{
    [Authorize]
    public class BildirimController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BildirimController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Bildirimleri listele
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var bildirimler = await _context.Bildirimler
                .Where(b => b.KullaniciId == userId)
                .OrderByDescending(b => b.Tarih)
                .ToListAsync();

            return View(bildirimler);
        }

        // Bildirimi okundu olarak işaretle
        [HttpPost]
        public IActionResult OkunduYap(int id)
        {
            var bildirim = _context.Bildirimler.FirstOrDefault(b => b.BildirimId == id);
            if (bildirim != null)
            {
                bildirim.OkunduMu = true;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Bildirimi sil
        [HttpPost]
        public IActionResult Sil(int id)
        {
            var bildirim = _context.Bildirimler.FirstOrDefault(b => b.BildirimId == id);
            if (bildirim != null)
            {
                _context.Bildirimler.Remove(bildirim);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}

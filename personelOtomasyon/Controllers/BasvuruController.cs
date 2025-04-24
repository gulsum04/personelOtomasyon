using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personelOtomasyon.Data;
using System.Security.Claims;

namespace personelOtomasyon.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BasvuruController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BasvuruController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Basvuru/
        public async Task<IActionResult> Index()
        {
            var basvurular = await _context.Basvurular
                .Include(b => b.Ilan)
                .Include(b => b.Aday)
                .ToListAsync();

            // 🔔 Jüri atama durumlarını kontrol et
            var basvuruIdler = basvurular.Select(b => b.BasvuruId).ToList();

            var juriAtamaDict = _context.BasvuruJuriAtamalari
                .Where(j => basvuruIdler.Contains(j.BasvuruId))
                .GroupBy(j => j.BasvuruId)
                .ToDictionary(g => g.Key, g => g.Any());

            ViewBag.JuriAtamaDurumlari = juriAtamaDict;

            return View(basvurular);
        }
      
    }
}
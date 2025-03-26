using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using personelOtomasyon.Data;

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

            return View(basvurular);
        }
    }
}

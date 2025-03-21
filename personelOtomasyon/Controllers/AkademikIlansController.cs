using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using personelOtomasyon.Data;
using personelOtomasyon.Models;

namespace personelOtomasyon.Controllers
{
    public class AkademikIlansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AkademikIlansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AkademikIlans
        public async Task<IActionResult> Index()
        {
            return View(await _context.AkademikIlanlar.ToListAsync());
        }

        // GET: AkademikIlans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var akademikIlan = await _context.AkademikIlanlar
                .FirstOrDefaultAsync(m => m.IlanId == id);
            if (akademikIlan == null)
            {
                return NotFound();
            }

            return View(akademikIlan);
        }

        // GET: AkademikIlans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AkademikIlans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IlanId,Baslik,Kategori,Aciklama,BasvuruBaslangicTarihi,BasvuruBitisTarihi,KullaniciAdminId")] AkademikIlan akademikIlan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(akademikIlan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(akademikIlan);
        }

        // GET: AkademikIlans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var akademikIlan = await _context.AkademikIlanlar.FindAsync(id);
            if (akademikIlan == null)
            {
                return NotFound();
            }
            return View(akademikIlan);
        }

        // POST: AkademikIlans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IlanId,Baslik,Kategori,Aciklama,BasvuruBaslangicTarihi,BasvuruBitisTarihi,KullaniciAdminId")] AkademikIlan akademikIlan)
        {
            if (id != akademikIlan.IlanId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(akademikIlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AkademikIlanExists(akademikIlan.IlanId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(akademikIlan);
        }

        // GET: AkademikIlans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var akademikIlan = await _context.AkademikIlanlar
                .FirstOrDefaultAsync(m => m.IlanId == id);
            if (akademikIlan == null)
            {
                return NotFound();
            }

            return View(akademikIlan);
        }

        // POST: AkademikIlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var akademikIlan = await _context.AkademikIlanlar.FindAsync(id);
            if (akademikIlan != null)
            {
                _context.AkademikIlanlar.Remove(akademikIlan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AkademikIlanExists(int id)
        {
            return _context.AkademikIlanlar.Any(e => e.IlanId == id);
        }
    }
}

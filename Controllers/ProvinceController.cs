using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CentralAddressSystem.Data;
using CentralAddressSystem.Models;

namespace CentralAddressSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProvinceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProvinceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Province
        public async Task<IActionResult> Index()
        {
            var provinces = await _context.Provinces
                .Include(p => p.Country)
                .ToListAsync();
            return View(provinces);
        }

        // GET: Province/Create
        public IActionResult Create()
        {
            ViewBag.Countries = _context.Countries.ToList();
            return View();
        }

        // POST: Province/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Province province)
        {
            if (ModelState.IsValid)
            {
                province.ProvinceID = Guid.NewGuid();
                province.CreatedAt = DateTime.UtcNow;
                _context.Add(province);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Countries = _context.Countries.ToList();
            return View(province);
        }

        // GET: Province/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var province = await _context.Provinces.FindAsync(id);
            if (province == null)
            {
                return NotFound();
            }
            ViewBag.Countries = _context.Countries.ToList();
            return View(province);
        }

        // POST: Province/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Province province)
        {
            if (id != province.ProvinceID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(province);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Provinces.Any(e => e.ProvinceID == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Countries = _context.Countries.ToList();
            return View(province);
        }

        // GET: Province/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var province = await _context.Provinces
                .Include(p => p.Country)
                .FirstOrDefaultAsync(p => p.ProvinceID == id);
            if (province == null)
            {
                return NotFound();
            }
            return View(province);
        }

        // POST: Province/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var province = await _context.Provinces.FindAsync(id);
            if (province != null)
            {
                _context.Provinces.Remove(province);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
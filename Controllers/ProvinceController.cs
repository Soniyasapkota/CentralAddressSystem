using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CentralAddressSystem.Data;
using CentralAddressSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CentralAddressSystem.Controllers
{
    [Authorize]
    public class ProvinceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProvinceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Province/Index
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            var provinces = await _context.Provinces
                .Include(p => p.Country)
                .ToListAsync();
            return View(provinces);
        }

        // GET: Province/Create
        public async Task<IActionResult> Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            ViewData["Countries"] = new SelectList(await _context.Countries.ToListAsync(), "CountryID", "CountryName");
            return View();
        }

        // POST: Province/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Province province)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                province.CreatedAt = DateTime.Now;
                _context.Add(province);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Province created successfully.";
                return RedirectToAction("Index");
            }

            ViewData["Countries"] = new SelectList(await _context.Countries.ToListAsync(), "CountryID", "CountryName", province.CountryID);
            return View(province);
        }

        // GET: Province/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var province = await _context.Provinces.FindAsync(id);
            if (province == null)
            {
                return NotFound();
            }

            ViewData["Countries"] = new SelectList(await _context.Countries.ToListAsync(), "CountryID", "CountryName", province.CountryID);
            return View(province);
        }

        // POST: Province/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Province province)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (id != province.ProvinceID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    province.CreatedAt = (await _context.Provinces.AsNoTracking().FirstOrDefaultAsync(p => p.ProvinceID == id))?.CreatedAt ?? DateTime.Now;
                    _context.Update(province);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Province updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProvinceExists(province.ProvinceID))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction("Index");
            }

            ViewData["Countries"] = new SelectList(await _context.Countries.ToListAsync(), "CountryID", "CountryName", province.CountryID);
            return View(province);
        }

        // GET: Province/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var province = await _context.Provinces
                .Include(p => p.Country)
                .FirstOrDefaultAsync(m => m.ProvinceID == id);
            if (province == null)
            {
                return NotFound();
            }

            return View(province);
        }

        // POST: Province/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            var province = await _context.Provinces.FindAsync(id);
            if (province != null)
            {
                _context.Provinces.Remove(province);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Province deleted successfully.";
            }
            return RedirectToAction("Index");
        }

        private bool ProvinceExists(Guid id)
        {
            return _context.Provinces.Any(e => e.ProvinceID == id);
        }
    }
}
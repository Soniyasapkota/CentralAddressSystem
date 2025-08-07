using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CentralAddressSystem.Data;
using CentralAddressSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace CentralAddressSystem.Controllers
{
    [Authorize]
    public class DistrictController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DistrictController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: District/Index
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            var districts = await _context.Districts
                .Include(d => d.Province)
                .ThenInclude(p => p.Country)
                .ToListAsync();
            return View(districts);
        }

        // GET: District/Create
        public async Task<IActionResult> Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            var provinces = await _context.Provinces.ToListAsync();
            if (provinces == null || !provinces.Any())
            {
                TempData["Error"] = "No provinces available. Please create a province first.";
                return RedirectToAction("Create", "Province");
            }

            // Debug: Log the provinces to ensure they are loaded
            Debug.WriteLine($"Provinces count: {provinces.Count}");
            foreach (var province in provinces)
            {
                Debug.WriteLine($"Province ID: {province.ProvinceID}, Name: {province.ProvinceName}");
            }

            ViewData["Provinces"] = new SelectList(provinces, "ProvinceID", "ProvinceName", null);
            return View();
    
}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(District district)
        {
            Debug.WriteLine($"POST Create called. DistrictName: {district?.DistrictName}, ProvinceID: {district?.ProvinceID}");

            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (district == null)
            {
                TempData["Error"] = "District data is null.";
                var provinces = await _context.Provinces.ToListAsync();
                ViewData["Provinces"] = new SelectList(provinces, "ProvinceID", "ProvinceName");
                return View();
            }

            // Debug: Log ModelState errors
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Debug.WriteLine("ModelState errors: " + string.Join("; ", errors));
                TempData["Error"] = "Validation failed: " + string.Join("; ", errors);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verify ProvinceID exists
                    var provinceExists = await _context.Provinces.AnyAsync(p => p.ProvinceID == district.ProvinceID);
                    if (!provinceExists)
                    {
                        TempData["Error"] = "Selected province does not exist.";
                        var provinces = await _context.Provinces.ToListAsync();
                        ViewData["Provinces"] = new SelectList(provinces, "ProvinceID", "ProvinceName", district.ProvinceID);
                        return View(district);
                    }

                    district.CreatedAt = DateTime.Now;
                    _context.Add(district);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "District created successfully.";
                    Debug.WriteLine("District created successfully.");
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Failed to create district: {ex.Message}";
                    Debug.WriteLine($"Error saving district: {ex}");
                }
            }

            var provincesReload = await _context.Provinces.ToListAsync();
            ViewData["Provinces"] = new SelectList(provincesReload, "ProvinceID", "ProvinceName", district.ProvinceID);
            return View(district);
        }

        // GET: District/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

            var district = await _context.Districts.FindAsync(id);
            if (district == null)
            {
                return NotFound();
            }

            ViewData["Provinces"] = new SelectList(await _context.Provinces.ToListAsync(), "ProvinceID", "ProvinceName", district.ProvinceID);
            return View(district);
        }

        // POST: District/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, District district)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (id != district.DistrictID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    district.CreatedAt = (await _context.Districts.AsNoTracking().FirstOrDefaultAsync(d => d.DistrictID == id))?.CreatedAt ?? DateTime.Now;
                    _context.Update(district);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "District updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DistrictExists(district.DistrictID))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction("Index");
            }

            ViewData["Provinces"] = new SelectList(await _context.Provinces.ToListAsync(), "ProvinceID", "ProvinceName", district.ProvinceID);
            return View(district);
        }

        // GET: District/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

            var district = await _context.Districts
                .Include(d => d.Province)
                .ThenInclude(p => p.Country)
                .FirstOrDefaultAsync(m => m.DistrictID == id);
            if (district == null)
            {
                return NotFound();
            }

            return View(district);
        }

        // POST: District/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            var district = await _context.Districts.FindAsync(id);
            if (district != null)
            {
                _context.Districts.Remove(district);
                await _context.SaveChangesAsync();
                TempData["Message"] = "District deleted successfully.";
            }
            return RedirectToAction("Index");
        }

        private bool DistrictExists(int id)
        {
            return _context.Districts.Any(e => e.DistrictID == id);
        }
    }
}
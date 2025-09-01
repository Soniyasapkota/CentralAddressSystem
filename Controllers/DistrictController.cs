using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CentralAddressSystem.Data;
 using CentralAddressSystem.Models;
using CentralAddressSystem.ViewModels;
 using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc.Rendering;
   using System.Diagnostics;

namespace CentralAddressSystem.Controllers {
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
                .ThenInclude(p => p!.Country)
                .Select(d => new DistrictViewModel
                {
                    DistrictID = d.DistrictID,
                    DistrictName = d.DistrictName,
                    ProvinceID = d.ProvinceID,
                    ProvinceName = d.Province!.ProvinceName,
                    CountryName = d.Province!.Country!.CountryName,
                    CreatedAt = d.CreatedAt
                })
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

            var viewModel = new DistrictViewModel
            {
                Provinces = new SelectList(provinces, "ProvinceID", "ProvinceName")
            };
            return View(viewModel);
        }

        // POST: District/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DistrictViewModel viewModel)
        {
            Debug.WriteLine($"POST Create called. DistrictName: {viewModel?.DistrictName}, ProvinceID: {viewModel?.ProvinceID}");

            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (viewModel == null)
            {
                TempData["Error"] = "District data is null.";
                var provinces = await _context.Provinces.ToListAsync();
                return View(new DistrictViewModel
                {
                    Provinces = new SelectList(provinces, "ProvinceID", "ProvinceName")
                });
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
                    var provinceExists = await _context.Provinces.AnyAsync(p => p.ProvinceID == viewModel.ProvinceID);
                    if (!provinceExists)
                    {
                        TempData["Error"] = "Selected province does not exist.";
                        var provinces = await _context.Provinces.ToListAsync();
                        viewModel.Provinces = new SelectList(provinces, "ProvinceID", "ProvinceName", viewModel.ProvinceID);
                        return View(viewModel);
                    }

                    var district = new District
                    {
                        DistrictID = Guid.NewGuid(),
                        DistrictName = viewModel.DistrictName,
                        ProvinceID = viewModel.ProvinceID,
                        CreatedAt = DateTime.Now
                    };

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
            viewModel.Provinces = new SelectList(provincesReload, "ProvinceID", "ProvinceName", viewModel.ProvinceID);
            return View(viewModel);
        }

        // GET: District/Edit/5
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

            var district = await _context.Districts
                .Include(d => d.Province)
                .FirstOrDefaultAsync(d => d.DistrictID == id);
            if (district == null)
            {
                return NotFound();
            }

            var viewModel = new DistrictViewModel
            {
                DistrictID = district.DistrictID,
                DistrictName = district.DistrictName,
                ProvinceID = district.ProvinceID,
                ProvinceName = district.Province?.ProvinceName,
                CreatedAt = district.CreatedAt,
                Provinces = new SelectList(await _context.Provinces.ToListAsync(), "ProvinceID", "ProvinceName", district.ProvinceID)
            };

            return View(viewModel);
        }

        // POST: District/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, DistrictViewModel viewModel)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (id != viewModel.DistrictID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var district = new District
                    {
                        DistrictID = viewModel.DistrictID,
                        DistrictName = viewModel.DistrictName,
                        ProvinceID = viewModel.ProvinceID,
                        CreatedAt = (await _context.Districts.AsNoTracking().FirstOrDefaultAsync(d => d.DistrictID == id))?.CreatedAt ?? DateTime.Now
                    };

                    _context.Update(district);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "District updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DistrictExists(viewModel.DistrictID))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction("Index");
            }

            var provinces = await _context.Provinces.ToListAsync();
            viewModel.Provinces = new SelectList(provinces, "ProvinceID", "ProvinceName", viewModel.ProvinceID);
            return View(viewModel);
        }

        // GET: District/Delete/5
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

            var district = await _context.Districts
                .Include(d => d.Province)
                .ThenInclude(p => p!.Country)
                .FirstOrDefaultAsync(m => m.DistrictID == id);
            if (district == null)
            {
                return NotFound();
            }

            var viewModel = new DistrictViewModel
            {
                DistrictID = district.DistrictID,
                DistrictName = district.DistrictName,
                ProvinceID = district.ProvinceID,
                ProvinceName = district.Province?.ProvinceName,
                CreatedAt = district.CreatedAt
            };

            return View(viewModel);
        }

        // POST: District/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
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

        private bool DistrictExists(Guid id)
        {
            return _context.Districts.Any(e => e.DistrictID == id);
        }
    }

}
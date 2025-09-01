using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CentralAddressSystem.Data;
using CentralAddressSystem.Models;
using CentralAddressSystem.ViewModels;

using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc.Rendering;

namespace CentralAddressSystem.Controllers {
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
                .Select(p => new ProvinceViewModel
                {
                    ProvinceID = p.ProvinceID,
                    ProvinceCode = p.ProvinceCode,
                    ProvinceName = p.ProvinceName,
                    Noofdistricts = p.Noofdistricts,
                    CountryID = p.CountryID,
                    CountryName = p.Country!.CountryName,
                    CreatedAt = p.CreatedAt
                })
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

            var viewModel = new ProvinceViewModel
            {
                Countries = new SelectList(await _context.Countries.ToListAsync(), "CountryID", "CountryName")
            };
            return View(viewModel);
        }

        // POST: Province/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProvinceViewModel viewModel)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var province = new Province
                {
                    ProvinceID = Guid.NewGuid(),
                    ProvinceCode = viewModel.ProvinceCode,
                    ProvinceName = viewModel.ProvinceName,
                    Noofdistricts = viewModel.Noofdistricts,
                    CountryID = viewModel.CountryID,
                    CreatedAt = DateTime.Now
                };

                _context.Add(province);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Province created successfully.";
                return RedirectToAction("Index");
            }

            viewModel.Countries = new SelectList(await _context.Countries.ToListAsync(), "CountryID", "CountryName", viewModel.CountryID);
            return View(viewModel);
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

            var province = await _context.Provinces
                .Include(p => p.Country)
                .FirstOrDefaultAsync(p => p.ProvinceID == id);

            if (province == null)
            {
                return NotFound();
            }

            var viewModel = new ProvinceViewModel
            {
                ProvinceID = province.ProvinceID,
                ProvinceCode = province.ProvinceCode,
                ProvinceName = province.ProvinceName,
                Noofdistricts = province.Noofdistricts,
                CountryID = province.CountryID,
                CountryName = province.Country?.CountryName,
                CreatedAt = province.CreatedAt,
                Countries = new SelectList(await _context.Countries.ToListAsync(), "CountryID", "CountryName", province.CountryID)
            };

            return View(viewModel);
        }

        // POST: Province/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProvinceViewModel viewModel)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (id != viewModel.ProvinceID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var province = new Province
                    {
                        ProvinceID = viewModel.ProvinceID,
                        ProvinceCode = viewModel.ProvinceCode,
                        ProvinceName = viewModel.ProvinceName,
                        Noofdistricts = viewModel.Noofdistricts,
                        CountryID = viewModel.CountryID,
                        CreatedAt = (await _context.Provinces.AsNoTracking().FirstOrDefaultAsync(p => p.ProvinceID == id))?.CreatedAt ?? DateTime.Now
                    };

                    _context.Update(province);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Province updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProvinceExists(viewModel.ProvinceID))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction("Index");
            }

            viewModel.Countries = new SelectList(await _context.Countries.ToListAsync(), "CountryID", "CountryName", viewModel.CountryID);
            return View(viewModel);
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

            var viewModel = new ProvinceViewModel
            {
                ProvinceID = province.ProvinceID,
                ProvinceCode = province.ProvinceCode,
                ProvinceName = province.ProvinceName,
                Noofdistricts = province.Noofdistricts,
                CountryID = province.CountryID,
                CountryName = province.Country?.CountryName,
                CreatedAt = province.CreatedAt
            };

            return View(viewModel);
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
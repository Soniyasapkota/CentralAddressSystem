using Microsoft.AspNetCore.Mvc;
 using Microsoft.EntityFrameworkCore;
using CentralAddressSystem.Data;
using CentralAddressSystem.Models;
 using CentralAddressSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace CentralAddressSystem.Controllers {
    [Authorize]
     public class CountryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CountryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Country/Index
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            var countries = await _context.Countries
                .Select(c => new CountryViewModel
                {
                    CountryID = c.CountryID,
                    CountryName = c.CountryName,
                    CountryCode = c.CountryCode,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
            return View(countries);
        }

        // GET: Country/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            return View(new CountryViewModel());
        }

        // POST: Country/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CountryViewModel viewModel)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var country = new Country
                {
                    CountryID = Guid.NewGuid(),
                    CountryName = viewModel.CountryName,
                    CountryCode = viewModel.CountryCode,
                    CreatedAt = DateTime.Now
                };

                _context.Add(country);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Country created successfully.";
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        // GET: Country/Edit/5
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

            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            var viewModel = new CountryViewModel
            {
                CountryID = country.CountryID,
                CountryName = country.CountryName,
                CountryCode = country.CountryCode,
                CreatedAt = country.CreatedAt
            };

            return View(viewModel);
        }

        // POST: Country/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CountryViewModel viewModel)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (id != viewModel.CountryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var country = new Country
                    {
                        CountryID = viewModel.CountryID,
                        CountryName = viewModel.CountryName,
                        CountryCode = viewModel.CountryCode,
                        CreatedAt = (await _context.Countries.AsNoTracking().FirstOrDefaultAsync(c => c.CountryID == id))?.CreatedAt ?? DateTime.Now
                    };

                    _context.Update(country);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Country updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(viewModel.CountryID))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        // GET: Country/Delete/5
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

            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            var viewModel = new CountryViewModel
            {
                CountryID = country.CountryID,
                CountryName = country.CountryName,
                CountryCode = country.CountryCode,
                CreatedAt = country.CreatedAt
            };

            return View(viewModel);
        }

        // POST: Country/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            var country = await _context.Countries.FindAsync(id);
            if (country != null)
            {
                _context.Countries.Remove(country);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Country deleted successfully.";
            }
            return RedirectToAction("Index");
        }

        private bool CountryExists(Guid id)
        {
            return _context.Countries.Any(e => e.CountryID == id);
        }
    }

}
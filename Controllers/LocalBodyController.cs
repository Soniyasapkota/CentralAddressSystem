using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CentralAddressSystem.Data;
 using CentralAddressSystem.Models;
using CentralAddressSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering; 
using System.Diagnostics; using System;

namespace CentralAddressSystem.Controllers {
    [Authorize]
    public class LocalBodyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LocalBodyController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: LocalBody/Index
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session?.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            var localBodies = await _context.LocalBodies
                .Include(lb => lb.District)
                .ThenInclude(d => d!.Province)
                .Select(lb => new LocalBodyViewModel
                {
                    LocalBodyID = lb.LocalBodyID,
                    LocalBodyName = lb.LocalBodyName,
                    DistrictID = lb.DistrictID,
                    DistrictName = lb.District!.DistrictName,
                    ProvinceName = lb.District!.Province!.ProvinceName
                })
                .ToListAsync();
            return View(localBodies);
        }

        // GET: LocalBody/Create
        public async Task<IActionResult> Create()
        {
            if (HttpContext.Session?.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            var districts = await _context.Districts.ToListAsync();
            if (districts == null || !districts.Any())
            {
                TempData["Error"] = "No districts available. Please create a district first.";
                return RedirectToAction("Create", "District");
            }

            Debug.WriteLine($"Districts count: {districts.Count}");
            foreach (var district in districts)
            {
                Debug.WriteLine($"District ID: {district.DistrictID}, Name: {district.DistrictName}");
            }

            var viewModel = new LocalBodyViewModel
            {
                Districts = new SelectList(districts, "DistrictID", "DistrictName")
            };
            return View(viewModel);
        }

        // POST: LocalBody/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LocalBodyViewModel viewModel)
        {
            Debug.WriteLine($"POST Create called. LocalBodyName: {viewModel?.LocalBodyName ?? "null"}, DistrictID: {viewModel?.DistrictID}");

            if (HttpContext.Session?.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (viewModel == null)
            {
                TempData["Error"] = "LocalBody data is null.";
                var districts = await _context.Districts.ToListAsync();
                viewModel = new LocalBodyViewModel
                {
                    Districts = new SelectList(districts, "DistrictID", "DistrictName")
                };
                return View(viewModel);
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Debug.WriteLine("ModelState errors: " + string.Join("; ", errors));
                TempData["Error"] = "Validation failed: " + string.Join("; ", errors);
                viewModel.Districts = new SelectList(await _context.Districts.ToListAsync(), "DistrictID", "DistrictName", viewModel.DistrictID);
                return View(viewModel);
            }

            try
            {
                var districtExists = await _context.Districts.AnyAsync(d => d.DistrictID == viewModel.DistrictID);
                if (!districtExists)
                {
                    TempData["Error"] = "Selected district does not exist.";
                    viewModel.Districts = new SelectList(await _context.Districts.ToListAsync(), "DistrictID", "DistrictName", viewModel.DistrictID);
                    return View(viewModel);
                }

                var localBody = new LocalBody
                {
                    LocalBodyID = Guid.NewGuid(),
                    LocalBodyName = viewModel.LocalBodyName,
                    DistrictID = viewModel.DistrictID
                };

                _context.Add(localBody);
                await _context.SaveChangesAsync();
                TempData["Message"] = "LocalBody created successfully.";
                Debug.WriteLine("LocalBody created successfully.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to create local body: {ex.Message}";
                Debug.WriteLine($"Error saving local body: {ex}");
                viewModel.Districts = new SelectList(await _context.Districts.ToListAsync(), "DistrictID", "DistrictName", viewModel.DistrictID);
                return View(viewModel);
            }
        }

        // GET: LocalBody/Edit/{id}
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (HttpContext.Session?.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var localBody = await _context.LocalBodies
                .Include(lb => lb.District)
                .ThenInclude(d => d!.Province)
                .FirstOrDefaultAsync(lb => lb.LocalBodyID == id);

            if (localBody == null)
            {
                return NotFound();
            }

            var viewModel = new LocalBodyViewModel
            {
                LocalBodyID = localBody.LocalBodyID,
                LocalBodyName = localBody.LocalBodyName,
                DistrictID = localBody.DistrictID,
                DistrictName = localBody.District?.DistrictName,
                ProvinceName = localBody.District?.Province?.ProvinceName,
                Districts = new SelectList(await _context.Districts.ToListAsync(), "DistrictID", "DistrictName", localBody.DistrictID)
            };

            return View(viewModel);
        }

        // POST: LocalBody/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, LocalBodyViewModel viewModel)
        {
            if (HttpContext.Session?.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (id != viewModel.LocalBodyID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var districtExists = await _context.Districts.AnyAsync(d => d.DistrictID == viewModel.DistrictID);
                    if (!districtExists)
                    {
                        TempData["Error"] = "Selected district does not exist.";
                        viewModel.Districts = new SelectList(await _context.Districts.ToListAsync(), "DistrictID", "DistrictName", viewModel.DistrictID);
                        return View(viewModel);
                    }

                    var localBody = await _context.LocalBodies.FindAsync(id);
                    if (localBody == null)
                    {
                        return NotFound();
                    }

                    localBody.LocalBodyName = viewModel.LocalBodyName;
                    localBody.DistrictID = viewModel.DistrictID;

                    _context.Update(localBody);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "LocalBody updated successfully.";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocalBodyExists(viewModel.LocalBodyID))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            viewModel.Districts = new SelectList(await _context.Districts.ToListAsync(), "DistrictID", "DistrictName", viewModel.DistrictID);
            return View(viewModel);
        }

        // GET: LocalBody/Delete/{id}
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (HttpContext.Session?.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var localBody = await _context.LocalBodies
                .Include(lb => lb.District)
                .ThenInclude(d => d!.Province)
                .FirstOrDefaultAsync(m => m.LocalBodyID == id);

            if (localBody == null)
            {
                return NotFound();
            }

            var viewModel = new LocalBodyViewModel
            {
                LocalBodyID = localBody.LocalBodyID,
                LocalBodyName = localBody.LocalBodyName,
                DistrictID = localBody.DistrictID,
                DistrictName = localBody.District?.DistrictName,
                ProvinceName = localBody.District?.Province?.ProvinceName
            };

            return View(viewModel);
        }

        // POST: LocalBody/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (HttpContext.Session?.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            var localBody = await _context.LocalBodies.FindAsync(id);
            if (localBody != null)
            {
                _context.LocalBodies.Remove(localBody);
                await _context.SaveChangesAsync();
                TempData["Message"] = "LocalBody deleted successfully.";
            }
            return RedirectToAction("Index");
        }

        private bool LocalBodyExists(Guid id)
        {
            return _context.LocalBodies.Any(e => e.LocalBodyID == id);
        }
    }

}
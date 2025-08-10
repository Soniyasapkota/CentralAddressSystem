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
                .ThenInclude(d => d.Province)
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

            // Debug: Log the districts to ensure they are loaded
            Debug.WriteLine($"Districts count: {districts.Count}");
            foreach (var district in districts)
            {
                Debug.WriteLine($"District ID: {district.DistrictID}, Name: {district.DistrictName}");
            }

            ViewData["Districts"] = new SelectList(districts, "DistrictID", "DistrictName");
            return View();
        }

        // POST: LocalBody/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LocalBody localBody)
        {
            Debug.WriteLine($"POST Create called. LocalBodyName: {localBody?.LocalBodyName ?? "null"}, DistrictID: {localBody?.DistrictID}");

            if (HttpContext.Session?.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access denied. Admins only.";
                return RedirectToAction("Index", "Home");
            }

            if (localBody == null)
            {
                TempData["Error"] = "LocalBody data is null.";
                var districts = await _context.Districts.ToListAsync();
                ViewData["Districts"] = new SelectList(districts, "DistrictID", "DistrictName");
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
                    // Verify DistrictID exists
                    var districtExists = await _context.Districts.AnyAsync(d => d.DistrictID == localBody.DistrictID);
                    if (!districtExists)
                    {
                        TempData["Error"] = "Selected district does not exist.";
                        var districts = await _context.Districts.ToListAsync();
                        ViewData["Districts"] = new SelectList(districts, "DistrictID", "DistrictName", localBody.DistrictID);
                        return View(localBody);
                    }

                    //localBody.CreatedAt = DateTime.Now;
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
                }
            }

            var districtsReload = await _context.Districts.ToListAsync();
            ViewData["Districts"] = new SelectList(districtsReload, "DistrictID", "DistrictName", localBody.DistrictID);
            return View(localBody);
        }

    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CentralAddressSystem.Data;
using CentralAddressSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        // GET: Distrcit/Index

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
      .Where(d => d.Province != null && 
                  d.Province.Country != null && 
                d.Province.Country.CountryName == "Nepal")
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

            ViewData["Provinces"] = new SelectList(
                await _context.Provinces
                    .Where(p => p.Country.CountryName == "Nepal")
                    .ToListAsync(),
                "ProvinceID", "ProvinceName");
            return View();
        }


        //POST: Create districts

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(District district)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                TempData["Error"] = "Access Denied. Admins Only.";
                return RedirectToAction("Index", "Home");

            }

            if (ModelState.IsValid)
            {
                district.CreatedAt = DateTime.Now;
                _context.Add(district);
                await _context.SaveChangesAsync();
                TempData["Message"] = "District Created Successfully.";
                return RedirectToAction("Index");
            }

            ViewData["Provinces"] = new SelectList(
             await _context.Provinces
                 .Where(p => p.Country.CountryName == "Nepal")
                 .ToListAsync(),
             "ProvinceID", "ProvinceName", district.ProvinceID);
            return View(district);

        }
    

            
        }


    }

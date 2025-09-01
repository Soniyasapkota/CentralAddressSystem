using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CentralAddressSystem.Data;
using CentralAddressSystem.Models;
using CentralAddressSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CentralAddressSystem.Controllers
{
    [Authorize]
    public class AddressController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AddressController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Address/Index
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["Error"] = "Unable to identify user. Please log in again.";
                return RedirectToAction("Auth", "Account");
            }

            var userRole = HttpContext.Session.GetString("UserRole");

            IQueryable<Address> addresses;
            if (userRole == "Admin")
            {
                addresses = _context.Addresses
                    .Include(a => a.User)
                    .Include(a => a.Country)
                    .Include(a => a.Province)
                    .Include(a => a.District)
                    .Include(a => a.LocalBody);
            }
            else
            {
                addresses = _context.Addresses
                    .Where(a => a.UserID == userId)
                    .Include(a => a.User)
                    .Include(a => a.Country)
                    .Include(a => a.Province)
                    .Include(a => a.District)
                    .Include(a => a.LocalBody);
            }

            var viewModels = await addresses.Select(a => new AddressViewModel
            {
                AddressID = a.AddressID,
                UserID = a.UserID,
                UserName = a.User != null ? a.User.UserName : null,
                Street = a.Street,
                CountryID = a.CountryID,
                CountryName = a.Country != null ? a.Country.CountryName : null,
                ProvinceID = a.ProvinceID,
                ProvinceName = a.Province != null ? a.Province.ProvinceName : null,
                DistrictID = a.DistrictID,
                DistrictName = a.District != null ? a.District.DistrictName : null,
                LocalBodyID = a.LocalBodyID,
                LocalBodyName = a.LocalBody != null ? a.LocalBody.LocalBodyName : null,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            }).ToListAsync();

            return View(viewModels);
        }

        // GET: Address/Create
        public async Task<IActionResult> Create()
        {
            if (!await _context.Countries.AnyAsync())
            {
                TempData["Error"] = "No countries available. Please add a country first.";
                return RedirectToAction("Index", "Country");
            }

            var viewModel = new AddressViewModel();
            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        // POST: Address/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddressViewModel viewModel)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["Error"] = "Unable to identify user. Please log in again.";
                return RedirectToAction("Auth", "Account");
            }

            if (ModelState.IsValid)
            {
                var address = new Address
                {
                    UserID = userId,
                    Street = viewModel.Street,
                    CountryID = viewModel.CountryID,
                    ProvinceID = viewModel.ProvinceID,
                    DistrictID = viewModel.DistrictID,
                    LocalBodyID = viewModel.LocalBodyID,
                    CreatedAt = DateTime.Now
                };

                try
                {
                    _context.Add(address);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Address created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Database Error: {ex.InnerException?.Message ?? ex.Message}");
                    TempData["Error"] = "Failed to create address due to a database error: " + (ex.InnerException?.Message ?? ex.Message);
                    await PopulateDropdowns(viewModel);
                    return View(viewModel);
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            Console.WriteLine("Validation Errors: " + string.Join(", ", errors));
            TempData["Error"] = "Please correct the errors in the form: " + string.Join(", ", errors);
            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        // GET: Address/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Addresses
                .Include(a => a.User)
                .Include(a => a.Country)
                .Include(a => a.Province)
                .Include(a => a.District)
                .Include(a => a.LocalBody)
                .FirstOrDefaultAsync(a => a.AddressID == id);

            if (address == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["Error"] = "Unable to identify user. Please log in again.";
                return RedirectToAction("Auth", "Account");
            }

            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin" && address.UserID != userId)
            {
                TempData["Error"] = "Access denied.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new AddressViewModel
            {
                AddressID = address.AddressID,
                UserID = address.UserID,
                UserName = address.User != null ? address.User.UserName : null,
                Street = address.Street,
                CountryID = address.CountryID,
                CountryName = address.Country != null ? address.Country.CountryName : null,
                ProvinceID = address.ProvinceID,
                ProvinceName = address.Province != null ? address.Province.ProvinceName : null,
                DistrictID = address.DistrictID,
                DistrictName = address.District != null ? address.District.DistrictName : null,
                LocalBodyID = address.LocalBodyID,
                LocalBodyName = address.LocalBody != null ? address.LocalBody.LocalBodyName : null,
                CreatedAt = address.CreatedAt,
                UpdatedAt = address.UpdatedAt
            };

            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        // POST: Address/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AddressViewModel viewModel)
        {
            if (id != viewModel.AddressID)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["Error"] = "Unable to identify user. Please log in again.";
                return RedirectToAction("Auth", "Account");
            }

            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin" && viewModel.UserID != userId)
            {
                TempData["Error"] = "Access denied.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the original address to preserve CreatedAt
                    var originalAddress = await _context.Addresses.AsNoTracking()
                        .FirstOrDefaultAsync(a => a.AddressID == id);
                    if (originalAddress == null)
                    {
                        return NotFound();
                    }

                    var address = new Address
                    {
                        AddressID = viewModel.AddressID,
                        UserID = viewModel.UserID,
                        Street = viewModel.Street,
                        CountryID = viewModel.CountryID,
                        ProvinceID = viewModel.ProvinceID,
                        DistrictID = viewModel.DistrictID,
                        LocalBodyID = viewModel.LocalBodyID,
                        CreatedAt = originalAddress.CreatedAt, // Preserve original CreatedAt
                        UpdatedAt = DateTime.Now
                    };

                    _context.Update(address);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Address updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddressExists(viewModel.AddressID))
                    {
                        return NotFound();
                    }
                    throw;
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Database Error: {ex.InnerException?.Message ?? ex.Message}");
                    TempData["Error"] = "Failed to update address due to a database error: " + (ex.InnerException?.Message ?? ex.Message);
                    await PopulateDropdowns(viewModel);
                    return View(viewModel);
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            Console.WriteLine("Validation Errors: " + string.Join(", ", errors));
            TempData["Error"] = "Please correct the errors in the form: " + string.Join(", ", errors);
            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        // GET: Address/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Addresses
                .Include(a => a.User)
                .Include(a => a.Country)
                .Include(a => a.Province)
                .Include(a => a.District)
                .Include(a => a.LocalBody)
                .FirstOrDefaultAsync(m => m.AddressID == id);

            if (address == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["Error"] = "Unable to identify user. Please log in again.";
                return RedirectToAction("Auth", "Account");
            }

            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin" && address.UserID != userId)
            {
                TempData["Error"] = "Access denied.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new AddressViewModel
            {
                AddressID = address.AddressID,
                UserID = address.UserID,
                UserName = address.User != null ? address.User.UserName : null,
                Street = address.Street,
                CountryID = address.CountryID,
                CountryName = address.Country != null ? address.Country.CountryName : null,
                ProvinceID = address.ProvinceID,
                ProvinceName = address.Province != null ? address.Province.ProvinceName : null,
                DistrictID = address.DistrictID,
                DistrictName = address.District != null ? address.District.DistrictName : null,
                LocalBodyID = address.LocalBodyID,
                LocalBodyName = address.LocalBody != null ? address.LocalBody.LocalBodyName : null,
                CreatedAt = address.CreatedAt,
                UpdatedAt = address.UpdatedAt
            };

            return View(viewModel);
        }

        // POST: Address/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["Error"] = "Unable to identify user. Please log in again.";
                return RedirectToAction("Auth", "Account");
            }

            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin" && address.UserID != userId)
            {
                TempData["Error"] = "Access denied.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Address deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database Error: {ex.InnerException?.Message ?? ex.Message}");
                TempData["Error"] = "Failed to delete address due to a database error: " + (ex.InnerException?.Message ?? ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        private bool AddressExists(Guid id)
        {
            return _context.Addresses.Any(e => e.AddressID == id);
        }

        private async Task PopulateDropdowns(AddressViewModel viewModel)
        {
            viewModel.Countries = new SelectList(await _context.Countries.ToListAsync() ?? new List<Country>(), "CountryID", "CountryName", viewModel.CountryID);
            viewModel.Provinces = new SelectList(await _context.Provinces.ToListAsync() ?? new List<Province>(), "ProvinceID", "ProvinceName", viewModel.ProvinceID);
            viewModel.Districts = new SelectList(await _context.Districts.ToListAsync() ?? new List<District>(), "DistrictID", "DistrictName", viewModel.DistrictID);
            viewModel.LocalBodies = new SelectList(await _context.LocalBodies.ToListAsync() ?? new List<LocalBody>(), "LocalBodyID", "LocalBodyName", viewModel.LocalBodyID);
        }
    }
}
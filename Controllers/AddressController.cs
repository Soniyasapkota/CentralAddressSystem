using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CentralAddressSystem.Data;
using CentralAddressSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

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
                    .Include(a => a.LocalBody)
                    .Include(a => a.State)
                    .Include(a => a.ZipCode);
            }
            else
            {
                addresses = _context.Addresses
                    .Where(a => a.UserID == userId)
                    .Include(a => a.User)
                    .Include(a => a.Country)
                    .Include(a => a.Province)
                    .Include(a => a.District)
                    .Include(a => a.LocalBody)
                    .Include(a => a.State)
                    .Include(a => a.ZipCode);
            }

            return View(await addresses.ToListAsync());
        }

        // GET: Address/Create
        public async Task<IActionResult> Create()
        {
            // Check if Countries table has data
            if (!await _context.Countries.AnyAsync())
            {
                TempData["Error"] = "No countries available. Please add a country first.";
                return RedirectToAction("Index", "Country");
            }

            await PopulateDropdowns();
            return View(new Address());
        }

        // POST: Address/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Street,CountryID,ProvinceID,DistrictID,LocalBodyID,StateID,ZipID")] Address address)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                TempData["Error"] = "Unable to identify user. Please log in again.";
                return RedirectToAction("Auth", "Account");
            }

            address.UserID = userId;
            address.CreatedAt = DateTime.Now;

            // Log form data for debugging
            Console.WriteLine($"UserID: {address.UserID}");
            Console.WriteLine($"Street: {address.Street}");
            Console.WriteLine($"CountryID: {address.CountryID}");
            Console.WriteLine($"ProvinceID: {address.ProvinceID}");
            Console.WriteLine($"DistrictID: {address.DistrictID}");
            Console.WriteLine($"LocalBodyID: {address.LocalBodyID}");
            Console.WriteLine($"StateID: {address.StateID}");
            Console.WriteLine($"ZipID: {address.ZipID}");

            // Revalidate the model
            TryValidateModel(address);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("Validation Errors: " + string.Join(", ", errors));
                TempData["Error"] = "Please correct the errors in the form: " + string.Join(", ", errors);
                await PopulateDropdowns(address);
                return View(address);
            }

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
                await PopulateDropdowns(address);
                return View(address);
            }
        }

        // GET: Address/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

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

            await PopulateDropdowns(address);
            return View(address);
        }

        // POST: Address/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AddressID,UserID,Street,CountryID,ProvinceID,DistrictID,LocalBodyID,StateID,ZipID,CreatedAt,UpdatedAt")] Address address)
        {
            if (id != address.AddressID)
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

            if (ModelState.IsValid)
            {
                try
                {
                    address.UpdatedAt = DateTime.Now;
                    var originalAddress = await _context.Addresses.AsNoTracking().FirstOrDefaultAsync(a => a.AddressID == id);
                    address.CreatedAt = originalAddress?.CreatedAt ?? DateTime.Now;
                    _context.Update(address);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Address updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddressExists(address.AddressID))
                    {
                        return NotFound();
                    }
                    throw;
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Database Error: {ex.InnerException?.Message ?? ex.Message}");
                    TempData["Error"] = "Failed to update address due to a database error: " + (ex.InnerException?.Message ?? ex.Message);
                    await PopulateDropdowns(address);
                    return View(address);
                }
                return RedirectToAction(nameof(Index));
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            Console.WriteLine("Validation Errors: " + string.Join(", ", errors));
            TempData["Error"] = "Please correct the errors in the form: " + string.Join(", ", errors);
            await PopulateDropdowns(address);
            return View(address);
        }

        // GET: Address/Delete/5
        public async Task<IActionResult> Delete(int? id)
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
                .Include(a => a.State)
                .Include(a => a.ZipCode)
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

            return View(address);
        }

        // POST: Address/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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

        private bool AddressExists(int id)
        {
            return _context.Addresses.Any(e => e.AddressID == id);
        }

        private async Task PopulateDropdowns(Address? address = null)
        {
            ViewData["Countries"] = new SelectList(await _context.Countries.ToListAsync() ?? new List<Country>(), "CountryID", "CountryName", address?.CountryID);
            ViewData["Provinces"] = new SelectList(await _context.Provinces.ToListAsync() ?? new List<Province>(), "ProvinceID", "ProvinceName", address?.ProvinceID);
            ViewData["Districts"] = new SelectList(await _context.Districts.ToListAsync() ?? new List<District>(), "DistrictID", "DistrictName", address?.DistrictID);
            ViewData["LocalBodies"] = new SelectList(await _context.LocalBodies.ToListAsync() ?? new List<LocalBody>(), "LocalBodyID", "LocalBodyName", address?.LocalBodyID);
            ViewData["States"] = new SelectList(await _context.States.ToListAsync() ?? new List<State>(), "StateID", "StateName", address?.StateID);
            ViewData["ZipCodes"] = new SelectList(await _context.ZipCodes.ToListAsync() ?? new List<ZipCode>(), "ZipID", "ZipCodeValue", address?.ZipID);
        }
    }
}
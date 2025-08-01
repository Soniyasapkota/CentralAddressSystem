using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CentralAddressSystem.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CentralAddressSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;

        public AdminController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            // Retrieve role from session for authorization
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // Retrieve username and email from cookies
            ViewData["UserName"] = Request.Cookies["UserName"];
            ViewData["UserEmail"] = Request.Cookies["UserEmail"];

            // Fetch all users from the database
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }
    }
}
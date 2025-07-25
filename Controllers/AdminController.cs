using Microsoft.AspNetCore.Mvc;

namespace CentralAddressSystem.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            // Retrieve role from session for display or authorization
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // Retrieve username and email from cookies
            ViewData["UserName"] = Request.Cookies["UserName"];
            ViewData["UserEmail"] = Request.Cookies["UserEmail"];

            return View();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CentralAddressSystem.ViewModels;
using CentralAddressSystem.Models;

namespace CentralAddressSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Auth(string? showLogin = null)
        {
            var model = new AuthViewModel
            {
                Login = new LoginViewModel(),
                Register = new RegisterViewModel()
            };
            ViewData["ShowLogin"] = showLogin == "true" ? "true" : "false";
            return View(model);
        }

        [HttpPost]
public async Task<IActionResult> Register(AuthViewModel model)
{
    // Clear ModelState for Login to avoid cross-validation
    foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Login")).ToList())
    {
        ModelState.Remove(key);
    }

    if (model.Register == null)
    {
        Console.WriteLine("Error: model.Register is null");
        TempData["Error"] = "Invalid form data.";
        return View("Auth", new AuthViewModel { Register = new RegisterViewModel() });
    }

    var registerModel = model.Register;
    Console.WriteLine($"FirstName: {registerModel.FirstName ?? "null"}");
    Console.WriteLine($"LastName: {registerModel.LastName ?? "null"}");
    Console.WriteLine($"Email: {registerModel.Email ?? "null"}");
    Console.WriteLine($"Username: {registerModel.Username ?? "null"}");
    Console.WriteLine($"Password: {registerModel.Password ?? "null"}");
    Console.WriteLine($"ConfirmPassword: {registerModel.ConfirmPassword ?? "null"}");
    
    // Check for null email
    if (string.IsNullOrEmpty(registerModel.Email))
    {
        ModelState.AddModelError("Register.Email", "Email is required.");
        TempData["Error"] = "Registration failed: Email is required.";
        return View("Auth", new AuthViewModel { Register = model.Register });
    }
    
    // Check for existing email
            var existingUser = await _userManager.FindByEmailAsync(registerModel.Email);
    if (existingUser != null)
    {
        ModelState.AddModelError("Register.Email", "This email address is already in use.");
        TempData["Error"] = "Registration failed: Email already in use.";
        return View("Auth", new AuthViewModel { Register = model.Register });
    }

    // Revalidate only the Register model
            TryValidateModel(model.Register);

    if (!ModelState.IsValid)
    {
        foreach (var key in ModelState.Keys)
        {
            var modelStateEntry = ModelState[key];
            if (modelStateEntry?.Errors.Any() == true)
            {
                Console.WriteLine($"Key: {key}, Errors: {string.Join(", ", modelStateEntry.Errors.Select(e => e.ErrorMessage))}");
            }
        }
        var errorMessages = ModelState.Values.SelectMany(v => v?.Errors ?? []).Select(e => e.ErrorMessage);
        TempData["Error"] = "Please correct the errors in the form: " + string.Join(", ", errorMessages);
        return View("Auth", new AuthViewModel { Register = model.Register });
    }

            var user = new User
            {
                FirstName = registerModel.FirstName ?? throw new InvalidOperationException("FirstName is required"),
                LastName = registerModel.LastName ?? throw new InvalidOperationException("LastName is required"),
                Email = registerModel.Email ?? throw new InvalidOperationException("Email is required"),
                UserName = registerModel.Username ?? throw new InvalidOperationException("Username is required"),
                Role = "Customer",
                CreatedAt = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password ?? throw new InvalidOperationException("Password is required"));
            if (result.Succeeded)
            {
                Console.WriteLine("User created successfully: " + user.Email);
                TempData["Message"] = "Registration successful. Please log in.";
                return RedirectToAction("Auth", new { showLogin = "true" });
            }

            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Identity Error: {error.Code} - {error.Description}");
                ModelState.AddModelError(string.Empty, error.Description);
            }
            TempData["Error"] = "Registration failed: " + string.Join(", ", result.Errors.Select(e => e.Description));
            return View("Auth", model);
        }



        [HttpPost]
        public async Task<IActionResult> Login(AuthViewModel model)
        {
            // Clear ModelState for Register to avoid cross-validation
            foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Register")).ToList())
            {
                ModelState.Remove(key);
            }

            // Revalidate only the Login model
            if (model.Login == null)
            {
                ModelState.AddModelError(string.Empty, "Login form data is missing.");
                TempData["Error"] = "Please fill in the login form.";
                return View("Auth", new AuthViewModel { Login = new LoginViewModel() });
            }

            TryValidateModel(model.Login);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("Validation Errors: " + string.Join(", ", errors));
                TempData["Error"] = "Please correct the errors in the form: " + string.Join(", ", errors);
                return View("Auth", new AuthViewModel { Login = model.Login });
            }

            var loginModel = model.Login;
            var user = await _userManager.FindByEmailAsync(loginModel.Email ?? throw new InvalidOperationException("Email is required"));
            if (user == null)
            {
                Console.WriteLine($"Login failed: User with email {loginModel.Email} not found.");
                TempData["Error"] = "Invalid email or password.";
                return View("Auth", new AuthViewModel { Login = loginModel });
            }

            if (user.UserName == null)
            {
                Console.WriteLine($"Login failed: User with email {loginModel.Email} has no username.");
                TempData["Error"] = "User does not have a valid username.";
                return View("Auth", new AuthViewModel { Login = loginModel });
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, loginModel.Password ?? throw new InvalidOperationException("Password is required"), isPersistent: true, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                Console.WriteLine($"Login failed: Invalid password for user {loginModel.Email}.");
                TempData["Error"] = "Invalid email or password.";
                return View("Auth", new AuthViewModel { Login = loginModel });
            }

            // Store role in session
            HttpContext.Session.SetString("UserRole", user.Role);

            // Store username and email in cookies
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Secure = false, // Set to false for local development (HTTP)
                SameSite = SameSiteMode.Strict
            };
            Response.Cookies.Append("UserName", user.UserName, cookieOptions);
            Response.Cookies.Append("UserEmail", user.Email ?? "", cookieOptions);

            // Redirect based on role
            Console.WriteLine($"Login successful: {user.Email ?? "null"}, Role: {user.Role}");
            if (user.Role == "Customer")
            {
                return RedirectToAction("Index", "Home");
            }
            else if (user.Role == "Admin")
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                Console.WriteLine($"Login failed: Invalid role {user.Role} for user {user.Email ?? "null"}.");
                TempData["Error"] = "Invalid user role.";
                return View("Auth", new AuthViewModel { Login = loginModel });
            }
        }


    }
}
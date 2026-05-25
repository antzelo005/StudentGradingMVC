using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentGradingMVC.Models;

namespace StudentGradingMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly StudentGradingContext _context;

        public AccountController(StudentGradingContext context)
        {
            _context = context;
        }

        // =========================
        // GET: Login
        // =========================
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // =========================
        // POST: Login
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users
                .AsNoTracking()
                .FirstOrDefault(u =>
                    u.Username == model.Username &&
                    u.Password == model.Password &&
                    u.Role == model.Role
                );

            if (user == null)
            {
                ModelState.AddModelError("", "Wrong username / password / role");
                return View(model);
            }

            var roleValue = (user.Role ?? "").Trim();
            var usernameValue = (user.Username ?? "").Trim();

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Secure = Request.IsHttps,   // Important for localhost
                Expires = DateTimeOffset.UtcNow.AddHours(2)
            };

            Response.Cookies.Append("UserRole", roleValue, cookieOptions);
            Response.Cookies.Append("Username", usernameValue, cookieOptions);


            return user.Role switch
            {
                "Student" => RedirectToAction("Index", "Student"),
                "Professor" => RedirectToAction("Index", "Professor"),
                "Secretary" => RedirectToAction("Index", "Secretary"),
                _ => RedirectToAction("Login", "Account")
            };
        }

        // =========================
        // POST: Logout
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("UserRole");
            Response.Cookies.Delete("Username");

            return RedirectToAction("Login", "Account");
        }
    }
}

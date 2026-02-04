using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp_Anti.Data;
using WebApp_Anti.Utils;

namespace WebApp_Anti.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Username and password are required.";
                return View();
            }

            var user = await _context.AppUsers.AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null || !PasswordUtils.Verify(password, user.PasswordHash))
            {
                ViewBag.Error = "Invalid credentials.";
                return View();
            }

            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("UserRole", user.Role);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}

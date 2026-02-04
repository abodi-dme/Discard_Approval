using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp_Anti.Data;
using WebApp_Anti.Models;
using WebApp_Anti.Utils;

namespace WebApp_Anti.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin";
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            if (!IsAdmin())
                return Forbid();

            var users = await _context.AppUsers.AsNoTracking()
                .OrderBy(u => u.UserName)
                .ToListAsync();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string username, string password, string role)
        {
            if (!IsAdmin())
                return Forbid();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                TempData["AdminError"] = "Username and password are required.";
                return RedirectToAction("Users");
            }

            if (role != "Manager" && role != "VP" && role != "Admin")
            {
                TempData["AdminError"] = "Invalid role.";
                return RedirectToAction("Users");
            }

            var exists = await _context.AppUsers.AnyAsync(u => u.UserName == username);
            if (exists)
            {
                TempData["AdminError"] = "User already exists.";
                return RedirectToAction("Users");
            }

            var user = new AppUser
            {
                UserName = username.Trim(),
                PasswordHash = PasswordUtils.Hash(password),
                Role = role
            };

            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();

            TempData["AdminSuccess"] = "User created.";
            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(int id, string role)
        {
            if (!IsAdmin())
                return Forbid();

            if (role != "Manager" && role != "VP" && role != "Admin")
            {
                TempData["AdminError"] = "Invalid role.";
                return RedirectToAction("Users");
            }

            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                TempData["AdminError"] = "User not found.";
                return RedirectToAction("Users");
            }

            user.Role = role;
            await _context.SaveChangesAsync();

            TempData["AdminSuccess"] = "Role updated.";
            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(int id, string password)
        {
            if (!IsAdmin())
                return Forbid();

            if (string.IsNullOrWhiteSpace(password))
            {
                TempData["AdminError"] = "Password is required.";
                return RedirectToAction("Users");
            }

            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                TempData["AdminError"] = "User not found.";
                return RedirectToAction("Users");
            }

            user.PasswordHash = PasswordUtils.Hash(password);
            await _context.SaveChangesAsync();

            TempData["AdminSuccess"] = "Password reset.";
            return RedirectToAction("Users");
        }
    }
}

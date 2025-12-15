using HabitTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HabitTracker.Data; 
using System.Security.Claims; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Controllers
{
    [Authorize] 
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return RedirectToAction("Login", "Account");

            var user = _context.Users.Find(int.Parse(userId));
            ViewBag.CurrentUser = user;

            var myHabits = _context.Habits
                                   .Include(h => h.HabitCompletions)
                                   .Include(h => h.Reminder)
                                   .Where(h => h.UserID == int.Parse(userId))
                                   .OrderByDescending(h => h.CreationDate) 
                                   .ToList();

            return View(myHabits);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult MakeMeAdmin()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _context.Users.Find(userId);

            if (user != null)
            {
                user.Role = "Admin"; 
                _context.SaveChanges();
                return Content("TEBRİKLER! Artık bu sistemin Adminisiniz. 👑");
            }

            return Content("Hata: Önce giriş yapmalısın.");
        }
    }
}
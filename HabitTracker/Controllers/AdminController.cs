using Microsoft.AspNetCore.Mvc;
using HabitTracker.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using HabitTracker.Models;

namespace HabitTracker.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _context.Users.Find(userId);
            return user != null && user.Role == "Admin";
        }

        public IActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.TotalHabits = _context.Habits.Count();
            ViewBag.TotalCompletions = _context.HabitCompletions.Count();

            var chartLabels = new List<string>();
            var chartData = new List<int>();

            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.Today.AddDays(-i);
                var count = _context.Users.Count(u => u.CreationDate.Date == date);

                chartLabels.Add(date.ToString("dd MMM"));
                chartData.Add(count);
            }

            var model = new AdminViewModel
            {
                Users = _context.Users.OrderByDescending(u => u.UserID).ToList(),

                Habits = _context.Habits.Include(h => h.User).OrderByDescending(h => h.CreationDate).ToList(),

                RecentActivities = _context.HabitCompletions
                                           .Include(hc => hc.Habit)
                                           .ThenInclude(h => h.User) 
                                           .OrderByDescending(hc => hc.CompletionDate) 
                                           .Take(10) 
                                           .ToList(),

                
                ChartLabels = chartLabels,
                ChartData = chartData
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ViewBag.Error = "Bu email zaten kayıtlı!";
                return View(user);
            }

            user.PasswordHash = HabitTracker.Helpers.PasswordHelper.HashPassword(user.PasswordHash);
            user.CreationDate = DateTime.Now;
            user.Level = 1;
            user.ExperiencePoints = 0;

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public IActionResult EditUser(User user)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");
            var existingUser = _context.Users.Find(user.UserID);
            if (existingUser != null)
            {
                existingUser.Username = user.Username;
                existingUser.Email = user.Email;
                existingUser.Role = user.Role;
                if (!string.IsNullOrEmpty(user.PasswordHash))
                {
                    existingUser.PasswordHash = HabitTracker.Helpers.PasswordHelper.HashPassword(user.PasswordHash);
                }
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteHabit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var habit = _context.Habits.Find(id);
            if (habit != null)
            {
                var completions = _context.HabitCompletions.Where(hc => hc.HabitID == id);
                _context.HabitCompletions.RemoveRange(completions);

                _context.Habits.Remove(habit);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
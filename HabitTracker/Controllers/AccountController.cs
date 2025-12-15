using Microsoft.AspNetCore.Mvc;
using HabitTracker.Data;
using HabitTracker.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using HabitTracker.Helpers;
using System.IO;

namespace HabitTracker.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AccountController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var hashedPassword = PasswordHelper.HashPassword(password);

            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == hashedPassword);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email veya şifre hatalı!";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ViewBag.Error = "Bu email zaten kayıtlı!";
                return View();
            }

            user.PasswordHash = PasswordHelper.HashPassword(user.PasswordHash);

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [Authorize]
        public IActionResult Profile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var user = _context.Users
                                   .Include(u => u.UserBadges)
                                   .ThenInclude(ub => ub.Badge)
                                   .FirstOrDefault(u => u.UserID == userId);

            var completionDates = _context.HabitCompletions
                                          .Where(hc => hc.Habit.UserID == userId)
                                          .Select(hc => hc.CompletionDate.Date)
                                          .Distinct()
                                          .OrderByDescending(d => d)
                                          .ToList();

            int totalCompletedCount = _context.HabitCompletions.Count(hc => hc.Habit.UserID == userId);

            int currentStreak = 0;
            var checkDate = DateTime.Today;

            if (completionDates.Contains(DateTime.Today))
            {
                currentStreak = 1;
                checkDate = DateTime.Today.AddDays(-1);
            }
            else if (completionDates.Contains(DateTime.Today.AddDays(-1)))
            {
                currentStreak = 0;
                checkDate = DateTime.Today.AddDays(-1);
            }
            else
            {
                currentStreak = 0;
            }

            if (currentStreak > 0 || checkDate == DateTime.Today.AddDays(-1))
            {
                while (completionDates.Contains(checkDate))
                {
                    currentStreak++;
                    checkDate = checkDate.AddDays(-1);
                }
            }

            int longestStreak = 0;
            int tempStreak = 0;
            var orderedDates = completionDates.OrderBy(d => d).ToList();

            for (int i = 0; i < orderedDates.Count; i++)
            {
                if (i == 0)
                {
                    tempStreak = 1;
                }
                else
                {
                    if ((orderedDates[i] - orderedDates[i - 1]).Days == 1)
                    {
                        tempStreak++;
                    }
                    else
                    {
                        if (tempStreak > longestStreak) longestStreak = tempStreak;
                        tempStreak = 1;
                    }
                }
            }
            if (tempStreak > longestStreak) longestStreak = tempStreak;

            var last30DaysActivityCount = completionDates.Count(d => d > DateTime.Today.AddDays(-30));
            int completionRate = (int)((double)last30DaysActivityCount / 30 * 100);
            if (completionRate > 100) completionRate = 100;

            var correctPoints = totalCompletedCount * 10;
            var correctLevel = 1 + (correctPoints / 100);
            if (user.ExperiencePoints != correctPoints)
            {
                user.ExperiencePoints = correctPoints;
                user.Level = correctLevel;
                _context.SaveChanges();
            }


            var categoryStats = _context.Habits
                                        .Where(h => h.UserID == userId)
                                        .GroupBy(h => h.Category)
                                        .Select(g => new { Name = g.Key, Count = g.Count() })
                                        .ToList();

            var last7DaysActivity = new List<int>();
            var last7DaysNames = new List<string>();

            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.Today.AddDays(-i);
                int count = _context.HabitCompletions
                                    .Count(hc => hc.Habit.UserID == userId && hc.CompletionDate == date);

                last7DaysActivity.Add(count);
                last7DaysNames.Add(date.ToString("dd MMM"));
            }

            var model = new ProfileViewModel
            {
                User = user,
                TotalCompleted = totalCompletedCount,
                CurrentStreak = currentStreak,
                LongestStreak = longestStreak,
                CompletionRate = completionRate,
                EarnedBadges = user.UserBadges.ToList(),

                Categories = categoryStats.Select(x => x.Name).ToList(),
                CategoryCounts = categoryStats.Select(x => x.Count).ToList(),
                Last7Days = last7DaysNames,
                Last7DaysActivity = last7DaysActivity
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(IFormFile profileImage)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _context.Users.Find(userId);

            if (profileImage != null && profileImage.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(profileImage.FileName);
                var uploadFolder = Path.Combine(_environment.WebRootPath, "profile-images");

                if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(stream);
                }

                user.ProfileImageUrl = "/profile-images/" + fileName;
                _context.SaveChanges();
            }

            return RedirectToAction("Profile");
        }
    }
}
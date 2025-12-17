using Microsoft.AspNetCore.Mvc;
using HabitTracker.Data;
using HabitTracker.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Controllers
{
    [Authorize]
    public class HabitController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HabitController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Habit habit)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            habit.UserID = int.Parse(userId);

            habit.CreationDate = DateTime.Now;

            _context.Habits.Add(habit);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var habit = _context.Habits.FirstOrDefault(h => h.HabitID == id && h.UserID == userId);
            var user = _context.Users.Find(userId);

            if (habit == null || user == null) return NotFound();

            var today = DateTime.Today;
            var existingCompletion = _context.HabitCompletions
                                            .FirstOrDefault(hc => hc.HabitID == id && hc.CompletionDate == today);

            if (existingCompletion != null)
            {
                _context.HabitCompletions.Remove(existingCompletion);

                user.ExperiencePoints = Math.Max(0, user.ExperiencePoints - 10);

                user.Level = 1 + (user.ExperiencePoints / 100);

                _context.SaveChanges();
            }
            else
            {
                var completion = new HabitCompletion
                {
                    HabitID = id,
                    CompletionDate = today,
                };
                _context.HabitCompletions.Add(completion);

                user.ExperiencePoints += 10;
                user.Level = 1 + (user.ExperiencePoints / 100);

                _context.SaveChanges();

                CheckBadges(userId);
            }

            return Ok();
        }

        private void CheckBadges(int userId)
        {
            var user = _context.Users.Include(u => u.UserBadges).FirstOrDefault(u => u.UserID == userId);

            int completedCount = _context.HabitCompletions.Count(hc => hc.Habit.UserID == userId);

            int badgeIdToEarn = 0;

            if (completedCount == 1) badgeIdToEarn = 1;
            else if (completedCount == 5) badgeIdToEarn = 2;
            else if (completedCount == 10) badgeIdToEarn = 3;
            else if (completedCount == 50) badgeIdToEarn = 4;

            if (badgeIdToEarn > 0)
            {
                bool alreadyHasIt = user.UserBadges.Any(ub => ub.BadgeID == badgeIdToEarn);

                if (!alreadyHasIt)
                {
                    var newBadge = new UserBadge
                    {
                        UserID = userId,
                        BadgeID = badgeIdToEarn,
                        DateEarned = DateTime.Now
                    };
                    _context.UserBadges.Add(newBadge);
                    _context.SaveChanges();
                }
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var habit = _context.Habits.FirstOrDefault(h => h.HabitID == id && h.UserID == userId);

            if (habit != null)
            {
                var completionCount = _context.HabitCompletions.Count(hc => hc.HabitID == id);

                if (completionCount > 0)
                {
                    var user = _context.Users.Find(userId);

                    int pointsToRemove = completionCount * 10;
                    user.ExperiencePoints = Math.Max(0, user.ExperiencePoints - pointsToRemove);

                    user.Level = 1 + (user.ExperiencePoints / 100);
                }

                var completions = _context.HabitCompletions.Where(hc => hc.HabitID == id);
                _context.HabitCompletions.RemoveRange(completions);

                _context.Habits.Remove(habit);

                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var habit = _context.Habits.Include(h => h.Reminder)
                                   .FirstOrDefault(h => h.HabitID == id && h.UserID == userId);

            if (habit == null) return NotFound();

            return View(habit);
        }

        [HttpPost]
        public IActionResult Edit(Habit habit)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var existingHabit = _context.Habits.Include(h => h.Reminder)
                                               .FirstOrDefault(h => h.HabitID == habit.HabitID && h.UserID == userId);

            if (existingHabit != null)
            {
                existingHabit.Name = habit.Name;
                existingHabit.Category = habit.Category;
                existingHabit.Frequency = habit.Frequency;


                if (habit.Reminder != null && habit.Reminder.IsActive)
                {
                    if (existingHabit.Reminder == null)
                    {
                        existingHabit.Reminder = new Reminder
                        {
                            ReminderTime = habit.Reminder.ReminderTime,
                            IsActive = true
                        };
                    }
                    else
                    {
                        existingHabit.Reminder.ReminderTime = habit.Reminder.ReminderTime;
                        existingHabit.Reminder.IsActive = true;
                    }
                }
                else
                {
                    if (existingHabit.Reminder != null)
                    {
                        _context.Reminders.Remove(existingHabit.Reminder);
                    }
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
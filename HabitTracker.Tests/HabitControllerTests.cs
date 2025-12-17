using Xunit;
using Microsoft.EntityFrameworkCore;
using HabitTracker.Controllers;
using HabitTracker.Data;
using HabitTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HabitTracker.Tests
{
    public class HabitControllerTests
    {
        private ApplicationDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public void Home_Index_ReturnsViewResult_WithListOfHabits()
        {
            var context = GetDatabaseContext();

            var user = new User
            {
                UserID = 1,
                Username = "TestUser",
                Email = "test@test.com",
                Role = "User",
                PasswordHash = "RastgeleSifre123" 
            };

            context.Users.Add(user);
            context.Habits.Add(new Habit { HabitID = 100, Name = "Kitap Oku", UserID = 1, Category = "Eðitim" });
            context.SaveChanges();

            var controller = new HomeController(context);

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "TestUser")
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userClaims }
            };

            var result = controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Habit>>(viewResult.ViewData.Model);

            Assert.Single(model);
            Assert.Equal("Kitap Oku", model[0].Name);
        }

        [Fact]
        public void Habit_Create_Post_AddsHabitToDatabase()
        {
            var context = GetDatabaseContext();

            context.Users.Add(new User
            {
                UserID = 1,
                Username = "Mustafa",
                Email = "mustafa@test.com",
                PasswordHash = "RastgeleSifre123" 
            });
            context.SaveChanges();

            var controller = new HabitController(context);

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userClaims }
            };

            var newHabit = new Habit { Name = "Spor Yap", Category = "Saðlýk" };

            var result = controller.Create(newHabit);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            var habitInDb = context.Habits.FirstOrDefault(h => h.Name == "Spor Yap");
            Assert.NotNull(habitInDb);
            Assert.Equal("Saðlýk", habitInDb.Category);
        }
    }
}
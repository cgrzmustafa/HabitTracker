using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public string? ProfileImageUrl { get; set; }

        public int ExperiencePoints { get; set; } = 0;
        public int Level { get; set; } = 1;

        public string Role { get; set; } = "User"; 

        public DateTime CreationDate { get; set; } = DateTime.Now;

        public ICollection<Habit> Habits { get; set; }
        public ICollection<UserBadge> UserBadges { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Models
{
    public class Habit
    {
        [Key]
        public int HabitID { get; set; }

        [Required]
        public string Name { get; set; }

        public string Category { get; set; }

        public string Frequency { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;

        // İlişkiler
        public int UserID { get; set; }
        public User User { get; set; }

        public ICollection<HabitCompletion> HabitCompletions { get; set; }

        public Reminder Reminder { get; set; }
    }
}
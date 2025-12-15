using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Models
{
    public class Reminder
    {
        [Key]
        public int ReminderID { get; set; }

        public TimeSpan ReminderTime { get; set; }
        public bool IsActive { get; set; }

        // İlişkiler
        public int HabitID { get; set; }
        public Habit Habit { get; set; }
    }
}
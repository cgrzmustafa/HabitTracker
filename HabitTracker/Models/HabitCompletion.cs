using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Models
{
    public class HabitCompletion
    {
        [Key]
        public int CompletionID { get; set; }

        public DateTime CompletionDate { get; set; }

        // İlişkiler
        public int HabitID { get; set; }
        public Habit Habit { get; set; }
    }
}
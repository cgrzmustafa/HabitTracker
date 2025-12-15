using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Models
{
    public class UserBadge
    {
        [Key]
        public int UserBadgeID { get; set; }

        public DateTime DateEarned { get; set; } = DateTime.Now;

        // İlişkiler
        public int UserID { get; set; }
        public User User { get; set; }

        public int BadgeID { get; set; }
        public Badge Badge { get; set; }
    }
}
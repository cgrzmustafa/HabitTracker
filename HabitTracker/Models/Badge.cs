using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Models
{
    public class Badge
    {
        [Key]
        public int BadgeID { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Criteria { get; set; }

        public ICollection<UserBadge> UserBadges { get; set; }
    }
}
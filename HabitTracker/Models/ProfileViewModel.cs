namespace HabitTracker.Models
{
    public class ProfileViewModel
    {
        public User User { get; set; }

        public int TotalCompleted { get; set; }       
        public int CurrentStreak { get; set; }        
        public int LongestStreak { get; set; }        
        public int CompletionRate { get; set; }       

        public List<UserBadge> EarnedBadges { get; set; }

        public List<string> Categories { get; set; } 
        public List<int> CategoryCounts { get; set; } 

        public List<string> Last7Days { get; set; } 
        public List<int> Last7DaysActivity { get; set; } 
    }
}
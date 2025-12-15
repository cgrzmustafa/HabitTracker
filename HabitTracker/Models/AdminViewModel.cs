using System.Collections.Generic;

namespace HabitTracker.Models
{
    public class AdminViewModel
    {
        public List<User> Users { get; set; }
        public List<Habit> Habits { get; set; }


        public List<HabitCompletion> RecentActivities { get; set; }

        public List<string> ChartLabels { get; set; } 
        public List<int> ChartData { get; set; }      
    }
}
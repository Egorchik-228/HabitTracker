using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace HabitTracker.Views
{
    public partial class ReminderListView : Window
    {
        public class NumberedReminder
        {
            public string Number { get; set; }
            public string Time { get; set; }
        }

        public ReminderListView(List<string> reminderTimes)
        {
            InitializeComponent();
            DataContext = reminderTimes
                .Select((time, index) => new NumberedReminder { Number = $"{index + 1}.", Time = time })
                .ToList();
        }
    }
}

using HabitTracker.Models;
using HabitTracker.Services;
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

        private readonly HabitService _habitService;
        private readonly Habit _habit;

        public ReminderListView(int habitId)
        {
            InitializeComponent();
            _habitService = new HabitService();
            _habit = _habitService.GetHabitById(habitId);

            LoadReminders();
        }

        private void LoadReminders()
        {
            if (_habit?.ReminderTimes != null && _habit.ReminderTimes.Any())
            {
                DataContext = _habit.ReminderTimes
                    .Select((time, index) => new NumberedReminder
                    {
                        Number = $"{index + 1}.",
                        Time = time
                    })
                    .ToList();
            }
            else
            {
                DataContext = new List<NumberedReminder>();
                MessageBox.Show("Для этой привычки нет напоминаний", "Информация",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
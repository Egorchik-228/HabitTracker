using HabitTracker.Models;
using HabitTracker.Services;
using HabitTracker.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace HabitTracker
{
    public partial class MainWindow : Window
    {
        private HabitService _habitService;
        private List<Habit> _habits;
        private Timer _reminderTimer;

        public MainWindow()
        {
            InitializeComponent();
            _habitService = new HabitService();
            LoadHabits();
            StartReminderTimer();
        }

        private void LoadHabits()
        {
            _habits = _habitService.LoadHabit();

            foreach (var habit in _habits)
            {
                if (habit.ReminderTimes == null)
                {
                    habit.ReminderTimes = new List<string>();
                }
            }

            HabitListView.ItemsSource = null;
            HabitListView.ItemsSource = _habits;
        }

        private void AddHabit_Click(object sender, RoutedEventArgs e)
        {
            HabitView habitView = new HabitView();
            if (habitView.ShowDialog() == true)
            {
                _habits.Add(habitView.Habit);
                _habitService.SaveHabits(_habits);
                LoadHabits();
            }
        }

        private void EditHabit_Click(object sender, RoutedEventArgs e)
        {
            if (HabitListView.SelectedItem is Habit selectedHabit)
            {
                HabitView habitView = new HabitView(selectedHabit);
                if (habitView.ShowDialog() == true)
                {
                    _habitService.SaveHabits(_habits);
                    LoadHabits();
                }
            }
        }

        private void DeleteHabit_Click(object sender, RoutedEventArgs e)
        {
            if (HabitListView.SelectedItem is Habit selectedHabit)
            {
                _habits.Remove(selectedHabit);
                _habitService.SaveHabits(_habits);
                LoadHabits();
            }
        }

        private void OpenAnalytics_Click(object sender, RoutedEventArgs e)
        {
            AnalyticsView analyticsWindow = new AnalyticsView();
            analyticsWindow.Show();
        }

        private void MarkHabitCompleted_Click(object sender, RoutedEventArgs e)
        {
            if (HabitListView.SelectedItem is Habit selectedHabit)
            {
                _habitService.MarkHabitAsCompleted(selectedHabit);
                LoadHabits();
            }
        }

        private void ShowMoreReminders_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Habit habit)
            {
                ReminderListView reminderWindow = new ReminderListView(habit.ReminderTimes);
                reminderWindow.ShowDialog();
            }
        }

        private void StartReminderTimer()
        {
            _reminderTimer = new Timer(60000);
            _reminderTimer.Elapsed += CheckReminders;
            _reminderTimer.AutoReset = true;
            _reminderTimer.Enabled = true;
        }

        private void CheckReminders(object sender, ElapsedEventArgs e)
        {
            foreach (var habit in _habits.Where(h => h.HasReminder))
            {
                foreach (var reminder in habit.ReminderTimes)
                {
                    if (TimeSpan.TryParse(reminder, out TimeSpan reminderTime) && DateTime.Now.TimeOfDay >= reminderTime)
                    {
                        Dispatcher.Invoke(() =>
                            MessageBox.Show($"Напоминание: {habit.Name} - {reminderTime:hh\\:mm}",
                            "Напоминание", MessageBoxButton.OK, MessageBoxImage.Information));
                    }
                }
            }
        }
    }
}

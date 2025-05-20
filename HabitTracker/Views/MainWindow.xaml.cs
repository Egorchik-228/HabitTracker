using HabitTracker.Models;
using HabitTracker.Services;
using HabitTracker.Views;
using System;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace HabitTracker
{
    public partial class MainWindow : Window
    {
        private readonly HabitService _habitService;
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
            var habits = _habitService.LoadHabits();
            HabitListView.ItemsSource = habits;
        }

        private void AddHabit_Click(object sender, RoutedEventArgs e)
        {
            var habitView = new HabitView();
            if (habitView.ShowDialog() == true)
            {
                LoadHabits();
            }
        }

        private void EditHabit_Click(object sender, RoutedEventArgs e)
        {
            if (HabitListView.SelectedItem is Habit selectedHabit)
            {
                var habitView = new HabitView(selectedHabit);
                if (habitView.ShowDialog() == true)
                {
                    LoadHabits();
                }
            }
        }

        private void DeleteHabit_Click(object sender, RoutedEventArgs e)
        {
            if (HabitListView.SelectedItem is Habit selectedHabit)
            {
                if (MessageBox.Show($"Удалить привычку '{selectedHabit.Name}'?", "Подтверждение",
                                   MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _habitService.DeleteHabit(selectedHabit);
                    LoadHabits();
                }
            }
        }

        private void OpenAnalytics_Click(object sender, RoutedEventArgs e)
        {
            new AnalyticsView().Show();
        }

        private void MarkHabitCompleted_Click(object sender, RoutedEventArgs e)
        {
            if (HabitListView.SelectedItem is Habit selectedHabit)
            {
                bool marked = _habitService.MarkHabitAsCompleted(selectedHabit);

                if (marked)
                {
                    LoadHabits();
                    MessageBox.Show($"Привычка отмечена! ({selectedHabit.TodayCompletions}/{selectedHabit.DailyTarget})",
                                  "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Лимит: {selectedHabit.DailyTarget} раз(а) в день",
                                  "Превышен лимит", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void ShowMoreReminders_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Habit habit)
            {
                new ReminderListView(habit.Id).ShowDialog();
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
            var now = DateTime.Now;
            var habits = _habitService.LoadHabits().Where(h => h.HasReminder);

            foreach (var habit in habits)
            {
                foreach (var reminder in habit.ReminderTimes)
                {
                    if (TimeSpan.TryParse(reminder, out TimeSpan reminderTime) &&
                        now.Hour == reminderTime.Hours && now.Minute == reminderTime.Minutes)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            new NotificationWindow(
                                $"Напоминание: {habit.Name}",
                                $"Время: {reminderTime:hh\\:mm}"
                            ).Show();
                        });
                    }
                }
            }
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
using HabitTracker.Models;
using HabitTracker.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HabitTracker.Views
{
    public partial class HabitView : Window
    {
        private readonly HabitService _habitService;
        public Habit Habit { get; private set; }
        public ObservableCollection<string> ReminderTimes { get; set; }

        public HabitView(Habit habit = null)
        {
            InitializeComponent();
            _habitService = new HabitService();

            Habit = habit ?? new Habit { StartDate = DateTime.Today };
            ReminderTimes = new ObservableCollection<string>(Habit.ReminderTimes ?? new List<string>());

            ReminderTimesList.ItemsSource = ReminderTimes;
            HabitNameTextBox.Text = Habit.Name;
            StartDatePicker.SelectedDate = Habit.StartDate;
            DescriptionTextBox.Text = Habit.Description;
            StartDatePicker.DisplayDateStart = DateTime.Today;
        }

        private void AddReminder_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewReminderTimeTextBox.Text))
            {
                MessageBox.Show("Введите время напоминания!", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (TimeSpan.TryParseExact(NewReminderTimeTextBox.Text, "hh\\:mm",
                CultureInfo.InvariantCulture, out _))
            {
                ReminderTimes.Add(NewReminderTimeTextBox.Text);
                NewReminderTimeTextBox.Text = "";
            }
            else
            {
                MessageBox.Show("Введите время в формате ЧЧ:мм", "Некорректный формат",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RemoveReminder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string reminderTime)
            {
                ReminderTimes.Remove(reminderTime);
            }
        }

        private void SaveHabit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(HabitNameTextBox.Text) || StartDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Заполните все обязательные поля!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            foreach (var time in ReminderTimes)
            {
                if (!TimeSpan.TryParseExact(time, "hh\\:mm", CultureInfo.InvariantCulture, out _))
                {
                    MessageBox.Show($"Некорректный формат времени: {time}. Используйте формат ЧЧ:мм",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            DateTime selectedDate = StartDatePicker.SelectedDate.Value;

            if (Habit.Id == 0 && selectedDate < DateTime.Today)
            {
                MessageBox.Show("Нельзя выбрать прошедшую дату!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Habit.Name = HabitNameTextBox.Text;
            Habit.StartDate = selectedDate;
            Habit.Description = DescriptionTextBox.Text;
            Habit.ReminderTimes = ReminderTimes.ToList();
            Habit.HasReminder = Habit.ReminderTimes.Any();

            try
            {
                _habitService.SaveHabit(Habit);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public class IndexToNumberConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value is int index ? $"Время {index + 1}" : "Время ?";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
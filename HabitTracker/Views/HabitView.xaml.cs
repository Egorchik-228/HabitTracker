using HabitTracker.Models;
using System;
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
        public Habit Habit { get; private set; }
        public ObservableCollection<string> ReminderTimes { get; set; }

        public HabitView(Habit habit = null)
        {
            InitializeComponent();
            Habit = habit ?? new Habit();

            // Инициализируем список ReminderTimes, чтобы он всегда ссылался на данные Habit
            ReminderTimes = habit?.ReminderTimes != null
                ? new ObservableCollection<string>(habit.ReminderTimes)
                : new ObservableCollection<string>();

            // Если список пуст (новая привычка или старая без напоминаний), то  добавляем одно время по умолчанию
            if (ReminderTimes.Count == 0)
            {
                ReminderTimes.Add("08:00");
            }

            ReminderTimesList.ItemsSource = ReminderTimes; // Привязываем список времени напоминаний

            if (habit != null)
            {
                HabitNameTextBox.Text = habit.Name;
                StartDatePicker.SelectedDate = habit.StartDate;
                DescriptionTextBox.Text = habit.Description;
            }

            StartDatePicker.DisplayDateStart = DateTime.Today; // Блокируем выбор прошедшей даты
        }

        private void AddReminder_Click(object sender, RoutedEventArgs e)
        {
            ReminderTimes.Add("08:00"); // Добавляет новое время напоминания
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
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime selectedDate = StartDatePicker.SelectedDate.Value;

            // Проверяем, что дата не в прошлом, только если это новая привычка
            if (Habit.StartDate == default(DateTime) && selectedDate < DateTime.Today)
            {
                MessageBox.Show("Нельзя выбрать прошедшую дату!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Habit.Name = HabitNameTextBox.Text;
            Habit.StartDate = selectedDate;
            Habit.Description = DescriptionTextBox.Text;
            Habit.ReminderTimes = ReminderTimes.ToList();

            DialogResult = true;
            Close();
        }


        public class IndexToNumberConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is int index)
                    return $"Время {index + 1}";
                return "Время ?";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}

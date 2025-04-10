using HabitTracker.Models;
using HabitTracker.Services;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HabitTracker.Views
{
    public partial class AnalyticsView : Window, INotifyPropertyChanged
    {
        private List<Habit> _habits;
        private PlotModel _plotModel;

        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set { _plotModel = value; OnPropertyChanged(nameof(PlotModel)); }
        }

        public AnalyticsView()
        {
            InitializeComponent();
            LoadHabits();
            DataContext = this; // Устанавливаем DataContext для привязки к PlotModel
            PlotModel = new PlotModel { Title = "Прогресс выполнения" }; // Инициализация модели графика
        }

        private void LoadHabits()
        {
            HabitService habitService = new HabitService();
            _habits = habitService.LoadHabit();

            HabitComboBox.ItemsSource = _habits;
            HabitComboBox.DisplayMemberPath = "Name";
        }

        private void HabitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //Информация о привычке
        {
            if (HabitComboBox.SelectedItem is Habit selectedHabit)
            {
                StartDateText.Text = selectedHabit.StartDate.ToString("dd.MM.yyyy");
                DescriptionText.Text = string.IsNullOrWhiteSpace(selectedHabit.Description) ? "Нет описания" : selectedHabit.Description;

                // Обновление информации о напоминаниях
                if (selectedHabit.ReminderTimes.Any())
                {
                    string reminders = string.Join("\n", selectedHabit.ReminderTimes);
                    ReminderText.Text = reminders;
                }
                else
                {
                    ReminderText.Text = "Нет напоминаний";
                }

                // Вычисление прогресса выполнения привычки
                int totalDays = (DateTime.Today - selectedHabit.StartDate).Days + 1;
                int completedDays = selectedHabit.CompletedDays;
                double completionRate = totalDays > 0 ? (completedDays / (double)totalDays) * 100 : 0;

                CompletionPercentageText.Text = $"Выполнено: {completionRate:F1}% ({completedDays} из {totalDays} дней)";

                // Обновление графика
                UpdateChart(selectedHabit);
            }
            else
            {
                StartDateText.Text = "";
                DescriptionText.Text = "";
                CompletionPercentageText.Text = "";
                ReminderText.Text = "";
                PlotModel = new PlotModel { Title = "Прогресс выполнения" }; // Очистка графика
            }
        }

        private void UpdateChart(Habit habit) //График
        {
            if (habit != null)
            {
                PlotModel = new PlotModel { Title = habit.Name }; // Заголовок графика - имя привычки
                PlotModel.Axes.Clear();
                PlotModel.Series.Clear();

                // Ось X - Даты
                PlotModel.Axes.Add(new DateTimeAxis
                {
                    Title = "Дата",
                    StringFormat = "dd.MM",
                    Minimum = DateTimeAxis.ToDouble(habit.StartDate.Date),
                    Maximum = DateTimeAxis.ToDouble(DateTime.Today.Date)
                });

                // Ось Y - Выполнено (0 или 1)
                PlotModel.Axes.Add(new LinearAxis
                {
                    Title = "Выполнено",
                    Minimum = -0.1, // Небольшой запас снизу
                    Maximum = 1.1,  // Небольшой запас сверху
                    MajorStep = 1,
                    MinorStep = 1,
                    TickStyle = TickStyle.None // Скрываем риски на оси Y
                });

                // Серия - Линейный график выполнения
                var lineSeries = new LineSeries
                {
                    Title = "Выполнение",
                    StrokeThickness = 2,
                    MarkerSize = 3,
                    MarkerType = MarkerType.Circle
                };

                for (int i = 0; i <= (DateTime.Today - habit.StartDate).Days; i++)
                {
                    DateTime currentDate = habit.StartDate.AddDays(i).Date;
                    double yValue = habit.CompletedDates.Contains(currentDate) ? 1 : 0;
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(currentDate), yValue));
                }

                PlotModel.Series.Add(lineSeries);

                // Обновление модели графика
                PlotModel.InvalidatePlot(true);
            }
            else
            {
                PlotModel = new PlotModel { Title = "Прогресс выполнения" };
            }
        }

        private void CloseAnalytics_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
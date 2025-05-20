using HabitTracker.Models;
using HabitTracker.Services;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HabitTracker.Views
{
    public partial class AnalyticsView : Window, INotifyPropertyChanged
    {
        private readonly HabitService _habitService;
        private PlotModel _plotModel;

        public PlotModel PlotModel
        {
            get => _plotModel;
            set { _plotModel = value; OnPropertyChanged(nameof(PlotModel)); }
        }

        public AnalyticsView()
        {
            InitializeComponent();
            _habitService = new HabitService();
            DataContext = this;
            PlotModel = new PlotModel { Title = "Прогресс выполнения" };
            LoadHabits();
        }

        private void LoadHabits()
        {
            var habits = _habitService.LoadHabits();
            HabitComboBox.ItemsSource = habits;
            HabitComboBox.DisplayMemberPath = "Name";
        }

        private void HabitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HabitComboBox.SelectedItem is Habit selectedHabit)
            {
                UpdateUI(selectedHabit);
                UpdateChart(selectedHabit);
            }
            else
            {
                ClearUI();
            }
        }

        private void UpdateUI(Habit habit)
        {
            StartDateText.Text = habit.StartDate.ToString("dd.MM.yyyy");
            DescriptionText.Text = string.IsNullOrWhiteSpace(habit.Description) ? "Нет описания" : habit.Description;
            ReminderText.Text = habit.ReminderTimes.Any() ? string.Join("\n", habit.ReminderTimes) : "Нет напоминаний";

            int todayCompletions = habit.TodayCompletions;
            int dailyTarget = habit.DailyTarget;
            double todayCompletionRate = dailyTarget > 0 ? (todayCompletions / (double)dailyTarget) * 100 : 0;
            CompletionPercentageText.Text = $"Выполнено сегодня: {todayCompletions}/{dailyTarget} ({todayCompletionRate:F1}%)";

            int totalCompletions = habit.TotalCompletions;
            int totalDays = (DateTime.Today - habit.StartDate.Date).Days + 1;
            int totalTarget = totalDays * dailyTarget;
            double totalCompletionRate = totalTarget > 0 ? (totalCompletions / (double)totalTarget) * 100 : 0;
            TotalCompletionText.Text = $"Выполнено всего: {totalCompletions}/{totalTarget} ({totalCompletionRate:F1}%)";

            CompletionHistoryText.Text = habit.CompletionHistory;
        }

        private void ClearUI()
        {
            StartDateText.Text = "";
            DescriptionText.Text = "";
            ReminderText.Text = "";
            CompletionPercentageText.Text = "";
            TotalCompletionText.Text = "";
            CompletionHistoryText.Text = "";
            PlotModel = new PlotModel { Title = "Прогресс выполнения" };
        }

        private void UpdateChart(Habit habit)
        {
            PlotModel = new PlotModel { Title = habit.Name };

            PlotModel.Axes.Add(new DateTimeAxis
            {
                Title = "Дата",
                StringFormat = "dd.MM",
                Minimum = DateTimeAxis.ToDouble(habit.StartDate.Date),
                Maximum = DateTimeAxis.ToDouble(DateTime.Today.Date)
            });

            PlotModel.Axes.Add(new LinearAxis
            {
                Title = "Выполнено раз",
                Minimum = -0.5,
                Maximum = habit.DailyTarget + 0.5,
                MajorStep = 1,
                MinorStep = 1
            });

            var lineSeries = new LineSeries
            {
                Title = "Выполнение",
                StrokeThickness = 2,
                MarkerSize = 3,
                MarkerType = MarkerType.Circle
            };

            var groupedCompletions = habit.CompletedDates
                .GroupBy(d => d.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            for (DateTime date = habit.StartDate.Date; date <= DateTime.Today; date = date.AddDays(1))
            {
                int completions = groupedCompletions.TryGetValue(date, out int count) ? count : 0;
                lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(date), completions));
            }

            PlotModel.Series.Add(lineSeries);
            PlotModel.InvalidatePlot(true);
        }

        private void CloseAnalytics_Click(object sender, RoutedEventArgs e) => Close();

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
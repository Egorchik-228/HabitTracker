using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Windows;

namespace HabitTracker.Models
{
    [Table("Habits")]
    public class Habit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Today;

        public string Description { get; set; }

        public bool HasReminder { get; set; }

        [NotMapped]
        public List<string> ReminderTimes { get; set; } = new List<string>();

        [Column("ReminderTimesSerialized")]
        public string ReminderTimesSerialized
        {
            get => string.Join(";", ReminderTimes);
            set => ReminderTimes = string.IsNullOrWhiteSpace(value)
                ? new List<string>()
                : value.Split(';').ToList();
        }

        [NotMapped]
        public List<DateTime> CompletedDates { get; set; } = new List<DateTime>();

        [Column("CompletionRecordsSerialized")]
        public string CompletionRecordsSerialized
        {
            get => string.Join(";", CompletedDates.Select(d => d.ToString("o")));
            set => CompletedDates = string.IsNullOrWhiteSpace(value)
                ? new List<DateTime>()
                : value.Split(';').Select(DateTime.Parse).ToList();
        }

        // Вычисляемые свойства
        public string ReminderInfo => HasReminder && ReminderTimes.Any()
            ? $"Напоминания:\n{string.Join("\n", ReminderTimes)}"
            : "Нет напоминаний";

        public string RemindersFormatted => ReminderTimes.Any()
            ? string.Join("\n", ReminderTimes)
            : "Нет напоминаний";

        public string ShortReminders => ReminderTimes.Count > 4
            ? string.Join(", ", ReminderTimes.Take(4)) + "..."
            : string.Join(", ", ReminderTimes);

        public Visibility ShowMoreButtonVisibility => ReminderTimes.Count > 4 ? Visibility.Visible : Visibility.Collapsed;

        public int TotalCompletions => CompletedDates.Count;
        public int TodayCompletions => CompletedDates.Count(d => d.Date == DateTime.Today);
        public int DailyTarget => ReminderTimes.Count > 0 ? ReminderTimes.Count : 1;
        public int TotalDays => (DateTime.Today - StartDate.Date).Days + 1;

        public bool CanMarkCompleted => TodayCompletions < DailyTarget;
        public int RemainingCompletions => DailyTarget - TodayCompletions;

        public string ProgressDisplay => $"{TodayCompletions}/{DailyTarget}";

        public string CompletionHistory
        {
            get
            {
                var grouped = CompletedDates
                    .GroupBy(d => d.Date)
                    .OrderByDescending(g => g.Key)
                    .Select(g => $"{g.Key:dd.MM.yyyy}: {g.Count()}/{DailyTarget}");

                return string.Join("\n", grouped);
            }
        }

        public override string ToString() => $"{Name} (с {StartDate:dd.MM.yyyy})";
    }
}
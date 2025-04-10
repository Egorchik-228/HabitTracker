using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace HabitTracker.Models
{
    public class Habit 
    {
        
        public string Name { get; set; } // Название
        public DateTime StartDate { get; set; } // Дата начала
        public string Description { get; set; } // Описание привычки
        


        // Напоминания
        public bool HasReminder { get; set; }
        public List<string> ReminderTimes { get; set; } = new List<string>(); // Список напоминаний

        public string ReminderInfo => HasReminder && ReminderTimes.Any()
            ? $"Напоминания:\n{string.Join("\n", ReminderTimes)}"
            : "Нет напоминаний";


        // Добавляем новое вычисляемое свойство для отображения напоминаний
        public string RemindersFormatted => ReminderTimes != null && ReminderTimes.Any()
            ? string.Join("\n", ReminderTimes)
            : "Нет напоминаний";

        // Отображение напоминаний, когда таймеров больше чем 4
        public string ShortReminders => ReminderTimes.Count > 4
    ?   string.Join(", ", ReminderTimes.Take(4)) + "..."
    :   string.Join(", ", ReminderTimes);

        public Visibility ShowMoreButtonVisibility => ReminderTimes.Count > 4 ? Visibility.Visible : Visibility.Collapsed;

        // Новый список для хранения дат, когда привычка была выполнена
        public List<DateTime> CompletedDates { get; set; } = new List<DateTime>();

        // Свойство для вычисления количества выполненных дней
        public int CompletedDays => CompletedDates.Count;

        public override string ToString()
        {
            return $"{Name}, {StartDate:dd.MM.yyyy}, {Description}";
        }
    }
}
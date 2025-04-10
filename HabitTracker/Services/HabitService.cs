using HabitTracker.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace HabitTracker.Services
{
    public class HabitService
    {
        private const string FilePath = "Data/habits.json";

        public List<Habit> LoadHabit()
        {
            try
            {
                string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                string filePath = Path.Combine(directoryPath, "habits.json");

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, "[]");
                }

                string json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
                Console.WriteLine("Содержимое JSON перед десериализацией:\n" + json);

                var habits = JsonConvert.DeserializeObject<List<Habit>>(json) ?? new List<Habit>();

                foreach (var habit in habits)
                {
                    if (habit.ReminderTimes == null)
                    {
                        habit.ReminderTimes = new List<string>();
                    }
                }

                Console.WriteLine($"Загружено {habits.Count} привычек.");
                return habits;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Habit>();
            }
        }


        public void SaveHabits(List<Habit> habits)
        {
            try
            {
                if (!Directory.Exists("Data"))
                {
                    Directory.CreateDirectory("Data");
                }

                Console.WriteLine($"Сохраняем {habits.Count} привычек...");

                string json = JsonConvert.SerializeObject(habits, Formatting.Indented);
                Console.WriteLine($"Сериализованный JSON:\n{json}"); // Проверка JSON перед записью

                File.WriteAllText(FilePath, json);
                Console.WriteLine("Данные успешно сохранены!");

                // Проверяем, записались ли данные в файл
                string checkJson = File.ReadAllText(FilePath);
                Console.WriteLine($"Проверка после записи. Содержимое файла:\n{checkJson}");

                MessageBox.Show("Данные сохранены!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении данных: {ex.Message}");
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    
    



public void MarkHabitAsCompleted(Habit habit)
        {
            List<Habit> habits = LoadHabit();
            Habit foundHabit = habits.FirstOrDefault(h => h.Name == habit.Name);

            if (foundHabit != null && !foundHabit.CompletedDates.Contains(DateTime.Today))
            {
                foundHabit.CompletedDates.Add(DateTime.Today);
                SaveHabits(habits);
            }
        }
    }
}

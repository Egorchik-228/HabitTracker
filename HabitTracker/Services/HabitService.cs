using HabitTracker.Data;
using HabitTracker.Models;
using HabitTracker.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace HabitTracker.Services
{
    public class HabitService
    {
        private readonly HabitRepository _repository;

        public HabitService()
        {
            var context = new HabitDbContext();
            _repository = new HabitRepository(context);
        }

        public List<Habit> LoadHabits()
        {
            try
            {
                var habits = _repository.GetAllHabits();
                Console.WriteLine($"Загружено {habits.Count} привычек из базы данных.");
                return habits;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<Habit>();
            }
        }

        public void SaveHabit(Habit habit)
        {
            try
            {
                if (habit.Id == 0)
                {
                    _repository.AddHabit(habit);
                    Console.WriteLine($"Добавлена новая привычка: {habit.Name}");
                }
                else
                {
                    _repository.UpdateHabit(habit);
                    Console.WriteLine($"Обновлена привычка: {habit.Name}");
                }

                MessageBox.Show("Данные сохранены!", "Сохранение",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении данных: {ex.Message}");
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool MarkHabitAsCompleted(Habit habit)
        {
            try
            {
                var dbHabit = _repository.GetHabitById(habit.Id);
                if (dbHabit == null)
                {
                    return false;
                }

                // Проверка лимита выполнений на сегодня
                int todayCompletions = dbHabit.CompletedDates.Count(d => d.Date == DateTime.Today);
                if (todayCompletions >= dbHabit.DailyTarget)
                {
                    return false; // Лимит достигнут
                }

                // Добавляем новое выполнение
                dbHabit.CompletedDates.Add(DateTime.Now);
                _repository.UpdateHabit(dbHabit);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отметке выполнения: {ex.Message}");
                return false;
            }
        }

        public void DeleteHabit(Habit habit)
        {
            try
            {
                _repository.DeleteHabit(habit.Id);
                Console.WriteLine($"Удалена привычка: {habit.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении: {ex.Message}");
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public Habit GetHabitById(int id)
        {
            return _repository.GetHabitById(id);
        }
    }
}
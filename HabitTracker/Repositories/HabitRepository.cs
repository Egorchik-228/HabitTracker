using HabitTracker.Models;
using System.Collections.Generic;
using System.Linq;
using HabitTracker.Data;

namespace HabitTracker.Repositories
{
    public class HabitRepository
    {
        private readonly HabitDbContext _context;

        public HabitRepository(HabitDbContext context)
        {
            _context = context;
        }

        public List<Habit> GetAllHabits()
        {
            return _context.Habits.ToList();
        }

        public void AddHabit(Habit habit)
        {
            _context.Habits.Add(habit);
            _context.SaveChanges();
        }

        public void UpdateHabit(Habit habit)
        {
            var existingHabit = _context.Habits.Find(habit.Id);
            if (existingHabit != null)
            {
                _context.Entry(existingHabit).CurrentValues.SetValues(habit);
                _context.SaveChanges();
            }
        }

        public void DeleteHabit(int id)
        {
            var habit = _context.Habits.Find(id);
            if (habit != null)
            {
                _context.Habits.Remove(habit);
                _context.SaveChanges();
            }
        }

        public Habit GetHabitById(int id)
        {
            return _context.Habits.Find(id);
        }
    }
}
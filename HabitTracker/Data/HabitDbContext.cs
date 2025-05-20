using System.Data.Entity;
using HabitTracker.Models;

namespace HabitTracker.Data
{
    public class HabitDbContext : DbContext
    {
        public HabitDbContext() : base("name=HabitDbContext")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<HabitDbContext>());
        }

        public DbSet<Habit> Habits { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Habit>()
                .Property(h => h.ReminderTimesSerialized)
                .HasColumnName("ReminderTimes");

            modelBuilder.Entity<Habit>()
                .Property(h => h.CompletionRecordsSerialized)
                .HasColumnName("CompletionRecords");

            base.OnModelCreating(modelBuilder);
        }
    }
}
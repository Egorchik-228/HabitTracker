using System.Data.Entity;
using HabitTracker.Data;

namespace HabitTracker
{
    public class HabitDbInitializer : CreateDatabaseIfNotExists<HabitDbContext>
    {
        protected override void Seed(HabitDbContext context)
        {
            // Здесь можно добавить тестовые привычки или оставить пустым
            base.Seed(context);
        }
    }
}

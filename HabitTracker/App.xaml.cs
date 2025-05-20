using System;
using System.Windows;
using System.Data.Entity;
using HabitTracker.Data;

namespace HabitTracker
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Устанавливаем инициализатор базы данных
            Database.SetInitializer(new HabitDbInitializer());

            // Пример однократного вызова для создания базы
            using (var db = new HabitDbContext())
            {
                db.Database.Initialize(force: false);
            }
        }
    }
}
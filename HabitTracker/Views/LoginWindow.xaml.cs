using System.Windows;

namespace HabitTracker.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Открываем основное окно и закрываем окно входа
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

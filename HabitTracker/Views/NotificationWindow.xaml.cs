using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace HabitTracker.Views
{
    public partial class NotificationWindow : Window
    {
        private const int AutoCloseMilliseconds = 30000; // 30 секунд висит

        public NotificationWindow(string title, string message)
        {
            InitializeComponent();
            TitleTextBlock.Text = title;
            MessageTextBlock.Text = message;

            // Звук уведомления
            System.Media.SystemSounds.Exclamation.Play();

            Loaded += NotificationWindow_Loaded;
        }

        private async void NotificationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Показываем окно снизу справа экрана
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width - 10;
            Top = desktopWorkingArea.Bottom - Height - 10;

            // Плавное появление
            this.Opacity = 0;
            var fadeIn = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(300)));
            this.BeginAnimation(Window.OpacityProperty, fadeIn);

            // Автозакрытие через длительное время (например, 30 секунд)
            await Task.Delay(AutoCloseMilliseconds);

            // Если пользователь не закрыл вручную — закрываем с анимацией
            if (IsVisible)
            {
                CloseWithAnimation();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseWithAnimation();
        }

        private void CloseWithAnimation()
        {
            var fadeOut = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(300)));
            fadeOut.Completed += (s, _) => this.Close();
            this.BeginAnimation(Window.OpacityProperty, fadeOut);
        }
    }
}

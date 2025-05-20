using System;
using System.Globalization;
using System.Windows.Data;

namespace HabitTracker.Converters
{
    public class CompletionsToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool canMark && canMark;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
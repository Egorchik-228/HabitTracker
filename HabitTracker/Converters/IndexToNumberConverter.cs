using System;
using System.Globalization;
using System.Windows.Data;

namespace HabitTracker.Converters
{
    public class IndexToNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
                return $"Время {index + 1}:";
            return "Время ?:";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
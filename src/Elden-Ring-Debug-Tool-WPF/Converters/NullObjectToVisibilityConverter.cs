using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Elden_Ring_Debug_Tool_WPF.Converters
{
    public class NullObjectToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {

            if (value == null)
                return Visibility.Hidden;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("There is no backwards conversion for count to bool");
        }
    }
}

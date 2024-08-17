using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Elden_Ring_Debug_Tool_WPF.Converters
{
    public class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
            {
                return new SolidColorBrush(Color.FromArgb(128, 0, 255, 0)); // Semi-transparent green
            }
            return new SolidColorBrush(Color.FromArgb(128, 255, 0, 0)); // Semi-transparent red
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("BooleanToColorConverter can only be used for one way conversion.");
        }
    }
}
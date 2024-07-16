using System;
using System.Globalization;
using System.Windows.Data;

namespace Elden_Ring_Debug_Tool_WPF.Converters
{
    internal class InverseBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            return !(bool)value;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            return !(bool)value;
        }
    }
}

using System;
using System.Globalization;
using System.Windows.Data;

namespace Elden_Ring_Debug_Tool_WPF.Converters
{
    class NumberToHexStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"0x{((int)value):X8}";
            //return ((int)value).ToString("X8");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.Parse(value?.ToString() ?? "0");
        }
    }
}

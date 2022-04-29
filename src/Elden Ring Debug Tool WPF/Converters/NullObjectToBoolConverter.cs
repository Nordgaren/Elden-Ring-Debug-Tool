using System;
using System.Globalization;
using System.Windows.Data;

namespace Elden_Ring_Debug_Tool_WPF.Converters
{
    public class NullObjectToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value != null ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("There is no backwards conversion for bool to null");
        }
    }
}

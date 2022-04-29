using System;
using System.Globalization;
using System.Windows.Data;

namespace Elden_Ring_Debug_Tool_WPF.Converters
{
    internal class MultiBoolConverter : IMultiValueConverter
    {
        public object Convert(object[]? values, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = true;
            for (int i = 0; i < values.Length; i++)
            {
                result &= (bool)values[i];
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

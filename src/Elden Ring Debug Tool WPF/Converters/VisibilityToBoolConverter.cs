using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows;

namespace Elden_Ring_Debug_Tool_WPF.Converters
{
    public class VisibilityToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            Visibility? visibility = (Visibility)value;

            if (visibility == null)
                throw new ArgumentNullException(nameof(visibility));

            return visibility == Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = (bool)value;

            if (val == null)
                throw new ArgumentNullException(nameof(val));

            return val ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}

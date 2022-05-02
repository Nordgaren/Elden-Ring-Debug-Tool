using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Elden_Ring_Debug_Tool_WPF.Converters
{
    internal class KeyToStringConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return "Unbound";

            if (!(value is Key key))
                throw new ArgumentException($"{nameof(value)} is not a Key");

            return key.ToString();
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string s))
                throw new ArgumentException($"{nameof(value)} is not a string");

            if (s == "Unbound")
                return null;

            KeyConverter k = new KeyConverter();
            Key mykey = (Key)k.ConvertFromInvariantString(s);

            return mykey;

        }
    }
}

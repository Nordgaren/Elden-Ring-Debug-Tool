using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Elden_Ring_Debug_Tool_WPF.Converters
{
    public class CountToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            int threshold = (int)parameter;

            if (threshold == null)
                throw new ArgumentNullException("Threshold");

            ICollectionView collection = (ICollectionView)value;
            if (collection == null)
                return false;

            return collection.IsEmpty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("There is no backwards conversion for count to bool");
        }
    }
}

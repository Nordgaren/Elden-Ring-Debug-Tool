using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Elden_Ring_Debug_Tool_WPF.DataTemplateSelectors
{
    internal class FieldDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NumericSigned { get; set; }
        public DataTemplate NumericUnsigned { get; set; }
        public DataTemplate FloatField { get; set; }
        public DataTemplate BitField { get; set; }
        public DataTemplate FixedStr { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FieldViewModel fieldViewModel = (FieldViewModel)item; 
            if (fieldViewModel is NumericSignedViewModel)
                return NumericSigned;

            if (fieldViewModel is NumericViewModel)
                return NumericUnsigned;

            if (fieldViewModel is FloatFieldViewModel)
                return FloatField;

            if (fieldViewModel is BitFieldViewModel)
                return BitField;

            if (fieldViewModel is FixedStrViewModel)
                return FixedStr;

            return null;
        }
    }
}

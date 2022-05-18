using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Elden_Ring_Debug_Tool_WPF.DataTemplateSelectors
{
    internal class FieldDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NumericSigned8 { get; set; }
        public DataTemplate NumericSigned16 { get; set; }
        public DataTemplate NumericSigned32 { get; set; }
        public DataTemplate NumericUnsigned8 { get; set; }
        public DataTemplate NumericUnsigned16 { get; set; }
        public DataTemplate NumericUnsigned32 { get; set; }
        public DataTemplate SingleField { get; set; }
        public DataTemplate BitField { get; set; }
        public DataTemplate FixedStr { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FieldViewModel fieldViewModel = (FieldViewModel)item;
            if (fieldViewModel is NumericSignedViewModel<sbyte>)
                return NumericSigned8;

            if (fieldViewModel is NumericSignedViewModel<short>)
                return NumericSigned16;

            if (fieldViewModel is NumericSignedViewModel<int>)
                return NumericSigned32;

            if (fieldViewModel is NumericViewModel<byte>)
                return NumericUnsigned8;

            if (fieldViewModel is NumericViewModel<ushort>)
                return NumericUnsigned16;

            if (fieldViewModel is NumericViewModel<uint>)
                return NumericUnsigned32;

            if (fieldViewModel is SingleFieldViewModel)
                return SingleField;

            if (fieldViewModel is PartialNumericViewModel)
                return NumericUnsigned8;

            if (fieldViewModel is BitFieldViewModel) 
                return BitField;

            if (fieldViewModel is FixedStrViewModel)
                return FixedStr;

            throw new System.Exception($"No Template Found for {item}");
        }
    }
}

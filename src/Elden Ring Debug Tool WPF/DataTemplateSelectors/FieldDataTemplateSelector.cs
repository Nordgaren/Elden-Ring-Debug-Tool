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
            if (fieldViewModel is NumericSignedViewModel sValue)
            {
                switch (sValue.Type)
                {
                    case "s8":
                        return NumericSigned8;
                    case "s16":
                        return NumericSigned16;
                    case "s32":
                        return NumericSigned32;
                    default:
                        break;
                }
            }

            if (fieldViewModel is NumericViewModel value)
            {
                switch (value.Type)
                {
                    case "u8":
                        return NumericUnsigned8;
                    case "dummy8":
                        if (value.ArrayLength == 2)
                            return NumericUnsigned16;
                        else if (value.ArrayLength == 4)
                            return NumericUnsigned32;

                        return NumericUnsigned8;
                    case "u16":
                        return NumericUnsigned16;
                    case "u32":
                        return NumericUnsigned32;
                    default:
                        break;
                }
            }

            if (fieldViewModel is SingleFieldViewModel)
                return SingleField;

            if (fieldViewModel is BitFieldViewModel)
                return BitField;

            if (fieldViewModel is FixedStrViewModel)
                return FixedStr;

            throw new System.Exception($"No Template Avalable {item}");
        }
    }
}

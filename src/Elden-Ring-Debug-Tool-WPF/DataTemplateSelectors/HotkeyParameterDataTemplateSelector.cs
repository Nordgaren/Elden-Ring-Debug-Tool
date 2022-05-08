using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Elden_Ring_Debug_Tool_WPF.DataTemplateSelectors
{
    internal class HotKeyParameterDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ComboLol { get; set; }
        public DataTemplate TextLol { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
                return TextLol;

            return ComboLol;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool_ViewModels.Attributes
{
    public class HotKeyParameterAttribute : Attribute
    {
        public enum ResourceType
        {
            None,
            ComboBox,
            NumericUpDown,
            DecimalUpDown,
            TextBox
        }
        public Type CommandType { get; set; }
        public string? SelectedItemPropertyName { get; set; }
        public ResourceType Type { get; set; }

        public HotKeyParameterAttribute(Type commandType, ResourceType type)
        {
            CommandType = commandType;
            Type = type;

            if (CommandType == null)
                throw new ArgumentNullException(nameof(CommandType));

            if (Type == ResourceType.None)
                throw new ArgumentNullException(nameof(Type));
        }
    }
}

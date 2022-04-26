using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoulsFormats.PARAMDEF;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class FieldViewModel : ViewModelBase
    {
        private Field _paramdefField;
        public DefType Type => _paramdefField.DisplayType;
        public string InternalName => _paramdefField.InternalName;
        public string DisplayName => _paramdefField.DisplayName;
        public string Description => _paramdefField.Description;
        public int Offset { get; }

        public FieldViewModel(Field field, int offset)
        {
            _paramdefField = field;
            Offset = offset;
        }

        public override string ToString()
        {
            return InternalName;
        }
    }
}

using Elden_Ring_Debug_Tool;
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
        private ERParam.Field _paramdefField;
        private string _type;
        public string Type
        {
            get => _type;
            set => SetField(ref _type, value);
        }
        private string _internalName;
        public string InternalName
        {
            get => _internalName;
            set => SetField(ref _internalName, value);
        }
        private string _displayName;
        public string DisplayName
        {
            get => _displayName;
            set => SetField(ref _displayName, value);
        }
        private string _description;
        public string Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }
        public int Offset { get; }

        public FieldViewModel(ERParam.Field field, int offset)
        {
            _paramdefField = field;
            Type = Enum.GetName(_paramdefField.Type) ?? "";
            InternalName = _paramdefField.InternalName;
            DisplayName = _paramdefField.DisplayName;
            Description = _paramdefField.Description;
            Offset = offset;
        }

        public override string ToString()
        {
            return InternalName;
        }
    }
}

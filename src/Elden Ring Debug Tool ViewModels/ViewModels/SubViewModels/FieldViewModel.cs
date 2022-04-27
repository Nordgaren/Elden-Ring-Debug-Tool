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
        private ERParam.Field _field;
        protected ParamViewerViewModel ParamViewerViewModel;
        protected ERParam Param => ParamViewerViewModel.SelectedParam.Param;
        public virtual string StringValue { get; } = string.Empty;
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
        public int ArrayLength;
        public int Offset => ParamViewerViewModel.SelectedRow?.DataOffset + FieldOffset ?? 0;
        public int FieldOffset { get; }

        public FieldViewModel(ParamViewerViewModel paramViewerViewModel, ERParam.Field field)
        {
            ParamViewerViewModel = paramViewerViewModel;
            _field = field;
            Type = Enum.GetName(_field.Type) ?? "";
            InternalName = _field.InternalName;
            DisplayName = _field.DisplayName;
            Description = _field.Description;
            ArrayLength = _field.ArrayLength;
            FieldOffset = _field.FieldOffset;
        }

        public virtual void Update()
        {
            throw new NotImplementedException("Not Implimented for FieldViewModel. Must impliment in class that inherited from FieldViewModel");
        }
        public int GetSize()
        {
            switch (Type)
            {
                case "s8":
                case "u8":
                case "dummy8":
                    return 1;
                case "s16":
                case "u16":
                    return 2;
                case "s32":
                case "u32":
                case "f32":
                    return 4;
                default:
                    throw new Exception($"Invalid size for field {InternalName}");
            }
        }

        public override string ToString()
        {
            return InternalName;
        }
    }
}

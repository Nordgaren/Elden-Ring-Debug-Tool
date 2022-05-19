using Erd_Tools.Models;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public abstract class FieldViewModel : ViewModelBase
    {
        private Param.Field _field;
        protected ParamViewViewModel ParamViewerViewModel;
        protected Param Param => ParamViewerViewModel.SelectedParam.Param;
        public virtual string StringValue => throw new NotImplementedException("String Value not implemented for abstract base class");
        public string Type => _field.Type.ToString();
        public virtual string InternalName => _field.InternalName;
        public string DisplayName => _field.DisplayName;
        public string Description => _field.Description;
        public int ArrayLength => _field.ArrayLength;
        public virtual object MinValue { get; }
        public virtual object MaxValue { get; }
        public int Offset => ParamViewerViewModel.SelectedRow?.DataOffset + FieldOffset ?? 0;
        public object Increment { get; }
        public int FieldOffset => _field.FieldOffset;

        public FieldViewModel(ParamViewViewModel paramViewerViewModel, Param.Field field)
        {
            ParamViewerViewModel = paramViewerViewModel;
            _field = field;
            Increment = GetIncrement();
            //Type = Enum.GetName(_field.Type) ?? "null";
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

        public object GetIncrement()
        {

            if (_field.Increment == null)
                return null;

            object inc = _field.Increment;

            switch (Type)
            {
                case "s8":
                    return Convert.ToSByte(inc);
                case "u8":
                    return Convert.ToByte(inc);
                case "dummy8":
                    return Convert.ToByte(inc);
                case "s16":
                    return Convert.ToInt16(inc);
                case "u16":
                    return Convert.ToUInt16(inc);
                case "s32":
                    return Convert.ToInt32(inc);
                case "u32":
                    return Convert.ToUInt32(inc);
                case "f32":
                    return Convert.ToSingle(inc);
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

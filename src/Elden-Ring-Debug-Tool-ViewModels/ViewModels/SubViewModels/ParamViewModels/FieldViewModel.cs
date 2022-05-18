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
        public string InternalName => _field.InternalName;
        public string DisplayName => _field.DisplayName;
        public string Description => _field.Description;
        public int ArrayLength => _field.ArrayLength;
        public virtual object MinValue { get; }
        public virtual object MaxValue { get; }
        public int Offset => ParamViewerViewModel.SelectedRow?.DataOffset + FieldOffset ?? 0;
        public object? Increment => GetIncrement();
        public int FieldOffset => _field.FieldOffset;

        public FieldViewModel(ParamViewViewModel paramViewerViewModel, Param.Field field)
        {
            ParamViewerViewModel = paramViewerViewModel;
            _field = field;
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

        public object? GetIncrement()
        {

            if (_field.Increment == null)
                return null;

            object inc = _field.Increment;

            switch (Type)
            {
                case "s8":
                    return inc as sbyte?;
                case "u8":
                    return inc as byte?;
                case "dummy8":
                    return inc as byte?;
                case "s16":
                    return inc as short?;
                case "u16":
                    return inc as ushort?;
                case "s32":
                    return inc as int?;
                case "u32":
                    return inc as uint?;
                case "f32":
                    return inc as float?;
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

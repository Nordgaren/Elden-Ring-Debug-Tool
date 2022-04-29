using Erd_Tools;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public abstract class FieldViewModel : ViewModelBase
    {
        private ERParam.Field _field;
        protected ParamViewerViewModel ParamViewerViewModel;
        protected ERParam Param => ParamViewerViewModel.SelectedParam.Param;
        public virtual string StringValue => Value?.ToString() ?? "";
        public virtual object Value { get; set; }
        public string Type;
        public string InternalName => _field.InternalName;
        public string DisplayName => _field.DisplayName;
        public string Description => _field.Description;
        public int ArrayLength => _field.ArrayLength;
        public virtual object MinValue { get; }
        public virtual object MaxValue { get; }
        public int Offset => ParamViewerViewModel.SelectedRow?.DataOffset + FieldOffset ?? 0;
        public object? Increment => GetIncrement();
        public int FieldOffset { get; }

        public FieldViewModel(ParamViewerViewModel paramViewerViewModel, ERParam.Field field)
        {
            ParamViewerViewModel = paramViewerViewModel;
            _field = field;
            Type = Enum.GetName(_field.Type) ?? "";
            FieldOffset = _field.FieldOffset;

            paramViewerViewModel.PropertyChanged += ParamViewerViewModel_PropertyChanged;
        }

        private void ParamViewerViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ParamViewerViewModel.SelectedRow))
            {
                OnPropertyChanged(nameof(Value));
            }
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

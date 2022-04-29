using static Erd_Tools.ERParam;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class NumericViewModel<T> : FieldViewModel
    {
        private NumericField _numericField;
        public override object MinValue { get; }
        public override object MaxValue { get; }
        public override string StringValue => Value?.ToString() ?? "null";
        public T? Value
        {
            get
            {
                if (ParamViewerViewModel.SelectedRow == null)
                    return default(T);

                return GetValue();
            }
            set
            {
                if (ParamViewerViewModel.SelectedRow == null || value == null)
                    return;

                byte[] buffer = GetBytes(value);
                byte[] bytes = new byte[GetSize()];
                Array.Copy(buffer, bytes, bytes.Length);
                Param.Pointer.WriteBytes(Offset, bytes);
                Array.Copy(bytes, 0, Param.Bytes, Offset, bytes.Length);
            }
        }

        private byte[] GetBytes(T val)
        {
            if (val is byte b) return new byte[] { b };
            if (val is ushort us) return BitConverter.GetBytes(us);
            if (val is uint ui) return BitConverter.GetBytes(ui);

            throw new NotImplementedException($"No conversion for this type {typeof(T)}");
        }

        private T? GetValue()
        {
            switch (GetSize())
            {
                case 1:
                    return (T?)(object)Param.Bytes[Offset];
                case 2:
                    return (T?)(object)BitConverter.ToUInt16(Param.Bytes, Offset);
                case 4:
                    return (T?)(object)BitConverter.ToUInt32(Param.Bytes, Offset);
                case 8:
                    return (T?)(object)BitConverter.ToUInt64(Param.Bytes, Offset);
                default:
                    return default(T?);
            }
        }

        public NumericViewModel(ParamViewerViewModel paramViewerViewModel, NumericField numericFfield) : base(paramViewerViewModel, numericFfield)
        {
            _numericField = numericFfield;
            switch (Type)
            {
                case "u8":
                    MinValue = byte.MinValue;
                    MaxValue = byte.MaxValue;
                    break;
                case "u16":
                    MinValue = ushort.MinValue;
                    MaxValue = ushort.MaxValue;
                    break;
                case "u32":
                    MinValue = uint.MinValue;
                    MaxValue = uint.MaxValue;
                    break;
                default:
                    break;
            }

            paramViewerViewModel.PropertyChanged += ParamViewerViewModel_PropertyChanged;
        }
        private void ParamViewerViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ParamViewerViewModel.SelectedRow))
            {
                OnPropertyChanged(nameof(Value));
            }
        }
    }
}

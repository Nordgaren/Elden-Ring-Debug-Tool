using static Erd_Tools.Models.Param;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class NumericSignedViewModel<T> : FieldViewModel
    {
        private NumericField _numericField;
        public override object MinValue { get; }
        public override object MaxValue { get; }
        public override string StringValue => Value?.ToString() ?? "null";
        public object Value
        {
            get
            {
                if (ParamViewerViewModel.SelectedRow == null)
                    return null;

                return GetValue();
            }
            set
            {
                if (ParamViewerViewModel.SelectedRow == null || value == null)
                    return;

                byte[] buffer = GetBytes((T)value);
                byte[] bytes = new byte[GetSize()];
                Array.Copy(buffer, bytes, bytes.Length);
                Param.Pointer.WriteBytes(Offset, bytes);
                Array.Copy(bytes, 0, Param.Bytes, Offset, bytes.Length);
            }
        }
        private byte[] GetBytes(T val)
        {
            if (val is sbyte sb) return new byte[] { (byte)sb };
            if (val is short s) return BitConverter.GetBytes(s);
            if (val is int i) return BitConverter.GetBytes(i);

            throw new NotImplementedException($"No conversion for this type {typeof(T)}");
        }

        private T? GetValue()
        {
            switch (GetSize())
            {
                case 1:
                    return (T?)(object)Convert.ToSByte(Param.Bytes[Offset]);
                case 2:
                    return (T?)(object)BitConverter.ToInt16(Param.Bytes, Offset);
                case 4:
                    return (T?)(object)BitConverter.ToInt32(Param.Bytes, Offset);
                case 8:
                    return (T?)(object)BitConverter.ToInt64(Param.Bytes, Offset);
                default:
                    throw new Exception($"Unknown size for type {Type}");
            }
        }
        public NumericSignedViewModel(ParamViewViewModel paramViewerViewModel, NumericField numericField) : base(paramViewerViewModel, numericField)
        {
            _numericField = numericField;
            switch (Type)
            {
                case "s8":
                    MinValue = sbyte.MinValue;
                    MaxValue = sbyte.MaxValue;
                    break;
                case "s16":
                    MinValue = short.MinValue;
                    MaxValue = short.MaxValue;
                    break;
                case "s32":
                    MinValue = int.MinValue;
                    MaxValue = int.MaxValue;
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

using static Erd_Tools.ERParam;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class NumericSignedViewModel : FieldViewModel
    {
        private NumericField _numericField;
        public override object MinValue { get; }
        public override object MaxValue { get; }

        public override object Value
        {
            get
            {
                if (ParamViewerViewModel.SelectedRow == null)
                    return null;

                return GetValue();
            }
            set
            {
                if (ParamViewerViewModel.SelectedRow == null)
                    return;

                byte[] buffer = BitConverter.GetBytes((long)value);
                byte[] bytes = new byte[GetSize()];
                Array.Copy(buffer, bytes, bytes.Length);
                Param.Pointer.WriteBytes(Offset, bytes);
                Array.Copy(bytes, 0, Param.Bytes, Offset, bytes.Length);
            }
        }
        private long GetValue()
        {
            switch (GetSize())
            {
                case 1:
                    return Param.Bytes[Offset];
                case 2:
                    return BitConverter.ToInt16(Param.Bytes, Offset);
                case 4:
                    return BitConverter.ToInt32(Param.Bytes, Offset);
                case 8:
                    return BitConverter.ToInt64(Param.Bytes, Offset);
                default:
                    return 0;
            }
        }
        public NumericSignedViewModel(ParamViewerViewModel paramViewerViewModel, NumericField numericField) : base(paramViewerViewModel, numericField)
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
        }
    }
}

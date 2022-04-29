using static Erd_Tools.ERParam;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class BitFieldViewModel : FieldViewModel
    {
        private BitField _bitField; 
        private int _bitPosition;
        public override string StringValue
        {
            get
            {
                if (Value == null)
                    return "null";

                return (bool)Value ? "1 True On" : "0 False Off";
            }
        }

        public bool? Value
        {
            get => ParamViewerViewModel.SelectedRow != null ?  ((Param.Bytes[Offset] & (1 << _bitPosition)) != 0) : null;
            set
            {
                if (ParamViewerViewModel.SelectedRow == null || value == null)
                    return;

                byte paramValue = Param.Bytes[Offset];
                if ((bool)value)
                    paramValue |= (byte)(1 << _bitPosition);
                else
                    paramValue &= (byte)~(1 << _bitPosition);

                Param.Pointer.WriteByte(Offset, paramValue);
                byte[] bytes = BitConverter.GetBytes(paramValue);
                Array.Copy(bytes, 0, Param.Bytes, Offset, bytes.Length);
            }
        }
        public BitFieldViewModel(ParamViewerViewModel paramViewerViewModel, BitField bitField) : base(paramViewerViewModel, bitField)
        {
            _bitField = bitField;
            _bitPosition = _bitField.BitPosition;

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

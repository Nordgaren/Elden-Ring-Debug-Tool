using static Erd_Tools.ERParam;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class BitFieldViewModel : FieldViewModel
    {
        private BitField _bitField; 
        private int _bitPosition;
        public override string StringValue => (bool)Value ? "1 True On" : "0 False Off";
        public override object Value
        {
            get => ParamViewerViewModel.SelectedRow != null ?  ((Param.Bytes[Offset] & (1 << _bitPosition)) != 0) : null;
            set
            {
                if (ParamViewerViewModel.SelectedRow == null)
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
        }
    }
}

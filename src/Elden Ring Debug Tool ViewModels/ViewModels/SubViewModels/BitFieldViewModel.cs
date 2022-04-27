using Elden_Ring_Debug_Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Elden_Ring_Debug_Tool.ERParam;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class BitFieldViewModel : FieldViewModel
    {
        private BitField _bitField; 
        private int _bitPosition;
        public override string StringValue => Value ? "1 True On" : "0 False Off";
        public bool Value
        {
            get => ((Param.Bytes[Offset] & (1 << _bitPosition)) != 0);
            set
            {
                var paramValue = Param.Bytes[Offset];
                if (value)
                    paramValue |= (byte)(1 << _bitPosition);
                else
                    paramValue &= (byte)~(1 << _bitPosition);

                Param.Pointer.WriteByte(Offset, paramValue);
                var bytes = BitConverter.GetBytes(paramValue);
                Array.Copy(bytes, 0, Param.Bytes, Offset, bytes.Length);
            }
        }
        public override void Update()
        {
            OnPropertyChanged(nameof(Value));
        }
        public BitFieldViewModel(ParamViewerViewModel paramViewerViewModel, BitField bitField) : base(paramViewerViewModel, bitField)
        {
            _bitField = bitField;
            _bitPosition = _bitField.BitPosition;
        }
    }
}

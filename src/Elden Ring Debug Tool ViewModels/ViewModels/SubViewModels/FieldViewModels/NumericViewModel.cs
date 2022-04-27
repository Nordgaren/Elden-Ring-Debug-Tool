using Elden_Ring_Debug_Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Elden_Ring_Debug_Tool.ERParam;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class NumericViewModel : FieldViewModel
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

                byte[] buffer = BitConverter.GetBytes((ulong)value);
                byte[] bytes = new byte[GetSize()];
                Array.Copy(buffer, bytes, bytes.Length);
                Param.Pointer.WriteBytes(Offset, bytes);
                Array.Copy(bytes, 0, Param.Bytes, Offset, bytes.Length);
            }
        }

        private ulong GetValue()
        {
            switch (GetSize())
            {
                case 1:
                    return Param.Bytes[Offset];
                case 2:
                    return BitConverter.ToUInt16(Param.Bytes, Offset);
                case 4:
                    return BitConverter.ToUInt32(Param.Bytes, Offset);
                case 8:
                    return BitConverter.ToUInt64(Param.Bytes, Offset);
                default:
                    return 0;
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
        }
    }
}

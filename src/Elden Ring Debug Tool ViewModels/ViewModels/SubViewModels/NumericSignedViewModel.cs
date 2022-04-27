using Elden_Ring_Debug_Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Elden_Ring_Debug_Tool.ERParam;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class NumericSignedViewModel : FieldViewModel
    {
        private NumericField _numericField;
        public override string StringValue => Value.ToString();

        public long Value
        {
            get
            {
                return GetValue();
            }
            set
            {
                var buffer = BitConverter.GetBytes(value);
                var bytes = new byte[GetSize()];
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

        public override void Update()
        {
            OnPropertyChanged(nameof(Value));
        }
        public NumericSignedViewModel(ParamViewerViewModel paramViewerViewModel, NumericField numericField) : base(paramViewerViewModel, numericField)
        {
            _numericField = numericField;
        }
    }
}

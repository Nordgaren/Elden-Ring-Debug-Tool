using Elden_Ring_Debug_Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Elden_Ring_Debug_Tool.ERParam;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class FixedStrViewModel : FieldViewModel
    {
        private FixedStr _fixedStr;
        private Encoding _encoding;
        public override string StringValue => Value;
        public string Value
        {
            get
            {
                var bytes = new byte[ArrayLength];
                Array.Copy(Param.Bytes, Offset, bytes, 0, ArrayLength);
                return Encoding.ASCII.GetString(bytes).Replace("\0", "");
            }
            set
            {
                if (value.Length > ArrayLength)
                {
                    OnPropertyChanged(nameof(Value));
                    return;
                }

                var bytes = Encoding.ASCII.GetBytes(value);
                var buffer = new byte[ArrayLength];
                Array.Copy(bytes, 0, buffer, 0, bytes.Length);
                Param.Pointer.WriteString(Offset, _encoding, (uint)ArrayLength, value);
                Array.Copy(buffer, 0, Param.Bytes, Offset, buffer.Length);
                OnPropertyChanged(nameof(Value));
            }
        }

        public override void Update()
        {
            OnPropertyChanged(nameof(Value));
        }

        public FixedStrViewModel(ParamViewerViewModel paramViewerViewModel, FixedStr fixedStr) : base(paramViewerViewModel, fixedStr)
        {
            _fixedStr = fixedStr;
            _encoding = _fixedStr.Encoding;
        }
    }
}

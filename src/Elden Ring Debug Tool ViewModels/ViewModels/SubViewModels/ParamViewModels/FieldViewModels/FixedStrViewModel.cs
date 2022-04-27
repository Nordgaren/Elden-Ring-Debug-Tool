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
         public override object Value
        {
            get
            {
                if (ParamViewerViewModel.SelectedRow == null)
                    return null;

                byte[] bytes = new byte[ArrayLength];
                Array.Copy(Param.Bytes, Offset, bytes, 0, ArrayLength);
                return _encoding.GetString(bytes).Replace("\0", "");
            }
            set
            {
                if (ParamViewerViewModel.SelectedRow == null)
                    return;

                string? input = value as string;
                if (input == null)
                    input = string.Empty;

                if (input.Length > ArrayLength)
                {
                    OnPropertyChanged(nameof(Value));
                    return;
                }

                byte[] bytes = _encoding.GetBytes(input);
                byte[] buffer = new byte[ArrayLength];
                Array.Copy(bytes, 0, buffer, 0, bytes.Length);
                Param.Pointer.WriteString(Offset, _encoding, (uint)ArrayLength, input);
                Array.Copy(buffer, 0, Param.Bytes, Offset, buffer.Length);
            }
        }
        public FixedStrViewModel(ParamViewerViewModel paramViewerViewModel, FixedStr fixedStr) : base(paramViewerViewModel, fixedStr)
        {
            _fixedStr = fixedStr;
            _encoding = _fixedStr.Encoding;
        }
    }
}

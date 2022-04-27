using Elden_Ring_Debug_Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Elden_Ring_Debug_Tool.ERParam;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class FloatFieldViewModel : FieldViewModel
    {
        private FloatField _floatField;
        public override object Value
        {
            get => (decimal)BitConverter.ToSingle(Param.Bytes, Offset);
            set
            {
                var lol = value;
                float kek = Convert.ToSingle(value);
                decimal kekD = (decimal)value;
                Param.Pointer.WriteSingle(Offset, (float)value);
                byte[] bytes = BitConverter.GetBytes((float)value);
                Array.Copy(bytes, 0, Param.Bytes, Offset, bytes.Length);
            }
        }
        public FloatFieldViewModel(ParamViewerViewModel paramViewerViewModel, FloatField floatField) : base(paramViewerViewModel, floatField)
        {
            _floatField = floatField;
        }
    }
}

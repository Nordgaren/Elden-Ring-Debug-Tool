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
        public override string StringValue => Value.ToString();
        public float Value
        {
            get => BitConverter.ToSingle(Param.Bytes, Offset);
            set
            {
                Param.Pointer.WriteSingle(Offset, value);
                var bytes = BitConverter.GetBytes(value);
                Array.Copy(bytes, 0, Param.Bytes, Offset, bytes.Length);
            }
        }
        public override void Update()
        {
            OnPropertyChanged(nameof(Value));
        }
        public FloatFieldViewModel(ParamViewerViewModel paramViewerViewModel, FloatField floatField) : base(paramViewerViewModel, floatField)
        {
            _floatField = floatField;
        }
    }
}

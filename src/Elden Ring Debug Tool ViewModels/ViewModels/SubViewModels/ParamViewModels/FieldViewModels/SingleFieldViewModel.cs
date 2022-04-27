using Erd_Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Erd_Tools.ERParam;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class SingleFieldViewModel : FieldViewModel
    {
        private SingleField _floatField;
        public override object MinValue => float.MinValue;
        public override object MaxValue => float.MaxValue;
        public override object Value
        {
            get => ParamViewerViewModel.SelectedRow != null ? BitConverter.ToSingle(Param.Bytes, Offset) : null;
            set
            {
                if (ParamViewerViewModel.SelectedRow == null)
                    return;

                float floatVal = (float)value;
                Param.Pointer.WriteSingle(Offset, floatVal);
                byte[] bytes = BitConverter.GetBytes(floatVal);
                Array.Copy(bytes, 0, Param.Bytes, Offset, bytes.Length);
            }
        }
        public SingleFieldViewModel(ParamViewerViewModel paramViewerViewModel, SingleField floatField) : base(paramViewerViewModel, floatField)
        {
            _floatField = floatField;
        }
    }
}

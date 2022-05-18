using System.Threading.Tasks.Dataflow;
using static Erd_Tools.Models.Param;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class PartialNumericViewModel : FieldViewModel
    {
        private PartialNumericField _partialNumericField;
        public override object MinValue { get; }
        public override object MaxValue { get; }
        public override string StringValue => Value?.ToString() ?? "null";
        private int _bitPosition  => _partialNumericField.BitPosition;
        private int _bitsUsed  => _partialNumericField.BitsUsed;
        public byte? Value
        {
            get
            {
                if (ParamViewerViewModel.SelectedRow == null)
                    return null;

                byte val = Param.Bytes[Offset];
                byte shift = (byte)(_bitPosition > 0 ? 8 % _bitPosition : 0);
                val = (byte)(val << shift);
                val = (byte)(val >> (8 - _bitPosition));

                return val;
            }
            set
            {
                if (ParamViewerViewModel.SelectedRow == null || value == null)
                    return;

                if (value.Value > Convert.ToByte(MaxValue))
                {
                    OnPropertyChanged(nameof(Value));
                    return;
                }

                byte mask = (byte)((int)MaxValue << _bitPosition);
                byte oldVal = Param.Bytes[Offset];
                byte val = (byte) (oldVal & ~mask);
                val |= (byte)(value.Value << _bitPosition);

                Param.Pointer.WriteByte(Offset, val);
                Param.Bytes[Offset] = val;
            }
        }

        public PartialNumericViewModel(ParamViewViewModel paramViewerViewModel, PartialNumericField partialNumericField) : base(paramViewerViewModel, partialNumericField)
        {
            _partialNumericField = partialNumericField;
            MaxValue = (byte)(1 << _bitsUsed) - 1;
            MinValue = byte.MinValue;
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

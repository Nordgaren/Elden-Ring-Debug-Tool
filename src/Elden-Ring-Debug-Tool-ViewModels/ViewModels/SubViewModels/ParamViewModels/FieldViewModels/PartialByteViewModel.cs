using System.Threading.Tasks.Dataflow;
using static Erd_Tools.Models.Param;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class PartialByteViewModel : FieldViewModel
    {
        private PartialByteField _partialByteField { get; }
        public override object MinValue { get; }
        private byte _maxValue { get; }
        public override object MaxValue => _maxValue;
        public override string StringValue => Value?.ToString() ?? "null";
        public override string InternalName => $"{_partialByteField.InternalName} ({_maxValue})";
        private int _bitPosition  => _partialByteField.BitPosition;
        private int _width  => _partialByteField.Width;
        private int _typeSize => GetSize() * 8;
        public byte? Value
        {
            get
            {
                if (ParamViewerViewModel.SelectedRow == null)
                    return null;

                byte val = Param.Bytes[Offset];
                val <<= _typeSize - _width - _bitPosition;
                val >>= 8 - _width;
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

                byte mask = (byte)((uint)_maxValue << _bitPosition);
                byte oldVal = Param.Bytes[Offset];
                byte val = (byte)(oldVal & ~mask);
                val |= (byte)(value.Value << _bitPosition);

                Param.Pointer.WriteByte(Offset, val);
                Param.Bytes[Offset] = val;
            }
        }
        public PartialByteViewModel(ParamViewViewModel paramViewerViewModel, PartialByteField partialByteField) : base(paramViewerViewModel, partialByteField)
        {
            _partialByteField = partialByteField;
            _maxValue = (byte)((1 << _width) - 1);
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

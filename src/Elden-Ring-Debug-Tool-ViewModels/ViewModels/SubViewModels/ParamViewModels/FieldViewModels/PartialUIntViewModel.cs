using System.Threading.Tasks.Dataflow;
using static Erd_Tools.Models.Param;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class PartialUintViewModel : FieldViewModel
    {
        private PartialUIntField _partialUIntField { get; }
        public override object MinValue { get; }
        public override object MaxValue { get; }
        public override string StringValue => Value?.ToString() ?? "null";
        private int _bitPosition  => _partialUIntField.BitPosition;
        private int _width  => _partialUIntField.Width;
        private int _typeSize => GetSize() * 8;
        public uint? Value
        {
            get
            {
                if (ParamViewerViewModel.SelectedRow == null)
                    return null;

                uint val = Param.Bytes[Offset];
                val <<= (_typeSize - _width - _bitPosition);
                val >>= 8 - _width;
                return val;
            }
            set
            {
                if (ParamViewerViewModel.SelectedRow == null || value == null)
                    return;

                if (value.Value > Convert.ToUInt32(MaxValue))
                {
                    OnPropertyChanged(nameof(Value));
                    return;
                }

                uint mask = (uint)((uint)MaxValue << _bitPosition);
                uint oldVal = Param.Bytes[Offset];
                uint val = (uint)(oldVal & ~mask);
                val |= (uint)(value.Value << _bitPosition);

                Param.Pointer.WriteUInt32(Offset, val);
                byte[] bytes = BitConverter.GetBytes(val);
                Array.Copy(bytes, 0, Param.Bytes, Offset, bytes.Length);
            }
        }
        public PartialUintViewModel(ParamViewViewModel paramViewerViewModel, PartialUIntField partialUIntField) : base(paramViewerViewModel, partialUIntField)
        {
            _partialUIntField = partialUIntField;
            MaxValue = (byte)(1 << _width) - 1;
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

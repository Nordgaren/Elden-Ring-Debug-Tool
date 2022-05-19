using System.Threading.Tasks.Dataflow;
using static Erd_Tools.Models.Param;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class PartialUShortViewModel : FieldViewModel
    {
        private PartialUShortField _partialUShortField { get; }
        public override object MinValue { get; }
        public override object MaxValue { get; }
        public override string StringValue => Value?.ToString() ?? "null";
        private int _bitPosition  => _partialUShortField.BitPosition;
        private int _width  => _partialUShortField.Width;
        private int _typeSize => GetSize() * 8;
        public ushort? Value
        {
            get
            {
                if (ParamViewerViewModel.SelectedRow == null)
                    return null;

                ushort val = Param.Bytes[Offset];
                val <<= (_typeSize - _width - _bitPosition);
                val >>= 8 - _width;
                return val;
            }
            set
            {
                if (ParamViewerViewModel.SelectedRow == null || value == null)
                    return;

                if (value.Value > Convert.ToUInt16(MaxValue))
                {
                    OnPropertyChanged(nameof(Value));
                    return;
                }

                ushort mask = (ushort)((int)MaxValue << _bitPosition);
                ushort oldVal = Param.Bytes[Offset];
                ushort val = (ushort)(oldVal & ~mask);
                val |= (ushort)(value.Value << _bitPosition);

                Param.Pointer.WriteUInt16(Offset, val);
                byte[] bytes = BitConverter.GetBytes(val); 
                Array.Copy(bytes, 0, Param.Bytes, Offset, bytes.Length);
            }
        }
        public PartialUShortViewModel(ParamViewViewModel paramViewerViewModel, PartialUShortField partialUShortField) : base(paramViewerViewModel, partialUShortField)
        {
            _partialUShortField = partialUShortField;
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

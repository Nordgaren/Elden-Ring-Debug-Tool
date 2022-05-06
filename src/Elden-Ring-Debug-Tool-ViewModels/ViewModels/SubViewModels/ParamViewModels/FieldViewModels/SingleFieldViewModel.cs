using static Erd_Tools.Models.Param;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class SingleFieldViewModel : FieldViewModel
    {
        private SingleField _floatField;
        public override object MinValue => float.MinValue;
        public override object MaxValue => float.MaxValue;
        public override string StringValue => Value?.ToString() ?? "null";
        public float? Value
        {
            get => ParamViewerViewModel.SelectedRow != null ? BitConverter.ToSingle(Param.Bytes, Offset) : null;
            set
            {
                if (ParamViewerViewModel.SelectedRow == null || value == null)
                    return;

                Param.Pointer.WriteSingle(Offset, value.Value);
                byte[] bytes = BitConverter.GetBytes(value.Value);
                Array.Copy(bytes, 0, Param.Bytes, Offset, bytes.Length);
            }
        }
        public SingleFieldViewModel(ParamViewViewModel paramViewerViewModel, SingleField floatField) : base(paramViewerViewModel, floatField)
        {
            _floatField = floatField;
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

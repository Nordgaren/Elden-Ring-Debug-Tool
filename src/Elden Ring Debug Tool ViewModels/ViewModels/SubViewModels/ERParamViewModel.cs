using Elden_Ring_Debug_Tool;
using System.Collections.ObjectModel;
using static Elden_Ring_Debug_Tool.ERParam;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class ERParamViewModel : ViewModelBase
    {
        public ParamViewerViewModel ParamViewerViewModel { get; }
        public ERParam Param { get; }
        public int Offset => Param.Offset;
        public string Name => Param.Name;
        public string Type => Param.Type;
        public int Length => Param.Length;
        public byte[] Bytes => Param.Bytes;
        public ObservableCollection<RowViewModel> Rows { get; }
        public ObservableCollection<FieldViewModel> Fields { get; }
        public Dictionary<int, string> NameDictionary => Param.NameDictionary;
        public Dictionary<int, int> OffsetDict  => Param.OffsetDict;
        public int RowLength => Param.RowLength;

        public ERParamViewModel(ParamViewerViewModel paramViewerViewModel, ERParam param)
        {
            ParamViewerViewModel = paramViewerViewModel;
            Param = param;
            Rows = new ObservableCollection<RowViewModel>();

            foreach (Row row in param.Rows)
            {
                Rows.Add(new RowViewModel(row));
            }

            Fields = new ObservableCollection<FieldViewModel>();
            foreach (Field field in param.Fields)
            {
                Fields.Add(GetFieldViewModel(field));
            }
        }

        private FieldViewModel GetFieldViewModel(Field field)
        {
            if (field is NumericField numField)
            {
                if (numField.IsSigned)
                    return new NumericSignedViewModel(ParamViewerViewModel, numField);
                else
                    return new NumericViewModel(ParamViewerViewModel, numField);
            }

            if (field is SingleField floatField)
            {
                return new SingleFieldViewModel(ParamViewerViewModel, floatField);
            }

            if (field is FixedStr fixedStr)
            {
                return new FixedStrViewModel(ParamViewerViewModel, fixedStr);
            }

            if (field is BitField bitField)
            {
                return new BitFieldViewModel(ParamViewerViewModel, bitField);
            }

            throw new Exception($"No view model found for {field.InternalName}");
        }

        public override string ToString()
        {
            return Name;
        }

    }
}

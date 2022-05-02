using Erd_Tools;
using System.Collections.ObjectModel;
using static Erd_Tools.ERParam;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class ERParamViewModel : ViewModelBase
    {
        public ParamViewViewModel ParamViewerViewModel { get; }
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

        public ERParamViewModel(ParamViewViewModel paramViewerViewModel, ERParam param)
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
                switch (field.Type)
                {
                    case SoulsFormats.PARAMDEF.DefType.s8:
                        return new NumericSignedViewModel<sbyte>(ParamViewerViewModel, numField);
                    case SoulsFormats.PARAMDEF.DefType.dummy8:
                    case SoulsFormats.PARAMDEF.DefType.u8:
                         return new NumericViewModel<byte>(ParamViewerViewModel, numField);
                    case SoulsFormats.PARAMDEF.DefType.s16:
                        return new NumericSignedViewModel<short>(ParamViewerViewModel, numField);
                    case SoulsFormats.PARAMDEF.DefType.u16:
                         return new NumericViewModel<ushort>(ParamViewerViewModel, numField);
                    case SoulsFormats.PARAMDEF.DefType.s32:
                        return new NumericSignedViewModel<int>(ParamViewerViewModel, numField);
                    case SoulsFormats.PARAMDEF.DefType.u32:
                         return new NumericViewModel<uint>(ParamViewerViewModel, numField);
                    case SoulsFormats.PARAMDEF.DefType.f32:
                    case SoulsFormats.PARAMDEF.DefType.fixstr:
                    case SoulsFormats.PARAMDEF.DefType.fixstrW:
                    default:
                        break;
                }
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

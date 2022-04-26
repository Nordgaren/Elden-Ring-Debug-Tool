using Elden_Ring_Debug_Tool;
using System.Collections.ObjectModel;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class ERParamViewModel : ViewModelBase
    {
        public ERParam Param { get; }
        //public PHPointer Pointer { get; private set; }
        public int Offset => Param.Offset;
        //public PARAMDEF ParamDef { get; private set; }
        public string Name => Param.Name;
        public string Type => Param.Type;
        public int Length => Param.Length;
        public byte[] Bytes => Param.Bytes;
        public ObservableCollection<ERParam.Row> Rows { get; }
        public ObservableCollection<FieldViewModel> Fields { get; }
        //public ERParam.Row SelectedRow => Param.SelectedRow;
        //public List<UserControl> Cells { get; set; }
        //public Dictionary<int, string> NameDictionary => Param.NameDictionary;
        //public Dictionary<int, int> OffsetDict  => Param.OffsetDict;
        public int RowLength => Param.RowLength;

        public ERParamViewModel(ERParam param)
        {
            Param = param;
            Rows = new ObservableCollection<ERParam.Row>(param.Rows);
        }
        public override string ToString()
        {
            return Name;
        }

    }
}

using Elden_Ring_Debug_Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class RowViewModel
    {
        private ERParam.Row _row;
        public string Name => _row.Name;
        public int ID => _row.ID;
        public int DataOffset => _row.DataOffset;

        public RowViewModel(ERParam.Row row)
        {
            _row = row;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

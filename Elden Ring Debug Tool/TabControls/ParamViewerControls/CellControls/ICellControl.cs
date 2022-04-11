using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool
{
    interface ICellControl
    {
        public string FieldName { get; set; }
        public string Value { get; }
    }
}
    
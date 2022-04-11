using PropertyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Elden_Ring_Debug_Tool
{
    /// <summary>
    /// Interaction logic for BitfieldControl.xaml
    /// </summary>
    public partial class BitfieldControl : UserControl, ICellControl
    {
        public ERParam Param { get; private set; }
        public int Offset { get; private set; }
        public int Position { get; private set; }
        public string FieldName { get; set; }
        public string Value { get => ParamValue ? "1" : "0"; }
        public bool ParamValue
        {
            get => ((Param.Bytes[Offset] & (1 << Position)) != 0);
            set
            {
                var paramValue = Param.Bytes[Offset];
                if (value)
                    paramValue |= (byte)(1 << Position);
                else
                   paramValue &= (byte)~(1 << Position);

                Param.Pointer.WriteByte(Offset, paramValue);
                var bytes = BitConverter.GetBytes(paramValue);
                Array.Copy(bytes, 0, Param.Bytes, Offset, bytes.Length);
            }
        }
        public BitfieldControl(ERParam param, int offset, int position, string name)
        {
            Param = param;
            Offset = offset;
            Position = position;
            FieldName = name;
            
            InitializeComponent();
        }
    }
}

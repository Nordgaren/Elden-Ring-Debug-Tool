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
    public partial class BitfieldControl : UserControl
    {
        public PHPointer Param { get; private set; }
        public int Offset { get; private set; }
        public int Position { get; private set; }
        public string FieldName { get; private set; }
        public bool ParamValue
        {
            get => ((Param?.ReadByte(Offset) & (1 << Position)) != 0);
            set
            {
                var paramValue = Param.ReadByte(Offset);
                if (value)
                    paramValue |= (byte)(1 << Position);
                else
                   paramValue &= (byte)~(1 << Position);

                Param.WriteByte(Offset, paramValue);
            }
        }
        public BitfieldControl(PHPointer param, int offset, int position, string name)
        {
            Param = param;
            Offset = offset;
            Position = position;
            FieldName = name;
            
            InitializeComponent();
        }
    }
}

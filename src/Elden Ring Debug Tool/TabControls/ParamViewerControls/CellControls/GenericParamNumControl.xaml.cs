using PropertyHook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
using Xceed.Wpf.Toolkit;

namespace Elden_Ring_Debug_Tool
{
    //https://stackoverflow.com/questions/3811179/wpf-usercontrol-with-generic-code-behind lol
    public abstract partial class GenericParamNumControl : UserControl
    {
        // If you use event handlers in GenericUserControl.xaml, you have to define 
        // them here as abstract and implement them in the generic class below, e.g.:

        // abstract protected void MouseClick(object sender, MouseButtonEventArgs e);
    }

    /// <summary>
    /// Interaction logic for ParamControl.xaml
    /// </summary>
    public partial class ParamNumControl<T> : GenericParamNumControl, ICellControl
    {
        public ERParam Param { get; private set; }
        public int Offset { get; private set; }
        public string FieldName { get; set; }
        public string Value { get=> ParamValue.ToString(); }
        public long ParamValue
        {
            get
            {
                //var bytes = Param.Pointer.ReadBytes(Offset, GetSize());
                return GetValue();
            }
            set 
            {
                var buffer = BitConverter.GetBytes(value);
                var bytes = new byte[GetSize()];
                Array.Copy(buffer, bytes, bytes.Length);
                Param.Pointer.WriteBytes(Offset, bytes);
                Array.Copy(bytes, 0, Param.Bytes, Offset, bytes.Length);
            }
        }

        private long GetValue()
        {
            switch (GetSize())
            {
                case 1:
                    return Param.Bytes[Offset];
                case 2:
                    return BitConverter.ToInt16(Param.Bytes, Offset);
                case 4:
                    return BitConverter.ToInt32(Param.Bytes, Offset);
                case 8:
                    return BitConverter.ToInt64(Param.Bytes, Offset);
                default:
                    return 0;
            }
        }


        private static uint GetSize()
        {
            return (uint)Marshal.SizeOf(typeof(T));
        }

        public ParamNumControl(ERParam param, int offset, string name)
        {
            Param = param;
            Offset = offset;
            FieldName = name;

            InitializeComponent();
            //NumControl.ValueChanged += NumControl_ValueChanged;
        }

    }


}

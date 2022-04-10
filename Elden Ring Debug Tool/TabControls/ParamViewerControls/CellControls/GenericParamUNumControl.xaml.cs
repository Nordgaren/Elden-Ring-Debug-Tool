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
    public abstract partial class GenericParamUNumControl : UserControl
    {
        // If you use event handlers in GenericUserControl.xaml, you have to define 
        // them here as abstract and implement them in the generic class below, e.g.:

        // abstract protected void MouseClick(object sender, MouseButtonEventArgs e);
    }

    /// <summary>
    /// Interaction logic for ParamControl.xaml
    /// </summary>
    public partial class ParamUNumControl<T> : GenericParamUNumControl
    {
        public PHPointer Param { get; private set; }
        public int Offset { get; private set; }
        public string FieldName { get; private set; }
        public ulong ParamValue
        {
            get
            {
                var bytes = Param.ReadBytes(Offset, GetSize());
                return GetValue(bytes);
            }
            set 
            {
                var buffer = BitConverter.GetBytes(value);
                var bytes = new byte[GetSize()];
                Array.Copy(buffer, bytes, bytes.Length);
                Param.WriteBytes(Offset, bytes);
            }
        }

        private ulong GetValue(byte[] bytes)
        {
            switch (GetSize())
            {
                case 1:
                    return bytes[0];
                case 2:
                    return BitConverter.ToUInt16(bytes);
                case 4:
                    return BitConverter.ToUInt32(bytes);
                case 8:
                    return BitConverter.ToUInt64(bytes);
                default:
                    return 0;
            }
        }

        private static uint GetSize()
        {
            return (uint)Marshal.SizeOf(typeof(T));
        }

        public ParamUNumControl(PHPointer param, int offset, string name)
        {
            Param = param;
            Offset = offset;
            FieldName = name;

            InitializeComponent();
            //UNumControl.ValueChanged += NumControl_ValueChanged;
        }

        private void NumControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //if (NumControl.Value == null)
            //    return;

            //Value = (T)Convert.ChangeType(NumControl.Value, typeof(T));
        }
    }


}

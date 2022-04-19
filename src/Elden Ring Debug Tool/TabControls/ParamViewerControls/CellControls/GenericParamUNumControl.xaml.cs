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
    public abstract partial class GenericParamUNumControl : UserControl, INotifyPropertyChanged
    { 
        // If you use event handlers in GenericUserControl.xaml, you have to define 
        // them here as abstract and implement them in the generic class below, e.g.:

    // abstract protected void MouseClick(object sender, MouseButtonEventArgs e);
    public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// Interaction logic for ParamControl.xaml
    /// </summary>
    public partial class ParamUNumControl<T> : GenericParamUNumControl, ICellControl
    {
        public ERParam Param { get; private set; }
        private int FieldOffset { get; set; }
        public int Offset => Param.SelectedRow?.DataOffset + FieldOffset ?? 0;
        public string FieldName { get; set; }
        public string Value { get => ParamValue.ToString(); }

        public ulong ParamValue
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


        private ulong GetValue()
        {
            switch (GetSize())
            {
                case 1:
                    return Param.Bytes[Offset];
                case 2:
                    return BitConverter.ToUInt16(Param.Bytes, Offset);
                case 4:
                    return BitConverter.ToUInt32(Param.Bytes, Offset);
                case 8:
                    return BitConverter.ToUInt64(Param.Bytes, Offset);
                default:
                    return 0;
            }
        }

        private static uint GetSize()
        {
            return (uint)Marshal.SizeOf(typeof(T));
        }

        public ParamUNumControl(ERParam param, int fieldOffset ,string name)
        {
            Param = param;
            FieldName = name;
            FieldOffset = fieldOffset;
            InitializeComponent();
            //UNumControl.ValueChanged += NumControl_ValueChanged;
        }

        private void NumControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //if (NumControl.Value == null)
            //    return;

            //Value = (T)Convert.ChangeType(NumControl.Value, typeof(T));
        }

        public void UpdateField()
        {
            OnPropertyChanged(nameof(ParamValue));
        }
    }


}

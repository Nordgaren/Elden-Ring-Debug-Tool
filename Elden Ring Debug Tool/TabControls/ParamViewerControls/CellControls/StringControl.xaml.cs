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
    /// <summary>
    /// Interaction logic for ParamControl.xaml
    /// </summary>
    public partial class StringControl : ICellControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public ERParam Param { get; private set; }
        private int FieldOffset { get; set; }
        private uint Length { get; set; }
        public int Offset => Param.SelectedRow?.DataOffset + FieldOffset ?? 0;
        public string FieldName { get; set; }
        public string Value { get => ParamValue.ToString(); }

        public string ParamValue
        {
            get
            {
                var bytes = new byte[Length];
                Array.Copy(Param.Bytes, Offset, bytes, 0, Length);
                return Encoding.ASCII.GetString(bytes).Replace("\0","");
            }
            set 
            {
                if (value.Length > Length)
                {
                    OnPropertyChanged(nameof(ParamValue));
                    return;
                }

                var bytes = Encoding.ASCII.GetBytes(value);
                var buffer = new byte[Length];
                Array.Copy(bytes, 0, buffer, 0, bytes.Length);
                Param.Pointer.WriteString(Offset, Encoding.ASCII, Length, value);
                Array.Copy(buffer, 0, Param.Bytes, Offset, buffer.Length);
                OnPropertyChanged(nameof(ParamValue));
            }
        }

        public StringControl(ERParam param, int fieldOffset , int length, string name)
        {
            Param = param;
            FieldName = name;
            FieldOffset = fieldOffset;
            Length = (uint)length;
            InitializeComponent();
        }

        public void UpdateField()
        {
            OnPropertyChanged(nameof(ParamValue));
            OnPropertyChanged(nameof(Value));
        }
    }


}

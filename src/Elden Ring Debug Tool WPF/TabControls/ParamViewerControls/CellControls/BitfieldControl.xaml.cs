using Elden_Ring_Debug_Tool;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace Elden_Ring_Debug_Tool_WPF
{
    /// <summary>
    /// Interaction logic for BitfieldControl.xaml
    /// </summary>
    public partial class BitfieldControl : UserControl, ICellControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public ERParam Param { get; private set; }
        private int FieldOffset { get; set; }
        public int Offset => Param.SelectedRow?.DataOffset + FieldOffset ?? 0;
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
        public BitfieldControl(ERParam param, int fieldOffset ,int position, string name)
        {
            Param = param;
            FieldOffset = fieldOffset;
            Position = position;
            FieldName = name;
            
            InitializeComponent();
        }

        public void UpdateField()
        {
            OnPropertyChanged(nameof(ParamValue));
        }
    }
}

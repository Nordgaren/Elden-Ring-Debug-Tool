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
    public abstract partial class GenericParamDecControl : UserControl
    {
        
        // If you use event handlers in GenericUserControl.xaml, you have to define 
        // them here as abstract and implement them in the generic class below, e.g.:

        // abstract protected void MouseClick(object sender, MouseButtonEventArgs e);
    }

    /// <summary>
    /// Interaction logic for ParamControl.xaml
    /// </summary>
    public partial class ParamDecControl<T> : GenericParamDecControl
    {
        public ERParam Param { get; private set; }
        public int Offset { get; private set; }
        public string FieldName { get; private set; }
        public float ParamValue
        {
            get => BitConverter.ToSingle(Param.Bytes, Offset);
            set => Param.Pointer.WriteSingle(Offset, value);
        }

        public ParamDecControl(ERParam param, int offset, string name)
        {
            Param = param;
            Offset = offset;
            FieldName = name;

            InitializeComponent();
        }

        private void DecControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
        }
    }
}

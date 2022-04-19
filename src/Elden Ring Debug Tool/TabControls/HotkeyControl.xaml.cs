using System.Windows;
using System.Windows.Controls;

namespace Elden_Ring_Debug_Tool
{
    /// <summary>
    /// Interaction logic for metaHotkey.xaml
    /// </summary>
    public partial class HotkeyControl : UserControl
    {
        public string HotkeyName
        {
            get { return (string)GetValue(HotkeyNameProperty); }
            set { SetValue(HotkeyNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HotkeyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HotkeyNameProperty =
            DependencyProperty.Register("HotkeyName", typeof(string), typeof(HotkeyControl), new PropertyMetadata(default));

        public HotkeyControl()
        {
            InitializeComponent();
        }
    }
}

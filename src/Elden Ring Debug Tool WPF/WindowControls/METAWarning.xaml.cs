using System.Windows;

namespace Elden_Ring_Debug_Tool_WPF
{
    /// <summary>
    /// Interaction logic for METAWarning.xaml
    /// </summary>
    public partial class DebugWarning : Window
    {
        public DebugWarning()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.Settings.ShowWarning = !cbxDoNotShow.IsChecked.Value;
        }
    }
}

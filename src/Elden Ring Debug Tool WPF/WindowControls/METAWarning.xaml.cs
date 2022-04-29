using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using System.Windows;

namespace Elden_Ring_Debug_Tool_WPF
{
    /// <summary>
    /// Interaction logic for METAWarning.xaml
    /// </summary>
    public partial class DebugWarning : Window
    {
        MainWindowViewModel _mainWindowViewModel;
        public DebugWarning(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _mainWindowViewModel.ShowWarning = !cbxDoNotShow.IsChecked.Value;
        }
    }
}

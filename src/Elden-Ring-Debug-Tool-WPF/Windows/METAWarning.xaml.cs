using System;
using System.Windows;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels;

namespace Elden_Ring_Debug_Tool_WPF.Windows
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

            if (cbxDoNotShow == null)
                throw new NullReferenceException($"Do Not Show ComboBox should not be null");

            _mainWindowViewModel.ShowWarning = !cbxDoNotShow.IsChecked.Value;
        }
    }
}

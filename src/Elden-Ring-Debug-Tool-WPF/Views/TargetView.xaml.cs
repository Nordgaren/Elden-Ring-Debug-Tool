using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace Elden_Ring_Debug_Tool_WPF.Views
{
    /// <summary>
    /// Interaction logic for TargetControl.xaml
    /// </summary>
    public partial class TargetView : UserControl
    {
        public TargetView()
        {
            InitializeComponent();
        }

        private TargetViewViewModel _targetViewViewModel;

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is TargetViewViewModel vm)
            {
                _targetViewViewModel = vm;
            }
        }

        private void Label_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Label label = sender as Label;

            if (label == null || string.IsNullOrWhiteSpace(label.Content.ToString()))
                return;

            try
            {
                Clipboard.SetText(label.Content.ToString());

            }
            catch (System.Exception)
            {

            }
        }

        private void IntegerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
        }
    }
}

using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace Elden_Ring_Debug_Tool_WPF.Views
{
    /// <summary>
    /// Interaction logic for TargetControl.xaml
    /// </summary>
    public partial class FaceView : UserControl
    {
        public FaceView()
        {
            InitializeComponent();
        }

        private FaceViewViewModel _faceViewViewModel;

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is FaceViewViewModel vm)
            {
                _faceViewViewModel = vm;
            }
        }
        
        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is ViewModelBase vm)
            {
                vm.IsActiveView = IsVisible; 
            }
        }
    }
}

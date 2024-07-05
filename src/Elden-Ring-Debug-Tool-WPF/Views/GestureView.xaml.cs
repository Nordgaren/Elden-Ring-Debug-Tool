using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Elden_Ring_Debug_Tool_WPF.Views
{
    public partial class GestureView : UserControl
    {
        public GestureView()
        {
            InitializeComponent();
        }
            
        private GestureViewViewModel _gestureViewViewModel;

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is GestureViewViewModel vm)
            {
                _gestureViewViewModel = vm;
            }
        }
    }
}
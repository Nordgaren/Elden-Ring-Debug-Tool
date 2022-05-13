using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Elden_Ring_Debug_Tool_WPF.Views
{
    /// <summary>
    /// Interaction logic for ParamViewerControl.xaml
    /// </summary>
    public partial class ParamView : UserControl
    {

        private ParamViewViewModel _paramViewModel;
        public ParamView()
        {
            InitializeComponent();
            DataContextChanged += new DependencyPropertyChangedEventHandler(OnDataContextChanged);
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is ParamViewViewModel vm)
            {
                _paramViewModel = vm;
            }
        }
    }
}

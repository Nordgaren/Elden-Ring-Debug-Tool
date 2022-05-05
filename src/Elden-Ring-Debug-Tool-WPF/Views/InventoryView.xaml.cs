using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Elden_Ring_Debug_Tool_WPF.Views
{
    /// <summary>
    /// Interaction logic for InventoryView.xaml
    /// </summary>
    public partial class InventoryView : UserControl
    {
        public InventoryView()
        {
            InitializeComponent();
            DataContextChanged += new DependencyPropertyChangedEventHandler(OnDataContextChanged);

        }
        private InventoryViewViewModel _itemGibViewModel;

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is InventoryViewViewModel vm)
            {
                _itemGibViewModel = vm;
            }
        }


    }
}

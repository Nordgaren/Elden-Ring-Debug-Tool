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
    /// Interaction logic for HotkeyView.xaml
    /// </summary>
    public partial class HotkeyView : UserControl
    {
        public HotkeyView()
        {
            InitializeComponent();
        }

        private HotkeyViewViewModel _itemGibViewModel;

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is HotkeyViewViewModel vm)
            {
                _itemGibViewModel = vm;
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            Key key = e.Key;
            if (key == Key.Escape)
                textBox.Text = "Unbound";
            else
                textBox.Text = key.ToString();
            e.Handled = true;

            DG.Focus();
        }

        private void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox == null)
                return;

            textBox.Background = Brushes.LightGreen;

        }

        private void TextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox == null)
                return;

            textBox.Background = Brushes.White;

        }
    }
}

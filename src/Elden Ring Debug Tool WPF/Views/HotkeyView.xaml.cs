using Elden_Ring_Debug_Tool_ViewModels.Attributes;
using Elden_Ring_Debug_Tool_ViewModels.Commands;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
using Xceed.Wpf.Toolkit;
using static Elden_Ring_Debug_Tool_ViewModels.Attributes.HotkeyParameterAttribute;

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

        private void DG_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            HotkeyViewModel hvm = e.Row.DataContext as HotkeyViewModel;
            if (hvm == null)
                return;


            StackPanel p = e.DetailsElement as StackPanel;
            if (p == null)
                return;

            e.Row.DetailsVisibility = Visibility.Hidden;

            IEnumerable<PropertyInfo> parentVmProperties = hvm.GetCustomAttributes();


            Type t = hvm.Command.GetType();
            if (hvm.Command is ToggleableCommand tCommand)
                t = tCommand.GetOriginalType();

            foreach (PropertyInfo prop in parentVmProperties)
            {
                HotkeyParameterAttribute? hKeyParamAttr = prop.GetCustomAttribute<HotkeyParameterAttribute>();

                if (hKeyParamAttr == null)
                    continue;

                if (hKeyParamAttr.CommandType != t)
                    continue;

                DescriptionAttribute? attr = prop.GetCustomAttribute<DescriptionAttribute>();

                if (attr == null)
                    throw new NullReferenceException(nameof(attr));

                e.Row.DetailsVisibility = Visibility.Visible;
                hvm.HasDependancies = true;

                StackPanel p2 = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };

                Label lab = new Label() 
                { 
                    Content = attr.Description, 
                    Margin = new Thickness(0,0,10,0),
                };
                p2.Children.Add(lab);

                Binding bItem = new Binding()
                {
                    Source = hvm.ParentViewModel,
                    Path = new PropertyPath(prop.Name)
                };

                switch (hKeyParamAttr.Type)
                {
                    case ResourceType.ComboBox:
                        ComboBox comboBox = new ComboBox()
                        {
                            DataContext = hvm.ParentViewModel
                        };
                        Binding bSelectedItem = new Binding()
                        {
                            Source = hvm.ParentViewModel,
                            Path = new PropertyPath(hKeyParamAttr.SelectedItemPropertyName)
                        };
                        BindingOperations.SetBinding(comboBox, ComboBox.SelectedValueProperty, bSelectedItem);
                        BindingOperations.SetBinding(comboBox, ComboBox.ItemsSourceProperty, bItem);
                        p2.Children.Add(comboBox);
                        break;
                    case ResourceType.NumericUpDown:
                        IntegerUpDown nud = new IntegerUpDown()
                        {
                            DataContext = hvm.ParentViewModel
                        };
                        BindingOperations.SetBinding(nud, IntegerUpDown.ValueProperty, bItem);
                        p2.Children.Add(nud);
                        break;
                    case ResourceType.DecimalUpDown:
                        DecimalUpDown dud = new DecimalUpDown()
                        {
                            DataContext = hvm.ParentViewModel
                        };
                        BindingOperations.SetBinding(dud, DecimalUpDown.ValueProperty, bItem);
                        p2.Children.Add(dud);
                        break;
                    case ResourceType.TextBox:
                        TextBox textBox = new TextBox()
                        {
                            DataContext = hvm.ParentViewModel
                        };
                        BindingOperations.SetBinding(textBox, TextBox.TextProperty, bItem);
                        p2.Children.Add(textBox);
                        break;
                    default:
                        break;
                }

                p.Children.Add(p2);
            }

        }
    }
}

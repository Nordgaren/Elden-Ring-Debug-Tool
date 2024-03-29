﻿using Elden_Ring_Debug_Tool_ViewModels.Attributes;
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
using static Elden_Ring_Debug_Tool_ViewModels.Attributes.HotKeyParameterAttribute;

namespace Elden_Ring_Debug_Tool_WPF.Views
{
    /// <summary>
    /// Interaction logic for HotKeyView.xaml
    /// </summary>
    public partial class HotKeyView : UserControl
    {
        public HotKeyView()
        {
            InitializeComponent();
        }

        private HotKeyViewViewModel _itemGibViewModel;

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is HotKeyViewViewModel vm)
            {
                _itemGibViewModel = vm;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            Key key = e.Key;
            if (key == Key.Escape)
                textBox.Text = "Unbound";
            else
                textBox.Text = key.ToString();
            e.Handled = true;
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
            e.Row.DetailsVisibility = Visibility.Hidden;

            HotKeyViewModel hvm = e.Row.DataContext as HotKeyViewModel;
            if (hvm == null)
                return;

            if (!hvm.HasDependencies)
                return;

            StackPanel p = e.DetailsElement as StackPanel;
            if (p == null)
                return;

            foreach ((PropertyInfo Prop, HotKeyParameterAttribute Parameter) dependancy in hvm.HotKeyParameterAttribute)
            {

                DescriptionAttribute? attr = dependancy.Prop.GetCustomAttribute<DescriptionAttribute>();

                if (attr == null)
                    throw new NullReferenceException(nameof(attr));

                e.Row.DetailsVisibility = Visibility.Visible;
                hvm.HasDependencies = true;

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
                    Path = new PropertyPath(dependancy.Prop.Name)
                };

                switch (dependancy.Parameter.Type)
                {
                    case ResourceType.ComboBox:
                        ComboBox comboBox = new ComboBox()
                        {
                            DataContext = hvm.ParentViewModel
                        };
                        Binding bSelectedItem = new Binding()
                        {
                            Source = hvm.ParentViewModel,
                            Path = new PropertyPath(dependancy.Parameter.SelectedItemPropertyName)
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

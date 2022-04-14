using PropertyHook;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Xml.Serialization;


namespace Elden_Ring_Debug_Tool
{
    /// <summary>
    /// Interaction logic for ParamViewerControl.xaml
    /// </summary>
    public partial class ParamViewerControl : DebugControl
    {
        private static XmlSerializer XML = new XmlSerializer(typeof(string[]));
        public List<ERParam> Params { get; private set; }
        public ParamViewerControl()
        {
            InitializeComponent();
        }

        public void HookParams()
        {
            Params = Hook.GetParams();
            FilterParams();
        }


        private void ComboBoxParams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterRows();
        }

        private void ListBoxRows_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterFields();

        }

        internal virtual void ReloadCtrl() 
        {

        }


        private void SearchBoxParam_TextChanged(object sender, TextChangedEventArgs e)
        {
            //ComboBoxParams.IsDropDownOpen = true;
            //FilterParams();
            //SearchBoxParams.Focus();
        }

        private void FilterParams()
        {
            if (string.IsNullOrWhiteSpace(SearchBoxParams.Text))
            {
                ComboBoxParams.ItemsSource = Params;
                ComboBoxParams.SelectedIndex = 0;
            }
            else
            {
                var filteredItems = new List<ERParam>();
                foreach (var param in Params)
                {
                    if (param.Name.ToLower().Contains(SearchBoxParams.Text.ToLower()))
                        filteredItems.Add(param);
                }
                ComboBoxParams.ItemsSource = filteredItems;
            }
        }
        private void SearchBoxRow_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterRows();
        }

        private void FilterRows()
        {
            var selectedParam = ((ERParam)ComboBoxParams.SelectedItem);
            if (selectedParam == null)
                return;

            ParamPanel.Children.Clear();
            if (string.IsNullOrWhiteSpace(SearchBoxRow.Text))
            {
                ListBoxRows.ItemsSource = selectedParam.Rows;
                ListBoxRows.SelectedIndex = 0;
            }
            else
            {
                var filteredItems = new List<ERParam.Row>();
                if (CbxIDSearch.IsChecked.Value)
                {
                    foreach (var param in selectedParam.Rows)
                    {
                        if (param.ID.ToString().Contains(SearchBoxRow.Text))
                            filteredItems.Add(param);
                    }
                }
                else
                {
                    foreach (var param in selectedParam.Rows)
                    {
                        if (param.Name.ToLower().Contains(SearchBoxRow.Text.ToLower()))
                            filteredItems.Add(param);
                    }
                }
                ListBoxRows.ItemsSource = filteredItems;
            }
        }

        private void SearchBoxField_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterFields();
        }

        private void FilterFields()
        {
            
            var row = ((ERParam.Row)ListBoxRows.SelectedItem);
            if (row == null)
                return;

            ParamPanel.Children.Clear();
            if (string.IsNullOrWhiteSpace(SearchBoxField.Text))
            {
                foreach (var control in row.Cells)
                {
                    ParamPanel.Children.Add(control);
                }
            }
            else
            {
                if (CbxValueSearch.IsChecked.Value)
                {
                    foreach (var control in row.Cells)
                    {
                        if (((ICellControl)control).Value.Contains(SearchBoxField.Text))
                            ParamPanel.Children.Add(control);
                    }
                }
                else
                {
                    foreach (var control in row.Cells)
                    {
                        if (((ICellControl)control).FieldName.ToLower().Contains(SearchBoxField.Text.ToLower()))
                            ParamPanel.Children.Add(control);
                    }
                }
                
            }
        }
        private string ParamSavePath => $@"{Directory.GetParent(Hook.Process.MainModule.FileName).Parent.FullName}\capture\param";
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var param = (ERParam)ComboBoxParams.SelectedItem;
            Hook.SaveParam(param);
            MessageBox.Show($@"{param.Name} saved to:
{ParamSavePath}\capture\param", "Param Saved", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", ParamSavePath);
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to reset the currently selected param to what it was when the debug tool loaded?", "Reset Param", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                var selectedParam = ((ERParam)ComboBoxParams.SelectedItem);
                selectedParam.RestoreParam();
            }
        }
    }
}

using PropertyHook;
using SoulsFormats;
using System;
using System.Collections.Generic;
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
using static SoulsFormats.PARAMDEF;


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

        public void GetParams()
        {
            Params = new List<ERParam>();
            var paramPath = $"{Util.ExeDir}/Resources/Params/";
            
            var pointerPath = $"{paramPath}/Pointers/";
            var paramPointers = Directory.GetFiles(pointerPath, "*.txt");
            foreach (var path in paramPointers)
            {
                var pointers = File.ReadAllLines(path);
                AddParam(paramPath, path, pointers);
            }

            FilterParams();
        }

        private void AddParam(string paramPath, string path, string[] pointers)
        {
            foreach (var entry in pointers)
            {
                if (!Util.IsValidTxtResource(entry))
                    continue;

                var info = entry.Split(':');
                var name = info[1];
                var defName = info.Length > 2 ? info[2] : name;

                var defPath = $"{paramPath}/Defs/{defName}.xml";
                if (!File.Exists(defPath))
                    throw new Exception($"The PARAMDEF {defName} does not exist for {entry}. If the PARAMDEF is named differently than the param name, add another \":\" and append the PARAMDEF name" +
                        $"Example: 3130:WwiseValueToStrParam_BgmBossChrIdConv:WwiseValueToStrConvertParamFormat");

                var offsetInt = new int[3] { int.Parse(info[0], System.Globalization.NumberStyles.HexNumber) ,0x80, 0x80};

                var pointer = Hook.GetParamPointer(offsetInt);

                var paramDef = XmlDeserialize(defPath);

                Params.Add(new ERParam(pointer, paramDef, name));
            }

            Params.Sort();
        }

        private void ComboBoxParams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterRows();
        }

        private void ListBoxRows_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var row = ((ERParam.Row)ListBoxRows.SelectedItem);
            if (row == null)
                return;

            ParamPanel.Children.Clear();
            foreach (var control in row.Cells)
            {
                ParamPanel.Children.Add(control);
            }
            ParamPanel.IsEnabled = true;

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
            if (ComboBoxParams.SelectedItem == null)
                return;

            ParamPanel.Children.Clear();
            var selectedParam = ((ERParam)ComboBoxParams.SelectedItem);
            if (string.IsNullOrWhiteSpace(SearchBoxRow.Text))
            {
                ListBoxRows.ItemsSource = ListBoxRows.ItemsSource = selectedParam.Rows;
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

        }

        
    }
}

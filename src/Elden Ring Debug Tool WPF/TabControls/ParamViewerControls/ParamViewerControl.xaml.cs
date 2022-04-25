using Elden_Ring_Debug_Tool;
using Elden_Ring_Debug_Tool_ViewModels;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Serialization;
using static Elden_Ring_Debug_Tool.ERParam;
using static SoulsFormats.PARAMDEF;

namespace Elden_Ring_Debug_Tool_WPF
{
    /// <summary>
    /// Interaction logic for ParamViewerControl.xaml
    /// </summary>
    public partial class ParamViewerControl : DebugControl
    {
        private ParamViewModel _paramViewModel;
        public ParamViewerControl()
        {
            InitializeComponent();
            DataContextChanged += new DependencyPropertyChangedEventHandler(OnDataContextChanged);
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is ParamViewModel vm)
            {
                _paramViewModel = vm;
            }
        }

        public void HookParams()
        {
            BuildParamCells();
        }


        private void ComboBoxParams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterRows();
        }

        private void ListBoxRows_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterFields();
        }

        internal override void ReloadCtrl() 
        {

        }
        private Row _selectedRow;
        public Row SelectedRow
        {
            get
            {
                return _selectedRow;
            }
            set
            {
                _selectedRow = value;
                var param = ((ERParam)ComboBoxParams.SelectedItem);
                if (param == null)
                    return;
                param.SelectedRow = value;
                foreach (ICellControl control in param.Cells)
                {
                    control.UpdateField();
                }
            }
        }

        List<Brush> BrushList = new List<Brush>() { new SolidColorBrush(Color.FromRgb(0xC3, 0xC3, 0xC3)), Brushes.LightGray };

        internal void BuildParamCells()
        {
            foreach (ERParam p in _paramViewModel.ParamCollectionView)
            {
                BuildCells(p);
            }
        }

        private void BuildCells(ERParam param)
        {
            param.Cells = new List<UserControl>();
            int totalSize = 0;
            for (int i = 0; i < param.ParamDef.Fields.Count; i++)
            {
                Field field = param.ParamDef.Fields[i];
                DefType type = field.DisplayType;
                var size = ParamUtil.IsArrayType(type) ? ParamUtil.GetValueSize(type) * field.ArrayLength : ParamUtil.GetValueSize(type);
                if (ParamUtil.IsArrayType(type))
                    totalSize += ParamUtil.GetValueSize(type) * field.ArrayLength;
                else
                    totalSize += ParamUtil.GetValueSize(type);

                if (ParamUtil.IsBitType(type) && field.BitSize != -1)
                {
                    int bitOffset = field.BitSize;
                    DefType bitType = type == DefType.dummy8 ? DefType.u8 : type;
                    int bitLimit = ParamUtil.GetBitLimit(bitType);
                    for (; i < param.ParamDef.Fields.Count - 1; i++)
                    {
                        var bitfield = new BitfieldControl(param, totalSize - size, bitOffset - 1, param.ParamDef.Fields[i].InternalName);
                        bitfield.Background = BrushList[i % BrushList.Count];
                        param.Cells.Add(bitfield);
                        Field nextField = param.ParamDef.Fields[i + 1];
                        DefType nextType = nextField.DisplayType;
                        if (!ParamUtil.IsBitType(nextType) || nextField.BitSize == -1 || bitOffset + nextField.BitSize > bitLimit
                            || (nextType == DefType.dummy8 ? DefType.u8 : nextType) != bitType)
                            break;
                        bitOffset += nextField.BitSize;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case DefType.u8:
                            var u8 = new ParamUNumControl<byte>(param, totalSize - size, field.InternalName);
                            u8.Background = BrushList[i % BrushList.Count];
                            param.Cells.Add(u8);
                            break;
                        case DefType.s8:
                            var s8 = new ParamNumControl<sbyte>(param, totalSize - size, field.InternalName);
                            s8.Background = BrushList[i % BrushList.Count];
                            param.Cells.Add(s8);
                            break;
                        case DefType.u16:
                            var u16 = new ParamUNumControl<ushort>(param, totalSize - size, field.InternalName);
                            u16.Background = BrushList[i % BrushList.Count];
                            param.Cells.Add(u16);
                            break;
                        case DefType.s16:
                            var s16 = new ParamNumControl<short>(param, totalSize - size, field.InternalName);
                            s16.Background = BrushList[i % BrushList.Count];
                            param.Cells.Add(s16);
                            break;
                        case DefType.u32:
                            var u32 = new ParamUNumControl<uint>(param, totalSize - size, field.InternalName);
                            u32.Background = BrushList[i % BrushList.Count];
                            param.Cells.Add(u32);
                            break;
                        case DefType.s32:
                            var s32 = new ParamNumControl<int>(param, totalSize - size, field.InternalName);
                            s32.Background = BrushList[i % BrushList.Count];
                            param.Cells.Add(s32);
                            break;
                        case DefType.f32:
                            var f32 = new ParamDecControl<float>(param, totalSize - size, field.InternalName);
                            f32.Background = BrushList[i % BrushList.Count];
                            param.Cells.Add(f32);
                            break;
                        case DefType.dummy8:
                            var dummy8 = new ParamNumControl<byte>(param, totalSize - size, field.InternalName);
                            dummy8.Background = BrushList[i % BrushList.Count];
                            param.Cells.Add(dummy8);
                            break;
                        case DefType.fixstr:
                        case DefType.fixstrW:
                            var fixStr = new StringControl(param, totalSize - size, field.ArrayLength, field.InternalName);
                            fixStr.Background = BrushList[i % BrushList.Count];
                            param.Cells.Add(fixStr);
                            break;
                        default:
                            throw new Exception($"No control for this type {type}");
                    }
                }
            }
        }


        private void SearchBoxParam_TextChanged(object sender, TextChangedEventArgs e)
        {
            //ComboBoxParams.IsDropDownOpen = true;
            //FilterParams();
            //SearchBoxParams.Focus();
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
            var selectedParam = ((ERParam)ComboBoxParams.SelectedItem);
            if (selectedParam == null)
                return;

            var row = ((ERParam.Row)ListBoxRows.SelectedItem);
            if (row == null)
                return;

            SelectedRow = row;

            ParamPanel.Children.Clear();
            if (string.IsNullOrWhiteSpace(SearchBoxField.Text))
            {
                foreach (var control in selectedParam.Cells)
                {
                    ParamPanel.Children.Add(control);
                }
            }
            else
            {
                if (CbxValueSearch.IsChecked.Value)
                {
                    foreach (var control in selectedParam.Cells)
                    {
                        if (((ICellControl)control).Value.Contains(SearchBoxField.Text))
                            ParamPanel.Children.Add(control);
                    }
                }
                else
                {
                    foreach (var control in selectedParam.Cells)
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

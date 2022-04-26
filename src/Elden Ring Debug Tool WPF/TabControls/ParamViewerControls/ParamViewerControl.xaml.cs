using Elden_Ring_Debug_Tool;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
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

        private ParamViewerViewModel _paramViewModel;
        public ParamViewerControl()
        {
            InitializeComponent();
            DataContextChanged += new DependencyPropertyChangedEventHandler(OnDataContextChanged);
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is ParamViewerViewModel vm)
            {
                _paramViewModel = vm;
            }
        }

        public void HookParams()
        {
            BuildParamCells();
        }

        internal override void ReloadCtrl() 
        {

        }
        List<Brush> BrushList = new List<Brush>() { new SolidColorBrush(Color.FromRgb(0xC3, 0xC3, 0xC3)), Brushes.LightGray };

        internal void BuildParamCells()
        {
            foreach (ERParamViewModel p in _paramViewModel.ParamCollectionView)
            {
                BuildCells(p.Param);
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
    }
}

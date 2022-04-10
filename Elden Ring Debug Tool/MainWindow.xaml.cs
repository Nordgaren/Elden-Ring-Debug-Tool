using PropertyHook;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xceed.Wpf.Toolkit;
using System.Xml;
using System.Xml.Serialization;

namespace Elden_Ring_Debug_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ERItemCategory.GetItemCategories();
            ERGem.GetGems();
            Hook.OnSetup += Hook_OnSetup;
        }

        private void Hook_OnSetup(object? sender, PropertyHook.PHEventArgs e)
        {
            Dispatcher.Invoke(() => 
            {
                DebugParam.GetParams();
            });
        }

        ERHook Hook => ViewModel.Hook;

        Timer UpdateTimer = new Timer();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTimer.Interval = 16;
            UpdateTimer.Elapsed += UpdateTimer_Elapsed;
            UpdateTimer.Enabled = true;
        }

        //private void GetOffsets(PARAMDEF paramDEF, PHPointer pointer)
        //{
        //    ParamPanel.Children.Clear();

        //    int totalSize = 0;
        //    paramDEF.GetRowSize();
        //    for (int i = 0; i < paramDEF.Fields.Count; i++)
        //    {
        //        Field field = paramDEF.Fields[i];
        //        DefType type = field.DisplayType;
        //        var size = ParamUtil.IsArrayType(type) ? ParamUtil.GetValueSize(type) * field.ArrayLength : ParamUtil.GetValueSize(type);
        //        if (ParamUtil.IsArrayType(type))
        //            totalSize += ParamUtil.GetValueSize(type) * field.ArrayLength;
        //        else
        //            totalSize += ParamUtil.GetValueSize(type);

        //        if (ParamUtil.IsBitType(type) && field.BitSize != -1)
        //        {
        //            int bitOffset = field.BitSize;
        //            DefType bitType = type == DefType.dummy8 ? DefType.u8 : type;
        //            int bitLimit = ParamUtil.GetBitLimit(bitType);
        //            for (; i < paramDEF.Fields.Count - 1; i++)
        //            {
        //                var bitfield = new BitfieldControl(pointer, totalSize - size, bitOffset - 1, paramDEF.Fields[i].InternalName);
        //                ParamPanel.Children.Add(bitfield);
        //                Field nextField = paramDEF.Fields[i + 1];
        //                DefType nextType = nextField.DisplayType;
        //                if (!ParamUtil.IsBitType(nextType) || nextField.BitSize == -1 || bitOffset + nextField.BitSize > bitLimit
        //                    || (nextType == DefType.dummy8 ? DefType.u8 : nextType) != bitType)
        //                    break;
        //                bitOffset += nextField.BitSize;
        //            }
        //        }
        //        else
        //        {
        //            switch (type)
        //            {
        //                case DefType.u8:
        //                    var u8 = new ParamUNumControl<byte>(pointer, totalSize - size, field.InternalName);
        //                    ParamPanel.Children.Add(u8);
        //                    break;
        //                case DefType.s8:
        //                    var s8 = new ParamNumControl<sbyte>(pointer, totalSize - size, field.InternalName);
        //                    ParamPanel.Children.Add(s8);
        //                    break;
        //                case DefType.u16:
        //                    var u16 = new ParamUNumControl<ushort>(pointer, totalSize - size, field.InternalName);
        //                    ParamPanel.Children.Add(u16);
        //                    break;
        //                case DefType.s16:
        //                    var s16 = new ParamNumControl<short>(pointer, totalSize - size, field.InternalName);
        //                    ParamPanel.Children.Add(s16);
        //                    break;
        //                case DefType.u32:
        //                    var u32 = new ParamUNumControl<uint>(pointer, totalSize - size, field.InternalName);
        //                    ParamPanel.Children.Add(u32);
        //                    break;
        //                case DefType.s32:
        //                    var s32 = new ParamNumControl<int>(pointer, totalSize - size, field.InternalName);
        //                    ParamPanel.Children.Add(s32);
        //                    break;
        //                case DefType.f32:
        //                    var f32 = new ParamDecControl<float>(pointer, totalSize - size, field.InternalName);
        //                    ParamPanel.Children.Add(f32);
        //                    break;
        //                case DefType.fixstr:
        //                case DefType.fixstrW:
        //                case DefType.dummy8:
        //                    var dummy8 = new ParamNumControl<byte>(pointer, totalSize - size, field.InternalName);
        //                    ParamPanel.Children.Add(dummy8);
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }

                
        //    }
        //}

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UpdateTimer.Stop();
        }
        private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Hook.Update();
            }));
        }
    }
}

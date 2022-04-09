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
using Xceed.Wpf.Toolkit;
using static SoulsFormats.PARAMDEF;

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
        }

        ERHook Hook => ViewModel.Hook;

        Timer UpdateTimer = new Timer();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTimer.Interval = 16;
            UpdateTimer.Elapsed += UpdateTimer_Elapsed;
            UpdateTimer.Enabled = true;
            var defPath = @"C:\Users\Nord\source\repos\Paramdex\ER\Defs\EquipParamWeapon.xml";
            var defs = XmlDeserialize(defPath);
            GetOffsets(defs);
        }

        private void GetOffsets(PARAMDEF paramDEF)
        {
            ParamPanel.Children.Clear();

            int totalSize = 0;
            for (int i = 0; i < paramDEF.Fields.Count; i++)
            {
                Field field = paramDEF.Fields[i];
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
                    Debug.WriteLine($"Bitoffset: {bitOffset}");
                    for (; i < paramDEF.Fields.Count - 1; i++)
                    {
                        Field nextField = paramDEF.Fields[i + 1];
                        DefType nextType = nextField.DisplayType;
                        if (!ParamUtil.IsBitType(nextType) || nextField.BitSize == -1 || bitOffset + nextField.BitSize > bitLimit
                            || (nextType == DefType.dummy8 ? DefType.u8 : nextType) != bitType)
                            break;
                        bitOffset += nextField.BitSize;
                        Debug.WriteLine($"Bitoffset??: {bitOffset}");
                    }

                }

                switch (type)
                {
                    case DefType.s8:
                        break;
                    case DefType.s16:
                        break;
                    case DefType.u16:
                        break;
                    case DefType.s32:
                        break;
                    case DefType.u32:
                        break;
                    case DefType.f32:
                        break;
                    case DefType.dummy8:
                        break;
                    case DefType.fixstr:
                        break;
                    case DefType.fixstrW:
                        break;
                    case DefType.u8:
                    default:
                        break;
                }
            }
        }

        public void Trash()
        {
            ParamPanel.Children.Clear();

                var bonfireControl = new IntegerUpDown();
                Binding binding = new Binding("Value")
                {
                    Source = Hook,
                    Path = new PropertyPath(bonfire.Replace(" ", "").Replace("'", ""))
                };
                bonfireControl.SetBinding(IntegerUpDown.ValueProperty, binding);
                bonfireControl.Minimum = 0;
                bonfireControl.Maximum = 99;
                bonfireControl.Label = bonfire;
                bonfireControl.nudValue.Margin = new Thickness(0, 5, 0, 0);
                ParamPanel.Children.Add(bonfireControl);
        }
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

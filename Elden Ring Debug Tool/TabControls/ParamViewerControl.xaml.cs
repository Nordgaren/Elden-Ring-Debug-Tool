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
            var paramPointers = Directory.GetFiles(pointerPath);
            foreach (var path in paramPointers)
            {
                var pointers = File.ReadAllLines(path);
                AddParam(paramPath, path, pointers);
            }

            ComboBoxParams.ItemsSource = Params;
            ComboBoxParams.SelectedIndex = 0;
        }

        private void AddParam(string paramPath, string path, string[] pointers)
        {
            foreach (var entry in pointers)
            {
                var info = entry.Split(':');
                var name = info[1];
                var defPath = $"{paramPath}/Defs/{name}.xml";
                if (!File.Exists(defPath))
                    continue;

                var offsetInt = new int[3] { int.Parse(info[0], System.Globalization.NumberStyles.HexNumber) ,0x80, 0x80};

                var pointer = Hook.GetParamPointer(offsetInt);

                var paramDef = XmlDeserialize(defPath);

                Params.Add(new ERParam(pointer, paramDef, name));
            }

            Params.Sort();
        }

        private void ComboBoxParams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ParamPanel.Children.Clear();
            ListBoxRows.ItemsSource = ((ERParam)ComboBoxParams.SelectedItem).Rows;
        }

        private void ListBoxRows_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var param = ((ERParam)ComboBoxParams.SelectedItem);
            var row = ((ERParam.Row)ListBoxRows.SelectedItem);
            if (row == null || param == null)
                return;

            GetOffsets(param.ParamDef, Hook.CreateBasePointer(param.Pointer.Resolve() + row.DataOffset));
        }

        private void GetOffsets(PARAMDEF paramDEF, PHPointer pointer)
        {
            ParamPanel.Children.Clear();

            int totalSize = 0;
            paramDEF.GetRowSize();
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
                    for (; i < paramDEF.Fields.Count - 1; i++)
                    {
                        var bitfield = new BitfieldControl(pointer, totalSize - size, bitOffset - 1, paramDEF.Fields[i].InternalName);
                        ParamPanel.Children.Add(bitfield);
                        Field nextField = paramDEF.Fields[i + 1];
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
                            var u8 = new ParamUNumControl<byte>(pointer, totalSize - size, field.InternalName);
                            ParamPanel.Children.Add(u8);
                            break;
                        case DefType.s8:
                            var s8 = new ParamNumControl<sbyte>(pointer, totalSize - size, field.InternalName);
                            ParamPanel.Children.Add(s8);
                            break;
                        case DefType.u16:
                            var u16 = new ParamUNumControl<ushort>(pointer, totalSize - size, field.InternalName);
                            ParamPanel.Children.Add(u16);
                            break;
                        case DefType.s16:
                            var s16 = new ParamNumControl<short>(pointer, totalSize - size, field.InternalName);
                            ParamPanel.Children.Add(s16);
                            break;
                        case DefType.u32:
                            var u32 = new ParamUNumControl<uint>(pointer, totalSize - size, field.InternalName);
                            ParamPanel.Children.Add(u32);
                            break;
                        case DefType.s32:
                            var s32 = new ParamNumControl<int>(pointer, totalSize - size, field.InternalName);
                            ParamPanel.Children.Add(s32);
                            break;
                        case DefType.f32:
                            var f32 = new ParamDecControl<float>(pointer, totalSize - size, field.InternalName);
                            ParamPanel.Children.Add(f32);
                            break;
                        case DefType.fixstr:
                        case DefType.fixstrW:
                        case DefType.dummy8:
                            var dummy8 = new ParamNumControl<byte>(pointer, totalSize - size, field.InternalName);
                            ParamPanel.Children.Add(dummy8);
                            break;
                        default:
                            break;
                    }
                }


            }
        }
    }
}

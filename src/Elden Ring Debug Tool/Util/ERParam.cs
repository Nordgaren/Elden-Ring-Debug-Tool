using PropertyHook;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using static SoulsFormats.PARAMDEF;

namespace Elden_Ring_Debug_Tool
{
    public class ERParam : IComparable<ERParam>
    {
        public PHPointer Pointer { get; private set; }
        public int Offset { get; private set; }
        public PARAMDEF ParamDef { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }
        public int Length { get; private set; }
        public byte[] Bytes { get; private set; }
        public List<Row> Rows { get; private set; }
        public List<UserControl> Cells { get; private set; }
        private static Regex ParamEntryRx = new Regex(@"^\s*(?<id>\S+)\s+(?<name>.*)$", RegexOptions.CultureInvariant);
        public Dictionary<int, string> NameDictionary { get; private set; }
        public Dictionary<int, int> OffsetDict { get; private set; }
        public int RowLength { get; private set; }
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
                foreach (ICellControl control in Cells)
                {
                    control.UpdateField();
                }
            }
        }

        private void BuildCells()
        {
            Cells = new List<UserControl>();
            int totalSize = 0;
            for (int i = 0; i < ParamDef.Fields.Count; i++)
            {
                Field field = ParamDef.Fields[i];
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
                    for (; i < ParamDef.Fields.Count - 1; i++)
                    {
                        var bitfield = new BitfieldControl(this, totalSize - size, bitOffset - 1, ParamDef.Fields[i].InternalName);
                        Cells.Add(bitfield);
                        Field nextField = ParamDef.Fields[i + 1];
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
                            var u8 = new ParamUNumControl<byte>(this, totalSize - size, field.InternalName);
                            Cells.Add(u8);
                            break;
                        case DefType.s8:
                            var s8 = new ParamNumControl<sbyte>(this, totalSize - size, field.InternalName);
                            Cells.Add(s8);
                            break;
                        case DefType.u16:
                            var u16 = new ParamUNumControl<ushort>(this, totalSize - size, field.InternalName);
                            Cells.Add(u16);
                            break;
                        case DefType.s16:
                            var s16 = new ParamNumControl<short>(this, totalSize - size, field.InternalName);
                            Cells.Add(s16);
                            break;
                        case DefType.u32:
                            var u32 = new ParamUNumControl<uint>(this, totalSize - size, field.InternalName);
                            Cells.Add(u32);
                            break;
                        case DefType.s32:
                            var s32 = new ParamNumControl<int>(this, totalSize - size, field.InternalName);
                            Cells.Add(s32);
                            break;
                        case DefType.f32:
                            var f32 = new ParamDecControl<float>(this, totalSize - size, field.InternalName);
                            Cells.Add(f32);
                            break;
                        case DefType.dummy8:
                            var dummy8 = new ParamNumControl<byte>(this, totalSize - size, field.InternalName);
                            Cells.Add(dummy8);
                            break;
                        case DefType.fixstr:
                        case DefType.fixstrW:
                            var fixStr = new StringControl(this, totalSize - size, field.ArrayLength, field.InternalName);
                            Cells.Add(fixStr);
                            break;
                        default:
                            throw new Exception($"No control for this type {type}");
                    }
                }


            }
        }
        public ERParam(PHPointer pointer, int offset, PARAMDEF Paramdef, string name)
        {
            Pointer = pointer;
            Offset = offset;
            ParamDef = Paramdef;
            Name = name;
            Type = Paramdef.ParamType;
            BuildNameDictionary();
            BuildOffsetDictionary();
            RowLength = ParamDef.GetRowSize();
            BuildCells();
        }
        private void BuildOffsetDictionary()
        {
            Rows = new List<Row>();
            OffsetDict = new Dictionary<int, int>();
            Length = Pointer.ReadInt32((int)EROffsets.Param.NameOffset);
            
            var paramType = Pointer.ReadString(Length, Encoding.UTF8, (uint)Type.Length);
            if (paramType != Type)
                throw new InvalidOperationException($"Incorrect Param Pointer: {paramType} should be {Type}");

            Bytes = Pointer.ReadBytes(0x0, (uint)Length);

            var tableLength = BitConverter.ToInt32(Bytes ,(int)EROffsets.Param.TableLength);
            var Param = 0x40;
            var ParamID = 0x0;
            var ParamOffset = 0x8;
            var nextParam = 0x18;

            while (Param < tableLength)
            {
                var itemID = BitConverter.ToInt32(Bytes, Param + ParamID);
                var itemParamOffset = BitConverter.ToInt32(Bytes, Param + ParamOffset);
                var name = $"{itemID} - ";
                if (NameDictionary.ContainsKey(itemID))
                    name += $"{NameDictionary[itemID]}";

                if (!OffsetDict.ContainsKey(itemID))
                    OffsetDict.Add(itemID, itemParamOffset);

                Rows.Add(new Row(this ,name, itemID, itemParamOffset));

                Param += nextParam;
            }
        }
        private void BuildNameDictionary()
        {
            NameDictionary = new Dictionary<int, string>();
            string result = Util.GetTxtResource(@$"Resources/Params/Names/{Name}.txt");
            if (string.IsNullOrWhiteSpace(result))
                return;

            foreach (string line in result.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!line.Contains("//")) //determine if line is a valid resource or not
                {
                    Match itemEntry = ParamEntryRx.Match(line);
                    var name = itemEntry.Groups["name"].Value.Replace("\r", "");
                    var id = Convert.ToInt32(itemEntry.Groups["id"].Value);
                    if (NameDictionary.ContainsKey(id))
                        continue;

                    NameDictionary.Add(id, name);
                }
            };
        }
        public override string ToString()
        {
            return Name;
        }
        public int CompareTo(ERParam? other)
        {
            return this.Name.CompareTo(other.Name);
        }
        internal void RestoreParam()
        {
            Pointer.WriteBytes(0 ,Bytes);
        }
        public class Row
        {
            public ERParam Param { get; private set; }
            public string Name { get; private set; }
            public int ID { get; private set; }
            public int DataOffset { get; private set; }

            public Row(ERParam param, string name, int id, int offset)
            {
                Param = param;
                Name = name;
                ID = id;
                DataOffset = offset;
            }


            public override string ToString()
            {
                return Name;
            }
        }

       
    }
}

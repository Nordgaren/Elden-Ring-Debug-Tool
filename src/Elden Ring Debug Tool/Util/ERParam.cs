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
using System.Windows.Media;
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
        public List<Field> Fields { get; private set; }
        private static Regex ParamEntryRx = new Regex(@"^\s*(?<id>\S+)\s+(?<name>.*)$", RegexOptions.CultureInvariant);
        public Dictionary<int, string> NameDictionary { get; private set; }
        public Dictionary<int, int> OffsetDict { get; private set; }
        public int RowLength { get; private set; }
       
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
            string[] result = Util.GetListResource(@$"Resources/Params/Names/{Name}.txt");
            if (result.Length == 0)
                return;

            foreach (string line in result)
            {
                if (!Util.IsValidTxtResource(line)) //determine if line is a valid resource or not
                    continue;

                Match itemEntry = ParamEntryRx.Match(line);
                var name = itemEntry.Groups["name"].Value;//.Replace("\r", "");
                var id = Convert.ToInt32(itemEntry.Groups["id"].Value);
                if (NameDictionary.ContainsKey(id))
                    continue;

                NameDictionary.Add(id, name);
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
        public void RestoreParam()
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

        private void BuildCells()
        {
            Fields = new List<Field>();
            int totalSize = 0;
            for (int i = 0; i < ParamDef.Fields.Count; i++)
            {
                PARAMDEF.Field field = ParamDef.Fields[i];
                DefType type = field.DisplayType;
                int size = ParamUtil.IsArrayType(type) ? ParamUtil.GetValueSize(type) * field.ArrayLength : ParamUtil.GetValueSize(type);
                if (ParamUtil.IsArrayType(type))
                    totalSize += ParamUtil.GetValueSize(type) * field.ArrayLength;
                else
                    totalSize += ParamUtil.GetValueSize(type);

                if (ParamUtil.IsBitType(type) && field.BitSize != -1)
                {
                    int bitOffset = field.BitSize;
                    DefType bitType = type == DefType.dummy8 ? DefType.u8 : type;
                    int bitLimit = ParamUtil.GetBitLimit(bitType);
                    Fields.Add(new BitField(field, totalSize - size, bitOffset - 1));

                    for (; i < ParamDef.Fields.Count - 1; i++)
                    {
                        PARAMDEF.Field nextField = ParamDef.Fields[i + 1];
                        DefType nextType = nextField.DisplayType;
                        if (!ParamUtil.IsBitType(nextType) || nextField.BitSize == -1 || bitOffset + nextField.BitSize > bitLimit
                            || (nextType == DefType.dummy8 ? DefType.u8 : nextType) != bitType)
                            break;
                        bitOffset += nextField.BitSize;
                        Fields.Add(new BitField(nextField, totalSize - size, bitOffset - 1));
                    }
                    continue;
                }


                switch (field.DisplayType)
                {
                    case DefType.s8:
                    case DefType.s16:
                    case DefType.s32:
                        Fields.Add(new NumericField(field, totalSize - size, true));
                        break;
                    case DefType.u8:
                    case DefType.dummy8:
                    case DefType.u16:
                    case DefType.u32:
                        Fields.Add(new NumericField(field, totalSize - size, false));
                        break;
                    case DefType.f32:
                        Fields.Add(new FloatField(field, totalSize - size));
                        break;
                    case DefType.fixstr:
                        Fields.Add(new FixedStr(field, totalSize - size, Encoding.Unicode));
                        break;
                    case DefType.fixstrW:
                        Fields.Add(new FixedStr(field, totalSize - size, Encoding.Unicode));
                        break;
                    default:
                        throw new Exception($"Unknown type: {field.DisplayType}");
                }
            }
        }

        public abstract class Field
        {
            private PARAMDEF.Field _paramdefField;
            public DefType Type => _paramdefField.DisplayType;
            public string InternalName => _paramdefField.InternalName;
            public string DisplayName => _paramdefField.DisplayName;
            public string Description => _paramdefField.Description;
            public int ArrayLength => _paramdefField.ArrayLength;
            public int FieldOffset { get; }

            public Field(PARAMDEF.Field field, int fieldOffset)
            {
                _paramdefField = field;
                FieldOffset = fieldOffset;
            }

            public override string ToString()
            {
                return InternalName;
            }
        }

        public class FixedStr : Field
        {
            public Encoding Encoding;
            public FixedStr(PARAMDEF.Field field, int fieldOffset, Encoding encoding) : base(field, fieldOffset)
            {
                Encoding = encoding;
            }
        }

        public class NumericField : Field
        {
            public bool IsSigned;
            public NumericField(PARAMDEF.Field field, int fieldOffset, bool isSigned) : base(field, fieldOffset)
            {
                IsSigned = isSigned;
            }
        }

        public class BitField : Field
        {
            public int BitPosition;
            public BitField(PARAMDEF.Field field, int fieldOffset, int bitPosition) : base(field, fieldOffset)
            {
                BitPosition = bitPosition;
            }
        }

        public class FloatField : Field
        {
            public FloatField(PARAMDEF.Field field, int fieldOffset) : base(field, fieldOffset)
            {
            }
        }
    }
}

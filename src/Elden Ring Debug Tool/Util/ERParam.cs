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
        public Row SelectedRow { get; set; }
        public List<UserControl> Cells { get; set; }
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

       
    }
}

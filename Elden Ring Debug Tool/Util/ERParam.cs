using PropertyHook;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool
{
    public class ERParam
    {
        public PHPointer Pointer { get; private set; }
        public PARAMDEF ParamDef { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }
        public int Length { get; private set; }
        public byte[] Bytes { get; private set; }
        public Dictionary<int, int> OffsetDictionary { get; private set; }
        private static Regex ParamEntryRx = new Regex(@"^\s*(?<id>\S+)\s+(?<name>.*)$");
        public Dictionary<int, string> RowDictionary { get; private set; }
        public int RowLength { get; private set; }

        public ERParam(PHPointer pointer, PARAMDEF paramdef, string name)
        {
            Pointer = pointer;
            ParamDef = paramdef;
            Name = name;
            Type = paramdef.ParamType;
            BuildOffsetDictionary();
            BuildRowDictionary();
            RowLength = ParamDef.GetRowSize();
        }
        private void BuildOffsetDictionary()
        {
            OffsetDictionary = new Dictionary<int, int>();
            Length = Pointer.ReadInt32((int)EROffsets.Param.NameOffset);
            
            var paramType = Pointer.ReadString(Length, Encoding.UTF8, (uint)Type.Length);
            if (paramType != Type)
                throw new InvalidOperationException($"Incorrect Param Pointer: {Type}");

            Bytes = Pointer.ReadBytes(0x0, (uint)Length);

            var tableLength = BitConverter.ToInt32(Bytes ,(int)EROffsets.Param.TableLength);
            var param = 0x40;
            var paramID = 0x0;
            var paramOffset = 0x8;
            var nextParam = 0x18;

            while (param < tableLength)
            {
                var itemID = BitConverter.ToInt32(Bytes, param + paramID);
                var itemParamOffset = BitConverter.ToInt32(Bytes, param + paramOffset);
                if (OffsetDictionary.ContainsKey(itemID))
                {
                    Debug.WriteLine(itemID);
                }
                OffsetDictionary.Add(itemID, itemParamOffset);

                param += nextParam;
            }

        }
        private void BuildRowDictionary()
        {
            RowDictionary = new Dictionary<int, string>();
            string result = Util.GetTxtResource(@$"Resources/Names/{Name}.txt");
            if (string.IsNullOrWhiteSpace(result))
                return;

            foreach (string line in result.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!line.Contains("//")) //determine if line is a valid resource or not
                {
                    Match itemEntry = ParamEntryRx.Match(line);
                    var name = itemEntry.Groups["name"].Value.Replace("\r", "");
                    var id = Convert.ToInt32(itemEntry.Groups["id"].Value);
                    if (RowDictionary.ContainsKey(id))
                    {
                        Debug.WriteLine(name);
                        Debug.WriteLine(id);
                    }
                    RowDictionary.Add(id, name);
                }
            };
        }
    }
}

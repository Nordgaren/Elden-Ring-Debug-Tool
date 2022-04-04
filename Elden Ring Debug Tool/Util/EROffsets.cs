using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool
{
    internal class EROffsets
    {
        #region BaseA
        public const string GameDataManSetupAoB = "48 8B 05 ? ? ? ? 48 85 C0 74 05 48 8B 40 58 C3 C3";
        public const string WorldCharManSetupAoB = "48 8B 05 ? ? ? ? 48 85 C0 74 0F 48 39 88 ? ? ? ? 75 06 89 B1 5C 03 00 00 0F 28 05 ? ? ? ? 4C 8D 45 E7";
        public const int BasePtrOffset1 = 0x3;
        public const int BasePtrOffset2 = 0x7;

        public const int PlayerGameDataOffset = 0x8;
        public const int PlayerInventoryOffset = 0x418;
        #endregion
    }
}
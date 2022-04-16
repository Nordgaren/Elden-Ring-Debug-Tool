using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Elden_Ring_Debug_Tool.ERItem;

namespace Elden_Ring_Debug_Tool
{
    class InventoryEntry
    {
        public byte[] Bytes { get; private set; }
        public int GaItemHandle 
        { 
            get 
            { 
                return BitConverter.ToInt32(Bytes, 0);
            } 
        }
        public int ItemID
        {
            get
            {
                return BitConverter.ToInt32(Bytes, 0);
            }
        }
        public Category Category { get; set; }
        public int Quantity { get; set; }
        public int DisplayId { get; set; }

        public InventoryEntry(byte[] bytes)
        {
            Bytes = bytes;
        }
    }
}

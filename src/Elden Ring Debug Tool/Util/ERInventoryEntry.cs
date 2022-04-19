using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Elden_Ring_Debug_Tool.ERItem;

namespace Elden_Ring_Debug_Tool
{
    class ERInventoryEntry
    {
        public byte[] Bytes { get; private set; }
        public string Name { get; private set; }
        public int GaItemHandle 
        { 
            get 
            { 
                return BitConverter.ToInt32(Bytes, (int)EROffsets.InventoryEntry.GaItemHandle);
            } 
        }
        public int ItemID
        {
            get
            {
                var buffer = new byte[4];
                Array.Copy(Bytes, (int)EROffsets.InventoryEntry.ItemID, buffer, 0, buffer.Length);
                buffer[3] &= 0xF;
                return BitConverter.ToInt32(buffer);
            }
        }
        public Category Category
        {
            get
            {
                var cat = Bytes[(int)EROffsets.InventoryEntry.ItemCategory];
                byte mask = 0xF0;
                cat &= mask;
                return (Category)(cat * 0x1000000);
            }
        }
        public int Quantity
        {
            get
            {
                return BitConverter.ToInt32(Bytes, (int)EROffsets.InventoryEntry.Quantity);
            }
        }
        public int DisplayID
        {
            get
            {
                return BitConverter.ToInt32(Bytes, (int)EROffsets.InventoryEntry.DispalyID);
            }
        }

        public ERInventoryEntry(byte[] bytes, ERHook hook)
        {
            Bytes = bytes;
            Name = "Unknown";
            switch (Category)
            {
                case Category.Weapons:
                    var id = Util.DeleteFromEnd(ItemID, 2) * 100;
                    var upgradeLevel = ItemID - id;
                    var levelString = upgradeLevel > 0 ? $"+{upgradeLevel.ToString() }" : "";
                    if (hook.EquipParamWeapon.NameDictionary.ContainsKey(id))
                        Name = $"{hook.EquipParamWeapon.NameDictionary[id]}{levelString}";
                    break;
                case Category.Protector:
                    if (hook.EquipParamProtector.NameDictionary.ContainsKey(ItemID))
                        Name = hook.EquipParamProtector.NameDictionary[ItemID];
                    break;
                case Category.Accessory:
                    if (hook.EquipParamAccessory.NameDictionary.ContainsKey(ItemID))
                        Name = hook.EquipParamAccessory.NameDictionary[ItemID];
                    break;
                case Category.Goods:
                    if (hook.EquipParamGoods.NameDictionary.ContainsKey(ItemID))
                        Name = hook.EquipParamGoods.NameDictionary[ItemID];
                    break;
                case Category.Gem:
                    if (hook.EquipParamGem.NameDictionary.ContainsKey(ItemID))
                        Name = hook.EquipParamGem.NameDictionary[ItemID];
                    break;
                default:
                    break;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool
{
    class ERItem
    {
        public enum Category : uint
        {
            Weapons = 0x00000000,
            Protector = 0x10000000,
            Accessory = 0x20000000,
            Goods = 0x40000000,
            Gem = 0x80000000
        }
        private static Regex ItemEntryRx = new Regex(@"^\s*(?<id>\S+)\s+(?<name>.*)$");

        public string Name;
        public int ID;
        public Category ItemCategory;

        public short MaxQuantity;
        public bool IsDrop;
        public bool IsMultiplayerShare;
        public bool CanAquireFromOtherPlayers => IsDrop && IsMultiplayerShare;

        public ERItem(string config, Category category)
        {
            Match itemEntry = ItemEntryRx.Match(config);
            Name = itemEntry.Groups["name"].Value.Replace("\r", "");
            ID = Convert.ToInt32(itemEntry.Groups["id"].Value);
            ItemCategory = category;
            MaxQuantity = 1;
        }

        public override string ToString()
        {
            return Name;
        }

        public virtual void SetupItem(ERParam param)
        {
            if (!param.OffsetDict.ContainsKey(ID))
                return;

            byte bitfield;
            switch (ItemCategory)
            {
                case Category.Protector:
                    bitfield = param.Bytes[param.OffsetDict[ID] + (int)EROffsets.EquipParamProtector.IsDiscard];
                    IsDrop = (bitfield & (1 << 1)) != 0;
                    IsMultiplayerShare = (bitfield & (1 << 2)) == 0;
                    break;
                case Category.Accessory:
                    bitfield = param.Bytes[param.OffsetDict[ID] + (int)EROffsets.EquipParamAccessory.IsDeposit];
                    IsMultiplayerShare = (bitfield & (1 << 2)) == 0;
                    IsDrop = (bitfield & (1 << 4)) != 0;
                    break;
                case Category.Goods:
                    MaxQuantity = BitConverter.ToInt16(param.Bytes, param.OffsetDict[ID] + (int)EROffsets.EquipParamGoods.MaxNum);

                    bitfield = param.Bytes[param.OffsetDict[ID] + (int)EROffsets.EquipParamGoods.IsFullSuppleItem];
                    IsMultiplayerShare = (bitfield & (1 << 3)) == 0;

                    bitfield = param.Bytes[param.OffsetDict[ID] + (int)EROffsets.EquipParamGoods.IsDrop];
                    IsDrop = (bitfield & (1 << 0)) != 0;
                    break;
                case Category.Gem:
                case Category.Weapons:
                    break;
                default:
                    throw new Exception("Item Does not have a proper category.");
            }
        }

    }
}

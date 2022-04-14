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
            Weapons = 0x80800000,
            Protector = 0x90800000,
            Accessory = 0xA0000000,
            Goods = 0xB0000000,
            Gem = 0xB0000001
        }
        private static Regex ItemEntryRx = new Regex(@"^\s*(?<id>\S+)\s+(?<name>.*)$");

        public string Name;
        public int ID;
        public Category ItemCategory;

        public ERItem(string config, Category category)
        {
            Match itemEntry = ItemEntryRx.Match(config);
            Name = itemEntry.Groups["name"].Value.Replace("\r", "");
            ID = Convert.ToInt32(itemEntry.Groups["id"].Value);
            ItemCategory = (Category)category;
        }

        public override string ToString()
        {
            return Name;
        }

        public virtual void SetupItem(ERParam param)
        {

        }

    }
}

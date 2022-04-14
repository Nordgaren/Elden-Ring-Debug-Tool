using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Category = Elden_Ring_Debug_Tool.ERItem.Category;

namespace Elden_Ring_Debug_Tool
{
    class ERItemCategory
    {

        public string Name;
        public List<ERItem> Items;
        public static List<ERItemCategory> All = new List<ERItemCategory>();

        private static Regex CategoryEntryRx = new Regex(@"^(?<category>\S+) (?<show>\S+) (?<path>\S+) (?<name>.+)$");

        private ERItemCategory(string name, ulong category, string itemList, bool showIDs)
        {
            Name = name;
            Items = new List<ERItem>();
            foreach (string line in itemList.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!line.Contains("/")) //determine if line is a valid resource or not
                    AddItem(line, (Category)category);
            };
        }

        private void AddItem(string line, Category category)
        {
            switch (category)
            {
                case Category.Weapons:
                    Items.Add(new ERWeapon(line, category));
                    break;
                case Category.Protector:
                    break;
                case Category.Accessory:
                    break;
                case Category.Goods:
                    break;
                case Category.Gem:
                    Items.Add(new ERGem(line, category));
                    break;
                default:
                    break;
            }
        }

        public static void GetItemCategories()
        {
            string result = Util.GetTxtResource("Resources/ERItemCategories.txt");
            All = new List<ERItemCategory>();

            foreach (string line in result.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!line.Contains("//")) //determine if line is a valid resource or not
                {
                    Match itemEntry = CategoryEntryRx.Match(line);
                    var name = itemEntry.Groups["name"].Value;
                    var show = Convert.ToBoolean(itemEntry.Groups["show"].Value);
                    var cat = itemEntry.Groups["category"].Value;
                    var category = Convert.ToUInt32(cat, 16);
                    var path = itemEntry.Groups["path"].Value;
                    All.Add(new ERItemCategory(name , category ,Util.GetTxtResource($"Resources/{path}"), show));
                }
            };
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

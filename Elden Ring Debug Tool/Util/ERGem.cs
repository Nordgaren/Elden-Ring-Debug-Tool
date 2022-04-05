using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Infusion = Elden_Ring_Debug_Tool.ERWeapon.Infusion;
using WeaponType = Elden_Ring_Debug_Tool.ERWeapon.WeaponType;

namespace Elden_Ring_Debug_Tool
{
    class ERGem
    {
        public static List<ERGem> Gems;
        private static Regex gemEntryRx = new Regex(@"^\s*(?<id>\S+)\s+(?<name>.*)$");

        public string Name;
        public int ID;
        public List<Infusion> Infusions = new List<Infusion>();
        public List<WeaponType> WeaponTypes = new List<WeaponType>();

        public ERGem(string config)
        {
            Match itemEntry = gemEntryRx.Match(config);
            Name = itemEntry.Groups["name"].Value.Replace("\r", "");
            ID = Convert.ToInt32(itemEntry.Groups["id"].Value);
        }

        public static void GetGems()
        {
            string result = Util.GetResource("Weapons.Gems.txt");
            Gems = new List<ERGem>();

            foreach (string line in result.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!line.Contains("//")) //determine if line is a valid resource or not
                {
                    Gems.Add(new ERGem(line));
                }
            };
        }
    }
}

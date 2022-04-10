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
        private static Regex ItemEntryRx = new Regex(@"^\s*(?<id>\S+)\s+(?<name>.*)$");

        public string Name;
        public int ID;

        public ERItem(string config)
        {
            Match itemEntry = ItemEntryRx.Match(config);
            Name = itemEntry.Groups["name"].Value.Replace("\r", "");
            ID = Convert.ToInt32(itemEntry.Groups["id"].Value);
        }

    }
}

using Erd_Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class ERInventoryEntryViewModel
    {
        private ERInventoryEntry _inventoryEntry;
        public string Name => _inventoryEntry.Name;
        public string GaItemHandle => $"0x{_inventoryEntry.GaItemHandle:X2}";
        public int ItemID => _inventoryEntry.ItemID;
        public string Category => _inventoryEntry?.Category.ToString() ?? "Unknown Category";
        public int Quantity => _inventoryEntry.Quantity;
        public int DisplayID => _inventoryEntry.DisplayID;
        public ERInventoryEntryViewModel(ERInventoryEntry inventoryEntry)
        {
            _inventoryEntry = inventoryEntry;
        }
    }
}

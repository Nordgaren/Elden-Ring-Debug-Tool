using Erd_Tools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class InventoryEntryViewModel
    {
        private InventoryEntry _inventoryEntry;
        public string Name => _inventoryEntry.Name;
        public string GaItemHandle => $"0x{_inventoryEntry.GaItemHandle:X2}";
        public int ItemID 
        {
            get => _inventoryEntry.ItemID;

            set => _inventoryEntry.ItemID = value;
        }
        public string Category => _inventoryEntry?.Category.ToString() ?? "Unknown Category";
        public int Quantity
        {
            get => _inventoryEntry.Quantity;

            set => _inventoryEntry.Quantity = value;
        }

        public int DisplayID => _inventoryEntry.DisplayID;
        public uint Index => _inventoryEntry.Index;
        public InventoryEntryViewModel(InventoryEntry inventoryEntry)
        {
            _inventoryEntry = inventoryEntry;
        }
    }
}

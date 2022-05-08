using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using Erd_Tools;
using Erd_Tools.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Erd_Tools.Hook;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    [Description("Inventory View")]
    public class InventoryViewViewModel : ViewModelBase
    {
        internal ErdHook Hook { get; set; }

        private ObservableCollection<InventoryEntryViewModel> _inventoryEntryViewModels;
        public ObservableCollection<InventoryEntryViewModel> PlayerInventory
        {
            get => _inventoryEntryViewModels;
            set
            {
                if (SetField(ref _inventoryEntryViewModels, value))
                {
                    OnPropertyChanged(nameof(InventoryCollectionView));
                    InventoryCollectionView.Filter += FilterInventory;
                }
            }
        }

        private string _inventoryFilter = string.Empty;
        public string InventoryFilter
        {
            get => _inventoryFilter;
            set
            {
                if (SetField(ref _inventoryFilter, value))
                {
                    InventoryCollectionView.Refresh();
                }
            }
        }

        private bool FilterInventory(object obj)
        {
            if (obj is InventoryEntryViewModel item)
            {
                return item.Name.Contains(InventoryFilter, StringComparison.InvariantCultureIgnoreCase) 
                    || item.Quantity.ToString().Contains(InventoryFilter, StringComparison.InvariantCultureIgnoreCase)
                    || item.ItemID.ToString().Contains(InventoryFilter, StringComparison.InvariantCultureIgnoreCase)
                    || item.Category.ToString().Contains(InventoryFilter, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        public ICollectionView InventoryCollectionView => CollectionViewSource.GetDefaultView(PlayerInventory);

        public InventoryViewViewModel()
        {
            PlayerInventory = new ObservableCollection<InventoryEntryViewModel>();
        }

        public void InitViewModel(ErdHook hook)
        {
            Hook = hook;
        }

        private int _lastInvetoryCount { get; set; }
        public void UpdateViewModel()
        {
            if (_lastInvetoryCount != Hook.InventoryEntries)
                GetInventory();
        }

        private void GetInventory()
        {
            _lastInvetoryCount = Hook.InventoryEntries;
            IEnumerable inventory = Hook.GetInventory();
            List<InventoryEntryViewModel> items = new List<InventoryEntryViewModel>();

            foreach (InventoryEntry entry in inventory)
            {
                items.Add(new InventoryEntryViewModel(entry));
            }

            PlayerInventory = new ObservableCollection<InventoryEntryViewModel>(items);
            InventoryCount = PlayerInventory.Count;
        }

        private int _inventoryCount;
        public int InventoryCount
        {
            get => _inventoryCount;
            set => SetField(ref _inventoryCount, value);
        }
    }
}

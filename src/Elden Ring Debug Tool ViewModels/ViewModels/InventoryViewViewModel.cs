using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using Erd_Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    [Description("Inventory View")]
    public class InventoryViewViewModel : ViewModelBase
    {
        internal ERHook Hook { get; set; }

        private ObservableCollection<ERInventoryEntryViewModel> _InventoryEntryViewModels;
        public ObservableCollection<ERInventoryEntryViewModel> PlayerInventory
        {
            get => _InventoryEntryViewModels;
            set
            {
                if (SetField(ref _InventoryEntryViewModels, value))
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
            if (obj is ERInventoryEntryViewModel item)
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
            PlayerInventory = new ObservableCollection<ERInventoryEntryViewModel>();
        }

        public void InitViewModel(ERHook hook)
        {
            Hook = hook;
        }

        public void UpdateViewModel()
        {
            if (PlayerInventory.Count != Hook.LastInventoryCount)
            {
                GetInventory();
            }
        }

        private void GetInventory()
        {
            IEnumerable inventory = Hook.GetInventory();
            List<ERInventoryEntryViewModel> items = new List<ERInventoryEntryViewModel>();

            foreach (ERInventoryEntry entry in inventory)
            {
                items.Add(new ERInventoryEntryViewModel(entry));
            }

            PlayerInventory = new ObservableCollection<ERInventoryEntryViewModel>(items);
        }

        private int _inventoryCount;
        public int InventoryCount
        {
            get => _inventoryCount;
            set => SetField(ref _inventoryCount, value);
        }
    }
}

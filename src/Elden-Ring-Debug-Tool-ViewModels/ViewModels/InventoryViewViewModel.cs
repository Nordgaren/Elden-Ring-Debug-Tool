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

        public SettingsViewViewModel SettingsViewViewModel { get; set; }

        public InventoryViewViewModel()
        {
            PlayerInventory = new ObservableCollection<InventoryEntryViewModel>();
        }

        public void InitViewModel(ErdHook hook, SettingsViewViewModel settingsViewViewModel)
        {
            Hook = hook;
            SettingsViewViewModel = settingsViewViewModel;
            //SettingsViewViewModel.PropertyChanged += SettingsViewViewModel_PropertyChanged;
        }


        private int _lastInvetoryCount { get; set; }
        public void UpdateViewModel()
        {
            Reading = true;
            if (_lastInvetoryCount != Hook.InventoryEntries)
                GetInventory();

            HeldNormalItems = Hook.HeldNormalItems;
            MaxNormalItems = Hook.MaxNormalItems;
            HeldSpecialItems = Hook.HeldSpecialItems;
            MaxSpecialItems = Hook.MaxSpecialItems;
            Reading = false;
        }

        public bool Reading { get; set; } = false;

        private void GetInventory()
        {
            _lastInvetoryCount = Hook.InventoryEntries;
            IEnumerable inventory = Hook.GetInventory();
            List<InventoryEntryViewModel> items = new();

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

        public int InventoryMax => MaxNormalItems + MaxSpecialItems;


        private int _heldNormalItems;

        public int HeldNormalItems
        {
            get => _heldNormalItems;
            set
            {
                if (SetField(ref _heldNormalItems, value) && !Reading)
                {
                    Hook.HeldNormalItems = HeldNormalItems;
                }
            }
        }

        private int _maxNormalItems;

        public int MaxNormalItems
        {
            get => _maxNormalItems;
            set
            {
                if (SetField(ref _maxNormalItems, value))
                {
                    OnPropertyChanged(nameof(InventoryMax));
                }
            }
        }

        private int _maxSpecialItems;

        public int MaxSpecialItems
        {
            get => _maxSpecialItems;
            set
            {
                if (SetField(ref _maxSpecialItems, value))
                {
                    OnPropertyChanged(nameof(InventoryMax));
                }
            }
        }

        private int _heldSpecialItems;

        public int HeldSpecialItems
        {
            get => _heldSpecialItems;
            set
            {
                if (SetField(ref _heldSpecialItems, value) && !Reading)
                {
                    Hook.HeldSpecialItems = HeldSpecialItems;
                }
            }
        }
    }
}

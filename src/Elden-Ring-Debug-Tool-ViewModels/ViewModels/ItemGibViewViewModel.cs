using Elden_Ring_Debug_Tool_ViewModels.Commands;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using Erd_Tools;
using Erd_Tools.Models;
using Erd_Tools.Models;
using PropertyHook;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using static Erd_Tools.Models.Weapon;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    [Description("Item Gib View")]
    public class ItemGibViewViewModel : ViewModelBase
    {
        internal ErdHook Hook { get; set; }
        private ObservableCollection<ItemCategoryViewModel> _categories { get; }
        private ObservableCollection<GemViewModel> _gems { get; }

        public ICollectionView CategoryCollectionView { get; }
        public ICollectionView ItemsCollectionView => CollectionViewSource.GetDefaultView(SearchAll ? ItemViewModel.All : SelectedItemCategory?.Items);
        public ICollectionView InfusionCollectionView { get; }
        public ICollectionView GemCollectionView { get; }

        public ICommand GibItemCommand { get; set; }

        public SettingsViewViewModel SettingsViewViewModel;

        public ItemGibViewViewModel()
        {
            _categories = new ObservableCollection<ItemCategoryViewModel>(new List<ItemCategoryViewModel>());
            CategoryCollectionView = CollectionViewSource.GetDefaultView(_categories);

            InfusionCollectionView = CollectionViewSource.GetDefaultView(new ObservableCollection<Infusion>(Gem.AllInfusions));
            InfusionCollectionView.Filter += FilterInfusions;

            _gems = new ObservableCollection<GemViewModel>(new List<GemViewModel>());
            GemCollectionView = CollectionViewSource.GetDefaultView(_gems);
            GemCollectionView.Filter += FiltGems;

      
        }

        private bool FilterInfusions(object obj)
        {
            if (obj is Infusion infusion && SelectedGem != null)
            {
                return SelectedGem.Infusions.Contains(infusion);
            }

            return false;
        }

        private bool FiltGems(object obj)
        {
            if (obj is GemViewModel gem && SelectedItem is WeaponViewModel weapon && !weapon.Unique)
            {
                return gem.WeaponTypes.Contains(weapon.Type);
            }

            return false;
        }

        public void InitViewModel(ErdHook hook, SettingsViewViewModel settingsViewViewModel)
        {
            SettingsViewViewModel = settingsViewViewModel;

            GibItemCommand = new GibItemCommand(this);
            Commands.Add(GibItemCommand);

            Hook = hook;
            Hook.OnSetup += Hook_OnSetup;
            Hook.OnUnhooked += Hook_OnUnhooked;
            foreach (ItemCategory itemCategory in ItemCategory.All)
            {
                ItemCategoryViewModel erICVM = new ItemCategoryViewModel(itemCategory);
                _categories.Add(erICVM);

                if (erICVM.Category == Item.Category.Gem)
                {
                    foreach (GemViewModel gem in erICVM.Items)
                    {
                        _gems.Add(gem);
                    }
                }
            }

            if (_categories.Count > 0)
                SelectedItemCategory = _categories[0];

        }

        private void Hook_OnUnhooked(object? sender, PHEventArgs e)
        {
            Setup = false;
        }

        private void Hook_OnSetup(object? sender, PHEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (ItemViewModel item in ItemViewModel.All)
                {
                    item.SetupItem();
                }
            });
        }

        public void UpdateViewModel()
        {
            Setup = Hook.Setup;
            Loaded = Hook.Loaded;
        }

        private bool _setup;
        public bool Setup
        {
            get => _setup;
            set => SetField(ref _setup, value);
        }

        private bool _loaded;
        public bool Loaded
        {
            get => _loaded;
            set => SetField(ref _loaded, value);
        }

        private ItemCategoryViewModel _selectedItemCategory;
        public ItemCategoryViewModel SelectedItemCategory
        {
            get => _selectedItemCategory;
            set 
            {
                if (SetField(ref _selectedItemCategory, value))
                {
                    OnPropertyChanged(nameof(ItemsCollectionView));
                    ItemsCollectionView.Filter += FilerItems;
                }
            }
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set => SetField(ref _quantity, value);
        }

        private int _upgradeLevel;
        public int UpgradeLevel
        {
            get => _upgradeLevel;
            set => SetField(ref _upgradeLevel, value);
        }

        private ItemViewModel? _selectedItem;
        public ItemViewModel? SelectedItem
        {
            get => _selectedItem;
            set { 
                if (SetField(ref _selectedItem, value))
                {
                    SelectedWeapon = SelectedItem as WeaponViewModel;
                    GemCollectionView.Refresh();
                    if (!GemCollectionView.IsEmpty && SelectedItem is WeaponViewModel weapon)
                    {
                        SelectedGem = _gems.FirstOrDefault(x => x.SwordArtID == weapon.SwordArtId);
                    }
                    MaxQuantity = SelectedItem?.MaxQuantity ?? 1;
                    Quantity = Max ? MaxQuantity : 1;
                }
            }
        }
        private bool _max = true;
        public bool Max
        {
            get => _max;
            set
            {
                if (SetField(ref _max, value))
                {
                    UpgradeLevel = Max ? MaxUpgrade : 0;
                    Quantity = Max ? MaxQuantity : 1;
                }
            }
        }

        private bool _restrict = true;
        public bool Restrict
        {
            get => _restrict;
            set
            {
                if (SetField(ref _restrict, value))
                {
                    MaxQuantity = SelectedItem?.MaxQuantity ?? 1;
                }
            }
        }

        private int _maxUpgrade;
        public int MaxUpgrade
        {
            get => _maxUpgrade;
            set => SetField(ref _maxUpgrade, value);
        }

        private int _maxQuantity;
        public int MaxQuantity
        {
            get => _maxQuantity;
            set => SetField(ref _maxQuantity, Restrict ? value : int.MaxValue);
        }

        private WeaponViewModel? _selectedWeapon;
        public WeaponViewModel? SelectedWeapon
        {
            get => _selectedWeapon;
            set
            {
                if (SetField(ref _selectedWeapon, value))
                {
                    MaxUpgrade = SelectedWeapon?.MaxUpgrade ?? 0;
                    UpgradeLevel = Max ? SelectedWeapon?.MaxUpgrade ?? 0 : 0;
                }
            }
        }

        private Infusion? _selectedInfusion;
        public Infusion? SelectedInfusion
        {
            get => _selectedInfusion;
            set => SetField(ref _selectedInfusion, value);
        }

        private GemViewModel? _selectedGem;
        public GemViewModel? SelectedGem
        {
            get => _selectedGem;
            set
            {
                if (SetField(ref _selectedGem, value))
                {
                    InfusionCollectionView.Refresh();
                    if (!InfusionCollectionView.IsEmpty)
                        SelectedInfusion = Infusion.Standard;
                }
            }
        }

        #region Search

        private bool _searchAll;
        public bool SearchAll
        {
            get => _searchAll;
            set
            {
                SetField(ref _searchAll, value);
                OnPropertyChanged(nameof(ItemsCollectionView));
            }
        }

        private string _itemFilter = string.Empty;
        public string ItemFilter
        {
            get => _itemFilter;
            set
            {
                SetField(ref _itemFilter, value);
                ItemsCollectionView.Refresh();
            }
        }

        private bool FilerItems(object obj)
        {
            if (obj is ItemViewModel item)
            {
                return item.Name.Contains(ItemFilter, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        } 
        #endregion
    }
}

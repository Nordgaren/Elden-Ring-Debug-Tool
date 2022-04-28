using Elden_Ring_Debug_Tool_ViewModels.Commands;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using Erd_Tools;
using PropertyHook;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using static Erd_Tools.ERWeapon;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    public class ItemGibViewModel : ViewModelBase
    {
        internal ERHook Hook { get; set; }

        private readonly ObservableCollection<ERItemCategoryViewModel> _categories;
        private readonly ObservableCollection<ERGemViewModel> _gems;

        public ICollectionView CategoryCollectionView { get; }
        public ICollectionView ItemsCollectionView => CollectionViewSource.GetDefaultView(SearchAll ? ERItemViewModel.All : SelectedItemCategory?.Items);
        public ICollectionView InfusionCollectionView { get; }
        public ICollectionView GemCollectionView { get; }

        public ICommand GibItemCommand { get; set; }

        public ItemGibViewModel()
        {
            _categories = new ObservableCollection<ERItemCategoryViewModel>(new List<ERItemCategoryViewModel>());
            CategoryCollectionView = CollectionViewSource.GetDefaultView(_categories);

            InfusionCollectionView = CollectionViewSource.GetDefaultView(new ObservableCollection<Infusion>(ERGem.AllInfusions));
            InfusionCollectionView.Filter += FilterInfusions;

            _gems = new ObservableCollection<ERGemViewModel>(new List<ERGemViewModel>());
            GemCollectionView = CollectionViewSource.GetDefaultView(_gems);
            GemCollectionView.Filter += FilterGems;

            GibItemCommand = new GibItemCommand(this);
        }

        private bool FilterInfusions(object obj)
        {
            if (obj is Infusion infusion && SelectedGem != null)
            {
                return SelectedGem.Infusions.Contains(infusion);
            }

            return false;
        }

        private bool FilterGems(object obj)
        {
            if (obj is ERGemViewModel gem && SelectedItem is ERWeaponViewModel weapon && !weapon.Unique)
            {
                return gem.WeaponTypes.Contains(weapon.Type);
            }

            return false;
        }

        public void InitViewModel(ERHook hook)
        {
            Hook = hook;
            Hook.OnSetup += Hook_OnSetup;
            Hook.OnUnhooked += Hook_OnUnhooked;
            foreach (ERItemCategory itemCategory in ERItemCategory.All)
            {
                ERItemCategoryViewModel erICVM = new ERItemCategoryViewModel(itemCategory);
                _categories.Add(erICVM);

                if (erICVM.Category == ERItem.Category.Gem)
                {
                    foreach (ERGemViewModel gem in erICVM.Items)
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
                foreach (ERItemViewModel item in ERItemViewModel.All)
                {
                    item.SetupItem();
                }
                Setup = true;
            });
        }

        private bool _setup;
        public bool Setup
        {
            get => _setup;
            set => SetField(ref _setup, value);
        }

        private ERItemCategoryViewModel _selectedItemCategory;
        public ERItemCategoryViewModel SelectedItemCategory
        {
            get => _selectedItemCategory;
            set 
            {
                if (SetField(ref _selectedItemCategory, value))
                {
                    OnPropertyChanged(nameof(ItemsCollectionView));
                    ItemsCollectionView.Filter += FilterItems;
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

        private ERItemViewModel? _selectedItem;
        public ERItemViewModel? SelectedItem
        {
            get => _selectedItem;
            set { 
                if (SetField(ref _selectedItem, value))
                {
                    SelectedWeapon = SelectedItem as ERWeaponViewModel;
                    GemCollectionView.Refresh();
                    if (!GemCollectionView.IsEmpty)
                    {
                        SelectedGem = _gems.FirstOrDefault(x => x.SwordArtID == ((ERWeaponViewModel)SelectedItem).SwordArtId) ?? null;
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

        private ERWeaponViewModel? _selectedWeapon;
        public ERWeaponViewModel? SelectedWeapon
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

        private ERGemViewModel? _selectedGem;
        public ERGemViewModel? SelectedGem
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

        private bool FilterItems(object obj)
        {
            if (obj is ERItemViewModel item)
            {
                return item.Name.Contains(ItemFilter, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        } 
        #endregion
    }
}

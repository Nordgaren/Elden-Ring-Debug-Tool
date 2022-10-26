using System.Collections;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using Erd_Tools;
using System.ComponentModel;
using System.Windows.Controls;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using Erd_Tools.Models.Items;
using static Erd_Tools.Models.Weapon;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    [Description("Gib Item")]
    public class GibItemCommand : AsyncCommandBase
    {
        private ItemGibViewViewModel _itemGibViewModel { get; }
        private ErdHook _hook => _itemGibViewModel.Hook;

        public GibItemCommand(ItemGibViewViewModel itemGibViewModel, CancellationTokenSource cts)
        {
            _itemGibViewModel = itemGibViewModel;
            _itemGibViewModel.PropertyChanged += ViewModel_PropertyChanged;
            _itemGibViewModel.SettingsViewViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {

            if (_itemGibViewModel.SelectedItem == null)
                return false;

            return (_hook?.Setup ?? false)
                && _hook.Loaded
                && (_itemGibViewModel.SelectedItem.CanAquireFromOtherPlayers || _itemGibViewModel.SettingsViewViewModel.SpawnUntradeable)
                && base.CanExecute(parameter);
        }

        public override async Task ExecuteAsync(object? parameter)
        {
            if (_itemGibViewModel.SelectedItem == null)
                throw new NullReferenceException("Selected Item cannot be null when trying to spawn items.");

            if (!(parameter is IList iList))
                return;

            _itemGibViewModel._cts = new CancellationTokenSource();

            if (iList.Count > 1)
            {
                _itemGibViewModel.CancelVisibility = true;
                try
                {
                    await SpawnMultiItem(iList);
                }
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    _itemGibViewModel.CancelVisibility = false;
                }
                return;
            }

            SpawnSingleItem(iList[0] as ItemViewModel);
            _itemGibViewModel.CancelVisibility = false;
        }

        private void SpawnSingleItem(ItemViewModel? itemViewModel)
        {
            if (itemViewModel == null)
                throw new ArgumentNullException(nameof(itemViewModel));

            int quantity = _itemGibViewModel.Quantity;
            int infusion = _itemGibViewModel.SelectedInfusion.HasValue ? (int)_itemGibViewModel.SelectedInfusion : 0;
            int upgrade = _itemGibViewModel.UpgradeLevel;

            int gem = _itemGibViewModel.SelectedGem?.ID ?? -1;

            _hook.GetItem(new ItemSpawnInfo(itemViewModel.ID, itemViewModel.ItemCategory, quantity, itemViewModel.MaxQuantity, infusion, upgrade, gem, itemViewModel.EventID));


        }

        private async Task SpawnMultiItem(IEnumerable iList)
        {
            bool max = _itemGibViewModel.Max;
            int upgradeLevel = _itemGibViewModel.UpgradeLevel;
            int quantityValue = _itemGibViewModel.MaxQuantity;
            GemViewModel? selectedGem = _itemGibViewModel.SelectedGem;
            Infusion? selectedInfusion = _itemGibViewModel.SelectedInfusion;

            await Task.Run(() =>
            {
                List<ItemSpawnInfo> items = new();
                foreach (ItemViewModel itemViewModel in iList)
                {
                    int quantity = max ? itemViewModel.MaxQuantity : Math.Min(quantityValue, itemViewModel.MaxQuantity);

                    int upgrade = max ? itemViewModel.MaxUpgrade : Math.Min(upgradeLevel, itemViewModel.MaxUpgrade);

                    int gem = -1;

                    int infusion = 0;
                    if (itemViewModel is WeaponViewModel weaponViewModel)
                    {
                        gem = selectedGem?.ID ?? -1;
                        infusion = selectedInfusion != null && selectedGem != null && selectedGem.Infusions.Contains(selectedInfusion.Value) ? (int)selectedInfusion.Value : 0;
                    }

                    items.Add(new ItemSpawnInfo(itemViewModel.ID, itemViewModel.ItemCategory,quantity,itemViewModel.MaxQuantity, infusion, upgrade, gem, itemViewModel.EventID));

                }
                _hook.GetItem(items, _itemGibViewModel._cts.Token);

            });

        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(ItemGibViewViewModel.SelectedItem)
                or nameof(ItemGibViewViewModel.Setup)
                or nameof(ItemGibViewViewModel.Loaded)
                or nameof(SettingsViewViewModel.SpawnUntradeable))
            {
                OnCanExecuteChanged();
            }
        }
    }
}

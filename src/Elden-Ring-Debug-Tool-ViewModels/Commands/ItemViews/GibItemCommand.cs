using System.Collections;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using Erd_Tools;
using System.ComponentModel;
using System.Windows.Controls;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
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

            int? id = itemViewModel.ID;
            id += (int)itemViewModel.ItemCategory;

            int quantity = _itemGibViewModel.Quantity;
            int infusion = _itemGibViewModel.SelectedInfusion.HasValue ? (int)_itemGibViewModel.SelectedInfusion : 0;
            int upgrade = _itemGibViewModel.UpgradeLevel;

            int gem = _itemGibViewModel.SelectedGem?.ID ?? -1;

            _hook.GetItem(id.Value, quantity, infusion, upgrade, gem);

            if (itemViewModel.EventID != -1)
                _hook.SetEventFlag(itemViewModel.EventID, true);
        }

        private async Task SpawnMultiItem(IEnumerable iList)
        {
            bool max = _itemGibViewModel.Max;
            int upgradeLevel = _itemGibViewModel.UpgradeLevel;
            GemViewModel? selectedGem = _itemGibViewModel.SelectedGem;
            Infusion? selectedInfusion = _itemGibViewModel.SelectedInfusion;

            await Task.Run(() =>
            {
                foreach (ItemViewModel itemViewModel in iList)
                {
                    int? id = itemViewModel.ID;
                    id += (int)itemViewModel.ItemCategory;

                    int quantity = max ? itemViewModel.MaxQuantity : _itemGibViewModel.Quantity;
                    int infusion = 0;

                    int upgrade = max ? itemViewModel.MaxUpgrade : Math.Max(upgradeLevel, itemViewModel.MaxUpgrade);

                    int gem = -1;

                    if (itemViewModel is WeaponViewModel weaponViewModel)
                    {
                        gem = selectedGem?.ID ?? -1;
                        infusion = selectedGem != null && selectedGem.Infusions.Contains((Infusion)infusion) ? selectedGem.ID : 0;
                    }

                    _itemGibViewModel._cts.Token.ThrowIfCancellationRequested();

                    _hook.GetItem(id.Value, quantity, infusion, upgrade, gem);

                    if (itemViewModel.EventID != -1)
                        _hook.SetEventFlag(itemViewModel.EventID, true);
                }
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

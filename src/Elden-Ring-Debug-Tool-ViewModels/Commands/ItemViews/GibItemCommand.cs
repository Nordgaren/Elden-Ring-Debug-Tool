using System.Collections;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using Erd_Tools;
using System.ComponentModel;
using System.Windows.Controls;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using Erd_Tools.Models;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    [Description("Gib Item")]
    public class GibItemCommand : CommandBase
    {
        private ItemGibViewViewModel _itemGibViewModel { get; }
        private ErdHook _hook => _itemGibViewModel.Hook;

        public GibItemCommand(ItemGibViewViewModel itemGibViewModel)
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

        public override void Execute(object? parameter)
        {
            if (_itemGibViewModel.SelectedItem == null)
                throw new NullReferenceException("Selected Item cannot be null when trying to spawn items.");

            if (!(parameter is IList iList))
                return;

            foreach (ItemViewModel itemViewModel in iList)
            {
                int? id = itemViewModel.ID;
                id += (int)itemViewModel.ItemCategory;

                int quantity = _itemGibViewModel.Quantity;
                int infusion = _itemGibViewModel.SelectedInfusion.HasValue ? (int)_itemGibViewModel.SelectedInfusion : 0;
                int upgrade = _itemGibViewModel.UpgradeLevel;
                int gem = _itemGibViewModel.SelectedGem?.ID ?? -1;

                _hook.GetItem(id.Value, quantity, infusion, upgrade, gem);

                if (itemViewModel.EventID != null)
                    _hook.SetEventFlag(itemViewModel.EventID);

            }

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

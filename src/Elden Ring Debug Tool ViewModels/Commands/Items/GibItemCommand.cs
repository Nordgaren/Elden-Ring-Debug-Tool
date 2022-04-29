using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using Erd_Tools;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    public class GibItemCommand : CommandBase
    {
        private ItemGibViewModel _itemGibViewModel;
        private ERHook _hook => _itemGibViewModel.Hook;

        public GibItemCommand(ItemGibViewModel itemGibViewModel)
        {
            _itemGibViewModel = itemGibViewModel;
            _itemGibViewModel.PropertyChanged += _itemGibViewModel_PropertyChanged1;
        }

        public override bool CanExecute(object? parameter)
        {
            return (_hook?.Setup ?? false) 
                && _hook.Loaded 
                && ((_itemGibViewModel.SelectedItem?.CanAquireFromOtherPlayers ?? false) || MainWindowViewModel.Settings.SpawnUndroppable)
                && base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            int id = _itemGibViewModel.SelectedItem.ID;
            int quantity = _itemGibViewModel.Quantity;
            int infusion = _itemGibViewModel.SelectedInfusion.HasValue ? (int)_itemGibViewModel.SelectedInfusion : 0;
            int upgrade = _itemGibViewModel.UpgradeLevel;
            int gem = _itemGibViewModel.SelectedGem?.ID ?? -1;

            _hook.GetItem(id, quantity, infusion, upgrade, gem);
        }

        private void _itemGibViewModel_PropertyChanged1(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ItemGibViewModel.SelectedItem) || 
                e.PropertyName == nameof(ItemGibViewModel.Setup)) 
                //e.PropertyName == nameof(ItemGibViewModel.Loaded))
            {
                OnCanExecuteChanged();
            }
        }
    }
}

using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using Erd_Tools;
using System.ComponentModel;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    [Description("Gib Item")]
    public class GibItemCommand : CommandBase
    {
        private ItemGibViewViewModel _itemGibViewModel;
        private ERHook _hook => _itemGibViewModel.Hook;

        public GibItemCommand(ItemGibViewViewModel itemGibViewModel)
        {
            _itemGibViewModel = itemGibViewModel;
            _itemGibViewModel.PropertyChanged += _itemGibViewModel_PropertyChanged1;
        }

        public override bool CanExecute(object? parameter)
        {
            return (_hook?.Setup ?? false) 
                && _hook.Loaded 
                && ((_itemGibViewModel.SelectedItem?.CanAquireFromOtherPlayers ?? false) || (MainWindowViewModel.Settings?.SpawnUndroppable ?? false))
                && base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            int? id = _itemGibViewModel.SelectedItem?.ID;

            if (id == null)
                throw new NullReferenceException("Selected Item cannot be null when trying to spawn items.");

            int quantity = _itemGibViewModel.Quantity;
            int infusion = _itemGibViewModel.SelectedInfusion.HasValue ? (int)_itemGibViewModel.SelectedInfusion : 0;
            int upgrade = _itemGibViewModel.UpgradeLevel;
            int gem = _itemGibViewModel.SelectedGem?.ID ?? -1;

            _hook.GetItem(id.Value, quantity, infusion, upgrade, gem);
        }

        private void _itemGibViewModel_PropertyChanged1(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ItemGibViewViewModel.SelectedItem) 
                || e.PropertyName == nameof(ItemGibViewViewModel.Setup)) 
                //|| e.PropertyName == nameof(ItemGibViewViewModel.Loaded))
            {
                OnCanExecuteChanged();
            }
        }
    }
}

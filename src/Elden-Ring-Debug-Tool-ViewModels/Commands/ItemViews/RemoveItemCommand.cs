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
    [Description("Remove Item")]
    public class RemoveItemCommand : CommandBase
    {
        private InventoryViewViewModel _inventoryViewViewViewModel { get; }
        private ErdHook _hook => _inventoryViewViewViewModel.Hook;

        public RemoveItemCommand(InventoryViewViewModel inventoryViewViewViewModel)
        {
            _inventoryViewViewViewModel = inventoryViewViewViewModel;
            _inventoryViewViewViewModel.PropertyChanged += ViewViewViewModelPropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return (_hook?.Setup ?? false)
                && _hook.Loaded
                && base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            if (!(parameter is InventoryEntryViewModel entry))
                return;
            
            _hook.RemoveItem(_inventoryViewViewViewModel.ShowStorage, entry.Index);
        }
        

        private void ViewViewViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(ItemGibViewViewModel.Setup)
                or nameof(ItemGibViewViewModel.Loaded))
            {
                OnCanExecuteChanged();
            }
        }
    }
}

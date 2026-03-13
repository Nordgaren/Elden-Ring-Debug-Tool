using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using Erd_Tools;
using System.ComponentModel;
using System.Windows;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    [Description("Get Address")]
    public class GetAddressCommand : CommandBase
    {
        private InventoryViewViewModel _inventoryViewViewViewModel { get; }
        private ErdHook _hook => _inventoryViewViewViewModel.Hook;

        public GetAddressCommand(InventoryViewViewModel inventoryViewViewViewModel)
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
            
            try
            {
                Clipboard.SetText(entry.Address);

            }
            catch (Exception)
            {

            }
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
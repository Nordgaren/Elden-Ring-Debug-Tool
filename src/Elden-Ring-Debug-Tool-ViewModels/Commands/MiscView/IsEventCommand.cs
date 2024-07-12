using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    public class IsEventCommand : CommandBase
    {
        private MiscViewViewModel _miscViewViewModel { get; }

        public IsEventCommand(MiscViewViewModel miscViewViewModel)
        {
            _miscViewViewModel = miscViewViewModel;
            _miscViewViewModel.PropertyChanged += _debugViewViewModel_PropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return _miscViewViewModel.Hook.Setup && _miscViewViewModel.Hook.CSFD4VirtualMemoryFlag.Resolve() != IntPtr.Zero && base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            if (!(parameter is int eventID))
                throw new ArgumentNullException(nameof(parameter), "parameter was null. parameter must be an int");

            _miscViewViewModel.IsEventFlag = _miscViewViewModel.Hook.IsEventFlag(eventID);
        }

        private void _debugViewViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(MiscViewViewModel.Setup) 
                or nameof(MiscViewViewModel.Loaded))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
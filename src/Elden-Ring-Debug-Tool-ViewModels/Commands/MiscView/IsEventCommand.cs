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
            int eventID;
            if (parameter == null)
                eventID = _miscViewViewModel.EventFlag;  // Use the current EventFlag if no parameter is passed
            else if (parameter is int id)
                eventID = id;
            else
                throw new ArgumentException("Parameter must be an int", nameof(parameter));

            // This checks the event flag and updates the ViewModel's IsEventFlag property
            _miscViewViewModel.IsEventFlag = _miscViewViewModel.Hook.IsEventFlag(eventID);
        }

        private void _debugViewViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MiscViewViewModel.Setup) || e.PropertyName == nameof(MiscViewViewModel.Loaded))
            {
                OnCanExecuteChanged();
            }
        }
    }
}

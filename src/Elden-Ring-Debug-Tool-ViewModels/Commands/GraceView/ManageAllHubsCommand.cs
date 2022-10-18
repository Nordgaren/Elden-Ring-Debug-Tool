using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    public class ManageAllHubsCommand : AsyncCommandBase
    {
        private GraceViewViewModel _graceViewViewModel { get; }

        public ManageAllHubsCommand(GraceViewViewModel graceViewViewModel)
        {
            _graceViewViewModel = graceViewViewModel;
            _graceViewViewModel.PropertyChanged += _debugViewViewModel_PropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return _graceViewViewModel.Hook.Setup && _graceViewViewModel.Hook.Loaded && base.CanExecute(parameter);
        }

        public override Task ExecuteAsync(object? parameter)
        {
            return Task.Run(() =>
            {
                bool enable;
                if (!(bool.TryParse(parameter as string, out enable)))
                    throw new ArgumentNullException(nameof(parameter), "parameter was null. parameter must be a bool");

                _graceViewViewModel.MassChange = true;
                foreach (GraceViewModel grace in _graceViewViewModel.SelectedHubViewModel.Graces)
                {
                    _graceViewViewModel.Hook.SetEventFlag(grace.EventFlagID, enable);
                }

                _graceViewViewModel.MassChange = false;
            });
        }

        private void _debugViewViewModel_PropertyChanged(object? sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(GraceViewViewModel.Setup)
                or nameof(GraceViewViewModel.Loaded))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
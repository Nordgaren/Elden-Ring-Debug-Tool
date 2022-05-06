using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    [Description("Force Weather")]

    public class ForceWeatherCommand : CommandBase, IToggleableCommand
    {
        private DebugViewViewModel _debugViewViewModel;

        private bool _state;
        public bool State
        {
            get => _state;
            set => SetField(ref _state, value);
        }

        public ForceWeatherCommand(DebugViewViewModel debugViewViewModel)
        {
            _debugViewViewModel = debugViewViewModel;
            _debugViewViewModel.PropertyChanged += _debugViewViewModel_PropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return _debugViewViewModel.Hook.Setup && _debugViewViewModel.Hook.Loaded && base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            if (_debugViewViewModel.SelectedWeather == null)
                throw new Exception($"{nameof(_debugViewViewModel.SelectedWeather)} cannot be null when trying to force weather");

            State = !State;
            _debugViewViewModel.Hook.SetForcedWeatherValue(_debugViewViewModel.SelectedWeather);
            _debugViewViewModel.Hook.ToggleForceWeather(State);
        }

        private void _debugViewViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DebugViewViewModel.Setup)
                || e.PropertyName == nameof(DebugViewViewModel.Loaded))
            {
                OnCanExecuteChanged();
            }
        }
    }
}

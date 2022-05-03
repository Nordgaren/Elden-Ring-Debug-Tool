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
    public class ForceWeatherCommand : CommandBase
    {
        private DebugViewViewModel _debugViewViewModel;

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
            if (!(parameter is bool state))
                throw new ArgumentException($"Argument {nameof(parameter)} must be a {typeof(bool)}");

            if (_debugViewViewModel.SelectedWeather == null)
                throw new Exception($"{nameof(_debugViewViewModel.SelectedWeather)} cannot be null when trying to force weather");

            _debugViewViewModel.Hook.SetForcedWeatherValue(_debugViewViewModel.SelectedWeather);
            _debugViewViewModel.Hook.ToggleForceWeather(state);
        }

        private void _debugViewViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DebugViewViewModel.Setup))
            {
                OnCanExecuteChanged();
            }
        }
    }
}

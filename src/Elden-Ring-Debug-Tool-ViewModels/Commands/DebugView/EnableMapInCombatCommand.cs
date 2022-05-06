using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    [Description("Enable Map In Combat")]
    public class EnableMapInCombatCommand : CommandBase
    {
        private DebugViewViewModel _debugViewViewModel;

        public EnableMapInCombatCommand(DebugViewViewModel debugViewViewModel)
        {
            _debugViewViewModel = debugViewViewModel;
            _debugViewViewModel.PropertyChanged += _debugViewViewModel_PropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return _debugViewViewModel.Hook.Setup && base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            if (!(parameter is bool state))
                throw new ArgumentException($"Argument {nameof(parameter)} must be a {typeof(bool)}");


            _debugViewViewModel.Hook.ToggleMapCombat(state);
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

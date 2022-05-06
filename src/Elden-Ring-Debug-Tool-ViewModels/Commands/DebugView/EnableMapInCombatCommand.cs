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
<<<<<<< HEAD
=======

>>>>>>> ae9e1ff649386255a43b62d99a99da6a0c3a8f45
    public class EnableMapInCombatCommand : CommandBase, IToggleableCommand
    {
        private DebugViewViewModel _debugViewViewModel;

        private bool _state;
        public bool State
        {
            get => _state;
            set => SetField(ref _state, value);
        }

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
            State = !State;
            _debugViewViewModel.Hook.ToggleMapCombat(State);
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

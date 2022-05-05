using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    public class ToggleableCommand : ICommand, INotifyPropertyChanged
    {
        private bool _state;
        public bool State
        {
            get => _state;
            set => SetField(ref _state, value);
        }

        public void Execute(object? parameter)
        {
            State = !State;
            _originalCommand.Execute(State);
        }

        private ICommand _originalCommand;
        public ToggleableCommand(ICommand command)
        {
            _originalCommand = command;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? CanExecuteChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName ?? "");
            return true;
        }

        public bool CanExecute(object? parameter)
        {
            return _originalCommand.CanExecute(parameter);
        }

        public Type GetOriginalType()
        {
            return _originalCommand.GetType();
        }

    }
}

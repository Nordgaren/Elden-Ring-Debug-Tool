using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    public abstract class AsyncCommandBase : ICommand, INotifyPropertyChanged 
    {
        public event EventHandler? CanExecuteChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            await ExecuteAsync(parameter);
        }

        public abstract Task ExecuteAsync(object? parameter);

        protected void OnCanExecuteChanged()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                CanExecuteChanged?.Invoke(this, new EventArgs());
            });
        }

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

    }
}

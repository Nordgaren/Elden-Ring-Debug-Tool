using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    public abstract class CommandBase : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public virtual bool CanExecute(object? parameter)
        {
            return true;
        }

        public abstract void Execute(object? parameter);

        protected void OnCanExecuteChanged()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                CanExecuteChanged?.Invoke(this, new EventArgs());
            });
        }
    }
}

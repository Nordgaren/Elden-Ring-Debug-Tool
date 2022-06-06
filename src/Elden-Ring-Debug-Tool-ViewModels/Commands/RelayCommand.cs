using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    internal class RelayCommand : CommandBase
    {
        private Action _callBack { get; }
        private Predicate<object> _canExecute { get; }

        public RelayCommand(Action callBack, Predicate<object> canExecute) : base()
        {
            _callBack = callBack ?? throw new ArgumentNullException(nameof(callBack));
            _canExecute = canExecute;
        }

        public RelayCommand(Action callBack) : this(callBack, o => true) { }

        public override bool CanExecute(object? parameter)
        {
            return base.CanExecute(parameter) && parameter != null && _canExecute.Invoke(parameter);
        }

        public override void Execute(object? parameter)
        {
            if (parameter != null) _callBack.Invoke();
        }
    }
}

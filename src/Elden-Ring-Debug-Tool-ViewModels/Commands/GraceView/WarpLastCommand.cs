using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    [Description("Warp to Last Grace")]
    public class WarpLastCommand : CommandBase
    {
        private GraceViewViewModel _graceViewViewModel { get; }

        public WarpLastCommand(GraceViewViewModel graceViewViewModel)
        {
            _graceViewViewModel = graceViewViewModel;
            _graceViewViewModel.PropertyChanged += _debugViewViewModel_PropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return _graceViewViewModel.Hook.Setup && _graceViewViewModel.Hook.Loaded && base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            _graceViewViewModel.Hook.Warp(_graceViewViewModel.LastGraceID);
        }

        private void _debugViewViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(GraceViewViewModel.Setup)
                or nameof(GraceViewViewModel.Loaded))
            {
                OnCanExecuteChanged();
            }
        }
    }
}

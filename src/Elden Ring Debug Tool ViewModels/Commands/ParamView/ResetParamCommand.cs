using Erd_Tools;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using System.Windows;
using System.ComponentModel;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    [Description("Reset Param")]
    public class ResetParamCommand : CommandBase
    {
        private ParamViewViewModel _paramViewerViewModel;
        private ERHook _hook => _paramViewerViewModel.Hook;

        public ResetParamCommand(ParamViewViewModel paramViewerViewModel)
        {
            _paramViewerViewModel = paramViewerViewModel;
            _paramViewerViewModel.PropertyChanged += _paramViewerViewModel_PropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return _hook?.Setup ?? false && base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to reset the currently selected param to what it was when the debug tool loaded?", 
                "Reset Param", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _paramViewerViewModel.SelectedParam.Param.RestoreParam();
            }
        }

        private void _paramViewerViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ParamViewViewModel.Setup))
            {
                OnCanExecuteChanged();
            }
        }
    }
}

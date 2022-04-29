using Erd_Tools;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using System.Diagnostics;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    public class OpenParamCaptureFolderCommand : CommandBase
    {
        private ParamViewerViewModel _paramViewerViewModel;
        private ERHook _hook => _paramViewerViewModel.Hook;

        public OpenParamCaptureFolderCommand(ParamViewerViewModel paramViewerViewModel)
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
            Process.Start("explorer.exe", _paramViewerViewModel.ParamSavePath);
        }

        private void _paramViewerViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ParamViewerViewModel.Setup))
            {
                OnCanExecuteChanged();
            }
        }
    }
}

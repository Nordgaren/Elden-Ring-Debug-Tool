using Erd_Tools;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using System.Diagnostics;
using System.ComponentModel;
using Erd_Tools.Hook;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    [Description("Open Param Capture Folder")]
    public class OpenParamCaptureFolderCommand : CommandBase
    {
        private ParamViewViewModel _paramViewerViewModel { get; }
        private ErdHook _hook => _paramViewerViewModel.Hook;

        public OpenParamCaptureFolderCommand(ParamViewViewModel paramViewerViewModel)
        {
            _paramViewerViewModel = paramViewerViewModel;
            _paramViewerViewModel.PropertyChanged += _paramViewerViewModel_PropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return (_hook.Setup) && base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            Process.Start("explorer.exe", _paramViewerViewModel.ParamSavePath);
        }

        private void _paramViewerViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(ParamViewViewModel.Setup))
            {
                OnCanExecuteChanged();
            }
        }
    }
}

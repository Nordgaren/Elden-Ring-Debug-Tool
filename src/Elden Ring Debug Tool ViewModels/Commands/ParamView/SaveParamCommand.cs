using Erd_Tools;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using System.Windows;
using System.ComponentModel;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    [Description("Save Param")]
    public class SaveParamCommand : CommandBase
    {
        private ParamViewViewModel _paramViewerViewModel;
        private ERHook _hook => _paramViewerViewModel.Hook;

        public SaveParamCommand(ParamViewViewModel paramViewerViewModel)
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
            ERParam param = _paramViewerViewModel.SelectedParam.Param;

            _paramViewerViewModel.Hook.SaveParam(param);

            MessageBox.Show($@"{param.Name} saved to:
            {_paramViewerViewModel.ParamSavePath}\capture\param", "Param Saved", MessageBoxButton.OK, MessageBoxImage.Information);
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

using Elden_Ring_Debug_Tool;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    public class SaveParamCommand : CommandBase
    {
        private ParamViewerViewModel _paramViewerViewModel;
        private ERHook _hook;

        public SaveParamCommand(ParamViewerViewModel paramViewerViewModel, ERHook hook)
        {
            _paramViewerViewModel = paramViewerViewModel;
            _hook = hook;
            _paramViewerViewModel.PropertyChanged += _paramViewerViewModel_PropertyChanged;
        }

        private void _paramViewerViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ParamViewerViewModel.Setup))
            {
                OnCanExecuteChanged();
            }
        }

        public override bool CanExecute(object? parameter)
        {
            return _hook?.Setup ?? false && base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            var param = _paramViewerViewModel.SelectedParam.Param;

            _hook.SaveParam(param);

            MessageBox.Show($@"{param.Name} saved to:
            {_paramViewerViewModel.ParamSavePath}\capture\param", "Param Saved", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}

﻿using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using Erd_Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    public class OpenGitHubCommand : CommandBase
    {
        private MainWindowViewModel _mainWindowViewModel;
        private ERHook _hook => _mainWindowViewModel.Hook;

        public OpenGitHubCommand(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public override bool CanExecute(object? parameter)
        {
            return _hook?.Setup ?? false && base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = _mainWindowViewModel.Uri.ToString(),
                UseShellExecute = true
            });
        }

    }
}

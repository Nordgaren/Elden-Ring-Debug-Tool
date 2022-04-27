using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
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

        public OpenGitHubCommand(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
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

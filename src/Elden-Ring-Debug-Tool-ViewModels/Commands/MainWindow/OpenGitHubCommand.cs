using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using Erd_Tools;
using System.Diagnostics;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    public class OpenGitHubCommand : CommandBase
    {
        private MainWindowViewModel _mainWindowViewModel { get; }
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

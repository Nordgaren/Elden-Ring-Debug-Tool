using Elden_Ring_Debug_Tool_ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class HotkeyViewModel : ViewModelBase
    {
        private ViewModelBase _parentViewModel { get;  }

        private string _parentViewModelName;
        public string ParentViewModelName
        {
            get => _parentViewModelName;
            set => SetField(ref _parentViewModelName, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        private ICommand _command;
        public ICommand Command
        {
            get => _command;
            set => SetField(ref _command, value);
        }

        private ToggleableCommand? _toggleableCommand;
        public ToggleableCommand? ToggleableCommand
        {
            get => _toggleableCommand;
            set => SetField(ref _toggleableCommand, value);
        }

        private Key? _key = null;
        public Key? Key
        {
            get => _key;
            set => SetField(ref _key, value);
        }

        public HotkeyViewModel() { }

        public HotkeyViewModel(ViewModelBase parentViewModel, ICommand command)
        {
            _parentViewModel = parentViewModel;

            ParentViewModelName = "Missing Parent Description";

            DescriptionAttribute? description = _parentViewModel.GetType().GetCustomAttribute<DescriptionAttribute>();

            if (description != null)
                ParentViewModelName = description.Description;

            Command = command;

            if (Command is ToggleableCommand tCommand)
                ToggleableCommand = tCommand;
            else
                ToggleableCommand = null;

            Name = Command.GetType().Name;

            DescriptionAttribute? commandDescription = Command.GetType().GetCustomAttribute<DescriptionAttribute>();

            if (commandDescription != null)
                Name = commandDescription.Description;


        }

    }
}

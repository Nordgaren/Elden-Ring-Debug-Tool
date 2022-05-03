using Elden_Ring_Debug_Tool_ViewModels.Attributes;
using Elden_Ring_Debug_Tool_ViewModels.Commands;
using Elden_Ring_Debug_Tool_ViewModels.Manager;
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
        private ViewModelBase _parentViewModel { get; }

        public ViewModelBase ParentViewModel =>  _parentViewModel;

        public IEnumerable<PropertyInfo> GetCustomAttributes()
        {
            return _parentViewModel.GetType().GetProperties().Where(x => x.CustomAttributes.Any(a => a.AttributeType == typeof(HotkeyParameterAttribute)));
        }

        private MainWindowViewModel _mainWindowViewModel;
        internal MainWindowViewModel MainWindowViewModel
        {
            get => _mainWindowViewModel;
            set => SetField(ref _mainWindowViewModel, value);
        }

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
            set
            {
                Key? oldKey = _key;
                if (SetField(ref _key, value))
                {
                    if (oldKey != null)
                        MainWindowViewModel.HotkeyManager.RemoveHotkey(oldKey.Value, Command);

                    if (Key != null)
                        MainWindowViewModel.HotkeyManager.AddHotkey(Key.Value, Command);
                }
            }
        }

        public List<string> StringList = new List<string> { "lol", "kek", "why", "plx", "work!"};

        public HotkeyViewModel() { }

        public HotkeyViewModel(ViewModelBase parentViewModel, MainWindowViewModel mainWindowViewModel,ICommand command)
        {

            _mainWindowViewModel = mainWindowViewModel;
            _parentViewModel = parentViewModel;

            ParentViewModelName = "Missing Parent Description";

            DescriptionAttribute? description = _parentViewModel.GetType().GetCustomAttribute<DescriptionAttribute>();

            if (description != null)
                ParentViewModelName = description.Description;

            Command = command;


            Name = Command.GetType().Name;

            DescriptionAttribute? commandDescription = Command.GetType().GetCustomAttribute<DescriptionAttribute>();

            if (commandDescription != null)
                Name = commandDescription.Description;

            if (Command is ToggleableCommand tCommand)
            {
                ToggleableCommand = tCommand;
                commandDescription = tCommand.GetOriginalType().GetCustomAttribute<DescriptionAttribute>();

                if (commandDescription != null)
                    Name = commandDescription.Description;
            }
            else
            {
                ToggleableCommand = null;
            }



        }

    }
}

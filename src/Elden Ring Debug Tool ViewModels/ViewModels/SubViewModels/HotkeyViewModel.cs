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
        private IHotkeyManager HotkeyManager => MainWindowViewModel.HotkeyViewViewModel.HotkeyManager;

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
                        HotkeyManager.RemoveHotkey(oldKey.Value, Command);

                    if (Key != null)
                        HotkeyManager.AddHotkey(Key.Value, Command);
                }
            }
        }

        public bool HasDependancies { get; set; }

        public List<(PropertyInfo prop, HotkeyParameterAttribute parameter)> HotkeyParameterAttribute { get; set; }
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

            IEnumerable<PropertyInfo> customAttrs = _parentViewModel.GetType().GetProperties().Where(x => x.CustomAttributes.Any(a => a.AttributeType == typeof(HotkeyParameterAttribute)));

            Type commantType = GetDescriptionAttr();

            HotkeyParameterAttribute = new List<(PropertyInfo Prop, HotkeyParameterAttribute Parameter)>();

            foreach (PropertyInfo prop in customAttrs)
            {
                HotkeyParameterAttribute? hKeyParamAttr = prop.GetCustomAttribute<HotkeyParameterAttribute>();

                if (hKeyParamAttr == null || hKeyParamAttr.CommandType != commantType)
                    continue;


                HotkeyParameterAttribute.Add((prop,hKeyParamAttr));
            }

            HasDependancies = HotkeyParameterAttribute.Count > 0;
        }

        private Type GetDescriptionAttr()
        {
            Type? type = Command.GetType();
            DescriptionAttribute? commandDescription = type.GetCustomAttribute<DescriptionAttribute>();

            if (commandDescription != null)
                Name = commandDescription.Description;

            if (Command is ToggleableCommand tCommand)
            {
                type = tCommand.GetOriginalType();

                ToggleableCommand = tCommand;
                commandDescription = type.GetCustomAttribute<DescriptionAttribute>();

                if (commandDescription != null)
                    Name = commandDescription.Description;
            }

            return type;
        }
    }
}

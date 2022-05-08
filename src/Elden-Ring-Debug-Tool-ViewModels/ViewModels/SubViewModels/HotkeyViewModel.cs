using Elden_Ring_Debug_Tool_ViewModels.Attributes;
using Elden_Ring_Debug_Tool_ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Elden_Ring_Debug_Tool_ViewModels.Managers;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels
{
    public class HotKeyViewModel : ViewModelBase
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

        private IToggleableCommand? _toggleableCommand;
        public IToggleableCommand? ToggleableCommand
        {
            get => _toggleableCommand;
            set => SetField(ref _toggleableCommand, value);
        }
        private IHotKeyManager _hotKeyManager => MainWindowViewModel.HotKeyViewViewModel.HotKeyManager;

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
                        _hotKeyManager.RemoveHotKey(oldKey.Value, Command);

                    if (Key != null)
                        _hotKeyManager.AddHotKey(Key.Value, Command);
                }
            }
        }

        public bool HasDependencies { get; set; }

        public List<(PropertyInfo prop, HotKeyParameterAttribute parameter)> HotKeyParameterAttribute { get; set; }

        public HotKeyViewModel(ViewModelBase parentViewModel, MainWindowViewModel mainWindowViewModel,ICommand command)
        {

            _mainWindowViewModel = mainWindowViewModel;
            _parentViewModel = parentViewModel;

            ParentViewModelName = "Missing Parent Description";

            DescriptionAttribute? description = _parentViewModel.GetType().GetCustomAttribute<DescriptionAttribute>();

            if (description != null)
                ParentViewModelName = description.Description;

            Command = command;


            Name = Command.GetType().Name;

            IEnumerable<PropertyInfo> customAttrs = _parentViewModel.GetType().GetProperties().Where(x => x.CustomAttributes.Any(a => a.AttributeType == typeof(HotKeyParameterAttribute)));

            Type commandType = GetDescriptionAttr();

            HotKeyParameterAttribute = new List<(PropertyInfo Prop, HotKeyParameterAttribute Parameter)>();

            foreach (PropertyInfo prop in customAttrs)
            {
                HotKeyParameterAttribute? hKeyParamAttr = prop.GetCustomAttribute<HotKeyParameterAttribute>();

                if (hKeyParamAttr == null || hKeyParamAttr.CommandType != commandType)
                    continue;


                HotKeyParameterAttribute.Add((prop,hKeyParamAttr));
            }

            HasDependencies = HotKeyParameterAttribute.Count > 0;
        }

        private Type GetDescriptionAttr()
        {
            Type? type = Command.GetType();
            DescriptionAttribute? commandDescription = type.GetCustomAttribute<DescriptionAttribute>();

            if (commandDescription != null)
                Name = commandDescription.Description;

            if (Command is IToggleableCommand tCommand)
                ToggleableCommand = tCommand;

            return type;
        }
    }
}

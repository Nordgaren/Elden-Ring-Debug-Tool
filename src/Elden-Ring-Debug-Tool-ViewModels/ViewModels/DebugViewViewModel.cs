using Elden_Ring_Debug_Tool_ViewModels.Attributes;
using Elden_Ring_Debug_Tool_ViewModels.Commands;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using Erd_Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Erd_Tools.Hook;
using static Elden_Ring_Debug_Tool_ViewModels.Attributes.HotKeyParameterAttribute;
using static Erd_Tools.Hook.ErdHook;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    [Description("Debug View")]
    public class DebugViewViewModel : ViewModelBase
    {
        private MainWindowViewModel _mainWindowViewModel { get; set; }
        internal ErdHook Hook { get; private set; }
        public ICommand EnableMapInCombatCommand { get; }
        public ICommand ForceWeatherCommand { get; }

        private ObservableCollection<HotKeyViewModel> _debugCommands;
        public ObservableCollection<HotKeyViewModel> DebugCommands
        {
            get => _debugCommands;
            set
            {
                if (SetField(ref _debugCommands, value))
                {
                    DebugCollectionView.Filter += FilterCommands;
                    OnPropertyChanged(nameof(DebugCollectionView));
                }
            }
        }
        public ICollectionView DebugCollectionView => CollectionViewSource.GetDefaultView(DebugCommands);

        public DebugViewViewModel()
        {
            EnableMapInCombatCommand = new EnableMapInCombatCommand(this);

            ForceWeatherCommand = new ForceWeatherCommand(this);

            Commands.Add(EnableMapInCombatCommand);
            Commands.Add(ForceWeatherCommand);

            DebugCommands = new ObservableCollection<HotKeyViewModel>();
        }

        public void InitViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            foreach (ICommand command in Commands)
            {
                DebugCommands.Add(new HotKeyViewModel(this, _mainWindowViewModel, command));
            }
            Hook = _mainWindowViewModel.Hook;
            Hook.OnSetup += Hook_OnSetup;
            Hook.OnUnhooked += Hook_OnUnhooked;
            WeatherTypes = new ObservableCollection<WeatherTypes>(Enum.GetValues(typeof(WeatherTypes)).Cast<WeatherTypes>());
        }
        public void UpdateViewModel()
        {
            Loaded = Hook.Loaded;

            if (((IToggleableCommand)ForceWeatherCommand).State)
                Hook.ForceWeather();
        }

        public void ResetViewModel()
        {
            Loaded = Hook.Loaded;

            if (((IToggleableCommand)ForceWeatherCommand).State)
                ForceWeatherCommand.Execute(null);
        }
        private void Hook_OnSetup(object? sender, PropertyHook.PHEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Setup = true;
            });
        }

        private void Hook_OnUnhooked(object? sender, PropertyHook.PHEventArgs e)
        {
            Setup = false;
        }

        public void Dispose()
        {
            if (((IToggleableCommand)ForceWeatherCommand).State)
                ForceWeatherCommand.Execute(null);

            if (((IToggleableCommand)EnableMapInCombatCommand).State)
                EnableMapInCombatCommand.Execute(null);
        }

        private bool _setup;
        public bool Setup
        {
            get => _setup;
            set
            {
                if (SetField(ref _setup, value))
                {
                    DebugCollectionView.Refresh();
                }
            }
        }

        private bool _loaded;
        public bool Loaded
        {
            get => _loaded;
            set
            {
                if (SetField(ref _loaded, value))
                {
                    DebugCollectionView.Refresh();
                }
            }
        }

        private ObservableCollection<WeatherTypes> _weatherTypes;
        [Description("Weather Types")]
        [HotKeyParameter(typeof(ForceWeatherCommand), ResourceType.ComboBox, SelectedItemPropertyName = nameof(SelectedWeather))]
        public ObservableCollection<WeatherTypes> WeatherTypes
        {
            get => _weatherTypes;
            set => SetField(ref _weatherTypes, value);
        }

        private WeatherTypes _selectedWeather;
        public WeatherTypes SelectedWeather
        {
            get => _selectedWeather;
            set
            {
                if (SetField(ref _selectedWeather, value))
                {
                    Hook.SetForcedWeatherValue(SelectedWeather);
                }
            }
        }

        #region Search

        private bool _searchAll;
        public bool SearchAll
        {
            get => _searchAll;
            set
            {
                SetField(ref _searchAll, value);
                OnPropertyChanged(nameof(DebugCollectionView));
            }
        }

        private string _commandFilter = string.Empty;

        public string CommandFilter
        {
            get => _commandFilter;
            set
            {
                SetField(ref _commandFilter, value);
                DebugCollectionView.Refresh();
            }
        }

        private bool FilterCommands(object obj)
        {
            if (obj is HotKeyViewModel hotkey)
            {
                return hotkey.Name.Contains(CommandFilter, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        #endregion
    }
}

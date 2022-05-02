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
using static Erd_Tools.ERHook;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    [Description("Debug View")]
    public class DebugViewViewModel : ViewModelBase
    {
        internal ERHook Hook { get; set; }
        public ICommand EnableMapInCombatCommand { get; }

        private ObservableCollection<HotkeyViewModel> _debugCommands;
        public ObservableCollection<HotkeyViewModel> DebugCommands
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
            ICommand enableMapInCombatCommand = new EnableMapInCombatCommand(this);
            EnableMapInCombatCommand = new ToggleableCommand(enableMapInCombatCommand);
            Commands.Add(EnableMapInCombatCommand);

            DebugCommands = new ObservableCollection<HotkeyViewModel>();

            foreach (ICommand command in Commands)
            {
                DebugCommands.Add(new HotkeyViewModel(this, command));
            }
        }

        public void InitViewModel(ERHook hook)
        {
            Hook = hook;
            Hook.OnSetup += Hook_OnSetup;
            Hook.OnUnhooked += Hook_OnUnhooked;
            WeatherTypes = new ObservableCollection<WeatherTypes>(Enum.GetValues(typeof(WeatherTypes)).Cast<WeatherTypes>());
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

        private bool _setup;
        public bool Setup
        {
            get => _setup;
            set => SetField(ref _setup, value);
        }

        private ObservableCollection<WeatherTypes> _weatherTypes;
        public ObservableCollection<WeatherTypes> WeatherTypes
        {
            get => _weatherTypes;
            set => SetField(ref _weatherTypes, value);
        }

        private WeatherTypes _selectedWeather;
        public WeatherTypes SelectedWeather
        {
            get => _selectedWeather;
            set => SetField(ref _selectedWeather, value);
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
            if (obj is HotkeyViewModel hotkey)
            {
                return hotkey.Name.Contains(CommandFilter, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        #endregion
    }
}

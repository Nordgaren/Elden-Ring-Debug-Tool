using Elden_Ring_Debug_Tool_ViewModels.Manager;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using Erd_Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    [Description("Hotkey View")]
    public class HotkeyViewViewModel : ViewModelBase
    {
        internal ErdHook Hook;

        private ObservableCollection<HotkeyViewModel> _hotkeys;
        public ObservableCollection<HotkeyViewModel> Hotkeys
        {
            get => _hotkeys;
            set
            {
                if (SetField(ref _hotkeys, value))
                {
                    OnPropertyChanged(nameof(HotkeyCollectionView));
                    HotkeyCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(HotkeyViewModel.ParentViewModelName)));
                    HotkeyCollectionView.Filter += FilterHotkeys;
                }
            }
        }

        public ICollectionView HotkeyCollectionView => CollectionViewSource.GetDefaultView(Hotkeys);

        private ObservableCollection<HotkeyViewModel> GetHotkeyViewModelCollection()
        {
            ObservableCollection<HotkeyViewModel> hotkeyCollection = new ObservableCollection<HotkeyViewModel>();
            foreach (ViewModelBase viewModel in _mainWindowViewModel.ViewModels)
            {
                if (viewModel is DebugViewViewModel debugViewViewModel)
                {
                    foreach (var hKeyVM in debugViewViewModel.DebugCommands)
                    {
                        hotkeyCollection.Add(hKeyVM);
                    }

                    continue;
                }

                foreach (ICommand command in viewModel.Commands)
                {
                    hotkeyCollection.Add(new HotkeyViewModel(viewModel, _mainWindowViewModel,command));
                }
            }

            return hotkeyCollection;
        }

        private MainWindowViewModel _mainWindowViewModel { get; set; }

        private SettingsViewViewModel _settingsViewViewModel { get; set; }

        public HotkeyViewViewModel() { }

        public void InitViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _settingsViewViewModel = _mainWindowViewModel.SettingsViewViewModel;
            _enableHotkeys = _settingsViewViewModel.EnableHotKeys;
            Hotkeys = new ObservableCollection<HotkeyViewModel>();
            Hook = _mainWindowViewModel.Hook;
            _hotkeyManager = new WindowsRegisteredMultiHotkeyManager(Hook);
            _hotkeyManager.SetHotkeyEnable(SettingsViewViewModel.Settings.EnableHotkeys);
            Hook.OnSetup += Hook_OnSetup;
            Hook.OnUnhooked += Hook_OnUnhooked;
            _mainWindowViewModel.PropertyChanged += ViewModel_PropertyChanged;
            Hotkeys = GetHotkeyViewModelCollection();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_mainWindowViewModel.ViewModels)
                || e.PropertyName == nameof(Commands))
            {
                Hotkeys = GetHotkeyViewModelCollection();
            }
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
        internal void UpdateViewModel()
        {
            Loaded = Hook.Loaded;
            HotkeyManager.Update();

        }

        internal void ResetViewModel()
        {
            Loaded = Hook.Loaded;
            HotkeyManager.Update();

        }

        private IHotkeyManager _hotkeyManager;
        internal IHotkeyManager HotkeyManager
        {
            get => _hotkeyManager;
            set => SetField(ref _hotkeyManager, value);
        }


        private bool _enableHotkeys;
        public bool EnableHotkeys
        {
            get => _enableHotkeys;
            set 
            {
                if (SetField(ref _enableHotkeys, value))
                {
                    HotkeyManager.SetHotkeyEnable(EnableHotkeys);
                    _settingsViewViewModel.EnableHotKeys = EnableHotkeys;
                }
            }
        }

        private bool _setup;
        public bool Setup
        {
            get => _setup;
            set
            {
                if (SetField(ref _setup, value))
                {
                    HotkeyCollectionView.Refresh();
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
                    HotkeyCollectionView.Refresh();
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
                OnPropertyChanged(nameof(HotkeyCollectionView));
            }
        }

        private string _hotkeyFilter = string.Empty;


        public string HotkeyFilter
        {
            get => _hotkeyFilter;
            set
            {
                SetField(ref _hotkeyFilter, value);
                HotkeyCollectionView.Refresh();
            }
        }

        private bool FilterHotkeys(object obj)
        {
            if (obj is HotkeyViewModel hotkey)
            {
                return hotkey.Name.Contains(HotkeyFilter, StringComparison.InvariantCultureIgnoreCase)
                    || hotkey.ParentViewModelName.Contains(HotkeyFilter, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        internal void Dispose()
        {
            Setup = false;
        }

        #endregion
    }
}

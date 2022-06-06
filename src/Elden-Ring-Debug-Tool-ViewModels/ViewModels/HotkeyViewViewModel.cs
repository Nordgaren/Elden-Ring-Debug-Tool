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
using Elden_Ring_Debug_Tool_ViewModels.Managers;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    [Description("HotKey View")]
    public class HotKeyViewViewModel : ViewModelBase
    {
        internal ErdHook Hook;

        private ObservableCollection<HotKeyViewModel> _hotKeys;
        public ObservableCollection<HotKeyViewModel> HotKeys
        {
            get => _hotKeys;
            set
            {
                if (SetField(ref _hotKeys, value))
                {
                    OnPropertyChanged(nameof(HotKeyCollectionView));
                    HotKeyCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(HotKeyViewModel.ParentViewModelName)));
                    HotKeyCollectionView.Filter += FilterHotKeys;
                }
            }
        }

        public ICollectionView HotKeyCollectionView => CollectionViewSource.GetDefaultView(HotKeys);

        private ObservableCollection<HotKeyViewModel> GetHotKeyViewModelCollection()
        {
            ObservableCollection<HotKeyViewModel> hotkeyCollection = new();
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
                    hotkeyCollection.Add(new HotKeyViewModel(viewModel, _mainWindowViewModel,command));
                }
            }

            return hotkeyCollection;
        }

        private MainWindowViewModel _mainWindowViewModel { get; set; }

        private SettingsViewViewModel _settingsViewViewModel { get; set; }

        public void InitViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _settingsViewViewModel = _mainWindowViewModel.SettingsViewViewModel;
            _enableHotKeys = _settingsViewViewModel.EnableHotKeys;
            HotKeys = new ObservableCollection<HotKeyViewModel>();
            Hook = _mainWindowViewModel.Hook;
            _hotKeyManager = new WindowsRegisteredMultiHotKeyManager(Hook);
            _hotKeyManager.SetHotKeyEnable(SettingsViewViewModel.Settings.EnableHotKeys);
            Hook.OnSetup += Hook_OnSetup;
            Hook.OnUnhooked += Hook_OnUnhooked;
            _mainWindowViewModel.PropertyChanged += ViewModel_PropertyChanged;
            HotKeys = GetHotKeyViewModelCollection();
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_mainWindowViewModel.ViewModels)
                || e.PropertyName == nameof(Commands))
            {
                HotKeys = GetHotKeyViewModelCollection();
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
            HotKeyManager.Update();

        }

        internal void ResetViewModel()
        {
            Loaded = Hook.Loaded;
            HotKeyManager.Update();

        }

        private IHotKeyManager _hotKeyManager;
        internal IHotKeyManager HotKeyManager
        {
            get => _hotKeyManager;
            set => SetField(ref _hotKeyManager, value);
        }


        private bool _enableHotKeys;
        public bool EnableHotKeys
        {
            get => _enableHotKeys;
            set 
            {
                if (SetField(ref _enableHotKeys, value))
                {
                    HotKeyManager.SetHotKeyEnable(EnableHotKeys);
                    _settingsViewViewModel.EnableHotKeys = EnableHotKeys;
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
                    HotKeyCollectionView.Refresh();
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
                    HotKeyCollectionView.Refresh();
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
                OnPropertyChanged(nameof(HotKeyCollectionView));
            }
        }

        private string _hotkeyFilter = string.Empty;


        public string HotKeyFilter
        {
            get => _hotkeyFilter;
            set
            {
                SetField(ref _hotkeyFilter, value);
                HotKeyCollectionView.Refresh();
            }
        }

        private bool FilterHotKeys(object obj)
        {
            if (obj is HotKeyViewModel hotkey)
            {
                return hotkey.Name.Contains(HotKeyFilter, StringComparison.InvariantCultureIgnoreCase)
                    || hotkey.ParentViewModelName.Contains(HotKeyFilter, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        #endregion
    }
}

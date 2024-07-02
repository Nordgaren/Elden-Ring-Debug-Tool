using Erd_Tools;
using System.Windows;
using System.Windows.Media;
using System.Timers;
using System.Diagnostics;
using System.Windows.Input;
using Elden_Ring_Debug_Tool_ViewModels.Commands;
using Octokit;
using System.Reflection;
using Bluegrams.Application;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Elden_Ring_Debug_Tool_ViewModels.Properties;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels.SubViewModels;
using Application = System.Windows.Application;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    [Description("Main Window")]
    public class MainWindowViewModel : ViewModelBase
    {
        public ErdHook Hook { get; }
        public bool Reading
        {
            get => ErdHook.Reading;
            set => ErdHook.Reading = value;
        }

        private System.Timers.Timer _updateTimer { get; } = new();
        public ICommand OpenGitHubCommand { get; set; }

        public bool ShowWarning
        {
            get => SettingsViewViewModel.ShowWarning;
            set => SettingsViewViewModel.ShowWarning = value;
        }

        private ObservableCollection<ViewModelBase> _viewModels;
        public ObservableCollection<ViewModelBase> ViewModels
        {
            get => _viewModels;
            set => SetField(ref _viewModels, value);
        }

        public MainWindowViewModel()
        {
            Hook = new ErdHook(5000, 1000, p => p.MainWindowTitle is "ELDEN RING™");
            Hook.OnSetup += Hook_OnSetup;
            Hook.OnUnhooked += Hook_OnUnhooked;
            OpenGitHubCommand = new OpenGitHubCommand(this);
            _uri = new Uri("https://github.com/Nordgaren/Elden-Ring-Debug-Tool");

            _settingsViewViewModel = new SettingsViewViewModel();
            Hook.LoadTimeout = SettingsViewViewModel.LoadTimeout;
            _paramViewViewModel = new ParamViewViewModel();
            _playerViewViewModel = new PlayerViewViewModel();
            _itemGibViewViewModel = new ItemGibViewViewModel();
            _inventoryViewViewModel = new InventoryViewViewModel();
            _debugViewViewModel = new DebugViewViewModel();
            _graceViewViewModel = new GraceViewViewModel();
            _hotKeyViewViewModel = new HotKeyViewViewModel();
            _targetViewViewModel = new TargetViewViewModel();
            _miscViewViewModel = new MiscViewViewModel();

            _viewModels = new ObservableCollection<ViewModelBase>();
            ViewModels.Add(this);
            ViewModels.Add(ParamViewViewModel);
            ViewModels.Add(PlayerViewViewModel);
            ViewModels.Add(ItemGibViewViewModel);
            ViewModels.Add(InventoryViewViewModel);
            ViewModels.Add(GraceViewViewModel);
            ViewModels.Add(DebugViewViewModel);
            ViewModels.Add(TargetViewViewModel);
            ViewModels.Add(HotKeyViewViewModel);
            ViewModels.Add(MiscViewViewModel);

            InitAllViewModels();
            Hook.Start();
            Application.Current.MainWindow.Loaded += Load;
            Application.Current.Exit += Dispose;
        }

        public async void Load(object sender, RoutedEventArgs e)
    {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo? fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string? version = fileVersionInfo.FileVersion;

            WindowTitle = $"Elden Ring Debug Tool {version}";
            EnableAllCtrls(false);

            try
            {
                GitHubClient gitHubClient = new(new ProductHeaderValue("Elden-Ring-Debug-Tool"));
                Release release = await gitHubClient.Repository.Release.GetLatest("Nordgaren", "Elden-Ring-Debug-Tool");
                Version gitVersion = Version.Parse(release.TagName.ToLower().Replace("v", ""));
                Version exeVersion = Version.Parse(version);

                if (gitVersion > exeVersion) //Compare latest version to current version
                {
                    Uri = new Uri(release.HtmlUrl);
                    UpdateRequired = true;
                    UpdateInfo = "New version available!";
                }
                else if (gitVersion == exeVersion)
                {
                    UpdateInfo = "App up to date";
                }
                else
                {
                    UpdateInfo = "App version unreleased. Be wary of bugs!";
                }
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is ApiException || ex is ArgumentException)
            {
                UpdateInfo = "Current app version unknown";
            }
            catch (Exception ex)
            {
                UpdateInfo = "Something is very broke, contact Elden Ring Debug Tool repo owner";
                MessageBox.Show(ex.Message);
            }
            _updateTimer.Interval = 16;
            _updateTimer.Elapsed += UpdateTimer_Elapsed;
            _updateTimer.Enabled = true;
        }

        private SettingsViewViewModel _settingsViewViewModel;
        public SettingsViewViewModel SettingsViewViewModel
        {
            get => _settingsViewViewModel;
            set => SetField(ref _settingsViewViewModel, value);
        }

        private ParamViewViewModel _paramViewViewModel;
        public ParamViewViewModel ParamViewViewModel
        {
            get => _paramViewViewModel;
            set => SetField(ref _paramViewViewModel, value);
        }

        private PlayerViewViewModel _playerViewViewModel;
        public PlayerViewViewModel PlayerViewViewModel
        {
            get => _playerViewViewModel;
            set => SetField(ref _playerViewViewModel, value);
        }

        private ItemGibViewViewModel _itemGibViewViewModel;
        public ItemGibViewViewModel ItemGibViewViewModel
        {
            get => _itemGibViewViewModel;
            set => SetField(ref _itemGibViewViewModel, value);
        }

        private InventoryViewViewModel _inventoryViewViewModel;
        public InventoryViewViewModel InventoryViewViewModel
        {
            get => _inventoryViewViewModel;
            set => SetField(ref _inventoryViewViewModel, value);
        }

        private HotKeyViewViewModel _hotKeyViewViewModel;
        public HotKeyViewViewModel HotKeyViewViewModel
        {
            get => _hotKeyViewViewModel;
            set => SetField(ref _hotKeyViewViewModel, value);
        }

        private DebugViewViewModel _debugViewViewModel;
        public DebugViewViewModel DebugViewViewModel
        {
            get => _debugViewViewModel;
            set => SetField(ref _debugViewViewModel, value);
        }

        private TargetViewViewModel _targetViewViewModel;
        public TargetViewViewModel TargetViewViewModel
        {
            get => _targetViewViewModel;
            set => SetField(ref _targetViewViewModel, value);
        }

        private GraceViewViewModel _graceViewViewModel;
        public GraceViewViewModel GraceViewViewModel
        {
            get => _graceViewViewModel;
            set => SetField(ref _graceViewViewModel, value);
        }

        private MiscViewViewModel _miscViewViewModel;
        public MiscViewViewModel MiscViewViewModel
        {
            get => _miscViewViewModel;
            set => SetField(ref _miscViewViewModel, value);
        }

        private Uri _uri;
        public Uri Uri
        {
            get => _uri;
            set => SetField(ref _uri, value);
        }

        private string _updateInfo = string.Empty;
        public string UpdateInfo
        {
            get => _updateInfo;
            set => SetField(ref _updateInfo, value);
        }

        private bool _updateRequired = false;
        public bool UpdateRequired
        {
            get => _updateRequired;
            set => SetField(ref _updateRequired, value);
        }

        private string _windowTitle = "Elden Ring Debug Tool";
        public string WindowTitle
        {
            get => _windowTitle;
            set => SetField(ref _windowTitle, value);
        }
        private void Hook_OnSetup(object? sender, PropertyHook.PHEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ID = Hook.ID;
            });
        }
        private void Hook_OnUnhooked(object? sender, PropertyHook.PHEventArgs e)
        {

        }
        private void Dispose(object sender, ExitEventArgs e)
        {
            _updateTimer.Stop();
            DebugViewViewModel.Dispose();
            SettingsViewViewModel.Dispose();
            SaveAllTabs();
        }
        private void UpdateTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                UpdateMainProperties();
                if (!Hook.Hooked) return;
                if (Hook.Loaded && Hook.Setup)
                {
                    if (!GameLoaded)
                    {
                        GameLoaded = true;
                        Reading = true;
                        ReloadAllViewModels();
                        Reading = false;
                        EnableAllCtrls(true);
                    }
                    else
                    {
                        Reading = true;
                        UpdateProperties();
                        UpdateAllViewModels();
                        Reading = false;
                    }
                }
                else if (GameLoaded)
                {
                    Reading = true;
                    UpdateProperties();
                    ResetAllViewModels();
                    //Hook.UpdateName();
                    EnableAllCtrls(false);
                    GameLoaded = false;
                    Reading = false;
                }
            });
        }

        private void UpdateMainProperties()
        {
            ID = Hook.ID;
            //ViewModel.ParamViewerViewModel.UpdateView();
            //Hook.UpdateMainProperties();
            //ViewModel.UpdateMainProperties();
            //CheckFocused();
            //OnPropertyChanged(nameof(ContentOnline));
            //OnPropertyChanged(nameof(ForegroundOnline));
            //OnPropertyChanged(nameof(GameLoaded));
        }

        private void InitAllViewModels()
        {
            ParamViewViewModel.InitViewModel(Hook);
            PlayerViewViewModel.InitViewModel(Hook);
            ItemGibViewViewModel.InitViewModel(Hook, SettingsViewViewModel);
            InventoryViewViewModel.InitViewModel(Hook, SettingsViewViewModel);
            DebugViewViewModel.InitViewModel(this);
            TargetViewViewModel.InitViewModel(Hook);
            GraceViewViewModel.InitViewModel(Hook);
            HotKeyViewViewModel.InitViewModel(this);
            MiscViewViewModel.InitViewModel(Hook);
        }
        private void UpdateProperties()
        {

        }
        private void EnableAllCtrls(bool enable)
        {
        }
        private void ReloadAllViewModels()
        {
            GraceViewViewModel.ReloadViewModel();
        }
        private void ResetAllViewModels()
        {
            DebugViewViewModel.ResetViewModel();
            HotKeyViewViewModel.ResetViewModel();
        }
        private void UpdateAllViewModels()
        {
            InventoryViewViewModel.UpdateViewModel();
            ItemGibViewViewModel.UpdateViewModel();
            ParamViewViewModel.UpdateViewModel();
            PlayerViewViewModel.UpdateViewModel();
            DebugViewViewModel.UpdateViewModel();
            GraceViewViewModel.UpdateViewModel();
            HotKeyViewViewModel.UpdateViewModel();
            TargetViewViewModel.UpdateViewModel();
            MiscViewViewModel.UpdateViewModel();
            //Hook.UpdateLastEnemy();
        }
        private void SaveAllTabs()
        {
            //SaveHotKeys();
        }


        private void SpawnUndroppable_Checked(object sender, RoutedEventArgs e)
        {
            //DebugItems.UpdateCreateEnabled();
        }

        private string _id = "Not Loaded";
        public string ID
        {
            get => _id;
            set
            {
                if (SetField(ref _id, value))
                {
                    OnPropertyChanged(nameof(ForegroundId));
                }
            }
        }
        public Brush ForegroundId
        {
            get
            {
                if (Hook.ID != "Not Hooked")
                    return Brushes.GreenYellow;
                return Brushes.IndianRed;
            }
        }
        private bool _loaded;
        public bool GameLoaded
        {
            get => _loaded;
            set
            {
                if (SetField(ref _loaded, value))
                {
                    OnPropertyChanged(nameof(ContentLoaded));
                    OnPropertyChanged(nameof(ForegroundLoaded));
                }
            }
        }
        public string ContentLoaded
        {
            get
            {
                if (GameLoaded)
                    return "Yes";
                return "No";
            }
        }
        public Brush ForegroundLoaded
        {
            get
            {
                if (GameLoaded)
                    return Brushes.GreenYellow;
                return Brushes.IndianRed;
            }
        }
    }
}

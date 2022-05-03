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
using Elden_Ring_Debug_Tool_ViewModels.Manager;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    [Description("Main Window")]
    public class MainWindowViewModel : ViewModelBase
    {
        internal static Properties.Settings Settings { get; }

        public ERHook Hook { get; private set; }

        private bool _gameLoaded;

        public bool Reading
        {
            get => ERHook.Reading;
            set => ERHook.Reading = value;
        }

        System.Timers.Timer UpdateTimer = new System.Timers.Timer();
        public ICommand OpenGitHubCommand { get; set; }

        public bool ShowWarning
        {
            get { return Settings.ShowWarning; }
            set { Settings.ShowWarning = value; }
        }

        private ObservableCollection<ViewModelBase> _viewModels;
        public ObservableCollection<ViewModelBase> ViewModels
        {
            get => _viewModels;
            set => SetField(ref _viewModels, value);
        }

        static MainWindowViewModel()
        {
            PortableJsonSettingsProvider.SettingsFileName = "ERDebug.settings.json";
            PortableSettingsProviderBase.SettingsDirectory = Environment.CurrentDirectory;
            PortableJsonSettingsProvider.ApplyProvider(Properties.Settings.Default);
            Settings = Properties.Settings.Default;
        }

        public MainWindowViewModel()
        {
            Hook = new ERHook(5000, 15000, p => p.MainWindowTitle == "ELDEN RING™");
            Hook.OnSetup += Hook_OnSetup;
            Hook.OnUnhooked += Hook_OnUnhooked;
            OpenGitHubCommand = new OpenGitHubCommand(this);
            _uri = new Uri("https://github.com/Nordgaren/Elden-Ring-Debug-Tool");

            _paramViewViewModel = new ParamViewViewModel();
            _itemGibViewModel = new ItemGibViewViewModel();
            _inventoryViewModel = new InventoryViewViewModel();
            _debugViewViewModel = new DebugViewViewModel();
            _hotkeyViewViewModel = new HotkeyViewViewModel();
            _hotkeyManager = new WindowsRegisteredMultiHotkeyManager(Hook);



            _viewModels = new ObservableCollection<ViewModelBase>();
            ViewModels.Add(this);
            ViewModels.Add(ParamViewViewModel);
            ViewModels.Add(ItemGibViewModel);
            ViewModels.Add(InventoryViewModel);
            ViewModels.Add(DebugViewViewModel);
            ViewModels.Add(HotkeyViewViewModel);

            InitAllViewModels();
            Hook.Start();
        }

        public async Task Load()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo? fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string? version = fileVersionInfo.FileVersion;

            WindowTitle = $"Elden Ring Debug Tool {version}";
            EnableAllCtrls(false);


            try
            {
                GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue("Elden-Ring-Debug-Tool"));
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
            UpdateTimer.Interval = 16;
            UpdateTimer.Elapsed += UpdateTimer_Elapsed;
            UpdateTimer.Enabled = true;
        }

        private ParamViewViewModel _paramViewViewModel;
        public ParamViewViewModel ParamViewViewModel
        {
            get => _paramViewViewModel;
            set => SetField(ref _paramViewViewModel, value);
        }

        private ItemGibViewViewModel _itemGibViewModel;
        public ItemGibViewViewModel ItemGibViewModel
        {
            get => _itemGibViewModel;
            set => SetField(ref _itemGibViewModel, value);
        }

        private InventoryViewViewModel _inventoryViewModel;
        public InventoryViewViewModel InventoryViewModel
        {
            get => _inventoryViewModel;
            set => SetField(ref _inventoryViewModel, value);
        }

        private HotkeyViewViewModel _hotkeyViewViewModel;
        public HotkeyViewViewModel HotkeyViewViewModel
        {
            get => _hotkeyViewViewModel;
            set => SetField(ref _hotkeyViewViewModel, value);
        }


        private DebugViewViewModel _debugViewViewModel;
        public DebugViewViewModel DebugViewViewModel
        {
            get => _debugViewViewModel;
            set => SetField(ref _debugViewViewModel, value);
        }

        private IHotkeyManager _hotkeyManager;
        internal IHotkeyManager HotkeyManager
        {
            get => _hotkeyManager;
            set => SetField(ref _hotkeyManager, value);
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
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ID = Hook.ID;
            });
        }
        private void Hook_OnUnhooked(object? sender, PropertyHook.PHEventArgs e)
        {

        }

        public void Dispose()
        {
            if (Hook.CombatMapEnabled)
                Hook.ToggleMapCombat(false);

            UpdateTimer.Stop();
            SaveAllTabs();
            Settings.Save();
        }
        private void UpdateTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {

            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                UpdateMainProperties();
                Hook.Update();
                if (Hook.Hooked)
                {
                    if (Hook.Loaded && Hook.Setup)
                    {
                        if (!GameLoaded)
                        {
                            GameLoaded = true;
                            Reading = true;
                            ReloadAllCtrls();
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
                        ResetAllCtrls();
                        //Hook.UpdateName();
                        EnableAllCtrls(false);
                        GameLoaded = false;
                        Reading = false;
                    }
                }
            }));
        }

        private void UpdateMainProperties()
        {
            ID = Hook.ID;
            GameLoaded = Hook.Loaded;
            //ViewModel.ParamViewerViewModel.UpdateView();
            //Hook.UpdateMainProperties();
            //ViewModel.UpdateMainProperties();
            //CheckFocused();
            //OnPropertyChanged(nameof(ContentOnline));
            //OnPropertyChanged(nameof(ForegroundOnline));
            OnPropertyChanged(nameof(GameLoaded));
        }

        private void InitAllViewModels()
        {
            ParamViewViewModel.InitViewModel(Hook);
            ItemGibViewModel.InitViewModel(Hook);
            InventoryViewModel.InitViewModel(Hook);
            DebugViewViewModel.InitViewModel(this);
            HotkeyViewViewModel.InitViewModel(this);
        }
        private void UpdateProperties()
        {

        }
        private void EnableAllCtrls(bool enable)
        {
        }
        private void ReloadAllCtrls()
        {
        }
        private void ResetAllCtrls()
        {
        }
        private void UpdateAllViewModels()
        {
            HotkeyManager.Update();
            InventoryViewModel.UpdateViewModel();
            ItemGibViewModel.UpdateViewModel();
            ParamViewViewModel.UpdateViewModel();
            DebugViewViewModel.UpdateViewModel();
            Hook.UpdateLastEnemy();
        }
        private void SaveAllTabs()
        {
            //SaveHotkeys();
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
                    OnPropertyChanged(nameof(ForegroundID));
                }
            }
        }
        public Brush ForegroundID
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

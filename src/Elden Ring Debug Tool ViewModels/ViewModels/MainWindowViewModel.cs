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

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    [Description("Main Window")]
    public class MainWindowViewModel : ViewModelBase
    {
        internal static Properties.Settings? Settings;

        public ERHook Hook { get; private set; }

        public bool GameLoaded { get; set; }
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

        public MainWindowViewModel()
        {

            PortableJsonSettingsProvider.SettingsFileName = "ERDebug.settings.json";
            PortableJsonSettingsProvider.SettingsDirectory = Environment.CurrentDirectory;
            PortableJsonSettingsProvider.ApplyProvider(Properties.Settings.Default);
            Settings = Properties.Settings.Default;

            Hook = new ERHook(5000, 15000, p => p.MainWindowTitle == "ELDEN RING™");
            Hook.OnSetup += Hook_OnSetup;
            Hook.OnUnhooked += Hook_OnUnhooked;
            OpenGitHubCommand = new OpenGitHubCommand(this);
            Uri = new Uri("https://github.com/Nordgaren/Elden-Ring-Debug-Tool");

            ParamViewViewModel = new ParamViewViewModel();
            ItemGibViewModel = new ItemGibViewViewModel();
            InventoryViewModel = new InventoryViewViewModel();
            DebugViewViewModel = new DebugViewViewModel();
            HotkeyViewViewModel = new HotkeyViewViewModel();



            ViewModels = new ObservableCollection<ViewModelBase>();
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

        private ParamViewViewModel _paramViewerViewModel;
        public ParamViewViewModel ParamViewViewModel
        {
            get => _paramViewerViewModel;
            set => SetField(ref _paramViewerViewModel, value);
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

        private Uri _uri;
        public Uri Uri
        {
            get => _uri;
            set => SetField(ref _uri, value);
        }

        private string _updateInfo;
        public string UpdateInfo
        {
            get => _updateInfo;
            set => SetField(ref _updateInfo, value);
        }

        private bool _updateRequired;
        public bool UpdateRequired
        {
            get => _updateRequired;
            set => SetField(ref _updateRequired, value);
        }

        private string _windowTitle;
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
                Hook.EnableMapCombat(false);

            UpdateTimer.Stop();
            SaveAllTabs();
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
            //ViewModel.ParamViewerViewModel.UpdateView();
            //Hook.UpdateMainProperties();
            //ViewModel.UpdateMainProperties();
            //CheckFocused();
            OnPropertyChanged(nameof(ForegroundID));
            OnPropertyChanged(nameof(ContentLoaded));
            OnPropertyChanged(nameof(ForegroundLoaded));
            //OnPropertyChanged(nameof(ContentOnline));
            //OnPropertyChanged(nameof(ForegroundOnline));
            OnPropertyChanged(nameof(ForegroundVersion));
            OnPropertyChanged(nameof(GameLoaded));
        }

        private void InitAllViewModels()
        {
            ParamViewViewModel.InitViewModel(Hook);
            ItemGibViewModel.InitViewModel(Hook);
            InventoryViewModel.InitViewModel(Hook);
            HotkeyViewViewModel.InitViewModel(this);
            DebugViewViewModel.InitViewModel(Hook);
        }
        private void UpdateProperties()
        {

        }
        private void EnableAllCtrls(bool enable)
        {
            //DebugItems.EnableCtrls(enable);
        }
        private void ReloadAllCtrls()
        {
            //DebugItems.ReloadCtrl();
        }
        private void ResetAllCtrls()
        {
            //DebugItems.ResetCtrl();
        }
        private void UpdateAllViewModels()
        {
            //DebugItems.UpdateCtrl();
            InventoryViewModel.UpdateViewModel();
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

        private string _id;
        public string ID
        {
            get => _id;
            set => SetField(ref _id, value);
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
        public string ContentLoaded
        {
            get
            {
                if (Hook.Loaded)
                    return "Yes";
                return "No";
            }
        }
        public Brush ForegroundLoaded
        {
            get
            {
                if (Hook.Loaded)
                    return Brushes.GreenYellow;
                return Brushes.IndianRed;
            }
        }

        public Brush ForegroundVersion
        {
            get
            {
                if (!Hook.Hooked)
                    return Brushes.Black;

                if (Hook.Is64Bit)
                    return Brushes.GreenYellow;
                return Brushes.IndianRed;
            }
        }
    }
}

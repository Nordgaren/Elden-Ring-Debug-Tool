using Elden_Ring_Debug_Tool;
using System.Windows;
using System.Windows.Media;
using System.Timers;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Windows.Input;
using Elden_Ring_Debug_Tool_ViewModels.Commands;
using Octokit;
using System.Reflection;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        internal ERHook Hook { get; private set; }

        public bool GameLoaded { get; set; }
        public bool Reading
        {
            get => ERHook.Reading;
            set => ERHook.Reading = value;
        }

        System.Timers.Timer UpdateTimer = new System.Timers.Timer();
        public ICommand OpenGitHubCommand { get; set; }

        public MainWindowViewModel()
        {
            Hook = new ERHook(5000, 15000, p => p.MainWindowTitle == "ELDEN RING™");
            Hook.OnSetup += Hook_OnSetup;
            Hook.OnUnhooked += Hook_OnUnhooked;
            OpenGitHubCommand = new OpenGitHubCommand(this);
            ParamViewerViewModel = new ParamViewerViewModel();
            ParamViewerViewModel.SetHook(Hook);
            Uri = new Uri("https://github.com/Nordgaren/Elden-Ring-Debug-Tool");
            Hook.Start();
        }

        public async Task Load()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.FileVersion;

            WindowTitle = $"Elden Ring Debug Tool {version}";
            EnableAllCtrls(false);
            InitAllCtrls();

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

        private ParamViewerViewModel _paramViewerViewModel;
        public ParamViewerViewModel ParamViewerViewModel
        {
            get
            {
                return _paramViewerViewModel;
            }
            set
            {
                _paramViewerViewModel = value;
                OnPropertyChanged();
            }
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
                ParamViewerViewModel.AddParams();
            });
        }
        private void Hook_OnUnhooked(object? sender, PropertyHook.PHEventArgs e)
        {
            ParamViewerViewModel.UnHook();
        }

        public void Dispose()
        {
            if (Hook.EnableMapCombat)
                Hook.EnableMapCombat = false;

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
                            UpdateAllCtrl();
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

        private void InitAllCtrls()
        {
            //DebugItems.InitCtrl();
            //DebugCheats.InitCtrl();
            //InitHotkeys();
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
        private void UpdateAllCtrl()
        {
            //DebugItems.UpdateCtrl();
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

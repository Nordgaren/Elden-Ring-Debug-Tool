using Elden_Ring_Debug_Tool;
using System.Windows;
using System.Windows.Media;
using System.Timers;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ERHook Hook { get; private set; }

        public bool GameLoaded { get; set; }
        public bool Reading
        {
            get => ERHook.Reading;
            set => ERHook.Reading = value;
        }

        System.Timers.Timer UpdateTimer = new System.Timers.Timer();

        public MainWindowViewModel()
        {
            Hook = new ERHook(5000, 15000, p => p.MainWindowTitle == "ELDEN RING™");
            Hook.OnSetup += Hook_OnSetup;
            ParamViewerViewModel = new ParamViewerViewModel();
            ParamViewerViewModel.SetHook(Hook);
            Hook.Start();
        }

        public void Load()
        {
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

        private void Hook_OnSetup(object? sender, PropertyHook.PHEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Hook.EnableMapCombat)
                    Hook.EnableMapCombat = false;

                UpdateTimer.Stop();
                SaveAllTabs();

                ParamViewerViewModel.AddParams();
            });
        }

        public void Dispose()
        {

        }
        private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

            Application.Current.Dispatcher.Invoke(new Action(() =>
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
                        FormLoaded = false;
                        Reading = false;
                    }
                }
            }));
        }

        private void UpdateMainProperties()
        {
            ViewModel.ParamViewerViewModel.UpdateView();
            //Hook.UpdateMainProperties();
            ViewModel.UpdateMainProperties();
            CheckFocused();
        }

        private void InitAllCtrls()
        {
            DebugItems.InitCtrl();
            DebugCheats.InitCtrl();
            InitHotkeys();
        }
        private void UpdateProperties()
        {

        }
        private void EnableAllCtrls(bool enable)
        {
            DebugItems.EnableCtrls(enable);
        }
        private void ReloadAllCtrls()
        {
            DebugItems.ReloadCtrl();
        }
        private void ResetAllCtrls()
        {
            DebugItems.ResetCtrl();
        }
        private void UpdateAllCtrl()
        {
            DebugItems.UpdateCtrl();
            Hook.UpdateLastEnemy();
        }
        private void SaveAllTabs()
        {
            SaveHotkeys();
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

        public void UpdateMainProperties()
        {
            OnPropertyChanged(nameof(ForegroundID));
            OnPropertyChanged(nameof(ContentLoaded));
            OnPropertyChanged(nameof(ForegroundLoaded));
            //OnPropertyChanged(nameof(ContentOnline));
            //OnPropertyChanged(nameof(ForegroundOnline));
            OnPropertyChanged(nameof(ForegroundVersion));
            OnPropertyChanged(nameof(GameLoaded));
        }

    }
}

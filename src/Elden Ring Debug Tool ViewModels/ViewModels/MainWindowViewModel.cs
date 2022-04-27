using Elden_Ring_Debug_Tool;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

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

        public MainWindowViewModel()
        {
            Hook = new ERHook(5000, 15000, p => p.MainWindowTitle == "ELDEN RING™");
            Hook.OnSetup += Hook_OnSetup;
            ParamViewerViewModel = new ParamViewerViewModel();
            ParamViewerViewModel.SetHook(Hook);
            Hook.Start();
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
                ParamViewerViewModel.AddParams();
            });
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

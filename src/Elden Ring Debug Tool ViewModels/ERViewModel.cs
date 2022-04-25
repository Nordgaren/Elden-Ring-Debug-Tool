using Elden_Ring_Debug_Tool;
using System.Windows.Media;

namespace Elden_Ring_Debug_Tool_ViewModels
{
    public class ERViewModel : ObservableObject
    {
        public ERHook Hook { get; private set; }

        public bool GameLoaded { get; set; }
        public bool Reading
        {
            get => ERHook.Reading;
            set => ERHook.Reading = value;
        }

        public ERViewModel()
        {
            Hook = new ERHook(5000, 15000, p => p.MainWindowTitle == "ELDEN RING™");
            Hook.Start();
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
        //public string ContentOnline
        //{
        //    get
        //    {
        //        if (!Hook.Hooked)
        //            return null;

        //        if (Hook.Online)
        //            return "Yes";
        //        return "No";
        //    }
        //}
        //public Brush ForegroundOnline
        //{
        //    get
        //    {
        //        if (!Hook.Hooked)
        //            return null;

        //        if (Hook.Online)
        //            return Brushes.GreenYellow;
        //        return Brushes.IndianRed;
        //    }
        //}

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

using Erd_Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Erd_Tools.ERHook;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    public class DebugViewViewModel : ViewModelBase
    {
        internal ERHook Hook { get; set; }

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
    }
}

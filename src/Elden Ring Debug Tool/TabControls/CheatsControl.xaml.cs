using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Elden_Ring_Debug_Tool.ERHook;

namespace Elden_Ring_Debug_Tool
{
    /// <summary>
    /// Interaction logic for Cheats.xaml
    /// </summary>
    public partial class CheatsControl : DebugControl
    {
        public CheatsControl()
        {
            InitializeComponent();
        }

        public override void InitCtrl()
        {
            WeatherTypes = new ObservableCollection<WeatherTypes>(Enum.GetValues(typeof(WeatherTypes)).Cast<WeatherTypes>());
        }


        internal override void UpdateCtrl()
        {
            if (Hook.ForceWeather)
                Hook.ForceSetWeather();
        }

        public ObservableCollection<WeatherTypes> WeatherTypes
        {
            get { return (ObservableCollection<WeatherTypes>)GetValue(WeatherTypesProperty); }
            set { SetValue(WeatherTypesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WeatherTypes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WeatherTypesProperty =
            DependencyProperty.Register("WeatherTypes", typeof(ObservableCollection<WeatherTypes>), typeof(CheatsControl), new PropertyMetadata(default(ObservableCollection<WeatherTypes>)));


        public ObservableCollection<WeatherTypes> WeatherList { get; set; }
    }
}

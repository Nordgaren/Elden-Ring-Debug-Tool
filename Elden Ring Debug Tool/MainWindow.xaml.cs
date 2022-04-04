using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Elden_Ring_Debug_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        ERHook Hook => ViewModel.Hook;

        Timer UpdateTimer = new Timer();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTimer.Interval = 16;
            UpdateTimer.Elapsed += UpdateTimer_Elapsed;
            UpdateTimer.Enabled = true;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UpdateTimer.Stop();
        }
        private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Hook.Update();
            }));
        }
    }
}

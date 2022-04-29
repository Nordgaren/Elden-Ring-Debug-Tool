using System.Windows;
using System.Windows.Input;

namespace Elden_Ring_Debug_Tool_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();

            if (!MainWindowViewModel.ShowWarning)
                return;

            DebugWarning warning = new DebugWarning(MainWindowViewModel)
            {
                Title = "Online Warning",
                Width = 350,
                Height = 240
            };
            warning.ShowDialog();
        }

        private void InitAllCtrls()
        {
            //DebugItems.InitCtrl();
            //DebugCheats.InitCtrl();
            InitHotkeys();
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await MainWindowViewModel.Load();
            InitAllCtrls();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindowViewModel.Dispose();
            App.Settings?.Save();
        }
        private void SpawnUndroppable_Checked(object sender, RoutedEventArgs e)
        {
            //DebugItems.UpdateCreateEnabled();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MainWindowClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }
    }
}

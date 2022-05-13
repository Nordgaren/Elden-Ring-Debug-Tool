using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using AvalonDock.Themes;
using Elden_Ring_Debug_Tool_WPF.Windows;

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

            DebugWarning warning = new(MainWindowViewModel)
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
            //InitHotKeys();
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //await MainWindowViewModel.Load();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //App.Settings?.Save();
        }
        private void SpawnUndroppable_Checked(object sender, RoutedEventArgs e)
        {
            //DebugItems.UpdateCreateEnabled();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                    double percentHorizontal = e.GetPosition(this).X / ActualWidth;
                    double targetHorizontal = RestoreBounds.Width * percentHorizontal;

                    double percentVertical = e.GetPosition(this).Y / ActualHeight;
                    double targetVertical = RestoreBounds.Height * percentVertical;

                    POINT lMousePosition;
                    GetCursorPos(out lMousePosition);
                    Left = lMousePosition.X - targetHorizontal;
                    Top = lMousePosition.Y - targetVertical;

                }
                DragMove();
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);


        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
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
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
    }
}

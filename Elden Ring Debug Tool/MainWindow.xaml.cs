using PropertyHook;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xceed.Wpf.Toolkit;
using System.Xml;
using System.Xml.Serialization;

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
            ERItemCategory.GetItemCategories();
            Hook.OnSetup += Hook_OnSetup;
        }

        private void Hook_OnSetup(object? sender, PropertyHook.PHEventArgs e)
        {
            Dispatcher.Invoke(() => 
            {
                DebugParam.HookParams();
            });
        }

        ERHook Hook => ViewModel.Hook;

        Timer UpdateTimer = new Timer();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitAllCtrls();
            UpdateTimer.Interval = 16;
            UpdateTimer.Elapsed += UpdateTimer_Elapsed;
            UpdateTimer.Enabled = true;
        }

        bool FormLoaded
        {
            get => ViewModel.GameLoaded;
            set => ViewModel.GameLoaded = value;
        }
        public bool Reading
        {
            get => ViewModel.Reading;
            set => ViewModel.Reading = value;
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UpdateTimer.Stop();
        }
        private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (Hook.Hooked)
                {
                    if (Hook.Loaded && Hook.Setup)
                    {
                        if (!FormLoaded)
                        {
                            FormLoaded = true;
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
                    else if (FormLoaded)
                    {
                        Reading = true;
                        UpdateProperties();
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
            //Hook.UpdateMainProperties();
            ViewModel.UpdateMainProperties();
            //CheckFocused();
        }

        private void InitAllCtrls()
        {
            DebugItems.InitCtrl();
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
        private void UpdateAllCtrl()
        {
            DebugItems.UpdateCtrl();
            Hook.Update();
        }

        private void link_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }

        //private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.ChangedButton == MouseButton.Left)
        //        DragMove();
        //}

        //private void Minimize_Click(object sender, RoutedEventArgs e)
        //{
        //    WindowState = System.Windows.WindowState.Minimized;
        //}

        //private void MainWindowClose_Click(object sender, RoutedEventArgs e)
        //{
        //    Close();
        //}

        //private void Maximize_Click(object sender, RoutedEventArgs e)
        //{
        //    if (WindowState == System.Windows.WindowState.Maximized)
        //        WindowState = System.Windows.WindowState.Normal;
        //    else
        //        WindowState = System.Windows.WindowState.Maximized;
        //}
    }
}

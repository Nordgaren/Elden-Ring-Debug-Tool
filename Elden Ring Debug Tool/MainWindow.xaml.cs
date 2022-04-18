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
using Bluegrams.Application;
using Octokit;
using System.Net.Http;

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

            if (App.Settings.ShowWarning)
            {
                var warning = new DebugWarning()
                {
                    Title = "Online Warning",
                    Width = 350,
                    Height = 240
                };
                warning.ShowDialog();
            }
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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fileVersionInfo.FileVersion;

            lblWindowName.Content = $"Elden Ring Debug Tool {version}";
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
                    link.NavigateUri = new Uri(release.HtmlUrl);
                    llbNewVersion.Visibility = Visibility.Visible;
                    labelCheckVersion.Visibility = Visibility.Hidden;
                }
                else if (gitVersion == exeVersion)
                {
                    labelCheckVersion.Content = "App up to date";
                }
                else
                {
                    labelCheckVersion.Content = "App version unreleased. Be wary of bugs!";
                }
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is ApiException || ex is ArgumentException)
            {
                labelCheckVersion.Content = "Current app version unknown";
            }
            catch (Exception ex)
            {
                labelCheckVersion.Content = "Something is very broke, contact Elden Ring Debug Tool repo owner";
                System.Windows.MessageBox.Show(ex.Message);
            }
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
            if (Hook.EnableMapCombat)
                Hook.EnableMapCombat = false;

            UpdateTimer.Stop();
            SaveAllTabs();

            App.Settings?.Save();
        }
        private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

            Dispatcher.Invoke(new Action(() =>
            {
                UpdateMainProperties();
                Hook.Update();
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
            //Hook.UpdateMainProperties();
            ViewModel.UpdateMainProperties();
            CheckFocused();
        }

        private void InitAllCtrls()
        {
            DebugItems.InitCtrl();
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
        }
        private void SaveAllTabs()
        {
            SaveHotkeys();
        }

        private void link_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.ToString(),
                UseShellExecute = true
            });
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
        }

        private void MainWindowClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Maximized)
                WindowState = System.Windows.WindowState.Normal;
            else
                WindowState = System.Windows.WindowState.Maximized;
        }
        private void SpawnUndroppable_Checked(object sender, RoutedEventArgs e)
        {
            DebugItems.UpdateCreateEnabled();
        }
    }
}

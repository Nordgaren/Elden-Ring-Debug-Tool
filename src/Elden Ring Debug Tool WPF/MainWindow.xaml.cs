using System;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Reflection;
using Octokit;
using System.Net.Http;
using Elden_Ring_Debug_Tool;

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

            if (App.Settings.ShowWarning)
            {
                DebugWarning warning = new DebugWarning()
                {
                    Title = "Online Warning",
                    Width = 350,
                    Height = 240
                };
                warning.ShowDialog();
            }
        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.FileVersion;

            lblWindowName.Content = $"Elden Ring Debug Tool {version}";

            ViewModel.Load();
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
            ViewModel.Dispose();
            App.Settings?.Save();
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

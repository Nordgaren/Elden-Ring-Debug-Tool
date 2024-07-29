using System;
using System.IO;
using System.Collections.Generic;
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
using Elden_Ring_Debug_Tool_ViewModels.Commands;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels;

namespace Elden_Ring_Debug_Tool_WPF.Views
{
    /// <summary>
    /// Interaction logic for MiscView.xaml
    /// </summary>
    public partial class MiscView : UserControl
    {
        public MiscView()
        {
            InitializeComponent();
        }

        private MiscViewViewModel _itemGibViewModel;

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is MiscViewViewModel vm)
            {
                _itemGibViewModel = vm;
            }
        }

        private void CheckAllEventFlags(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    var eventFlags = File.ReadAllLines(filePath);
                    var resultsListBox = FindName("EventFlagResultsListBox") as ListBox;
                    resultsListBox?.Items.Clear();

                    foreach (var line in eventFlags)
                    {
                        var parts = line.Split(new[] { ' ' }, 2);  // Split the line into two parts at the first space
                        if (parts.Length == 2 && int.TryParse(parts[0], out int eventFlag))
                        {
                            _itemGibViewModel.EventFlag = eventFlag;
                            (_itemGibViewModel.CheckEventFlag as IsEventCommand)?.Execute(null);
                            resultsListBox?.Items.Add($"{_itemGibViewModel.IsEventFlag}: {parts[0]} - {parts[1]}");
                        }
                        else
                        {
                            MessageBox.Show($"Invalid line format or event flag: {line}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show($"{System.IO.Path.GetFileName(filePath)} not found in the Resources folder.");
            }
        }

        private void CheckAllMimicTears_Click(object sender, RoutedEventArgs e)
        {
            CheckAllEventFlags(@"Resources\MimicTearsEventFlags.txt");
        }

        private void CheckAllLostAshes_Click(object sender, RoutedEventArgs e)
        {
            CheckAllEventFlags(@"Resources\LostAshesEventFlags.txt");
        }

        private void CheckAllAncientStones_Click(object sender, RoutedEventArgs e)
        {
            CheckAllEventFlags(@"Resources\LostAncientStonesEventFlags.txt");
        }

        private void CheckAllSomberAncientStones_Click(object sender, RoutedEventArgs e)
        {
            CheckAllEventFlags(@"Resources\LostAncientSomberStonesEventFlags.txt");
        }

        private void CheckAllGreatGraveGloves_Click(object sender, RoutedEventArgs e)
        {
            CheckAllEventFlags(@"Resources\LostGreatGraveGlovewortEventFlags.txt");
        }

        private void CheckAllGreatGhostGloves_Click(object sender, RoutedEventArgs e)
        {
            CheckAllEventFlags(@"Resources\LostGreatGhostGlovewortEventFlags.txt");
        }
    }
}

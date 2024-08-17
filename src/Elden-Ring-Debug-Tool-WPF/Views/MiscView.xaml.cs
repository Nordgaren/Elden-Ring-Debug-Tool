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
using System.Xml.Linq;
using System.Collections.ObjectModel;

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

        private void CheckAllEventFlags(string filePath, string itemTypeFilter)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    XDocument doc = XDocument.Load(filePath);
                    XNamespace ns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

                    var items = doc.Descendants(ns + "Item");
                    var resultsListBox = FindName("EventFlagResultsListBox") as ListBox;
                    if (resultsListBox != null) resultsListBox.Items.Clear();

                    foreach (var item in items)
                    {
                        var eventID = item.Element(ns + "EventID")?.Value;
                        var description = item.Element(ns + "Description")?.Value;
                        var url = item.Element(ns + "URL")?.Value;
                        var itemType = item.Element(ns + "ItemType")?.Value;
                        int eventIDint = Int32.Parse(eventID);
                        bool checkFlag = _itemGibViewModel.Hook.IsEventFlag(eventIDint);

                        if (int.TryParse(eventID, out int eventFlag) && itemType == itemTypeFilter)
                        {
                            if (resultsListBox != null)
                                resultsListBox.Items.Add(new EventFlagItem
                                {
                                    EventID = eventFlag,
                                    DisplayText = $"{checkFlag} - {eventID} - {description}",
                                    URL = url,
                                    IsEventFlag = _itemGibViewModel.Hook.IsEventFlag(eventIDint)
                        });
                            Console.WriteLine("Added: " + description); // Or use Debug.WriteLine if in a WPF application
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
                MessageBox.Show($"{System.IO.Path.GetFileName(filePath)} not found.");
            }
        }
        private void CheckAllLarvalTears_Click(object sender, RoutedEventArgs e)
        {
            CheckAllEventFlags(@"Resources\Items.xml", "Larval Tear");
        }

        private void CheckAllLostAshes_Click(object sender, RoutedEventArgs e)
        {
            CheckAllEventFlags(@"Resources\Items.xml", "Lost Ashes of War");
        }

        private void CheckAllAncientStones_Click(object sender, RoutedEventArgs e)
        {
            CheckAllEventFlags(@"Resources\Items.xml","Ancient Dragon Smithing Stone");
        }

        private void CheckAllSomberAncientStones_Click(object sender, RoutedEventArgs e)
        {
            CheckAllEventFlags(@"Resources\Items.xml", "Somber Ancient Dragon Smithing Stone");
        }

        private void CheckAllGreatGraveGloves_Click(object sender, RoutedEventArgs e)
        {
            CheckAllEventFlags(@"Resources\Items.xml", "Great Grave Glovewort");
        }

        private void CheckAllGreatGhostGloves_Click(object sender, RoutedEventArgs e)
        {
            CheckAllEventFlags(@"Resources\Items.xml", "Great Ghost Glovewort");
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });
            e.Handled = true;
        }
    }
}

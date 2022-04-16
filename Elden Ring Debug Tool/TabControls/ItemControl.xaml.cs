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
using Infusion = Elden_Ring_Debug_Tool.ERWeapon.Infusion;

namespace Elden_Ring_Debug_Tool
{
    /// <summary>
    /// Interaction logic for ItemControl.xaml
    /// </summary>
    public partial class ItemControl : DebugControl
    {
        public ItemControl()
        {
            InitializeComponent();
        }

        public override void InitCtrl()
        {
            cmbCategory.ItemsSource = ERItemCategory.All;
            cmbCategory.SelectedIndex = 0;
            FilterItems();
            InventoryTimer.Interval = 100;
            InventoryTimer.Elapsed += InventoryTimer_Elapsed;
        }
        internal override void UpdateCtrl() 
        {
            DataGridInventory.ItemsSource = Hook.GetInventory();
        }

        internal override void ReloadCtrl()
        {
            lbxItems.SelectedIndex = -1;
            lbxItems.SelectedIndex = 0;
        }

        internal override void ResetCtrl()
        {
            Hook.ResetInventory();
            DataGridInventory.ItemsSource = new List<ERInventoryEntry>();
        }

        private void InventoryTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                //if (Properties.Settings.Default.UpdateMaxLive)
                //    HandleMaxAvailable();
            }));
        }

        private void HandleMaxAvailable()
        {
            ERItem item = lbxItems.SelectedItem as ERItem;
            if (item == null)
                return;

            nudQuantity.Maximum = cbxQuantityRestrict.IsChecked.Value ? item.MaxQuantity : int.MaxValue;

        }

        internal override void EnableCtrls(bool enable)
        {
            InventoryTimer.Enabled = enable;
            btnCreate.IsEnabled = enable;

            if (enable)
                UpdateCreateEnabled();
        }

        Timer InventoryTimer = new Timer();
        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterItems();
        }

        //Clear items and add the ones that match text in search box
        private void FilterItems()
        {
            lbxItems.Items.Clear();

            if (SearchAllCheckbox.IsChecked.Value && txtSearch.Text != "")
            {
                //search every item category
                foreach (ERItemCategory category in cmbCategory.Items)
                {
                    foreach (ERItem item in category.Items)
                    {
                        if (item.ToString().ToLower().Contains(txtSearch.Text.ToLower()))
                            lbxItems.Items.Add(item);
                    }
                }
            }
            else
            {
                //only search selected item category
                ERItemCategory category = cmbCategory.SelectedItem as ERItemCategory;
                foreach (ERItem item in category.Items)
                {
                    if (item.ToString().ToLower().Contains(txtSearch.Text.ToLower()))
                        lbxItems.Items.Add(item);
                }
            }

            if (lbxItems.Items.Count > 0)
                lbxItems.SelectedIndex = 0;

            HandleSearchLabel();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterItems();
        }

        //Handles the "Searching..." label on the text box
        private void HandleSearchLabel()
        {
            if (txtSearch.Text == "")
                lblSearch.Visibility = Visibility.Visible;
            else
                lblSearch.Visibility = Visibility.Hidden;

        }

        private void cbxQuantityRestrict_Checked(object sender, RoutedEventArgs e)
        {
            if (lbxItems == null)
                return;

            ERItem item = lbxItems.SelectedItem as ERItem;
            if (item == null)
                return;

            if (!cbxQuantityRestrict.IsChecked.Value)
            {
                nudQuantity.IsEnabled = true;
                nudQuantity.Maximum = int.MaxValue;
            }
            else if (lbxItems.SelectedIndex != -1)
            {
                nudQuantity.Maximum = item.MaxQuantity;
            }
        }

        private void cmbInfusion_SelectedIndexChanged(object sender, EventArgs e)
        {
             //var infusion = cmbInfusion.SelectedItem as DS2SInfusion;
            //Checks if cbxMaxUpgrade is checked and sets the value to max value
            HandleMaxItemCheckbox();

        }

        private void lbxItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!Hook.Setup) return;

            ERItem item = lbxItems.SelectedItem as ERItem;
            if (item == null)
                return;

            if (!cbxQuantityRestrict.IsChecked.Value)
                nudQuantity.Maximum = int.MaxValue;
            else
                nudQuantity.Maximum = item.MaxQuantity;

            //{
            //    txtMaxHeld.Visibility = nudQuantity.Maximum > 0 ? Visibility.Hidden : Visibility.Visible;

            //    //var max = Hook.GetMaxQuantity(item);
            //    //var held = Hook.GetHeld(item);
            //    //nudQuantity.IsEnabled = true;
            //    //txtMaxHeld.Visibility = max - held > 0 ? Visibility.Hidden : Visibility.Visible;
            //}

            nudUpgrade.Maximum = 0;

            cmbInfusion.Items.Clear();

            cmbGems.Items.Clear();
            cmbGems.Items.Add(new ERGem("-1 None", ERItem.Category.Gem));

            if (item.ItemCategory == ERItem.Category.Weapons)
            {
                var weapon = item as ERWeapon;
                if (weapon.Infisible)
                    foreach (var infusion in weapon.DefaultGem.Infusions)
                        cmbInfusion.Items.Add(infusion);


                if (!weapon.Unique)
                    foreach (var gem in ERGem.All)
                        if (gem.WeaponTypes.Contains(weapon.Type))
                            cmbGems.Items.Add(gem);


                nudUpgrade.Maximum = weapon.Unique ? 10 : 25;
            }
          
            if (cmbInfusion.Items.Count == 0)
                cmbInfusion.Items.Add(Infusion.Standard);

            nudUpgrade.IsEnabled = nudUpgrade.Maximum > 0;

            cmbInfusion.SelectedIndex = 0;
            cmbInfusion.IsEnabled = cmbInfusion.Items.Count > 1;

            cmbGems.SelectedIndex = 0;
            cmbGems.IsEnabled = cmbGems.Items.Count > 1;

            btnCreate.IsEnabled = item.CanAquireFromOtherPlayers || cbxLimit.IsChecked.Value;
            //if (!Properties.Settings.Default.UpdateMaxLive)
            //    HandleMaxAvailable();
            HandleMaxItemCheckbox();
        }

        public void UpdateCreateEnabled()
        {
            ERItem item = lbxItems.SelectedItem as ERItem;
            if (item == null)
                return;

            btnCreate.IsEnabled = item.CanAquireFromOtherPlayers || cbxLimit.IsChecked.Value;//Properties.Settings.Default.SpawnUndroppable;
        }

        internal void EnableStats(bool enable)
        {
            ERItem item = lbxItems.SelectedItem as ERItem;
            var canTrade = false;
            if (item != null)
                canTrade = item.CanAquireFromOtherPlayers;

            btnCreate.IsEnabled = enable && canTrade;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            CreateItem();
        }

        //Apply hair to currently loaded character
        public void CreateItem()
        {
            //if (!Properties.Settings.Default.UpdateMaxLive)
            //    HandleMaxAvailable();
            //Check if the button is enabled and the selected item isn't null
            if (btnCreate.IsEnabled && lbxItems.SelectedItem != null)
            {
                _ = ChangeColor(Brushes.DarkGray);
                ERItem item = lbxItems.SelectedItem as ERItem;
                if (item == null)
                    return;

                var id = item.ID;

                var infusion = (Infusion)cmbInfusion.SelectedItem;

                id += (int)item.ItemCategory;

                var gem = cmbGems.SelectedItem as ERGem;

                Hook.GetItem(id, (int)nudQuantity.Value, (int)infusion, (int)nudUpgrade.Value, gem.ID);
                //if (!Properties.Settings.Default.UpdateMaxLive)
                //    HandleMaxAvailable();
            }
        }

        //handles up and down scrolling
        private void ScrollListbox(KeyEventArgs e)
        {
            //Scroll down through Items listbox and go back to bottom at end
            if (e.Key == Key.Up)
            {
                e.Handled = true;//Do not pass keypress along

                //One liner meme that does the exact same thing as the code above
                lbxItems.SelectedIndex = ((lbxItems.SelectedIndex - 1) + lbxItems.Items.Count) % lbxItems.Items.Count;
                lbxItems.ScrollIntoView(lbxItems.SelectedItem);
                return;
            }

            //Scroll down through Items listbox and go back to top at end
            if (e.Key == Key.Down)
            {
                e.Handled = true;//Do not pass keypress along

                //One liner meme that does the exact same thing as the code above
                lbxItems.SelectedIndex = (lbxItems.SelectedIndex + 1) % lbxItems.Items.Count;
                lbxItems.ScrollIntoView(lbxItems.SelectedItem);
                return;
            }
        }

        //Changes the color of the Apply button
        private async Task ChangeColor(Brush new_color)
        {
            btnCreate.Background = new_color;

            await Task.Delay(TimeSpan.FromSeconds(.25));

            btnCreate.Background = default(Brush);
        }

        //handles escape
        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                txtSearch.Clear();
                return;
            }

            //Create selected index as item
            if (e.Key == Key.Enter)
            {
                e.Handled = true; //Do not pass keypress along
                CreateItem();
                return;
            }

            //Return if sender is cmbInfusion so that arrow Key are handled correctly
            if (sender == cmbInfusion)
                return;
            //Prevents up and down Key from moving the cursor left and right when nothing in item box
            if (lbxItems.Items.Count == 0)
            {
                if (e.Key == Key.Up)
                    e.Handled = true; //Do not pass keypress along
                if (e.Key == Key.Down)
                    e.Handled = true; //Do not pass keypress along
                return;
            }

            ScrollListbox(e);
        }

        //Select number in nud
        private void nudUpgrade_Click(object sender, EventArgs e)
        {
            nudUpgrade.Focus();
        }

        private void SearchAllCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            //checkbox changed, refresh search filter (if txtSearch is not empty)
            if (txtSearch.Text != "")
                FilterItems();
        }

        private void cbxMaxUpgrade_Checked(object sender, RoutedEventArgs e)
        {
            HandleMaxItemCheckbox();
            if (cbxMax.IsChecked.Value)
            {
                nudUpgrade.Value = nudUpgrade.Maximum;
                nudQuantity.Value = nudQuantity.Maximum;
            }
            else
            {
                nudUpgrade.Value = nudUpgrade.Minimum;
                nudQuantity.Value = nudQuantity.Minimum;
            }
        }

        private void HandleMaxItemCheckbox()
        {
            //Set upgrade nud to max if max checkbox is ticked
            if (cbxMax.IsChecked.Value)
            {
                nudUpgrade.Value = nudUpgrade.Maximum;
                nudQuantity.Value = nudQuantity.Maximum;
            }
        }

        private void cmbInfusion_KeyDown(object sender, KeyEventArgs e)
        {
            //Create selected index as item
            if (e.Key == Key.Enter)
            {
                e.Handled = true; //Do not pass keypress along
                CreateItem();
                return;
            }
        }
        //Select all text in search box
        private void txtSearch_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            txtSearch.SelectAll();
            txtSearch.Focus();
            e.Handled = true;
        }

        private void SearchAllCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text != "")
                FilterItems();
        }
    }
}

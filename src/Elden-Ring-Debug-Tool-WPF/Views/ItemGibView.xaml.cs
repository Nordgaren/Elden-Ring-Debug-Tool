using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Elden_Ring_Debug_Tool_ViewModels.ViewModels;
using Erd_Tools;

namespace Elden_Ring_Debug_Tool_WPF.Views
{
    /// <summary>
    /// Interaction logic for ItemControl.xaml
    /// </summary>
    public partial class ItemGibView : UserControl
    {
        public ItemGibView()
        {
            InitializeComponent();
            DataContextChanged += new DependencyPropertyChangedEventHandler(OnDataContextChanged);

        }
        private ItemGibViewViewModel _itemGibViewModel;

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is ItemGibViewViewModel vm)
            {
                _itemGibViewModel = vm;
            }
        }

        ////public override void InitCtrl()
        ////{
        ////    //cmbCategory.ItemsSource = ItemCategory.All;
        ////    //cmbCategory.SelectedIndex = 0;
        ////    FiltItems();
        ////}
        ////internal override void UpdateCtrl() 
        ////{
        ////    DataGridInventory.ItemsSource = Hook.GetInventory();
        ////}

        ////internal override void ReloadCtrl()
        ////{
        ////    //lbxItems.SelectedIndex = -1;
        ////    //lbxItems.SelectedIndex = 0;
        ////}

        //internal override void ResetCtrl()
        //{
        //    new ErdHook().ResetInventory();
        //    DataGridInventory.ItemsSource = new List<InventoryEntry>();
        //}

        ////internal override void EnableCtrls(bool enable)
        ////{
        ////    //btnCreate.IsEnabled = enable;

        ////    if (enable)
        ////        UpdateCreateEnabled();
        ////}

        //private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    FiltItems();
        //}

        ////Clear items and add the ones that match text in search box
        //private void FiltItems()
        //{
        //    //lbxItems.Items.Clear();

        //    //if (SearchAllCheckbox.IsChecked.Value && txtSearch.Text != "")
        //    //{
        //    //    //search every item category
        //    //    foreach (ItemCategory category in cmbCategory.Items)
        //    //    {
        //    //        foreach (Item item in category.Items)
        //    //        {
        //    //            if (item.ToString().ToLower().Contains(txtSearch.Text.ToLower()))
        //    //                lbxItems.Items.Add(item);
        //    //        }
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    //only search selected item category
        //    //    ItemCategory category = cmbCategory.SelectedItem as ItemCategory;
        //    //    foreach (Item item in category.Items)
        //    //    {
        //    //        if (item.ToString().ToLower().Contains(txtSearch.Text.ToLower()))
        //    //            lbxItems.Items.Add(item);
        //    //    }
        //    //}

        //    //if (lbxItems.Items.Count > 0)
        //    //    lbxItems.SelectedIndex = 0;

        //    //HandleSearchLabel();
        //}

        //private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    FiltItems();
        //}

        ////Handles the "Searching..." label on the text box
        //private void HandleSearchLabel()
        //{
        //    //if (txtSearch.Text == "")
        //    //    lblSearch.Visibility = Visibility.Visible;
        //    //else
        //    //    lblSearch.Visibility = Visibility.Hidden;

        //}

        //private void cbxQuantityRestrict_Checked(object sender, RoutedEventArgs e)
        //{
        //    //if (lbxItems == null)
        //    //    return;

        //    //Item item = lbxItems.SelectedItem as Item;
        //    //if (item == null)
        //    //    return;

        //    //if (!cbxQuantityRestrict.IsChecked.Value)
        //    //{
        //    //    nudQuantity.IsEnabled = true;
        //    //    nudQuantity.Maximum = int.MaxValue;
        //    //}
        //    //else if (lbxItems.SelectedIndex != -1)
        //    //{
        //    //    nudQuantity.Maximum = item.MaxQuantity;
        //    //}
        //}

        //private void cmbInfusion_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    //Checks if cbxMaxUpgrade is checked and sets the value to max value
        //    HandleMaxItemCheckbox();
        //}

        //private void lbxItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    //if (!Hook.Setup) return;

        //    //Item item = lbxItems.SelectedItem as Item;
        //    //if (item == null)
        //    //    return;

        //    //if (!cbxQuantityRestrict.IsChecked.Value)
        //    //    nudQuantity.Maximum = int.MaxValue;
        //    //else
        //    //    nudQuantity.Maximum = item.MaxQuantity;


        //    //nudUpgrade.Maximum = 0;

        //    //cmbInfusion.Items.Clear();
        //    //cmbInfusion.Items.Add(Infusion.Standard);
        //    //cmbInfusion.SelectedIndex = 0;

        //    //cmbGems.Items.Clear();
        //    //cmbGems.Items.Add(Gem.Default);
        //    //cmbGems.SelectedIndex = 0;

        //    //if (item.ItemCategory == Item.Category.Weapons)
        //    //{
        //    //    ERWeapon? weapon = item as ERWeapon;
        //    //    if (weapon == null)
        //    //        return;

        //    //    if (weapon.Infisible)
        //    //        GetInfusions(weapon.DefaultGem);

        //    //    if (!weapon.Unique)
        //    //        foreach (Gem gem in Gem.All)
        //    //            if (gem.WeaponTypes.Contains(weapon.Type))
        //    //                cmbGems.Items.Add(gem);

        //    //    cmbInfusion.SelectedIndex = 0;
        //    //    cmbInfusion.IsEnabled = cmbInfusion.Items.Count > 1;

        //    //    cmbGems.SelectedItem = weapon.DefaultGem;
        //    //    if (cmbGems.SelectedItem == null)
        //    //        cmbGems.SelectedItem = Gem.Default;

        //    //    nudUpgrade.Maximum = weapon.MaxUpgrade;
        //    //}

        //    //nudUpgrade.IsEnabled = nudUpgrade.Maximum > 0;
        //    //cmbInfusion.IsEnabled = cmbInfusion.Items.Count > 1;
        //    //cmbGems.IsEnabled = cmbGems.Items.Count > 1;
        //    //btnCreate.IsEnabled = item.CanAquireFromOtherPlayers || App.Settings.SpawnUndroppable;
        //    ////if (!Properties.Settings.Default.UpdateMaxLive)
        //    ////    HandleMaxAvailable();
        //    //HandleMaxItemCheckbox();
        //}

        //private void cmbGems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    //if (!Hook.Setup) return;

        //    //Gem gem = cmbGems.SelectedItem as Gem;
        //    //if (gem == null) return;

        //    //cmbInfusion.Items.Clear();
        //    //GetInfusions(gem);
        //}

        //private void GetInfusions(Gem? gem)
        //{
        //    //foreach (Infusion infusion in gem.Infusions)
        //    //    cmbInfusion.Items.Add(infusion);

        //    //cmbInfusion.SelectedIndex = 0;
        //}

        //public void UpdateCreateEnabled()
        //{
        //    //Item? item = lbxItems.SelectedItem as Item;
        //    //if (item == null)
        //    //    return;

        //    //btnCreate.IsEnabled = item.CanAquireFromOtherPlayers || App.Settings.SpawnUndroppable;
        //}

        //internal void EnableStats(bool enable)
        //{
        //    //Item? item = lbxItems.SelectedItem as Item;
        //    //bool canTrade = false;
        //    //if (item != null)
        //    //    canTrade = item.CanAquireFromOtherPlayers;

        //    //btnCreate.IsEnabled = enable && canTrade;
        //}

        ////private void btnCreate_Click(object sender, RoutedEventArgs e)
        ////{
        ////    CreateItem();
        ////}

        ////Apply hair to currently loaded character
        //public void CreateItem()
        //{
        //    //Check if the button is enabled and the selected item isn't null
        //    //if (btnCreate.IsEnabled && lbxItems.SelectedItem != null)
        //    //{
        //    //    _ = ChangeColor(Brushes.DarkGray);
        //    //    Item? item = lbxItems.SelectedItem as Item;
        //    //    if (item == null)
        //    //        return;

        //    //    int id = item.ID;

        //    //    Infusion infusion = (Infusion)cmbInfusion.SelectedItem;

        //    //    id += (int)item.ItemCategory;

        //    //    Gem? gem = cmbGems.SelectedItem as Gem;

        //    //    if (item.EventID != -1)
        //    //        Hook.SetEventFlag(item.EventID);

        //    //    Hook.GetItem(id, (int)nudQuantity.Value, (int)infusion, (int)nudUpgrade.Value, gem.ID);

        //    //}
        //}

        ////handles up and down scrolling
        //private void ScrollListbox(KeyEventArgs e)
        //{
        //    //Scroll down through Items listbox and go back to bottom at end
        //    //if (e.Key == Key.Up)
        //    //{
        //    //    e.Handled = true;//Do not pass keypress along

        //    //    //One liner meme that does the exact same thing as the code above
        //    //    lbxItems.SelectedIndex = ((lbxItems.SelectedIndex - 1) + lbxItems.Items.Count) % lbxItems.Items.Count;
        //    //    lbxItems.ScrollIntoView(lbxItems.SelectedItem);
        //    //    return;
        //    //}

        //    ////Scroll down through Items listbox and go back to top at end
        //    //if (e.Key == Key.Down)
        //    //{
        //    //    e.Handled = true;//Do not pass keypress along

        //    //    //One liner meme that does the exact same thing as the code above
        //    //    lbxItems.SelectedIndex = (lbxItems.SelectedIndex + 1) % lbxItems.Items.Count;
        //    //    lbxItems.ScrollIntoView(lbxItems.SelectedItem);
        //    //    return;
        //    //}
        //}

        ////Changes the color of the Apply button
        //private async Task ChangeColor(Brush new_color)
        //{
        //    //btnCreate.Background = new_color;

        //    //await Task.Delay(TimeSpan.FromSeconds(.25));

        //    //btnCreate.Background = default(Brush);
        //}

        ////handles escape
        //private void KeyPressed(object sender, KeyEventArgs e)
        //{
        //    //if (e.Key == Key.Escape)
        //    //{
        //    //    txtSearch.Clear();
        //    //    return;
        //    //}

        //    ////Create selected index as item
        //    //if (e.Key == Key.Enter)
        //    //{
        //    //    e.Handled = true; //Do not pass keypress along
        //    //    CreateItem();
        //    //    return;
        //    //}

        //    ////Return if sender is cmbInfusion so that arrow Key are handled correctly
        //    //if (sender == cmbInfusion)
        //    //    return;
        //    ////Prevents up and down Key from moving the cursor left and right when nothing in item box
        //    //if (lbxItems.Items.Count == 0)
        //    //{
        //    //    if (e.Key == Key.Up)
        //    //        e.Handled = true; //Do not pass keypress along
        //    //    if (e.Key == Key.Down)
        //    //        e.Handled = true; //Do not pass keypress along
        //    //    return;
        //    //}

        //    //ScrollListbox(e);
        //}

        ////Select number in nud
        //private void nudUpgrade_Click(object sender, EventArgs e)
        //{
        //    //nudUpgrade.Focus();
        //}

        //private void SearchAllCheckbox_CheckedChanged(object sender, EventArgs e)
        //{
        //    //checkbox changed, refresh search filter (if txtSearch is not empty)
        //    //if (txtSearch.Text != "")
        //    //    FiltItems();
        //}

        //private void cbxMaxUpgrade_Checked(object sender, RoutedEventArgs e)
        //{
        //    //HandleMaxItemCheckbox();
        //    //if (cbxMax.IsChecked.Value)
        //    //{
        //    //    nudUpgrade.Value = nudUpgrade.Maximum;
        //    //    nudQuantity.Value = nudQuantity.Maximum;
        //    //}
        //    //else
        //    //{
        //    //    nudUpgrade.Value = nudUpgrade.Minimum;
        //    //    nudQuantity.Value = nudQuantity.Minimum;
        //    //}
        //}

        //private void HandleMaxItemCheckbox()
        //{
        //    //Set upgrade nud to max if max checkbox is ticked
        //    //if (cbxMax.IsChecked.Value)
        //    //{
        //    //    nudUpgrade.Value = nudUpgrade.Maximum;
        //    //    nudQuantity.Value = nudQuantity.Maximum;
        //    //}
        //}

        //private void cmbInfusion_KeyDown(object sender, KeyEventArgs e)
        //{
        //    //Create selected index as item
        //    //if (e.Key == Key.Enter)
        //    //{
        //    //    e.Handled = true; //Do not pass keypress along
        //    //    CreateItem();
        //    //    return;
        //    //}
        //}
        ////Select all text in search box
        //private void txtSearch_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    //txtSearch.SelectAll();
        //    //txtSearch.Focus();
        //    //e.Handled = true;
        //}

        //private void SearchAllCheckbox_Checked(object sender, RoutedEventArgs e)
        //{
        //    //if (txtSearch.Text != "")
        //    //    FiltItems();
        //}


    }
}

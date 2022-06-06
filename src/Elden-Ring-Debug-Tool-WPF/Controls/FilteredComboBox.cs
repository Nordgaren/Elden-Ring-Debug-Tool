using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Elden_Ring_Debug_Tool_WPF.Controls
{
    //https://stackoverflow.com/questions/2001842/dynamic-filter-of-wpf-combobox-based-on-text-input/41986141#41986141
    public class FilteredComboBox : ComboBox
    {
        private string _oldFilter = string.Empty;

        private string _currentFilter = string.Empty;

        protected TextBox EditableTextBox => GetTemplateChild("PART_EditableTextBox") as TextBox;


        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (newValue != null)
            {
                ICollectionView? view = CollectionViewSource.GetDefaultView(newValue);
                view.Filter += FilterItem;
            }

            if (oldValue != null)
            {
                ICollectionView? view = CollectionViewSource.GetDefaultView(oldValue);
                if (view != null) view.Filter -= FilterItem;
            }

            base.OnItemsSourceChanged(oldValue, newValue);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Tab:
                case Key.Enter:
                    IsDropDownOpen = false;
                    break;
                case Key.Escape:
                    IsDropDownOpen = false;
                    SelectedIndex = -1;
                    Text = _currentFilter;
                    break;
                default:
                    if (e.Key == Key.Down) IsDropDownOpen = true;

                    base.OnPreviewKeyDown(e);
                    break;
            }

            // Cache text
            _oldFilter = Text;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                case Key.Down:
                    break;
                case Key.Tab:
                case Key.Enter:

                    ClearFilter();
                    break;
                default:
                    if (Text != _oldFilter)
                    {
                        RefreshFilter();
                        IsDropDownOpen = true;

                        EditableTextBox.SelectionStart = int.MaxValue;
                    }

                    base.OnKeyUp(e);
                    _currentFilter = Text;
                    break;
            }
        }

        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            ClearFilter();
            int temp = SelectedIndex;
            SelectedIndex = -1;
            Text = string.Empty;
            SelectedIndex = temp;
            base.OnPreviewLostKeyboardFocus(e);
        }

        private void RefreshFilter()
        {
            if (ItemsSource == null) return;

            ICollectionView? view = CollectionViewSource.GetDefaultView(ItemsSource);
            view.Refresh();
        }

        private void ClearFilter()
        {
            _currentFilter = string.Empty;
            RefreshFilter();
        }

        private bool FilterItem(object value)
        {
            //var type = value.GetType();

            if (value == null) return false;
            if (Text.Length == 0) return true;

            string[] words = Text.ToLower().Split(' ');
            string lower = value.ToString().ToLower();
            return words.All(lower.Contains);
        }
    }
}
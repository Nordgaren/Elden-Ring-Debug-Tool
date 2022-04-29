using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Elden_Ring_Debug_Tool_WPF
{
    public partial class MainWindow : Window
    {
        public List<DebugHotkey> Hotkeys = new List<DebugHotkey>();

        private void InitHotkeys()
        {
            cbxEnableHotkeys.IsChecked = App.Settings?.EnableHotkeys;
            cbxHandleHotkeys.IsChecked = App.Settings?.HandleHotkeys;


            Hotkeys.Add(new DebugHotkey("CreateItem", hkeyCreateItem.tbxHotkey, tabHotkeys, (hotkey) =>
            {
                //DebugItems.CreateItem();
            }, this));

        }

        private void SaveHotkeys()
        {
            App.Settings.EnableHotkeys = cbxEnableHotkeys.IsChecked.Value;
            App.Settings.HandleHotkeys = cbxHandleHotkeys.IsChecked.Value;
            foreach (DebugHotkey hotkey in Hotkeys)
                hotkey.Save();
        }

        private bool HotkeysSet = false;
        private void CheckFocused()
        {
            //if (Hook.Focused && !HotkeysSet)
            //    RegisterHotkeys();

            //if (!Hook.Focused && HotkeysSet)
            //    UnregisterHotkeys();
        }

        private void RegisterHotkeys()
        {
            foreach (DebugHotkey hotkey in Hotkeys)
            {
                hotkey.RegisterHotkey();
            }
            HotkeysSet = true;
        }

        private void UnregisterHotkeys()
        {
            foreach (DebugHotkey hotkey in Hotkeys)
            {
                Key key = hotkey.Key;
                hotkey.UnregisterHotkey();
            }
            HotkeysSet = false;
        }

    }
}

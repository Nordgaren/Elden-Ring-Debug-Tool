using mrousavy;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Elden_Ring_Debug_Tool_WPF
{
    public class DebugHotkey
    {
        private string SettingsName;
        private TextBox HotkeyTextBox;
        private TabItem HotkeyTabPage;
        private Window Window;
        private Action<HotKey> HotkeyAction;
        private Brush DefaultColor;

        public Key Key;
        public HotKey? HotKey;

        public DebugHotkey(string settingsName, TextBox setTextBox, TabItem setTabPage, Action<HotKey> setAction, Window window)
        {
            SettingsName = settingsName;
            HotkeyTextBox = setTextBox;
            DefaultColor = HotkeyTextBox.Background;
            HotkeyTabPage = setTabPage;
            HotkeyAction = setAction;
            Window = window;

            Key = KeyInterop.KeyFromVirtualKey((int)Properties.Settings.Default[SettingsName]);

            if (Key == Key.Escape)
                HotkeyTextBox.Text = "Unbound";
            else
                HotkeyTextBox.Text = Key.ToString();

            HotkeyTextBox.MouseEnter += HotkeyTextBox_MouseEnter;
            HotkeyTextBox.MouseLeave += HotkeyTextBox_MouseLeave;
            HotkeyTextBox.KeyUp += HotkeyTextBox_KeyUp;
        }

        public void RegisterHotkey()
        {
            UnregisterHotkey();

            if (Key != Key.Escape)
                HotKey = new HotKey(ModifierKeys.None, Key, Window, HotkeyAction);
        }

        public void UnregisterHotkey()
        {
            if (HotKey == null)
                return;

            HotKey.Dispose();
            HotKey = null;
        }

        private void HotkeyTextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Key = e.Key;
            if (Key == Key.Escape)
                HotkeyTextBox.Text = "Unbound";
            else
                HotkeyTextBox.Text = Key.ToString();
            e.Handled = true;

            UnregisterHotkey();

            MainWindow? mWindow = Window as MainWindow;
            //DebugHotkey? existingKey = mWindow?.Hotkeys.Find(hKey => hKey.Key == Key && hKey.SettingsName != SettingsName && hKey.Key != Key.Escape);
            //if (existingKey != null)
            //{
            //    KeyEventArgs? args = new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, Key.Escape);
            //    args.RoutedEvent = e.RoutedEvent;
            //    existingKey.HotkeyTextBox_KeyUp(existingKey.HotkeyTextBox, args); 
            //}
            HotkeyTabPage.Focus();

        }
        private void HotkeyTextBox_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            HotkeyTextBox.Background = DefaultColor;
        }

        private void HotkeyTextBox_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            HotkeyTextBox.Background = Brushes.LightGreen;
        }

        public void Save()
        {
            Properties.Settings.Default[SettingsName] = KeyInterop.VirtualKeyFromKey(Key);
        }
    }
}

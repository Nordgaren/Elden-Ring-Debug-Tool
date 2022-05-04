using Erd_Tools;
using mrousavy;
using System.Windows;
using System.Windows.Input;

namespace Elden_Ring_Debug_Tool_ViewModels.Manager
{
    internal class WindowsRegisteredMultiHotkeyManager : IHotkeyManager
    {
        private ERHook _hook { get; }
        private Dictionary<Key, List<ICommand>> _hotkeyDictionary { get; }
        private List<HotKey> _hotkeyList { get; }

        public WindowsRegisteredMultiHotkeyManager(ERHook hook)
        {
            _hook = hook;
            _hotkeyDictionary = new Dictionary<Key, List<ICommand>>();
            _hotkeyList = new List<HotKey>();
        }
        public void AddHotkey(Key key, ICommand command)
        {
            if (!_hotkeyDictionary.ContainsKey(key))
            {
                _hotkeyDictionary.Add(key, new List<ICommand>() { command });
                return;
            }

            _hotkeyDictionary[key].Add(command);
        }

        private void HotKeyPressed(HotKey obj)
        {

            foreach (ICommand command in _hotkeyDictionary[obj.Key])
            {
                if (command.CanExecute(null))
                    command.Execute(null);
            }
        }

        public void RemoveHotkey(Key key, ICommand command)
        {
            bool success = _hotkeyDictionary[key].Remove(command);

            if (!success)
                throw new Exception("No HotKey found in HotKey dictionary");

            if (_hotkeyDictionary[key].Count == 0)
            {
                _hotkeyDictionary.Remove(key);
            }
        }
        private bool _enableHotkeys { get; set; }

        public void SetHotkeyEnable(bool enable)
        {
            _enableHotkeys = enable;
        }

        public void Update()
        {
            if (_enableHotkeys && _hook.Focused && !(_hotkeyList.Count > 0) && _hotkeyDictionary.Count > 0)
                RegisterHotKeys();

            if (!_hook.Focused && _hotkeyList.Count > 0)
                UnregisterHotKeys();
        }

        private void RegisterHotKeys()
        {
            foreach (Key key in _hotkeyDictionary.Keys)
            {
                _hotkeyList.Add(new HotKey(ModifierKeys.None, key, Application.Current.MainWindow, HotKeyPressed));
            }
        }

        private void UnregisterHotKeys()
        {
            foreach (HotKey hotKey in _hotkeyList)
            {
                hotKey.Dispose();
            }
            _hotkeyList.Clear();
        }



    }
}

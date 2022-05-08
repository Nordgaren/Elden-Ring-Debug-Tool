using System.Windows;
using System.Windows.Input;
using Erd_Tools.Hook;
using mrousavy;

namespace Elden_Ring_Debug_Tool_ViewModels.Managers
{
    internal class WindowsRegisteredMultiHotKeyManager : IHotKeyManager
    {
        private ErdHook _hook { get; }
        private Dictionary<Key, List<ICommand>> _hotkeyDictionary { get; }
        private List<HotKey> _hotkeyList { get; }

        public WindowsRegisteredMultiHotKeyManager(ErdHook hook)
        {
            _hook = hook;
            _hotkeyDictionary = new Dictionary<Key, List<ICommand>>();
            _hotkeyList = new List<HotKey>();
        }
        public void AddHotKey(Key key, ICommand command)
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

        public void RemoveHotKey(Key key, ICommand command)
        {
            bool success = _hotkeyDictionary[key].Remove(command);

            if (!success)
                throw new Exception("No HotKey found in HotKey dictionary");

            if (_hotkeyDictionary[key].Count == 0)
            {
                _hotkeyDictionary.Remove(key);
            }
        }
        private bool _enableHotKeys { get; set; }

        public void SetHotKeyEnable(bool enable)
        {
            _enableHotKeys = enable;
        }

        public void Update()
        {
            if (_enableHotKeys && _hook.Focused && !(_hotkeyList.Count > 0) && _hotkeyDictionary.Count > 0)
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

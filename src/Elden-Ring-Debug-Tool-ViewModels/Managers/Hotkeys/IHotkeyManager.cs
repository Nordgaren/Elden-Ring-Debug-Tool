using System.Windows.Input;

namespace Elden_Ring_Debug_Tool_ViewModels.Managers
{
    internal interface IHotKeyManager
    {
        public void AddHotKey(Key key, ICommand command);

        public void RemoveHotKey(Key key, ICommand command);

        public void Update();
        public void SetHotKeyEnable(bool enable);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Elden_Ring_Debug_Tool_ViewModels.Manager
{
    internal interface IHotkeyManager
    {
        public void AddHotkey(Key key, ICommand command);

        public void RemoveHotkey(Key key, ICommand command);

        public void Update();
        public void SetHotkeyEnable(bool enable);
    }
}

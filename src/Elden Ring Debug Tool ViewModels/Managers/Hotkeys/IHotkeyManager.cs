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
        public abstract void AddHotkey(Key key, ICommand command);

        public abstract void RemoveHotkey(Key key, ICommand command);

        public abstract void Update();
    }
}

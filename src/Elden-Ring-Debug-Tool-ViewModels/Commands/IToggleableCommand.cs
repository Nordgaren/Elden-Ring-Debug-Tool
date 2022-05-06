using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Elden_Ring_Debug_Tool_ViewModels.Commands
{
    public interface IToggleableCommand
    {
        public bool State { get; set; }

    }
}

using PropertyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool
{
    internal class ERViewModel : ObservableObject
    {
        public ERHook Hook { get; private set; }

        public ERViewModel()
        {
            Hook = new ERHook(5000, 5000);
            Hook.OnHooked += Hook_OnHooked;
            Hook.OnUnhooked += Hook_OnUnhooked;
            Hook.Start();
        }
        private void Hook_OnHooked(object? sender, PHEventArgs e)
        {
            
        }
        private void Hook_OnUnhooked(object? sender, PHEventArgs e)
        {

        }
    }
}

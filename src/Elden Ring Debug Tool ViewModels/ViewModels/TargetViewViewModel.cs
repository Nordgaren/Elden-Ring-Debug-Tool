using Erd_Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool_ViewModels.ViewModels
{
    [Description("Target View")]
    public class TargetViewViewModel
    {
        internal ErdHook Hook { get; private set; }

        public TargetViewViewModel()
        {

        }
        public void InitViewModel(ErdHook hook)
        {
            Hook = hook;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Elden_Ring_Debug_Tool_ViewModels.Utils
{
    internal static class ExtensionMethods
    {

        public static Type GetCommandType(this ICommand command)
        {
            return command.GetType();
        }

    }
}

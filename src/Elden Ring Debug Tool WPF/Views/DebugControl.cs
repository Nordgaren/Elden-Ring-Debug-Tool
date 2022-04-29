using Erd_Tools;
using System.Windows;
using System.Windows.Controls;

namespace Elden_Ring_Debug_Tool_WPF
{
    public class DebugControl : UserControl
    {
        public ERHook Hook
        {
            get { return (ERHook)GetValue(HookProperty); }
            set { SetValue(HookProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Hook.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HookProperty =
            DependencyProperty.Register("Hook", typeof(ERHook), typeof(DebugControl), new PropertyMetadata(default));

        public bool GameLoaded
        {
            get { return (bool)GetValue(GameLoadedProperty); }
            set { SetValue(GameLoadedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Loaded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GameLoadedProperty =
            DependencyProperty.Register("GameLoaded", typeof(bool), typeof(DebugControl), new PropertyMetadata(default));

        internal virtual void UpdateCtrl() { }
        internal virtual void ReloadCtrl() { }
        internal virtual void EnableCtrls(bool v) { }
        public virtual void InitCtrl() { }
        internal virtual void ResetCtrl() { }


    }
}

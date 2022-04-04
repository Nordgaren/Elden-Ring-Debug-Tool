using System;

namespace PropertyHook
{
    /// <summary>
    /// Args passed to the hook and unhook events of a PHook.
    /// </summary>
    public class PHEventArgs : EventArgs
    {
        /// <summary>
        /// The PHook that hooked or unhooked.
        /// </summary>
        public PHook Hook { get; }

        /// <summary>
        /// Create a new PHEventArgs with the given hook.
        /// </summary>
        public PHEventArgs(PHook hook)
        {
            Hook = hook;
        }
    }
}

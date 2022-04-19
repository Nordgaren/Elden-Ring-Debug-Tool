using System;

namespace PropertyHook
{
    /// <summary>
    /// A dynamic pointer starting from the result of another pointer's resolution.
    /// </summary>
    public class PHPointerChild : PHPointer
    {
        /// <summary>
        /// The parent pointer to start from.
        /// </summary>
        public PHPointer BasePointer { get; set; }

        /// <summary>
        /// Creates a new child pointer.
        /// </summary>
        public PHPointerChild(PHook parent, PHPointer pointer, params int[] offsets) : base(parent, offsets)
        {
            BasePointer = pointer;
        }

        /// <summary>
        /// Returns the final address of the parent pointer.
        /// </summary>
        protected override IntPtr ResolveSpecific()
        {
            return BasePointer.Resolve();
        }
    }
}

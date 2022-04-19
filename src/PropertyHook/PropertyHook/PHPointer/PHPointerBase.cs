using System;

namespace PropertyHook
{
    /// <summary>
    /// A dynamic pointer starting from a fixed address.
    /// </summary>
    public class PHPointerBase : PHPointer
    {
        /// <summary>
        /// The fixed address to start from.
        /// </summary>
        public IntPtr BaseAddress { get; set; }

        /// <summary>
        /// Creates a new base pointer.
        /// </summary>
        public PHPointerBase(PHook parent, IntPtr address, params int[] offsets) : base(parent, offsets)
        {
            BaseAddress = address;
        }

        /// <summary>
        /// Returns the base address.
        /// </summary>
        protected override IntPtr ResolveSpecific()
        {
            return BaseAddress;
        }
    }
}

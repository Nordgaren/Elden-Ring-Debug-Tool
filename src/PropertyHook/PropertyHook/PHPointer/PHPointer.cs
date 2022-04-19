using System;
using System.Text;

namespace PropertyHook
{
    /// <summary>
    /// A dynamic pointer starting from an AOB, fixed address, or other pointer. Provides functions for reading and writing memory.
    /// </summary>
    public abstract class PHPointer
    {
        /// <summary>
        /// The parent PHook whose handle is used for remote operations.
        /// </summary>
        protected PHook Hook { get; }

        /// <summary>
        /// After finding the base address, for each offset in this array the pointer will advance by the offset, read another address, and follow it.
        /// </summary>
        public int[] Offsets { get; set; }

        /// <summary>
        /// Creates a new PHPointer.
        /// </summary>
        protected PHPointer(PHook parent, int[] offsets)
        {
            Hook = parent;
            Offsets = offsets;
        }

        /// <summary>
        /// Evaluates the base address and follows each offset to find the final address.
        /// </summary>
        public IntPtr Resolve()
        {
            IntPtr address = ResolveSpecific();
            foreach (int offset in Offsets)
            {
                address = Kernel32.ReadIntPtr(Hook.Handle, address + offset, Hook.Is64Bit);
            }
            return address;
        }

        /// <summary>
        /// Evaluates the pointer's base address.
        /// </summary>
        protected abstract IntPtr ResolveSpecific();


        /// <summary>
        /// Read length number of bytes from the given offset.
        /// </summary>
        public byte[] ReadBytes(int offset, uint length)
        {
            return Kernel32.ReadBytes(Hook.Handle, Resolve() + offset, length);
        }

        /// <summary>
        /// Write length number of bytes at the given offset.
        /// </summary>
        public bool WriteBytes(int offset, byte[] bytes)
        {
            return Kernel32.WriteBytes(Hook.Handle, Resolve() + offset, bytes);
        }

        /// <summary>
        /// Read an address from the given offset.
        /// </summary>
        public IntPtr ReadIntPtr(int offset)
        {
            return Kernel32.ReadIntPtr(Hook.Handle, Resolve() + offset, Hook.Is64Bit);
        }

        /// <summary>
        /// Read a 4-byte bitfield and return whether the bit specified by the given mask is set.
        /// </summary>
        public bool ReadFlag32(int offset, uint mask)
        {
            return Kernel32.ReadFlag32(Hook.Handle, Resolve() + offset, mask);
        }

        /// <summary>
        /// Set the state of a bit specified by the given mask in a 4-byte bitfield.
        /// </summary>
        public bool WriteFlag32(int offset, uint mask, bool state)
        {
            return Kernel32.WriteFlag32(Hook.Handle, Resolve() + offset, mask, state);
        }


        /// <summary>
        /// Read a 1-byte signed integer.
        /// </summary>
        public sbyte ReadSByte(int offset)
        {
            return Kernel32.ReadSByte(Hook.Handle, Resolve() + offset);
        }

        /// <summary>
        /// Write a 1-byte signed integer.
        /// </summary>
        public bool WriteSByte(int offset, sbyte value)
        {
            return Kernel32.WriteSByte(Hook.Handle, Resolve() + offset, value);
        }


        /// <summary>
        /// Read a 1-byte unsigned integer.
        /// </summary>
        public byte ReadByte(int offset)
        {
            return Kernel32.ReadByte(Hook.Handle, Resolve() + offset);
        }

        /// <summary>
        /// Write a 1-byte unsigned integer.
        /// </summary>
        public bool WriteByte(int offset, byte value)
        {
            return Kernel32.WriteByte(Hook.Handle, Resolve() + offset, value);
        }


        /// <summary>
        /// Read a 1-byte boolean value.
        /// </summary>
        public bool ReadBoolean(int offset)
        {
            return Kernel32.ReadBoolean(Hook.Handle, Resolve() + offset);
        }

        /// <summary>
        /// Write a 1-byte boolean value.
        /// </summary>
        public bool WriteBoolean(int offset, bool value)
        {
            return Kernel32.WriteBoolean(Hook.Handle, Resolve() + offset, value);
        }


        /// <summary>
        /// Read a 2-byte signed integer.
        /// </summary>
        public short ReadInt16(int offset)
        {
            return Kernel32.ReadInt16(Hook.Handle, Resolve() + offset);
        }

        /// <summary>
        /// Write a 2-byte signed integer.
        /// </summary>
        public bool WriteInt16(int offset, short value)
        {
            return Kernel32.WriteInt16(Hook.Handle, Resolve() + offset, value);
        }


        /// <summary>
        /// Read a 2-byte unsigned integer.
        /// </summary>
        public ushort ReadUInt16(int offset)
        {
            return Kernel32.ReadUInt16(Hook.Handle, Resolve() + offset);
        }

        /// <summary>
        /// Write a 2-byte unsigned integer.
        /// </summary>
        public bool WriteUInt16(int offset, ushort value)
        {
            return Kernel32.WriteUInt16(Hook.Handle, Resolve() + offset, value);
        }


        /// <summary>
        /// Read a 4-byte signed integer.
        /// </summary>
        public int ReadInt32(int offset)
        {
            return Kernel32.ReadInt32(Hook.Handle, Resolve() + offset);
        }

        /// <summary>
        /// Write a 4-byte signed integer.
        /// </summary>
        public bool WriteInt32(int offset, int value)
        {
            return Kernel32.WriteInt32(Hook.Handle, Resolve() + offset, value);
        }


        /// <summary>
        /// Read a 4-byte unsigned integer.
        /// </summary>
        public uint ReadUInt32(int offset)
        {
            return Kernel32.ReadUInt32(Hook.Handle, Resolve() + offset);
        }

        /// <summary>
        /// Write a 4-byte unsigned integer.
        /// </summary>
        public bool WriteUInt32(int offset, uint value)
        {
            return Kernel32.WriteUInt32(Hook.Handle, Resolve() + offset, value);
        }


        /// <summary>
        /// Read an 8-byte signed integer.
        /// </summary>
        public long ReadInt64(int offset)
        {
            return Kernel32.ReadInt64(Hook.Handle, Resolve() + offset);
        }

        /// <summary>
        /// Write an 8-byte signed integer.
        /// </summary>
        public bool WriteInt64(int offset, long value)
        {
            return Kernel32.WriteInt64(Hook.Handle, Resolve() + offset, value);
        }


        /// <summary>
        /// Read an 8-byte unsigned integer.
        /// </summary>
        public ulong ReadUInt64(int offset)
        {
            return Kernel32.ReadUInt64(Hook.Handle, Resolve() + offset);
        }

        /// <summary>
        /// Write an 8-byte unsigned integer.
        /// </summary>
        public bool WriteUInt64(int offset, ulong value)
        {
            return Kernel32.WriteUInt64(Hook.Handle, Resolve() + offset, value);
        }


        /// <summary>
        /// Read a 4-byte floating point number.
        /// </summary>
        public float ReadSingle(int offset)
        {
            return Kernel32.ReadSingle(Hook.Handle, Resolve() + offset);
        }

        /// <summary>
        /// Write a 4-byte floating point number.
        /// </summary>
        public bool WriteSingle(int offset, float value)
        {
            return Kernel32.WriteSingle(Hook.Handle, Resolve() + offset, value);
        }


        /// <summary>
        /// Read an 8-byte floating point number.
        /// </summary>
        public double ReadDouble(int offset)
        {
            return Kernel32.ReadDouble(Hook.Handle, Resolve() + offset);
        }

        /// <summary>
        /// Write an 8-byte floating point number.
        /// </summary>
        public bool WriteDouble(int offset, double value)
        {
            return Kernel32.WriteDouble(Hook.Handle, Resolve() + offset, value);
        }


        /// <summary>
        /// Reads and decodes a fixed amount of bytes as a string. If trim is set, terminates after the first null character.
        /// </summary>
        public string ReadString(int offset, Encoding encoding, uint byteCount, bool trim = true)
        {
            return Kernel32.ReadString(Hook.Handle, Resolve() + offset, encoding, byteCount, trim);
        }

        /// <summary>
        /// Encodes and writes a string as a fixed amount of bytes. If the string is too long to fit, it is truncated before encoding.
        /// </summary>
        public bool WriteString(int offset, Encoding encoding, uint byteCount, string value)
        {
            return Kernel32.WriteString(Hook.Handle, Resolve() + offset, encoding, byteCount, value);
        }
    }
}

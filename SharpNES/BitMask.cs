using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNES
{
    public static class BitMask
    {
        public static bool InRange(this ushort value, ushort min, ushort max)
        {
            return value >= min && value <= max;
        }
        public static bool Matches(this ushort value, params ushort[] vs)
        {
            foreach (ushort b in vs)
            {
                if (value == b) return true;
            }
            return false;
        }
        public static bool Contains(byte value, byte flags)
        {
            return (byte)(value & flags) == flags;
        }
        public static byte Set(byte value, byte flags)
        {
            value |= flags;
            return value;
        }
        public static byte Set(byte value, byte flags, bool status)
        {
            if (status) return Set(value, flags);
            return UnSet(value, flags);
        }
        public static byte UnSet(byte value, byte flags)
        {
            value &= (byte)~flags;
            return value;
        }
        public static byte Toggle(byte value, byte flags)
        {
            value ^= flags;
            return value;
        }
    }
}

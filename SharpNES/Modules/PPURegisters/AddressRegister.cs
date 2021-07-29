using System;

namespace SharpNES.Modules.PPURegisters
{
    public struct AddrRegister
    {
        public Tuple<byte, byte> value;
        public bool hi_ptr;
        void Set(ushort data)
        {
            byte v1 = (byte)(data >> 8);
            byte v2 = (byte)(data & 0xff);
            value = new Tuple<byte, byte>(v1, v2);
        }
        public void Update(byte data)
        {
            byte v1 = value.Item1;
            byte v2 = value.Item2;
            if (hi_ptr)
            {
                v1 = data;
            }
            else
            {
                v2 = data;
            }
            value = new Tuple<byte, byte>(v1, v2);
            if (Get() > 0x3fff)
            {
                Set((ushort)(Get() & 0b11111111111111));
            }
            hi_ptr = !hi_ptr;
        }
        public void Increment(byte inc)
        {
            byte hi0 = value.Item1;
            byte lo = value.Item2;
            byte lo1 = (byte)(lo + inc);
            if (lo > lo1)
            {
                hi0++;
            }
            value = new Tuple<byte, byte>(hi0, lo1);
            if (Get() > 0x3fff)
            {
                Set((ushort)(Get() & 0b11111111111111));
            }
        }
        public void ResetLatch()
        {
            hi_ptr = true;
        }
        public ushort Get()
        {
            return (ushort)((ushort)(value.Item1 << 8) | value.Item2);
        }
    }
}

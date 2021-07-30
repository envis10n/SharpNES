namespace SharpNES.Modules.PPURegisters
{
    public class AddrRegister
    {
        public (byte, byte) value = (0, 0);
        public bool hi_ptr = true;
        void Set(ushort data)
        {
            value.Item1 = (byte)(data >> 8);
            value.Item2 = (byte)(data & 0xff);
        }
        public void Update(byte data)
        {
            if (hi_ptr)
            {
                value.Item1 = data;
            }
            else
            {
                value.Item2 = data;
            }
            if (Get() > 0x3fff)
            {
                Set((ushort)(Get() & 0b11111111111111));
            }
            hi_ptr = !hi_ptr;
        }
        public void Increment(byte inc)
        {
            byte lo = value.Item2;
            byte lo1 = (byte)(lo + inc);
            if (lo > lo1)
            {
                value.Item1++;
            }
            value.Item2 = lo1;
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

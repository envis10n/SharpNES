namespace SharpNES.Modules.PPURegisters
{
    public class ScrollRegister
    {
        public byte scroll_x = 0;
        public byte scroll_y = 0;
        public bool latch = false;
        public void Write(byte data)
        {
            if (!latch) scroll_x = data;
            else scroll_y = data;
            latch = !latch;
        }
        public void ResetLatch()
        {
            latch = false;
        }
    }
}

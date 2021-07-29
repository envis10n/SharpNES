namespace SharpNES.Modules.PPURegisters
{
    public struct ScrollRegister
    {
        public byte scroll_x { get; set; }
        public byte scroll_y { get; set; }
        public bool latch { get; set; }
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

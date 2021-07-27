namespace SharpNES.Modules
{
    public interface IMemory
    {
        abstract byte MemRead(ushort addr);
        abstract void MemWrite(ushort addr, byte data);
    }
}

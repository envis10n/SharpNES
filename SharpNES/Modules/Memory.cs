using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNES.Modules
{
    public interface IMemory
    {
        abstract byte MemRead(ushort addr);
        abstract void MemWrite(ushort addr, byte data);
    }
    class Memory
    {
    }
}

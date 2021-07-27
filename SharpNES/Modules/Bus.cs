using System;

namespace SharpNES.Modules
{
    public enum BusAccessType
    {
        NONE,
        RAM,
        PPU,
        ROM,
    }
    public class Bus : IMemory
    {
        byte[] cpu_vram = new byte[2048];
        Rom rom;
        static ushort RAM = 0x0000;
        static ushort RAM_MIRRORS_END = 0x1FFF;
        static ushort PPU_REGISTERS = 0x2000;
        static ushort PPU_REGISTERS_MIRRORS_END = 0x3FFF;
        public Bus(Rom _rom)
        {
            rom = _rom;
        }
        public BusAccessType GetAccessType(ushort addr)
        {
            if (addr >= RAM && addr <= RAM_MIRRORS_END) return BusAccessType.RAM;
            else if (addr >= PPU_REGISTERS && addr <= PPU_REGISTERS_MIRRORS_END) return BusAccessType.PPU;
            else if (addr >= 0x8000 && addr <= 0xFFFF) return BusAccessType.ROM;
            else return BusAccessType.NONE;
        }
        public void MemWrite(ushort addr, byte data)
        {
            BusAccessType accessType = GetAccessType(addr);
            switch (accessType)
            {
                case BusAccessType.NONE:
                    Console.WriteLine($"Ignoring memory access at 0x{addr.ToString("hex")}");
                    break;
                case BusAccessType.RAM:
                    {
                        ushort mirror_down_addr = (ushort)(addr & 0b00000111_11111111);
                        cpu_vram[mirror_down_addr] = data;
                        break;
                    }
                case BusAccessType.PPU:
                    {
                        ushort mirror_down_addr = (ushort)(addr & 0b00100000_00000111);
                        throw new Exception("PPU is not supported yet.");
                    }
                case BusAccessType.ROM:
                    throw new Exception("Attempt to access cartridge ROM space.");
                default:
                    throw new Exception("Invalid bus access type.");
            }
        }
        public byte MemRead(ushort addr)
        {
            BusAccessType accessType = GetAccessType(addr);
            switch (accessType)
            {
                case BusAccessType.NONE:
                    Console.WriteLine($"Ignoring memory access at 0x{addr.ToString("hex")}");
                    return 0;
                case BusAccessType.RAM:
                    {
                        ushort mirror_down_addr = (ushort)(addr & 0b00000111_11111111);
                        return cpu_vram[mirror_down_addr];
                    }
                case BusAccessType.PPU:
                    {
                        ushort mirror_down_addr = (ushort)(addr & 0b00100000_00000111);
                        throw new Exception("PPU is not supported yet.");
                    }
                case BusAccessType.ROM:
                    return ReadPrgRom(addr);
                default:
                    throw new Exception("Invalid bus access type.");
            }
        }
        byte ReadPrgRom(ushort addr)
        {
            ushort _addr = (ushort)(addr - 0x8000);
            if (rom.prg_rom.Length == 0x4000 && _addr >= 0x4000)
            {
                _addr = (ushort)(_addr % 0x4000);
            }
            return rom.prg_rom[_addr];
        }
    }
}

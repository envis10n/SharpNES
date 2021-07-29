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
        byte[] prg_rom;
        PPU ppu;
        uint cycles = 0;
        static ushort RAM = 0x0000;
        static ushort RAM_MIRRORS_END = 0x1FFF;
        static ushort PPU_REGISTERS = 0x2000;
        static ushort PPU_REGISTERS_MIRRORS_END = 0x3FFF;
        public Bus(Rom rom)
        {
            ppu = new PPU(rom.chr_rom, rom.screen_mirroring);
            prg_rom = rom.prg_rom;
        }
        public static BusAccessType GetAccessType(ushort addr)
        {
            if (addr >= RAM && addr <= RAM_MIRRORS_END) return BusAccessType.RAM;
            else if (addr >= PPU_REGISTERS && addr <= PPU_REGISTERS_MIRRORS_END) return BusAccessType.PPU;
            else if (addr >= 0x8000 && addr <= 0xFFFF) return BusAccessType.ROM;
            else return BusAccessType.NONE;
        }
        public void Tick(byte cycles)
        {
            this.cycles += cycles;
            ppu.Tick((byte)(cycles * 3));
        }
        public byte? PollNMIStatus()
        {
            return ppu.nmi_interrupt;
        }
        public void MemWrite(ushort addr, byte data)
        {
            BusAccessType accessType = GetAccessType(addr);
            switch (accessType)
            {
                case BusAccessType.NONE:
                    Console.WriteLine($"Ignoring memory access at 0x{addr.ToString("X4")}");
                    break;
                case BusAccessType.RAM:
                    {
                        ushort mirror_down_addr = (ushort)(addr & 0b00000111_11111111);
                        cpu_vram[mirror_down_addr] = data;
                        break;
                    }
                case BusAccessType.PPU:
                    {
                        switch (addr)
                        {
                            case 0x2000:
                                ppu.WriteToCtrl(data);
                                break;
                            case 0x2001:
                                ppu.WriteToMask(data);
                                break;
                            case 0x2002:
                                throw new Exception("Attempt to write to PPU status register.");
                            case 0x2003:
                                ppu.WriteToOAMAddr(data);
                                break;
                            case 0x2004:
                                ppu.WriteToOAMData(data);
                                break;
                            case 0x2005:
                                ppu.WriteToScroll(data);
                                break;
                            case 0x2006:
                                ppu.WriteToPPUAddr(data);
                                break;
                            case 0x2007:
                                ppu.WriteToData(data);
                                break;
                            default:
                                ushort mirror_down_addr = (ushort)(addr & 0b00100000_00000111);
                                MemWrite(mirror_down_addr, data);
                                break;
                        }
                        break;
                    }
                case BusAccessType.ROM:
                    Console.WriteLine($"Ignoring memory access at 0x{addr.ToString("X4")}");
                    break;
                    //throw new Exception("Attempt to access cartridge ROM space.");
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
                    Console.WriteLine($"Ignoring memory access at 0x{addr.ToString("X4")}");
                    return 0;
                case BusAccessType.RAM:
                    {
                        ushort mirror_down_addr = (ushort)(addr & 0b00000111_11111111);
                        return cpu_vram[mirror_down_addr];
                    }
                case BusAccessType.PPU:
                    {
                        switch (addr)
                        {
                            case 0x2000:
                            case 0x2001:
                            case 0x2003:
                            case 0x2005:
                            case 0x2006:
                            case 0x4014:
                                throw new Exception($"Attempt to read from write-only PPU address {addr.ToString("X4")}");
                            case 0x2002:
                                return ppu.ReadStatus();
                            case 0x2004:
                                return ppu.ReadOAMData();
                            case 0x2007:
                                return ppu.ReadData();
                            case 0x2008:
                                {
                                    ushort mirror_down_addr = (ushort)(addr & 0b00100000_00000111);
                                    return MemRead(mirror_down_addr);
                                }
                        }
                        Console.WriteLine(string.Format("Ignoring mem access at {0:X4}", addr));
                        return 0;
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
            if (prg_rom.Length == 0x4000 && _addr >= 0x4000)
            {
                _addr = (ushort)(_addr % 0x4000);
            }
            return prg_rom[_addr];
        }
    }
}

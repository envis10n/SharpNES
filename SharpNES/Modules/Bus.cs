using System;

namespace SharpNES.Modules
{
    public enum BusAccessType
    {
        NONE,
        RAM,
        PPU,
        ROM,
        APU,
        JOYPAD1,
        JOYPAD2,
    }
    public class Bus : IMemory
    {
        byte[] cpu_vram = new byte[2048];
        byte[] prg_rom;
        PPU ppu;
        JoypadMultiplexer joypads = new JoypadMultiplexer();
        uint cycles = 0;
        static ushort RAM = 0x0000;
        static ushort RAM_MIRRORS_END = 0x1FFF;
        static ushort PPU_REGISTERS_MIRRORS_END = 0x3FFF;

        Action<PPU, JoypadMultiplexer> gameloop_callback;
        public Bus(Rom rom, Action<PPU, JoypadMultiplexer> gameloop)
        {
            ppu = new PPU(rom.chr_rom, rom.screen_mirroring);
            prg_rom = rom.prg_rom;
            gameloop_callback = gameloop;
        }
        public void Tick(byte cycles)
        {
            this.cycles += cycles;
            bool nmi_before = ppu.nmi_interrupt.HasValue;
            ppu.Tick((byte)(cycles * 3));
            bool nmi_after = ppu.nmi_interrupt.HasValue;
            if (!nmi_before && nmi_after)
            {
                gameloop_callback(ppu, joypads);
            }
        }
        public byte? PollNMIStatus()
        {
            return ppu.PollNMI_Interrupt();
        }
        public void MemWrite(ushort addr, byte data)
        {
            if (addr.InRange(RAM, RAM_MIRRORS_END))
            {
                ushort mirror_down_addr = (ushort)(addr & 0b11111111111);
                cpu_vram[mirror_down_addr] = data;
            }
            else if (addr == 0x2000) ppu.WriteToCtrl(data);
            else if (addr == 0x2001) ppu.WriteToMask(data);
            else if (addr == 0x2002) throw new Exception("Attempt to write to PPU status register!");
            else if (addr == 0x2003) ppu.WriteToOAMAddr(data);
            else if (addr == 0x2004) ppu.WriteToOAMData(data);
            else if (addr == 0x2005) ppu.WriteToScroll(data);
            else if (addr == 0x2006) ppu.WriteToPPUAddr(data);
            else if (addr == 0x2007) ppu.WriteToData(data);
            else if (addr.InRange(0x4000, 0x4013) || addr == 0x4015) { } // Ignore APU
            else if (addr == 0x4016) joypads.Write(data);
            else if (addr == 0x4017) { } // Ignore APU Write-Only
            else if (addr == 0x4014)
            {
                byte[] buffer = new byte[256];
                ushort _data = data;
                ushort hi = (ushort)(_data << 8);
                for (ushort i = 0; i < 256; i++)
                {
                    buffer[i] = MemRead((ushort)(hi + i));
                }
                ppu.WriteOamDMA(buffer);
            }
            else if (addr.InRange(0x2008, PPU_REGISTERS_MIRRORS_END))
            {
                ushort mirror_down_addr = (ushort)(addr & 0b00100000_00000111);
                MemWrite(mirror_down_addr, data);
            }
            else if (addr.InRange(0x8000, 0xFFFF)) throw new Exception("Attempt to write to Cartridge ROM space");
            else Console.WriteLine(string.Format("Ignoring memory write-access at {0:X4}", addr));
        }
        public byte MemRead(ushort addr)
        {
            if (addr.InRange(RAM, RAM_MIRRORS_END))
            {
                ushort mirror_down_addr = (ushort)(addr & 0b00000111_11111111);
                return cpu_vram[mirror_down_addr];
            }
            else if (addr.Matches(0x2000 | 0x2001 | 0x2003 | 0x2005 | 0x2006 | 0x4014)) return 0; // ignore PPU write-only
            else if (addr == 0x2002) return ppu.ReadStatus();
            else if (addr == 0x2004) return ppu.ReadOAMData();
            else if (addr == 0x2007) return ppu.ReadData();
            else if (addr.InRange(0x4000, 0x4015)) return 0; // Ignore APU
            else if (addr == 0x4016) return joypads.Read(0);
            else if (addr == 0x4017) return joypads.Read(1);
            else if (addr.InRange(0x2008, PPU_REGISTERS_MIRRORS_END))
            {
                ushort mirror_down_addr = (ushort)(addr & 0b00100000_00000111);
                return MemRead(mirror_down_addr);
            }
            else if (addr.InRange(0x8000, 0xFFFF)) return ReadPrgRom(addr);
            else
            {
                Console.WriteLine(string.Format("Ignoring memory access at {0:X4}", addr));
                return 0;
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

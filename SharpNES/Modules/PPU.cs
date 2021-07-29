using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNES.Modules
{
    using PPURegisters;
    public class PPU
    {
        public byte[] chr_rom;
        public byte[] palette_table = new byte[32];
        public byte[] vram = new byte[2048];
        public byte oam_addr = 0;
        public byte[] oam_data = new byte[256];
        public ushort scanline = 0;
        public uint cycles = 0;
        public byte? nmi_interrupt = null;
        public Mirroring mirroring;
        public AddrRegister addr = new AddrRegister() { hi_ptr = true, value = new Tuple<byte, byte>(0, 0) };
        public ControlRegister ctrl = ControlRegister.NONE;
        public MaskRegister mask = MaskRegister.NONE;
        public StatusRegister status = StatusRegister.NONE;
        public ScrollRegister scroll = new ScrollRegister() { latch = false, scroll_x = 0, scroll_y = 0 };
        public byte internal_data_buf = 0;
        public PPU(byte[] _chr_rom, Mirroring _mirroring)
        {
            chr_rom = _chr_rom;
            mirroring = _mirroring;
        }
        public static PPU NewEmptyRom()
        {
            return new PPU(new byte[2048], Mirroring.HORIZONTAL);
        }
        public byte? PollNMI_Interrupt()
        {
            return nmi_interrupt;
        }
        public bool Tick(byte cycles)
        {
            this.cycles += cycles;
            if (this.cycles >= 341)
            {
                this.cycles = this.cycles - 341;
                scanline++;

                if (scanline == 241)
                {
                    if (ctrl.GenerateVBlankNMI())
                    {
                        status.SetVBlankStatus(true);
                        Console.WriteLine("TODO: Trigger NMI Interrupt");
                    }
                }

                if (scanline >= 262)
                {
                    scanline = 0;
                    status.ResetVBlankStatus();
                    return true;
                }
            }
            return false;
        }
        public void WriteToCtrl(byte data)
        {
            ctrl.Update(data);
        }
        public void WriteToMask(byte value)
        {
            mask.Update(value);
        }
        public void WriteToPPUAddr(byte data)
        {
            addr.Update(data);
        }
        public byte ReadStatus()
        {
            byte data = status.Snapshot();
            status.ResetVBlankStatus();
            addr.ResetLatch();
            scroll.ResetLatch();
            return data;
        }
        public void WriteToOAMAddr(byte value)
        {
            oam_addr = value;
        }
        public void WriteToOAMData(byte value)
        {
            oam_data[oam_addr] = value;
            oam_addr++;
        }
        public byte ReadOAMData()
        {
            return oam_data[oam_addr];
        }
        public void WriteToScroll(byte value)
        {
            scroll.Write(value);
        }
        public void WriteToData(byte value)
        {
            ushort _addr = addr.Get();
            if (_addr.InRange(0, 0x1fff)) Console.WriteLine(string.Format("Attempt to write to chr rom space {0:X4}", _addr));
            else if (_addr.InRange(0x2000, 0x2fff)) vram[MirrorVRAMAddr(_addr)] = value;
            else if (_addr.InRange(0x3000, 0x3eff)) Console.WriteLine(string.Format("{0:X4} should not be used.", _addr));
            else if (_addr.Matches(0x3f10, 0x3f14, 0x3f18, 0x3f1c))
            {
                ushort add_mirror = (ushort)(_addr - 0x10);
                palette_table[(ushort)(add_mirror - 0x3f00)] = value;
            }
            else if (_addr.InRange(0x3f00, 0x3fff)) palette_table[(ushort)(_addr - 0x3f00)] = value;
            else throw new Exception(string.Format("Unexpected access to mirrored space {0:X4}", _addr));
        }
        public void IncrementVRAMAddr()
        {
            addr.Increment(ctrl.VRAMAddrIncrement());
        }
        public ushort MirrorVRAMAddr(ushort addr)
        {
            ushort mirrored_vram = (ushort)(addr & 0b10111111111111);
            ushort vram_index = (ushort)(mirrored_vram - 0x2000);
            ushort name_table = (ushort)(vram_index / 0x400);
            if (mirroring == Mirroring.VERTICAL && (name_table == 2 || name_table == 3))
            {
                return (ushort)(vram_index - 0x800);
            }
            else if (mirroring == Mirroring.HORIZONTAL)
            {
                if (name_table == 2 || name_table == 1) return (ushort)(vram_index - 0x400);
                else if (name_table == 3) return (ushort)(vram_index - 0x800);
            }
            return vram_index;
        }
        public byte ReadData()
        {
            ushort _addr = addr.Get();
            IncrementVRAMAddr();

            if (_addr.InRange(0, 0x1fff))
            {
                byte result = internal_data_buf;
                internal_data_buf = chr_rom[_addr];
                return result;
            }
            else if (_addr.InRange(0x2000, 0x2fff))
            {
                byte result = internal_data_buf;
                internal_data_buf = vram[MirrorVRAMAddr(_addr)];
                return result;
            }
            else if (_addr >= 0x3000 && _addr <= 0x3eff) { throw new Exception("Addr space 0x3000..0x3eff is not expected to be used."); }
            else if (_addr.Matches(0x3f10, 0x3f14, 0x3f18, 0x3f1c))
            {
                ushort add_mirror = (ushort)(_addr - 0x10);
                return palette_table[(ushort)(add_mirror - 0x3f00)];
            }
            else if (_addr.InRange(0x3f00, 0x3fff))
            {
                return palette_table[(_addr - 0x3f00)];
            }
            throw new Exception("Unexpected access to mirrored space");
        }
        public void WriteOamDMA(byte[] data)
        {
            foreach (byte x in data)
            {
                oam_data[oam_addr] = x;
                oam_addr++;
            }
        }
    }
}

using System;

namespace SharpNES.Modules
{
    public enum Mirroring
    {
        VERTICAL,
        HORIZONTAL,
        FOUR_SCREEN,
    }
    public class Rom
    {
        public static byte[] NES_TAG = new byte[] { 0x4E, 0x45, 0x53, 0x1A };
        public static uint PRG_ROM_PAGE_SIZE = 16384;
        public static uint CHR_ROM_PAGE_SIZE = 8192;
        public byte[] prg_rom;
        public byte[] chr_rom;
        public byte mapper;
        public Mirroring screen_mirroring;
        static bool ContainsNESTag(byte[] v)
        {
            if (v.Length != 4) return false;
            for (uint i = 0; i < 4; i++)
            {
                if (v[i] != NES_TAG[i]) return false;
            }
            return true;
        }
        public Rom(byte[] raw)
        {
            if (!ContainsNESTag(raw[0..4]))
            {
                throw new Exception("File is not in iNES file format");
            }

            byte _mapper = (byte)((byte)(raw[7] & 0b1111_0000) | (byte)(raw[6] >> 4));
            byte ines_ver = (byte)((byte)(raw[7] >> 2) & 0b11);
            if (ines_ver != 0)
            {
                throw new Exception("NES2.0 format is not supported");
            }

            bool four_screen = (byte)(raw[6] & 0b1000) != 0;
            bool vertical_mirroring = (byte)(raw[6] & 0b1) != 0;
            Mirroring mirroring;
            if (four_screen) mirroring = Mirroring.FOUR_SCREEN;
            else if (vertical_mirroring) mirroring = Mirroring.VERTICAL;
            else mirroring = Mirroring.HORIZONTAL;

            ushort prg_rom_size = (ushort)(raw[4] * PRG_ROM_PAGE_SIZE);
            ushort chr_rom_size = (ushort)(raw[5] * CHR_ROM_PAGE_SIZE);

            bool skip_trainer = (byte)(raw[6] & 0b100) != 0;

            ushort prg_rom_start = 16;
            if (skip_trainer) prg_rom_start += 512;
            ushort chr_rom_start = (ushort)(prg_rom_start + prg_rom_size);

            prg_rom = raw[prg_rom_start..(prg_rom_start + prg_rom_size)];
            chr_rom = raw[chr_rom_start..(chr_rom_start + chr_rom_size)];
            mapper = _mapper;
            screen_mirroring = mirroring;
        }
    }
}

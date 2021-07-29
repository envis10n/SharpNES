using System;

namespace SharpNES.Modules.PPURegisters
{
    [Flags]
    public enum ControlRegister : byte
    {
        NONE = 0b00000000,
        NAMETABLE1 = 0b00000001,
        NAMETABLE2 = 0b00000010,
        VRAM_ADD_INCREMENT = 0b00000100,
        SPRITE_PATTERN_ADDR = 0b00001000,
        BACKROUND_PATTERN_ADDR = 0b00010000,
        SPRITE_SIZE = 0b00100000,
        MASTER_SLAVE_SELECT = 0b01000000,
        GENERATE_NMI = 0b10000000,
    }
    public static class ControlRegisterExtensions
    {
        public static byte VRAMAddrIncrement(this ControlRegister ctrl)
        {
            if (!ctrl.HasFlag(ControlRegister.VRAM_ADD_INCREMENT)) return 1;
            return 32;
        }
        public static ushort SprtPatternAddr(this ControlRegister ctrl)
        {
            if (!ctrl.HasFlag(ControlRegister.SPRITE_PATTERN_ADDR)) return 0;
            return 0x1000;
        }
        public static ushort BkndPatternAddr(this ControlRegister ctrl)
        {
            if (!ctrl.HasFlag(ControlRegister.BACKROUND_PATTERN_ADDR)) return 0;
            return 0x1000;
        }
        public static byte SpriteSize(this ControlRegister ctrl)
        {
            if (!ctrl.HasFlag(ControlRegister.SPRITE_SIZE)) return 8;
            return 16;
        }
        public static byte MasterSlaveSelect(this ControlRegister ctrl)
        {
            if (!ctrl.HasFlag(ControlRegister.MASTER_SLAVE_SELECT)) return 0;
            return 1;
        }
        public static bool GenerateVBlankNMI(this ControlRegister ctrl)
        {
            return ctrl.HasFlag(ControlRegister.GENERATE_NMI);
        }
        public static void Update(this ref ControlRegister ctrl, byte data)
        {
            ctrl = (ControlRegister)data;
        }
    }
}

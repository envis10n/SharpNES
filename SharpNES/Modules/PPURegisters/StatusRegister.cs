using System;

namespace SharpNES.Modules.PPURegisters
{
    [Flags]
    public enum StatusRegister : byte
    {
        NONE = 0b00000000,
        NOTUSED = 0b00000001,
        NOTUSED2 = 0b00000010,
        NOTUSED3 = 0b00000100,
        NOTUSED4 = 0b00001000,
        NOTUSED5 = 0b00010000,
        SPRITE_OVERFLOW = 0b00100000,
        SPRITE_ZERO_HIT = 0b01000000,
        VBLANK_STARTED = 0b10000000,
}
    public static class StatusRegisterExtensions
    {
        public static void SetVBlankStatus(this ref StatusRegister self, bool status)
        {
            self = (StatusRegister)BitMask.Set((byte)self, (byte)StatusRegister.VBLANK_STARTED, status);
        }
        public static void SetSpriteZeroHit(this ref StatusRegister self, bool status)
        {
            self = (StatusRegister)BitMask.Set((byte)self, (byte)StatusRegister.SPRITE_ZERO_HIT, status);
        }
        public static void SetSpriteOverflow(this ref StatusRegister self, bool status)
        {
            self = (StatusRegister)BitMask.Set((byte)self, (byte)StatusRegister.SPRITE_OVERFLOW, status);
        }
        public static void ResetVBlankStatus(this ref StatusRegister self)
        {
            self = (StatusRegister)BitMask.UnSet((byte)self, (byte)StatusRegister.VBLANK_STARTED);
        }
        public static bool IsInVBlank(this StatusRegister self)
        {
            return self.HasFlag(StatusRegister.VBLANK_STARTED);
        }
        public static byte Snapshot(this StatusRegister self)
        {
            return (byte)self;
        }
    }
}

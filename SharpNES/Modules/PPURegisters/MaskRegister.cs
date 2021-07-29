using System;
using System.Collections.Generic;

namespace SharpNES.Modules.PPURegisters
{
    [Flags]
    public enum MaskRegister : byte
    {
        NONE = 0b00000000,
        GREYSCALE = 0b00000001,
        LEFTMOST_8PXL_BACKGROUND = 0b00000010,
        LEFTMOST_8PXL_SPRITE = 0b00000100,
        SHOW_BACKGROUND = 0b00001000,
        SHOW_SPRITES = 0b00010000,
        EMPHASISE_RED = 0b00100000,
        EMPHASISE_GREEN = 0b01000000,
        EMPHASISE_BLUE = 0b10000000,
    }
    public enum MaskColor
    {
        Red,
        Green,
        Blue,
    }
    public static class MaskRegisterExtensions
    {
        public static MaskRegister New()
        {
            return MaskRegister.NONE;
        }
        public static bool IsGrayscale(this MaskRegister mask)
        {
            return mask.HasFlag(MaskRegister.GREYSCALE);
        }
        public static bool LeftMost8pxlBackground(this MaskRegister mask)
        {
            return mask.HasFlag(MaskRegister.LEFTMOST_8PXL_BACKGROUND);
        }
        public static bool LeftMost8pxlSprite(this MaskRegister mask)
        {
            return mask.HasFlag(MaskRegister.LEFTMOST_8PXL_SPRITE);
        }
        public static bool ShowBackground(this MaskRegister mask)
        {
            return mask.HasFlag(MaskRegister.SHOW_BACKGROUND);
        }
        public static bool ShowSprites(this MaskRegister mask)
        {
            return mask.HasFlag(MaskRegister.SHOW_SPRITES);
        }
        public static MaskColor[] Emphasise(this MaskRegister mask)
        {
            List<MaskColor> result = new List<MaskColor>();
            if (mask.HasFlag(MaskRegister.EMPHASISE_RED)) result.Add(MaskColor.Red);
            if (mask.HasFlag(MaskRegister.EMPHASISE_BLUE)) result.Add(MaskColor.Blue);
            if (mask.HasFlag(MaskRegister.EMPHASISE_GREEN)) result.Add(MaskColor.Green);
            return result.ToArray();
        }
        public static void Update(this ref MaskRegister mask, byte data)
        {
            mask = (MaskRegister)data;
        }
    }
}

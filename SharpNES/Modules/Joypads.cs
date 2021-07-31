using System;

namespace SharpNES.Modules
{
    [Flags]
    public enum JoypadButton : byte
    {
        NONE = 0b00000000,
        RIGHT = 0b10000000,
        LEFT = 0b01000000,
        DOWN = 0b00100000,
        UP = 0b00010000,
        START = 0b00001000,
        SELECT = 0b00000100,
        BUTTON_B = 0b00000010,
        BUTTON_A = 0b00000001,
    }
    public static class JoypadButtonExtenstions
    {
        public static void Set(this ref JoypadButton button, JoypadButton flag)
        {
            button |= flag;
        }
        public static void UnSet(this ref JoypadButton button, JoypadButton flag)
        {
            button &= ~flag;
        }
        public static void Set(this ref JoypadButton button, JoypadButton flag, bool state)
        {
            if (state) button.Set(flag);
            else button.UnSet(flag);
        }
    }
    public class Joypad
    {
        public bool strobe = false;
        public byte button_index = 0;
        public JoypadButton button_status = JoypadButton.NONE;
        public void Write(byte data)
        {
            strobe = (byte)(data & 1) == 1;
            if (strobe) button_index = 0;
        }
        public byte Read()
        {
            if (button_index > 7) return 1;
            byte response = (byte)(((byte)button_status & (byte)(1 << button_index)) >> button_index);
            if (!strobe && button_index <= 7) button_index++;
            return response;
        }
        public void SetButtonPressedStatus(JoypadButton button, bool status)
        {
            button_status.Set(button, status);
        }
    }
    public class JoypadMultiplexer
    {
        public Joypad[] ports = new Joypad[] { new Joypad(), new Joypad() };
        public Joypad this[uint index]
        {
            get {
                if (index == 2) index = 0;
                else if (index == 3) index = 1;
                return ports[index];
            }
        }
        public byte Read(uint port)
        {
            return this[port].Read();
        }
        public void Write(byte data)
        {
            foreach (Joypad pad in ports)
            {
                pad.Write(data);
            }
        }
        public void SetButtonPressed(uint port, JoypadButton button, bool status)
        {
            this[port].SetButtonPressedStatus(button, status);
        }
    }
}

using System;
using System.Collections.Generic;
using SFML.Window;
using SFML.Graphics;
using SFML.System;
using System.Threading;

namespace SharpNES
{
    using Modules;
    using Modules.PPURegisters;
    using Render;
    class Program
    {
        static float GAME_SCALE = 3.0f;
        static void Render(PPU ppu, Frame frame)
        {
            ushort bg_bank = ppu.ctrl.BkndPatternAddr();
            // Render Background
            for (uint i = 0; i < 0x3c0; i++)
            {
                ushort tile = ppu.vram[i];
                uint tile_x = i % 32;
                uint tile_y = i / 32;
                byte[] bg_tile = ppu.chr_rom[(bg_bank + tile * 16)..(bg_bank + tile * 16 + 16)];
                byte[] pallete = Pallete.BGPallete(ppu, tile_x, tile_y);

                for (int y = 0; y <= 7; y++)
                {
                    byte upper = bg_tile[y];
                    byte lower = bg_tile[y + 8];

                    for (int x = 7; x >= 0; x--)
                    {
                        byte value = (byte)((byte)((byte)(1 & lower) << 1) | (byte)(1 & upper));
                        upper = (byte)(upper >> 1);
                        lower = (byte)(lower >> 1);
                        (byte, byte, byte) rgb;
                        if (value == 0) rgb = Pallete.SYSTEM_PALLETE[ppu.palette_table[0]];
                        else if (value == 1) rgb = Pallete.SYSTEM_PALLETE[pallete[1]];
                        else if (value == 2) rgb = Pallete.SYSTEM_PALLETE[pallete[2]];
                        else if (value == 3) rgb = Pallete.SYSTEM_PALLETE[pallete[3]];
                        else throw new Exception("Invalid pallete value.");

                        frame.SetPixel((uint)(tile_x * 8 + x), (uint)(tile_y * 8 + y), rgb);
                    }
                }
            }
            // Render Sprites
            int oam_index = ppu.oam_data.Length - 4;
            while (oam_index >= 0)
            {
                ushort tile_idx = ppu.oam_data[oam_index + 1];
                uint tile_x = ppu.oam_data[oam_index + 3];
                uint tile_y = ppu.oam_data[oam_index];

                bool flip_vertical = ((ppu.oam_data[oam_index + 2] >> 7) & 1) == 1;
                bool flip_horizontal = ((ppu.oam_data[oam_index + 2] >> 6) & 1) == 1;
                uint pallete_idx = (byte)(ppu.oam_data[oam_index + 2] & 0b11);
                byte[] sprite_pallete = Pallete.SpritePallete(ppu, (byte)pallete_idx);
                ushort sprite_bank = ppu.ctrl.SprtPatternAddr();

                byte[] sprite_tile = ppu.chr_rom[(sprite_bank + tile_idx * 16)..(sprite_bank + tile_idx * 16 + 15 + 1)];

                for (uint y = 0; y <= 7; y++)
                {
                    byte upper = sprite_tile[y];
                    byte lower = sprite_tile[y + 8];
                    for (int x = 7; x >= 0; x--)
                    {
                        byte value = (byte)((byte)((byte)(1 & lower) << 1) | (byte)(1 & upper));
                        upper = (byte)(upper >> 1);
                        lower = (byte)(lower >> 1);
                        (byte, byte, byte) rgb;
                        if (value == 0) continue;
                        else if (value == 1) rgb = Pallete.SYSTEM_PALLETE[sprite_pallete[1]];
                        else if (value == 2) rgb = Pallete.SYSTEM_PALLETE[sprite_pallete[2]];
                        else if (value == 3) rgb = Pallete.SYSTEM_PALLETE[sprite_pallete[3]];
                        else throw new Exception("Invalid pallete value.");
                        switch ((flip_horizontal, flip_vertical))
                        {
                            case (false, false):
                                frame.SetPixel(tile_x + (uint)x, tile_y + y, rgb);
                                break;
                            case (true, false):
                                frame.SetPixel(tile_x + 7 - (uint)x, tile_y + y, rgb);
                                break;
                            case (false, true):
                                frame.SetPixel(tile_x + (uint)x, tile_y + 7 - y, rgb);
                                break;
                            case (true, true):
                                frame.SetPixel(tile_x + 7 - (uint)x, tile_y + 7 - y, rgb);
                                break;
                        }
                    }
                }
                oam_index -= 4;
            }
        }
        static Dictionary<Keyboard.Key, JoypadButton> key_map = new Dictionary<Keyboard.Key, JoypadButton>()
        {
            {Keyboard.Key.Down, JoypadButton.DOWN },
            {Keyboard.Key.Up, JoypadButton.UP },
            {Keyboard.Key.Right, JoypadButton.RIGHT },
            {Keyboard.Key.Left, JoypadButton.LEFT },
            {Keyboard.Key.Space, JoypadButton.SELECT },
            {Keyboard.Key.Enter, JoypadButton.START },
            {Keyboard.Key.A, JoypadButton.BUTTON_A },
            {Keyboard.Key.S, JoypadButton.BUTTON_B },
        };
        struct KeyEvent
        {
            public bool pressed { get; }
            public JoypadButton button { get; }
            public KeyEvent(JoypadButton button, bool pressed)
            {
                this.button = button;
                this.pressed = pressed;
            }
        }
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                throw new Exception("No ROM filepath provided.");
            }
            string NES_ROM_PATH = args[0];
            RenderWindow window = new RenderWindow(new VideoMode(256 * (uint)GAME_SCALE, 240 * (uint)GAME_SCALE), NES_ROM_PATH);
            Texture texture = new Texture(256, 240);
            Sprite sprite = new Sprite(texture);
            sprite.Scale = new Vector2f(GAME_SCALE, GAME_SCALE);

            Queue<KeyEvent> keyboard_events = new Queue<KeyEvent>();
            Mutex QueueLock = new Mutex();

            byte[] bytes = System.IO.File.ReadAllBytes(NES_ROM_PATH);
            Rom rom = new Rom(bytes);

            Frame frame = Frame.New();

            //keyboard_events.Enqueue(new KeyEvent(JoypadButton.UP, true));

            Bus bus = new Bus(rom, (ppu, joypad1) =>
            {
                window.Clear();
                Render(ppu, frame);
                texture.Update(frame.data);
                window.Draw(sprite);
                window.Display();
                window.DispatchEvents();
                QueueLock.WaitOne();
                if (keyboard_events.Count > 0)
                {
                    while (keyboard_events.TryDequeue(out KeyEvent ev))
                    {
                        joypad1.SetButtonPressedStatus(ev.button, ev.pressed);
                    }
                }
                QueueLock.ReleaseMutex();
            });

            window.KeyPressed += (sender, args) =>
            {
                if (args.Code == Keyboard.Key.Escape)
                {
                    window.Close();
                    Environment.Exit(0);
                }
                else if (key_map.TryGetValue(args.Code, out JoypadButton btn))
                {
                    QueueLock.WaitOne();
                    keyboard_events.Enqueue(new KeyEvent(btn, true));
                    QueueLock.ReleaseMutex();
                }
            };

            window.KeyReleased += (sender, args) =>
            {
                if (key_map.TryGetValue(args.Code, out JoypadButton btn))
                {
                    QueueLock.WaitOne();
                    keyboard_events.Enqueue(new KeyEvent(btn, false));
                    QueueLock.ReleaseMutex();
                }
            };

            window.Closed += (sender, args) =>
            {
                window.Close();
                Environment.Exit(0);
            };

            CPU cpu = new CPU(bus);
            cpu.Reset();
            cpu.Run();
        }
    }
}

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
        static float AXIS_DEADZONE = 10.0f;
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
        /* 
            XInput A - 0
            XInput B - 1
            XInput X - 2
            XInput Y - 3
            XInput RB - 5
            XInput LB - 4
            XInput RSB - 9
            XInput LSB - 8
            XInput Start - 7
            XInput Select - 6
            XInput PoVY = DPAD DOWN/UP = -100:0:100
            XInput PoVX = DPAD LEFT/RIGHT = -100:0:100
            XInput U = RS = UP/DOWN
            XInput V = RS = LEFT/RIGHT
            XInput X = LS = UP/DOWN
            XInput Y = LS = LEFT/RIGHT
        */
        enum GamepadButton : uint
        {
            A = 0,
            B = 1,
            X = 2,
            Y = 3,
            LB = 4,
            RB = 5,
            SELECT = 6,
            START = 7,
            LSB = 8,
            RSB = 9
        }
        static Dictionary<Keyboard.Key, JoypadButton> key_map_plr1 = new Dictionary<Keyboard.Key, JoypadButton>()
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
        static Dictionary<Keyboard.Key, JoypadButton> key_map_plr2 = new Dictionary<Keyboard.Key, JoypadButton>()
        {
            {Keyboard.Key.Numpad2, JoypadButton.DOWN },
            {Keyboard.Key.Numpad8, JoypadButton.UP },
            {Keyboard.Key.Numpad6, JoypadButton.RIGHT },
            {Keyboard.Key.Numpad4, JoypadButton.LEFT },
            {Keyboard.Key.C, JoypadButton.SELECT },
            {Keyboard.Key.V, JoypadButton.START },
            {Keyboard.Key.Z, JoypadButton.BUTTON_A },
            {Keyboard.Key.X, JoypadButton.BUTTON_B },
        };
        static Dictionary<GamepadButton, JoypadButton> gamepad_map = new Dictionary<GamepadButton, JoypadButton>()
        {
            {GamepadButton.A, JoypadButton.BUTTON_A },
            {GamepadButton.X, JoypadButton.BUTTON_B },
            {GamepadButton.Y, JoypadButton.BUTTON_A },
            {GamepadButton.B, JoypadButton.BUTTON_B },
            {GamepadButton.START, JoypadButton.START },
            {GamepadButton.SELECT, JoypadButton.SELECT },
        };
        struct KeyEvent
        {
            public uint index { get; }
            public bool pressed { get; }
            public JoypadButton button { get; }
            public KeyEvent(uint index, JoypadButton button, bool pressed)
            {
                this.index = index;
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
            Joystick.Update();
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

            Bus bus = new Bus(rom, (ppu, joypads) =>
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
                        joypads.SetButtonPressed(ev.index, ev.button, ev.pressed);
                    }
                }
                QueueLock.ReleaseMutex();
            });

            if (Joystick.IsConnected(0)) bus.SetJoypadConnected(0, true);
            if (Joystick.IsConnected(1)) bus.SetJoypadConnected(1, true);

            window.JoystickButtonPressed += (sender, args) =>
            {
                GamepadButton button = (GamepadButton)args.Button;
                if (gamepad_map.TryGetValue(button, out JoypadButton btn))
                {
                    QueueLock.WaitOne();
                    keyboard_events.Enqueue(new KeyEvent(args.JoystickId, btn, true));
                    QueueLock.ReleaseMutex();
                }
            };

            window.JoystickButtonReleased += (sender, args) =>
            {
                GamepadButton button = (GamepadButton)args.Button;
                if (gamepad_map.TryGetValue(button, out JoypadButton btn))
                {
                    QueueLock.WaitOne();
                    keyboard_events.Enqueue(new KeyEvent(args.JoystickId, btn, false));
                    QueueLock.ReleaseMutex();
                }
            };

            Dictionary<(uint, Joystick.Axis), float> gamepad_positions = new Dictionary<(uint, Joystick.Axis), float>()
            {
                {(0, Joystick.Axis.PovX), 0.0f },
                {(1, Joystick.Axis.PovX), 0.0f },
                {(0, Joystick.Axis.PovY), 0.0f },
                {(1, Joystick.Axis.PovY), 0.0f },
                {(0, Joystick.Axis.X), 0.0f },
                {(1, Joystick.Axis.X), 0.0f },
                {(0, Joystick.Axis.Y), 0.0f },
                {(1, Joystick.Axis.Y), 0.0f },
            };

            Dictionary<(Joystick.Axis, float), JoypadButton> gamepad_dpad_map = new Dictionary<(Joystick.Axis, float), JoypadButton>()
            {
                {(Joystick.Axis.PovX, 100.0f), JoypadButton.RIGHT },
                {(Joystick.Axis.PovX, 0.0f), JoypadButton.NONE },
                {(Joystick.Axis.PovX, -100.0f), JoypadButton.LEFT },
                {(Joystick.Axis.PovY, 100.0f), JoypadButton.UP },
                {(Joystick.Axis.PovY, 0.0f), JoypadButton.NONE },
                {(Joystick.Axis.PovY, -100.0f), JoypadButton.DOWN },
            };

            window.JoystickMoved += (sender, args) =>
            {
                KeyEvent? ev = null;
                var map_key = (args.JoystickId, args.Axis);
                if (gamepad_positions.TryGetValue(map_key, out float old_position)) {
                    switch (args.Axis)
                    {
                        case Joystick.Axis.PovX:
                        case Joystick.Axis.PovY:
                            float pos = Math.Abs(args.Position) <= AXIS_DEADZONE ? 0 : args.Position > AXIS_DEADZONE ? 100 : -100;
                            JoypadButton old_button = gamepad_dpad_map[(args.Axis, old_position)];
                            JoypadButton new_button = gamepad_dpad_map[(args.Axis, pos)];
                            if (old_button == JoypadButton.NONE && new_button != JoypadButton.NONE)
                            {
                                // Pressed
                                ev = new KeyEvent(args.JoystickId, new_button, true);
                            } else
                            {
                                // Released
                                ev = new KeyEvent(args.JoystickId, old_button, false);
                            }
                            gamepad_positions[map_key] = pos;
                            break;
                        case Joystick.Axis.X:
                            // UP/DOWN
                            break;
                        case Joystick.Axis.Y:
                            // LEFT/RIGHT
                            break;
                    }
                }
                if (ev.HasValue)
                {
                    QueueLock.WaitOne();
                    keyboard_events.Enqueue(ev.Value);
                    QueueLock.ReleaseMutex();
                }
            };

            window.JoystickConnected += (sender, args) =>
            {
                if (args.JoystickId <= 1)
                {
                    Console.WriteLine($"Joypad {args.JoystickId} connected.");
                    bus.SetJoypadConnected(args.JoystickId, true);
                }
            };

            window.JoystickDisconnected += (sender, args) =>
            {
                if (args.JoystickId <= 1)
                {
                    Console.WriteLine($"Joypad {args.JoystickId} disconnected.");
                    bus.SetJoypadConnected(args.JoystickId, false);
                }
            };

            window.KeyPressed += (sender, args) =>
            {
                JoypadButton btn;
                if (args.Code == Keyboard.Key.Escape)
                {
                    window.Close();
                    Environment.Exit(0);
                }
                else if (key_map_plr1.TryGetValue(args.Code, out btn))
                {
                    QueueLock.WaitOne();
                    keyboard_events.Enqueue(new KeyEvent(0, btn, true));
                    QueueLock.ReleaseMutex();
                }
                else if (key_map_plr2.TryGetValue(args.Code, out btn))
                {
                    QueueLock.WaitOne();
                    keyboard_events.Enqueue(new KeyEvent(1, btn, true));
                    QueueLock.ReleaseMutex();
                }
            };

            window.KeyReleased += (sender, args) =>
            {
                JoypadButton btn;
                if (key_map_plr1.TryGetValue(args.Code, out btn))
                {
                    QueueLock.WaitOne();
                    keyboard_events.Enqueue(new KeyEvent(0, btn, false));
                    QueueLock.ReleaseMutex();
                } else if (key_map_plr2.TryGetValue(args.Code, out btn))
                {
                    QueueLock.WaitOne();
                    keyboard_events.Enqueue(new KeyEvent(1, btn, false));
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

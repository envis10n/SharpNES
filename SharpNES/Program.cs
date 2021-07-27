using System;
using SFML.Window;
using SFML.Graphics;
using SFML.System;
using System.Threading;

namespace SharpNES
{
    using Modules;
    class Program
    {
        static float GAME_SCALE = 20.0f;
        static Color GetColor(byte data)
        {
            switch (data)
            {
                case 0: return Color.Black;
                case 1: return Color.White;
                case 2: case 9: return new Color(128, 128, 128);
                case 3: case 10: return Color.Red;
                case 4: case 11: return Color.Green;
                case 5: case 12: return Color.Blue;
                case 6: case 13: return Color.Magenta;
                case 7: case 14: return Color.Yellow;
                default: return Color.Cyan;
            }
        }
        static bool ReadScreenState(CPU cpu, byte[] frame)
        {
            uint frame_idx = 0;
            bool update = false;
            for (ushort i = 0x0200; i < 0x600; i++)
            {
                byte color_idx = cpu.MemRead(i);
                Color clr = GetColor(color_idx);
                byte b1, b2, b3;
                b1 = clr.R;
                b2 = clr.G;
                b3 = clr.B;

                byte f1, f2, f3;
                f1 = frame[frame_idx];
                f2 = frame[frame_idx + 1];
                f3 = frame[frame_idx + 2];

                if (b1 != f1 || b2 != f2 || b3 != f3)
                {
                    frame[frame_idx] = b1;
                    frame[frame_idx + 1] = b2;
                    frame[frame_idx + 2] = b3;
                    frame[frame_idx + 3] = 255;
                    update = true;
                }
                frame_idx += 4;
            }
            return update;
        }
        static void Main(string[] args)
        {
            RenderWindow window = new RenderWindow(new VideoMode(32 * (uint)GAME_SCALE, 32 * (uint)GAME_SCALE), "Snake Game");
            Random rand = new Random();

            byte[] screen_state = new byte[32 * 4 * 32];

            Texture texture = new Texture(32, 32);
            Sprite sprite = new Sprite(texture);
            sprite.Scale = new Vector2f(GAME_SCALE, GAME_SCALE);
            Keyboard.Key? last_key = null;

            window.KeyPressed += (sender, args) =>
            {
                last_key = args.Code;
            };

            window.Closed += (sender, args) =>
            {
                Environment.Exit(0);
            };

            byte[] raw_cart = System.IO.File.ReadAllBytes("snake.nes");
            Rom rom = new Rom(raw_cart);
            Bus bus = new Bus(rom);
            CPU cpu = new CPU(bus);
            cpu.Reset();

            cpu.RunWithCallback((_cpu, code, opcode) =>
            {
                window.DispatchEvents();
                Console.WriteLine($"0x{BitConverter.ToString(new byte[] { code })} {opcode.mnemonic}");
                if (last_key != null)
                {
                    // Handle key
                    switch(last_key)
                    {
                        case Keyboard.Key.Escape:
                            Environment.Exit(0);
                            break;
                        case Keyboard.Key.W:
                            _cpu.MemWrite(0xff, 0x77);
                            break;
                        case Keyboard.Key.S:
                            _cpu.MemWrite(0xff, 0x73);
                            break;
                        case Keyboard.Key.A:
                            _cpu.MemWrite(0xff, 0x61);
                            break;
                        case Keyboard.Key.D:
                            _cpu.MemWrite(0xff, 0x64);
                            break;
                    }
                    last_key = null;
                }
                byte rng = (byte)rand.Next(1, 16);
                _cpu.MemWrite(0xfe, rng);

                if (ReadScreenState(_cpu, screen_state))
                {
                    window.Clear();
                    texture.Update(screen_state);
                    window.Draw(sprite);
                    window.Display();
                }
                Thread.Sleep(1000/1790);
            });
        }
    }
}

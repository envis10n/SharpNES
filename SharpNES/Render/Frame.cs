using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNES.Render
{
    public struct Frame
    {
        public static uint WIDTH = 256;
        public static uint HEIGHT = 240;
        public byte[] data { get; set; }
        public static Frame New()
        {
            return new Frame()
            {
                data = new byte[WIDTH * HEIGHT * 4],
            };
        }
        public void SetPixel(uint x, uint y, (byte, byte, byte) rgb)
        {
            uint ba = (y * WIDTH + x) * 4;
            if ((ba + 3) < data.Length)
            {
                data[ba] = rgb.Item1;
                data[ba + 1] = rgb.Item2;
                data[ba + 2] = rgb.Item3;
                data[ba + 3] = 255;
            }
        }
    }
}

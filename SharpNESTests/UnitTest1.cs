using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNES;
using SharpNES.Modules;

namespace SharpNESTests
{
    struct TestRom
    {
        public byte[] header { get; set; }
        public byte[]? trainer { get; set; }
        public byte[] pgp_rom { get; set; }
        public byte[] chr_rom { get; set; }
    }
    [TestClass]
    public class CPUTests
    {
        [TestMethod]
        public void AddressInRange()
        {
            ushort addr = 0x8000;
            Assert.IsTrue(addr.InRange(0x8000, 0xffff));
            Assert.IsTrue(addr.Matches(0xffff, 0x0000, 0x8000, 0x8fff));
        }
        [TestMethod]
        public void JoypadTest()
        {
            var joypad1 = new Joypad() { strobe = false, button_status = JoypadButton.NONE, button_index = 0 };
            joypad1.Write(0);
            joypad1.SetButtonPressedStatus(JoypadButton.RIGHT, true);
            joypad1.SetButtonPressedStatus(JoypadButton.LEFT, true);
            joypad1.SetButtonPressedStatus(JoypadButton.SELECT, true);
            joypad1.SetButtonPressedStatus(JoypadButton.BUTTON_B, true);

            for (int i = 0; i <= 0; i++)
            {
                Assert.AreEqual(joypad1.Read(), 0);
                Assert.AreEqual(joypad1.Read(), 1);
                Assert.AreEqual(joypad1.Read(), 1);
                Assert.AreEqual(joypad1.Read(), 0);
                Assert.AreEqual(joypad1.Read(), 0);
                Assert.AreEqual(joypad1.Read(), 0);
                Assert.AreEqual(joypad1.Read(), 1);
                Assert.AreEqual(joypad1.Read(), 1);
                for (int i1 = 0; i1 < 10; i1++)
                {
                    Assert.AreEqual(joypad1.Read(), 1);
                }
                joypad1.Write(1);
                joypad1.Write(0);
            }
        }
    }
}

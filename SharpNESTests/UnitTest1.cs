using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        byte[] CreateROM(TestRom rom)
        {
            byte[] result = new byte[
                rom.header.Length
                + (rom.trainer == null ? 0 : rom.trainer.Length)
                + rom.pgp_rom.Length
                + rom.chr_rom.Length];

            return result;
        }
        Rom MakeTestRom()
        {
            TestRom _trom = new TestRom();
            _trom.header = new byte[]
            {
                0x4E, 0x45, 0x53, 0x1A, 0x02, 0x01, 0x31, 00, 00, 00, 00, 00, 00, 00, 00, 00,
            };
            _trom.trainer = null;
            _trom.pgp_rom = new byte[2 * Rom.PRG_ROM_PAGE_SIZE];
            _trom.chr_rom = new byte[Rom.CHR_ROM_PAGE_SIZE];
            System.Array.Fill<byte>(_trom.pgp_rom, 1);
            System.Array.Fill<byte>(_trom.chr_rom, 2);
            byte[] test_rom = CreateROM(_trom);
            return new Rom(test_rom);
        }
        [TestMethod]
        public void TestA9LDAImmediateLoadData()
        {
            var rom = MakeTestRom();
            var bus = new Bus(rom);
            var cpu = new CPU(bus);
            cpu.LoadAndRun(new byte[] { 0xa9, 0x05, 0x00 });
            Assert.AreEqual(cpu.register_a, 0x05);
            Assert.IsTrue((byte)(cpu.status & (CpuFlags)0b0000_0010) == 0b00);
            Assert.IsTrue((byte)(cpu.status & (CpuFlags)0b1000_0000) == 0);
        }
        [TestMethod]
        public void TestA9LDAZeroFlag()
        {
            var rom = MakeTestRom();
            var bus = new Bus(rom);
            var cpu = new CPU(bus);
            cpu.LoadAndRun(new byte[] { 0xa9, 0x00, 0x00 });
            Assert.IsTrue((byte)(cpu.status & (CpuFlags)0b0000_0010) == 0b10);
        }
        [TestMethod]
        public void TestAATAXMoveAToX()
        {
            var rom = MakeTestRom();
            var bus = new Bus(rom);
            var cpu = new CPU(bus);
            cpu.LoadAndRun(new byte[] { 0xa9, 10, 0xaa, 0x00 });
            Assert.AreEqual(cpu.register_x, 10);
        }
        [TestMethod]
        public void Test5OpsWorkingTogether()
        {
            var rom = MakeTestRom();
            var bus = new Bus(rom);
            var cpu = new CPU(bus);
            cpu.LoadAndRun(new byte[] { 0xa9, 0xc0, 0xaa, 0xe8, 0x00 });
            Assert.AreEqual(cpu.register_x, 0xc1);
        }
        [TestMethod]
        public void TestINXOverflow()
        {
            var rom = MakeTestRom();
            var bus = new Bus(rom);
            var cpu = new CPU(bus);
            cpu.LoadAndRun(new byte[] {0xa9, 0xff, 0xaa, 0xe8, 0xe8, 0x00 });
            Assert.AreEqual(cpu.register_x, 1);
        }
        [TestMethod]
        public void TestLDAFromMemory()
        {
            var rom = MakeTestRom();
            var bus = new Bus(rom);
            var cpu = new CPU(bus);
            cpu.MemWrite(0x10, 0x55);
            cpu.LoadAndRun(new byte[] { 0xa5, 0x10, 0x00 });
            Assert.AreEqual(cpu.register_a, 0x55);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNES.Modules;

namespace SharpNESTests
{
    [TestClass]
    public class CPUTests
    {
        [TestMethod]
        public void TestA9LDAImmediateLoadData()
        {
            var cpu = new CPU();
            cpu.LoadAndRun(new byte[] { 0xa9, 0x05, 0x00 });
            Assert.AreEqual(cpu.register_a, 0x05);
            Assert.IsTrue((byte)(cpu.status & (CpuFlags)0b0000_0010) == 0b00);
            Assert.IsTrue((byte)(cpu.status & (CpuFlags)0b1000_0000) == 0);
        }
        [TestMethod]
        public void TestA9LDAZeroFlag()
        {
            var cpu = new CPU();
            cpu.LoadAndRun(new byte[] { 0xa9, 0x00, 0x00 });
            Assert.IsTrue((byte)(cpu.status & (CpuFlags)0b0000_0010) == 0b10);
        }
        [TestMethod]
        public void TestAATAXMoveAToX()
        {
            var cpu = new CPU();
            cpu.LoadAndRun(new byte[] { 0xa9, 10, 0xaa, 0x00 });
            Assert.AreEqual(cpu.register_x, 10);
        }
        [TestMethod]
        public void Test5OpsWorkingTogether()
        {
            var cpu = new CPU();
            cpu.LoadAndRun(new byte[] { 0xa9, 0xc0, 0xaa, 0xe8, 0x00 });
            Assert.AreEqual(cpu.register_x, 0xc1);
        }
        [TestMethod]
        public void TestINXOverflow()
        {
            var cpu = new CPU();
            cpu.LoadAndRun(new byte[] {0xa9, 0xff, 0xaa, 0xe8, 0xe8, 0x00 });
            Assert.AreEqual(cpu.register_x, 1);
        }
        [TestMethod]
        public void TestLDAFromMemory()
        {
            var cpu = new CPU();
            cpu.MemWrite(0x10, 0x55);
            cpu.LoadAndRun(new byte[] { 0xa5, 0x10, 0x00 });
            Assert.AreEqual(cpu.register_a, 0x55);
        }
    }
}

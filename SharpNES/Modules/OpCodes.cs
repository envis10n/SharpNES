using System.Collections.Generic;

namespace SharpNES.Modules
{
    public struct OpCode
    {
        public string mnemonic { get; set; }
        public byte len { get; set; }
        public byte cycles { get; set; }
        public AddressingMode mode { get; set; }
        public OpCode(string mnemonic, byte len, byte cycles, AddressingMode mode)
        {
            this.mnemonic = mnemonic;
            this.len = len;
            this.cycles = cycles;
            this.mode = mode;
        }
    }
    public enum AddressingMode
    {
        Immediate,
        ZeroPage,
        ZeroPage_X,
        ZeroPage_Y,
        Absolute,
        Absolute_X,
        Absolute_Y,
        Indirect_X,
        Indirect_Y,
        NoneAddressing
    }
    class OpCodes
    {
        public static Dictionary<byte, OpCode> OPCODES_MAP = new Dictionary<byte, OpCode>() {
            /* General Ops */
            {0x00, new OpCode("BRK", 1, 7, AddressingMode.NoneAddressing)},
            {0xea, new OpCode("NOP", 1, 2, AddressingMode.NoneAddressing)},

            /* Arithmetic */
            {0x69, new OpCode("ADC", 2, 2, AddressingMode.Immediate)},
            {0x65, new OpCode("ADC", 2, 3, AddressingMode.ZeroPage)},
            {0x75, new OpCode("ADC", 2, 4, AddressingMode.ZeroPage_X)},
            {0x6d, new OpCode("ADC", 3, 4, AddressingMode.Absolute)},
            {0x7d, new OpCode("ADC", 3, 4, AddressingMode.Absolute_X)},
            {0x79, new OpCode("ADC", 3, 4, AddressingMode.Absolute_Y)},
            {0x61, new OpCode("ADC", 2, 6, AddressingMode.Indirect_X)},
            {0x71, new OpCode("ADC", 2, 5, AddressingMode.Indirect_Y)},

            {0xe9, new OpCode("SBC", 2, 2, AddressingMode.Immediate)},
            {0xe5, new OpCode("SBC", 2, 3, AddressingMode.ZeroPage)},
            {0xf5, new OpCode("SBC", 2, 4, AddressingMode.ZeroPage_X)},
            {0xed, new OpCode("SBC", 3, 4, AddressingMode.Absolute)},
            {0xfd, new OpCode("SBC", 3, 4, AddressingMode.Absolute_X)},
            {0xf9, new OpCode("SBC", 3, 4, AddressingMode.Absolute_Y)},
            {0xe1, new OpCode("SBC", 2, 6, AddressingMode.Indirect_X)},
            {0xf1, new OpCode("SBC", 2, 5, AddressingMode.Indirect_Y)},

            {0x29, new OpCode("AND", 2, 2, AddressingMode.Immediate)},
            {0x25, new OpCode("AND", 2, 3, AddressingMode.ZeroPage)},
            {0x35, new OpCode("AND", 2, 4, AddressingMode.ZeroPage_X)},
            {0x2d, new OpCode("AND", 3, 4, AddressingMode.Absolute)},
            {0x3d, new OpCode("AND", 3, 4, AddressingMode.Absolute_X)},
            {0x39, new OpCode("AND", 3, 4, AddressingMode.Absolute_Y)},
            {0x21, new OpCode("AND", 2, 6, AddressingMode.Indirect_X)},
            {0x31, new OpCode("AND", 2, 5, AddressingMode.Indirect_Y)},

            {0x49, new OpCode("EOR", 2, 2, AddressingMode.Immediate)},
            {0x45, new OpCode("EOR", 2, 3, AddressingMode.ZeroPage)},
            {0x55, new OpCode("EOR", 2, 4, AddressingMode.ZeroPage_X)},
            {0x4d, new OpCode("EOR", 3, 4, AddressingMode.Absolute)},
            {0x5d, new OpCode("EOR", 3, 4, AddressingMode.Absolute_X)},
            {0x59, new OpCode("EOR", 3, 4, AddressingMode.Absolute_Y)},
            {0x41, new OpCode("EOR", 2, 6, AddressingMode.Indirect_X)},
            {0x51, new OpCode("EOR", 2, 5, AddressingMode.Indirect_Y)},

            {0x09, new OpCode("ORA", 2, 2, AddressingMode.Immediate)},
            {0x05, new OpCode("ORA", 2, 3, AddressingMode.ZeroPage)},
            {0x15, new OpCode("ORA", 2, 4, AddressingMode.ZeroPage_X)},
            {0x0d, new OpCode("ORA", 3, 4, AddressingMode.Absolute)},
            {0x1d, new OpCode("ORA", 3, 4, AddressingMode.Absolute_X)},
            {0x19, new OpCode("ORA", 3, 4, AddressingMode.Absolute_Y)},
            {0x01, new OpCode("ORA", 2, 6, AddressingMode.Indirect_X)},
            {0x11, new OpCode("ORA", 2, 5, AddressingMode.Indirect_Y)},

            /* Shifts */
            {0x0a, new OpCode("ASL", 1, 2, AddressingMode.NoneAddressing)},
            {0x06, new OpCode("ASL", 2, 5, AddressingMode.ZeroPage)},
            {0x16, new OpCode("ASL", 2, 6, AddressingMode.ZeroPage_X)},
            {0x0e, new OpCode("ASL", 3, 6, AddressingMode.Absolute)},
            {0x1e, new OpCode("ASL", 3, 7, AddressingMode.Absolute_X)},

            {0x4a, new OpCode("LSR", 1, 2, AddressingMode.NoneAddressing)},
            {0x46, new OpCode("LSR", 2, 5, AddressingMode.ZeroPage)},
            {0x56, new OpCode("LSR", 2, 6, AddressingMode.ZeroPage_X)},
            {0x4e, new OpCode("LSR", 3, 6, AddressingMode.Absolute)},
            {0x5e, new OpCode("LSR", 3, 7, AddressingMode.Absolute_X)},

            {0x2a, new OpCode("ROL", 1, 2, AddressingMode.NoneAddressing)},
            {0x26, new OpCode("ROL", 2, 5, AddressingMode.ZeroPage)},
            {0x36, new OpCode("ROL", 2, 6, AddressingMode.ZeroPage_X)},
            {0x2e, new OpCode("ROL", 3, 6, AddressingMode.Absolute)},
            {0x3e, new OpCode("ROL", 3, 7, AddressingMode.Absolute_X)},

            {0x6a, new OpCode("ROR", 1, 2, AddressingMode.NoneAddressing)},
            {0x66, new OpCode("ROR", 2, 5, AddressingMode.ZeroPage)},
            {0x76, new OpCode("ROR", 2, 6, AddressingMode.ZeroPage_X)},
            {0x6e, new OpCode("ROR", 3, 6, AddressingMode.Absolute)},
            {0x7e, new OpCode("ROR", 3, 7, AddressingMode.Absolute_X)},

            {0xe6, new OpCode("INC", 2, 5, AddressingMode.ZeroPage)},
            {0xf6, new OpCode("INC", 2, 6, AddressingMode.ZeroPage_X)},
            {0xee, new OpCode("INC", 3, 6, AddressingMode.Absolute)},
            {0xfe, new OpCode("INC", 3, 7, AddressingMode.Absolute_X)},

            {0xe8, new OpCode("INX", 1, 2, AddressingMode.NoneAddressing)},
            {0xc8, new OpCode("INY", 1, 2, AddressingMode.NoneAddressing)},

            {0xc6, new OpCode("DEC", 2, 5, AddressingMode.ZeroPage)},
            {0xd6, new OpCode("DEC", 2, 6, AddressingMode.ZeroPage_X)},
            {0xce, new OpCode("DEC", 3, 6, AddressingMode.Absolute)},
            {0xde, new OpCode("DEC", 3, 7, AddressingMode.Absolute_X)},

            {0xca, new OpCode("DEX", 1, 2, AddressingMode.NoneAddressing)},
            {0x88, new OpCode("DEY", 1, 2, AddressingMode.NoneAddressing)},

            {0xc9, new OpCode("CMP", 2, 2, AddressingMode.Immediate)},
            {0xc5, new OpCode("CMP", 2, 3, AddressingMode.ZeroPage)},
            {0xd5, new OpCode("CMP", 2, 4, AddressingMode.ZeroPage_X)},
            {0xcd, new OpCode("CMP", 3, 4, AddressingMode.Absolute)},
            {0xdd, new OpCode("CMP", 3, 4, AddressingMode.Absolute_X)},
            {0xd9, new OpCode("CMP", 3, 4, AddressingMode.Absolute_Y)},
            {0xc1, new OpCode("CMP", 2, 6, AddressingMode.Indirect_X)},
            {0xd1, new OpCode("CMP", 2, 5, AddressingMode.Indirect_Y)},

            {0xc0, new OpCode("CPY", 2, 2, AddressingMode.Immediate)},
            {0xc4, new OpCode("CPY", 2, 3, AddressingMode.ZeroPage)},
            {0xcc, new OpCode("CPY", 3, 4, AddressingMode.Absolute)},

            {0xe0, new OpCode("CPX", 2, 2, AddressingMode.Immediate)},
            {0xe4, new OpCode("CPX", 2, 3, AddressingMode.ZeroPage)},
            {0xec, new OpCode("CPX", 3, 4, AddressingMode.Absolute)},


            /* Branching */

            {0x4c, new OpCode("JMP", 3, 3, AddressingMode.NoneAddressing)}, //AddressingMode that acts as Immidiate
            {0x6c, new OpCode("JMP", 3, 5, AddressingMode.NoneAddressing)}, //AddressingMode:Indirect with 6502 bug

            {0x20, new OpCode("JSR", 3, 6, AddressingMode.NoneAddressing)},
            {0x60, new OpCode("RTS", 1, 6, AddressingMode.NoneAddressing)},

            {0x40, new OpCode("RTI", 1, 6, AddressingMode.NoneAddressing)},

            {0xd0, new OpCode("BNE", 2, 2 /*(+1 if branch succeeds +2 if to a new page)*/, AddressingMode.NoneAddressing)},
            {0x70, new OpCode("BVS", 2, 2 /*(+1 if branch succeeds +2 if to a new page)*/, AddressingMode.NoneAddressing)},
            {0x50, new OpCode("BVC", 2, 2 /*(+1 if branch succeeds +2 if to a new page)*/, AddressingMode.NoneAddressing)},
            {0x30, new OpCode("BMI", 2, 2 /*(+1 if branch succeeds +2 if to a new page)*/, AddressingMode.NoneAddressing)},
            {0xf0, new OpCode("BEQ", 2, 2 /*(+1 if branch succeeds +2 if to a new page)*/, AddressingMode.NoneAddressing)},
            {0xb0, new OpCode("BCS", 2, 2 /*(+1 if branch succeeds +2 if to a new page)*/, AddressingMode.NoneAddressing)},
            {0x90, new OpCode("BCC", 2, 2 /*(+1 if branch succeeds +2 if to a new page)*/, AddressingMode.NoneAddressing)},
            {0x10, new OpCode("BPL", 2, 2 /*(+1 if branch succeeds +2 if to a new page)*/, AddressingMode.NoneAddressing)},

            {0x24, new OpCode("BIT", 2, 3, AddressingMode.ZeroPage)},
            {0x2c, new OpCode("BIT", 3, 4, AddressingMode.Absolute)},


            /* Stores, Loads */
            {0xa9, new OpCode("LDA", 2, 2, AddressingMode.Immediate)},
            {0xa5, new OpCode("LDA", 2, 3, AddressingMode.ZeroPage)},
            {0xb5, new OpCode("LDA", 2, 4, AddressingMode.ZeroPage_X)},
            {0xad, new OpCode("LDA", 3, 4, AddressingMode.Absolute)},
            {0xbd, new OpCode("LDA", 3, 4, AddressingMode.Absolute_X)},
            {0xb9, new OpCode("LDA", 3, 4, AddressingMode.Absolute_Y)},
            {0xa1, new OpCode("LDA", 2, 6, AddressingMode.Indirect_X)},
            {0xb1, new OpCode("LDA", 2, 5, AddressingMode.Indirect_Y)},

            {0xa2, new OpCode("LDX", 2, 2, AddressingMode.Immediate)},
            {0xa6, new OpCode("LDX", 2, 3, AddressingMode.ZeroPage)},
            {0xb6, new OpCode("LDX", 2, 4, AddressingMode.ZeroPage_Y)},
            {0xae, new OpCode("LDX", 3, 4, AddressingMode.Absolute)},
            {0xbe, new OpCode("LDX", 3, 4, AddressingMode.Absolute_Y)},

            {0xa0, new OpCode("LDY", 2, 2, AddressingMode.Immediate)},
            {0xa4, new OpCode("LDY", 2, 3, AddressingMode.ZeroPage)},
            {0xb4, new OpCode("LDY", 2, 4, AddressingMode.ZeroPage_X)},
            {0xac, new OpCode("LDY", 3, 4, AddressingMode.Absolute)},
            {0xbc, new OpCode("LDY", 3, 4, AddressingMode.Absolute_X)},


            {0x85, new OpCode("STA", 2, 3, AddressingMode.ZeroPage)},
            {0x95, new OpCode("STA", 2, 4, AddressingMode.ZeroPage_X)},
            {0x8d, new OpCode("STA", 3, 4, AddressingMode.Absolute)},
            {0x9d, new OpCode("STA", 3, 5, AddressingMode.Absolute_X)},
            {0x99, new OpCode("STA", 3, 5, AddressingMode.Absolute_Y)},
            {0x81, new OpCode("STA", 2, 6, AddressingMode.Indirect_X)},
            {0x91, new OpCode("STA", 2, 6, AddressingMode.Indirect_Y)},

            {0x86, new OpCode("STX", 2, 3, AddressingMode.ZeroPage)},
            {0x96, new OpCode("STX", 2, 4, AddressingMode.ZeroPage_Y)},
            {0x8e, new OpCode("STX", 3, 4, AddressingMode.Absolute)},

            {0x84, new OpCode("STY", 2, 3, AddressingMode.ZeroPage)},
            {0x94, new OpCode("STY", 2, 4, AddressingMode.ZeroPage_X)},
            {0x8c, new OpCode("STY", 3, 4, AddressingMode.Absolute)},


            /* Flags clear */

            {0xD8, new OpCode("CLD", 1, 2, AddressingMode.NoneAddressing)},
            {0x58, new OpCode("CLI", 1, 2, AddressingMode.NoneAddressing)},
            {0xb8, new OpCode("CLV", 1, 2, AddressingMode.NoneAddressing)},
            {0x18, new OpCode("CLC", 1, 2, AddressingMode.NoneAddressing)},
            {0x38, new OpCode("SEC", 1, 2, AddressingMode.NoneAddressing)},
            {0x78, new OpCode("SEI", 1, 2, AddressingMode.NoneAddressing)},
            {0xf8, new OpCode("SED", 1, 2, AddressingMode.NoneAddressing)},

            {0xaa, new OpCode("TAX", 1, 2, AddressingMode.NoneAddressing)},
            {0xa8, new OpCode("TAY", 1, 2, AddressingMode.NoneAddressing)},
            {0xba, new OpCode("TSX", 1, 2, AddressingMode.NoneAddressing)},
            {0x8a, new OpCode("TXA", 1, 2, AddressingMode.NoneAddressing)},
            {0x9a, new OpCode("TXS", 1, 2, AddressingMode.NoneAddressing)},
            {0x98, new OpCode("TYA", 1, 2, AddressingMode.NoneAddressing)},

            /* Stack */
            {0x48, new OpCode("PHA", 1, 3, AddressingMode.NoneAddressing)},
            {0x68, new OpCode("PLA", 1, 4, AddressingMode.NoneAddressing)},
            {0x08, new OpCode("PHP", 1, 3, AddressingMode.NoneAddressing)},
            {0x28, new OpCode("PLP", 1, 4, AddressingMode.NoneAddressing)},
        };
    }
}

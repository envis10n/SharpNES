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

            /* Unofficial */
            {0xc7, new OpCode("*DCP", 2, 5, AddressingMode.ZeroPage)},
            {0xd7, new OpCode("*DCP", 2, 6, AddressingMode.ZeroPage_X)},
            {0xCF,  new OpCode("*DCP", 3, 6, AddressingMode.Absolute)},
            {0xdF,  new OpCode("*DCP", 3, 7, AddressingMode.Absolute_X)},
            {0xdb,  new OpCode("*DCP", 3, 7, AddressingMode.Absolute_Y)},
            {0xd3,  new OpCode("*DCP", 2, 8, AddressingMode.Indirect_Y)},
            {0xc3,  new OpCode("*DCP", 2, 8, AddressingMode.Indirect_X)},


            {0x27,  new OpCode("*RLA", 2, 5, AddressingMode.ZeroPage)},
            {0x37,  new OpCode("*RLA", 2, 6, AddressingMode.ZeroPage_X)},
            {0x2F,  new OpCode("*RLA", 3, 6, AddressingMode.Absolute)},
            {0x3F,  new OpCode("*RLA", 3, 7, AddressingMode.Absolute_X)},
            {0x3b,  new OpCode("*RLA", 3, 7, AddressingMode.Absolute_Y)},
            {0x33,  new OpCode("*RLA", 2, 8, AddressingMode.Indirect_Y)},
            {0x23,  new OpCode("*RLA", 2, 8, AddressingMode.Indirect_X)},

            {0x07,  new OpCode("*SLO", 2, 5, AddressingMode.ZeroPage)},
            {0x17,  new OpCode("*SLO", 2, 6, AddressingMode.ZeroPage_X)},
            {0x0F,  new OpCode("*SLO", 3, 6, AddressingMode.Absolute)},
            {0x1f,  new OpCode("*SLO", 3, 7, AddressingMode.Absolute_X)},
            {0x1b,  new OpCode("*SLO", 3, 7, AddressingMode.Absolute_Y)},
            {0x03,  new OpCode("*SLO", 2, 8, AddressingMode.Indirect_X)},
            {0x13,  new OpCode("*SLO", 2, 8, AddressingMode.Indirect_Y)},

            {0x47,  new OpCode("*SRE", 2, 5, AddressingMode.ZeroPage)},
            {0x57,  new OpCode("*SRE", 2, 6, AddressingMode.ZeroPage_X)},
            {0x4F,  new OpCode("*SRE", 3, 6, AddressingMode.Absolute)},
            {0x5f,  new OpCode("*SRE", 3, 7, AddressingMode.Absolute_X)},
            {0x5b,  new OpCode("*SRE", 3, 7, AddressingMode.Absolute_Y)},
            {0x43,  new OpCode("*SRE", 2, 8, AddressingMode.Indirect_X)},
            {0x53,  new OpCode("*SRE", 2, 8, AddressingMode.Indirect_Y)},


            {0x80,  new OpCode("*NOP", 2,2, AddressingMode.Immediate)},
            {0x82,  new OpCode("*NOP", 2,2, AddressingMode.Immediate)},
            {0x89,  new OpCode("*NOP", 2,2, AddressingMode.Immediate)},
            {0xc2,  new OpCode("*NOP", 2,2, AddressingMode.Immediate)},
            {0xe2,  new OpCode("*NOP", 2,2, AddressingMode.Immediate)},


            {0xCB,  new OpCode("*AXS", 2,2, AddressingMode.Immediate)},

            {0x6B,  new OpCode("*ARR", 2,2, AddressingMode.Immediate)},

            {0xeb,  new OpCode("*SBC", 2,2, AddressingMode.Immediate)},

            {0x0b,  new OpCode("*ANC", 2,2, AddressingMode.Immediate)},
            {0x2b,  new OpCode("*ANC", 2,2, AddressingMode.Immediate)},

            {0x4b,  new OpCode("*ALR", 2,2, AddressingMode.Immediate)},
            // new OpCode(0xCB, "IGN", 3,4 /* or 5*/, AddressingMode.Absolute_X)},

            {0x04,  new OpCode("*NOP", 2,3, AddressingMode.ZeroPage)},
            {0x44,  new OpCode("*NOP", 2,3, AddressingMode.ZeroPage)},
            {0x64,  new OpCode("*NOP", 2,3, AddressingMode.ZeroPage)},
            {0x14,  new OpCode("*NOP", 2, 4, AddressingMode.ZeroPage_X)},
            {0x34,  new OpCode("*NOP", 2, 4, AddressingMode.ZeroPage_X)},
            {0x54,  new OpCode("*NOP", 2, 4, AddressingMode.ZeroPage_X)},
            {0x74,  new OpCode("*NOP", 2, 4, AddressingMode.ZeroPage_X)},
            {0xd4,  new OpCode("*NOP", 2, 4, AddressingMode.ZeroPage_X)},
            {0xf4,  new OpCode("*NOP", 2, 4, AddressingMode.ZeroPage_X)},
            {0x0c,  new OpCode("*NOP", 3, 4, AddressingMode.Absolute)},
            {0x1c,  new OpCode("*NOP", 3, 4 /*or 5*/, AddressingMode.Absolute_X)},
            {0x3c,  new OpCode("*NOP", 3, 4 /*or 5*/, AddressingMode.Absolute_X)},
            {0x5c,  new OpCode("*NOP", 3, 4 /*or 5*/, AddressingMode.Absolute_X)},
            {0x7c,  new OpCode("*NOP", 3, 4 /*or 5*/, AddressingMode.Absolute_X)},
            {0xdc,  new OpCode("*NOP", 3, 4 /* or 5*/, AddressingMode.Absolute_X)},
            {0xfc,  new OpCode("*NOP", 3, 4 /* or 5*/, AddressingMode.Absolute_X)},

            {0x67,  new OpCode("*RRA", 2, 5, AddressingMode.ZeroPage)},
            {0x77,  new OpCode("*RRA", 2, 6, AddressingMode.ZeroPage_X)},
            {0x6f,  new OpCode("*RRA", 3, 6, AddressingMode.Absolute)},
            {0x7f,  new OpCode("*RRA", 3, 7, AddressingMode.Absolute_X)},
            {0x7b,  new OpCode("*RRA", 3, 7, AddressingMode.Absolute_Y)},
            {0x63,  new OpCode("*RRA", 2, 8, AddressingMode.Indirect_X)},
            {0x73,  new OpCode("*RRA", 2, 8, AddressingMode.Indirect_Y)},


            {0xe7,  new OpCode("*ISB", 2,5, AddressingMode.ZeroPage)},
            {0xf7,  new OpCode("*ISB", 2,6, AddressingMode.ZeroPage_X)},
            {0xef,  new OpCode("*ISB", 3,6, AddressingMode.Absolute)},
            {0xff,  new OpCode("*ISB", 3,7, AddressingMode.Absolute_X)},
            {0xfb,  new OpCode("*ISB", 3,7, AddressingMode.Absolute_Y)},
            {0xe3,  new OpCode("*ISB", 2,8, AddressingMode.Indirect_X)},
            {0xf3,  new OpCode("*ISB", 2,8, AddressingMode.Indirect_Y)},

            {0x02,  new OpCode("*NOP", 1,2, AddressingMode.NoneAddressing)},
            {0x12,  new OpCode("*NOP", 1,2, AddressingMode.NoneAddressing)},
            {0x22,  new OpCode("*NOP", 1,2, AddressingMode.NoneAddressing)},
            {0x32,  new OpCode("*NOP", 1,2, AddressingMode.NoneAddressing)},
            {0x42,  new OpCode("*NOP", 1,2, AddressingMode.NoneAddressing)},
            {0x52,  new OpCode("*NOP", 1,2, AddressingMode.NoneAddressing)},
            {0x62,  new OpCode("*NOP", 1,2, AddressingMode.NoneAddressing)},
            {0x72,  new OpCode("*NOP", 1,2, AddressingMode.NoneAddressing)},
            {0x92,  new OpCode("*NOP", 1,2, AddressingMode.NoneAddressing)},
            {0xb2,  new OpCode("*NOP", 1,2, AddressingMode.NoneAddressing)},
            {0xd2,  new OpCode("*NOP", 1,2, AddressingMode.NoneAddressing)},
            {0xf2,  new OpCode("*NOP", 1,2, AddressingMode.NoneAddressing)},

            {0x1a,  new OpCode("*NOP", 1,2, AddressingMode.NoneAddressing)},
            {0x3a,  new OpCode("*NOP", 1,2, AddressingMode.NoneAddressing)},
            {0x5a,  new OpCode("*NOP", 1,2, AddressingMode.NoneAddressing)},
            {0x7a,  new OpCode("*NOP", 1,2, AddressingMode.NoneAddressing)},
            {0xda,  new OpCode("*NOP", 1,2, AddressingMode.NoneAddressing)},
            //{0xea,  new OpCode("NOP", 1,2, AddressingMode.NoneAddressing)},
            {0xfa,  new OpCode("*NOP", 1,2, AddressingMode.NoneAddressing)},

            {0xab,  new OpCode("*LXA", 2, 3, AddressingMode.Immediate)}, //todo: highly unstable and not used
            //http://visual6502.org/wiki/index.php?title=6502_Opcode_8B_%28XAA,_ANE%29
            {0x8b,  new OpCode("*XAA", 2, 3, AddressingMode.Immediate)}, //todo: highly unstable and not used
            {0xbb,  new OpCode("*LAS", 3, 2, AddressingMode.Absolute_Y)}, //todo: highly unstable and not used
            {0x9b,  new OpCode("*TAS", 3, 2, AddressingMode.Absolute_Y)}, //todo: highly unstable and not used
            {0x93,  new OpCode("*AHX", 2, /* guess */ 8, AddressingMode.Indirect_Y)}, //todo: highly unstable and not used
            {0x9f,  new OpCode("*AHX", 3, /* guess */ 4/* or 5*/, AddressingMode.Absolute_Y)}, //todo: highly unstable and not used
            {0x9e,  new OpCode("*SHX", 3, /* guess */ 4/* or 5*/, AddressingMode.Absolute_Y)}, //todo: highly unstable and not used
            {0x9c,  new OpCode("*SHY", 3, /* guess */ 4/* or 5*/, AddressingMode.Absolute_X)}, //todo: highly unstable and not used

            {0xa7,  new OpCode("*LAX", 2, 3, AddressingMode.ZeroPage)},
            {0xb7,  new OpCode("*LAX", 2, 4, AddressingMode.ZeroPage_Y)},
            {0xaf,  new OpCode("*LAX", 3, 4, AddressingMode.Absolute)},
            {0xbf,  new OpCode("*LAX", 3, 4, AddressingMode.Absolute_Y)},
            {0xa3,  new OpCode("*LAX", 2, 6, AddressingMode.Indirect_X)},
            {0xb3,  new OpCode("*LAX", 2, 5, AddressingMode.Indirect_Y)},

            {0x87,  new OpCode("*SAX", 2, 3, AddressingMode.ZeroPage)},
            {0x97,  new OpCode("*SAX", 2, 4, AddressingMode.ZeroPage_Y)},
            {0x8f,  new OpCode("*SAX", 3, 4, AddressingMode.Absolute)},
            {0x83,  new OpCode("*SAX", 2, 6, AddressingMode.Indirect_X)},
        };
    }
}

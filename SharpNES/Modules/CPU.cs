﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNES.Modules
{
    [Flags]
    public enum CpuFlags
    {
        NONE = 0b00000000,
        CARRY = 0b00000001,
        ZERO = 0b00000010,
        INTERRUPT_DISABLE = 0b00000100,
        DECIMAL_MODE = 0b00001000,
        BREAK = 0b00010000,
        BREAK2 = 0b00100000,
        OVERFLOW = 0b01000000,
        NEGATIVE = 0b10000000,
    }
    public class CPU : IMemory
    {
        static ushort STACK = 0x0100;
        static byte STACK_RESET = 0xfd;
        public byte register_a = 0;
        public byte register_x = 0;
        public byte register_y = 0;
        public byte stack_pointer = STACK_RESET;
        public CpuFlags status = (CpuFlags)0b100100;
        public ushort program_counter = 0;
        public Bus bus;
        public CPU(Bus _bus)
        {
            bus = _bus;
        }
        private ushort GetOperandAddress(AddressingMode mode)
        {
            switch (mode)
            {
                case AddressingMode.Immediate:
                    return program_counter;
                case AddressingMode.ZeroPage:
                    return MemRead(program_counter);
                case AddressingMode.Absolute:
                    return MemReadShort(program_counter);
                case AddressingMode.ZeroPage_X:
                    {
                        byte pos = MemRead(program_counter);
                        return (ushort)(pos + register_x);
                    }
                case AddressingMode.ZeroPage_Y:
                    {
                        byte pos = MemRead(program_counter);
                        return (ushort)(pos + register_y);
                    }
                case AddressingMode.Absolute_X:
                    {
                        ushort ba = MemReadShort(program_counter);
                        ushort x = register_x;
                        return (ushort)(ba + x);
                    }
                case AddressingMode.Absolute_Y:
                    {
                        ushort ba = MemReadShort(program_counter);
                        ushort y = register_y;
                        return (ushort)(ba + y);
                    }
                case AddressingMode.Indirect_X:
                    {
                        byte ba = MemRead(program_counter);
                        byte ptr = (byte)(ba + register_x);
                        ushort lo = MemRead(ptr);
                        ushort hi = MemRead((byte)(ptr + 1));
                        return (ushort)((ushort)(hi << 8) | lo);
                    }
                case AddressingMode.Indirect_Y:
                    {
                        byte ba = MemRead(program_counter);

                        ushort lo = MemRead(ba);
                        ushort hi = MemRead((byte)(ba + 1));
                        ushort deref_base = (ushort)((ushort)(hi << 8) | lo);
                        return (ushort)(deref_base + register_y);
                    }
                case AddressingMode.NoneAddressing:
                default:
                    throw new Exception("Unused Addressing Mode!");
            }
        }
        public byte MemRead(ushort addr)
        {
            return bus.MemRead(addr);
        }
        public void MemWrite(ushort addr, byte data)
        {
            bus.MemWrite(addr, data);
        }
        public ushort MemReadShort(ushort addr)
        {
            ushort lo = MemRead(addr);
            ushort hi = MemRead((ushort)(addr + 1));
            return (ushort)((ushort)(hi << 8) | lo);
        }
        public void MemWriteShort(ushort addr, ushort data)
        {
            byte hi = (byte)(data >> 8);
            byte lo = (byte)(data & 0xff);
            MemWrite(addr, lo);
            MemWrite((ushort)(addr + 1), hi);
        }
        public void Reset()
        {
            register_a = 0;
            register_y = 0;
            register_x = 0;
            stack_pointer = STACK_RESET;
            status = (CpuFlags)0b100100;
            program_counter = MemReadShort(0xFFFC);
        }
        private void SetCarryFlag()
        {
            status |= CpuFlags.CARRY;
        }
        private void ClearCarryFlag()
        {
            status &= ~CpuFlags.CARRY;
        }
        private void AddToRegisterA(byte data)
        {
            ushort carry_flag = 0;
            if ((status & CpuFlags.CARRY) != 0) carry_flag = 1;
            ushort sum = (ushort)(register_a
                + data
                + carry_flag);
            bool carry = sum > 0xFF;

            if (carry)
            {
                SetCarryFlag();
            } else
            {
                ClearCarryFlag();
            }

            byte result = (byte)sum;

            if (((byte)(data ^ result) & (byte)(result ^ register_a) & 0x80) != 0)
            {
                status |= CpuFlags.OVERFLOW;
            } else
            {
                status &= ~CpuFlags.OVERFLOW;
            }

            SetRegisterA(result);
        }
        private void SBC(AddressingMode mode)
        {
            ushort addr = GetOperandAddress(mode);
            byte data = MemRead(addr);
            AddToRegisterA((byte)(-data - 1));
        }
        private void ADC(AddressingMode mode)
        {
            ushort addr = GetOperandAddress(mode);
            AddToRegisterA(MemRead(addr));
        }
        private byte StackPop()
        {
            stack_pointer++;
            return MemRead((ushort)(STACK + stack_pointer));
        }
        private void StackPush(byte data)
        {
            MemWrite((ushort)(STACK + stack_pointer), data);
            stack_pointer--;
        }
        private void StackPushShort(ushort data)
        {
            byte hi = (byte)(data >> 8);
            byte lo = (byte)(data & 0xff);
            StackPush(hi);
            StackPush(lo);
        }
        private ushort StackPopShort()
        {
            byte lo = StackPop();
            byte hi = StackPop();

            return (ushort)((hi << 8) | lo);
        }
        private void ASLAccumulator()
        {
            byte data = register_a;
            if ((data >> 7) == 1)
            {
                SetCarryFlag();
            } else
            {
                ClearCarryFlag();
            }
            data = (byte)(data << 1);
            SetRegisterA(data);
        }
        private byte ASL(AddressingMode mode)
        {
            ushort addr = GetOperandAddress(mode);
            byte data = MemRead(addr);
            if ((byte)(data >> 7) == 1)
            {
                SetCarryFlag();
            } else
            {
                ClearCarryFlag();
            }
            data = (byte)(data << 1);
            MemWrite(addr, data);
            UpdateZeroAndNegativeFlags(data);
            return data;
        }
        private void LSRAccumulator()
        {
            byte data = register_a;

            if ((data & 1) == 1)
            {
                SetCarryFlag();
            } else
            {
                ClearCarryFlag();
            }
            data = (byte)(data >> 1);
            SetRegisterA(data);
        }
        private byte LSR(AddressingMode mode)
        {
            ushort addr = GetOperandAddress(mode);
            byte data = MemRead(addr);
            if ((data & 1) == 1)
            {
                SetCarryFlag();
            } else
            {
                ClearCarryFlag();
            }
            data = (byte)(data >> 1);
            MemWrite(addr, data);
            UpdateZeroAndNegativeFlags(data);
            return data;
        }
        private byte ROL(AddressingMode mode)
        {
            ushort addr = GetOperandAddress(mode);
            byte data = MemRead(addr);

            bool old_carry = (status & CpuFlags.CARRY) != 0;

            if ((data >> 7) == 1)
            {
                SetCarryFlag();
            } else
            {
                ClearCarryFlag();
            }
            data = (byte)(data << 1);
            if (old_carry)
            {
                data |= 1;
            }
            MemWrite(addr, data);
            UpdateZeroAndNegativeFlags(data);
            return data;
        }
        private void ROLAccumulator()
        {
            byte data = register_a;
            bool old_carry = (status & CpuFlags.CARRY) != 0;

            if ((data >> 7) == 1)
            {
                SetCarryFlag();
            } else
            {
                ClearCarryFlag();
            }
            data = (byte)(data << 1);
            if (old_carry)
            {
                data |= 1;
            }
            SetRegisterA(data);
        }
        private byte ROR(AddressingMode mode)
        {
            ushort addr = GetOperandAddress(mode);
            byte data = MemRead(addr);
            bool old_carry = (status & CpuFlags.CARRY) != 0;

            if ((data & 1) == 1)
            {
                SetCarryFlag();
            } else
            {
                ClearCarryFlag();
            }
            data = (byte)(data >> 1);
            if (old_carry)
            {
                data |= 0b10000000;
            }
            MemWrite(addr, data);
            UpdateZeroAndNegativeFlags(data);
            return data;
        }
        private void RORAccumulator()
        {
            byte data = register_a;
            bool old_carry = (status & CpuFlags.CARRY) != 0;

            if ((data & 1) == 1)
            {
                SetCarryFlag();
            } else
            {
                ClearCarryFlag();
            }
            data = (byte)(data >> 1);
            if (old_carry)
            {
                data |= 0b10000000;
            }
            SetRegisterA(data);
        }
        private byte INC(AddressingMode mode)
        {
            ushort addr = GetOperandAddress(mode);
            byte data = MemRead(addr);
            data++;
            MemWrite(addr, data);
            UpdateZeroAndNegativeFlags(data);
            return data;
        }
        private void DEY()
        {
            register_y--;
            UpdateZeroAndNegativeFlags(register_y);
        }
        private void DEX()
        {
            register_x--;
            UpdateZeroAndNegativeFlags(register_x);
        }
        private byte DEC(AddressingMode mode)
        {
            ushort addr = GetOperandAddress(mode);
            byte data = MemRead(addr);
            data--;
            MemWrite(addr, data);
            UpdateZeroAndNegativeFlags(data);
            return data;
        }
        private void PLA()
        {
            byte data = StackPop();
            SetRegisterA(data);
        }
        private void PLP()
        {
            status = (CpuFlags)StackPop();
            status &= ~CpuFlags.BREAK;
            status |= CpuFlags.BREAK2;
        }
        private void PHP()
        {
            CpuFlags flags = status;
            flags |= CpuFlags.BREAK;
            flags |= CpuFlags.BREAK2;
            StackPush((byte)flags);
        }
        private void BIT(AddressingMode mode)
        {
            ushort addr = GetOperandAddress(mode);
            byte data = MemRead(addr);
            byte and = (byte)(register_a & data);
            if (and == 0)
            {
                status |= CpuFlags.ZERO;
            } else
            {
                status &= ~CpuFlags.ZERO;
            }
            if ((data & 0b10000000) > 0) status |= CpuFlags.NEGATIVE;
            if ((data & 0b01000000) > 0) status |= CpuFlags.OVERFLOW;
        }
        private void COMPARE(AddressingMode mode, byte compare_with)
        {
            ushort addr = GetOperandAddress(mode);
            byte data = MemRead(addr);
            if (data <= compare_with)
            {
                status |= CpuFlags.CARRY;
            } else
            {
                status &= ~CpuFlags.CARRY;
            }
            UpdateZeroAndNegativeFlags((byte)(compare_with - data));
        }
        private void BRANCH(bool condition)
        {
            if (condition)
            {
                sbyte jump = (sbyte)MemRead(program_counter);
                ushort jump_addr = (ushort)(program_counter + 1 + jump);
                program_counter = jump_addr;
            }
        }
        public void Load(byte[] program)
        {
            //Buffer.BlockCopy(program, 0, memory, 0x0600, program.Length);
            for (ushort i = 0; i < program.Length; i++)
            {
                MemWrite((ushort)(0x0600 + i), program[i]);
            }
            MemWriteShort(0xFFFC, 0x0600);
        }
        private void LDY(AddressingMode mode)
        {
            ushort addr = GetOperandAddress(mode);
            register_y = MemRead(addr);
            UpdateZeroAndNegativeFlags(register_y);
        }
        private void LDX(AddressingMode mode)
        {
            ushort addr = GetOperandAddress(mode);
            register_x = MemRead(addr);
            UpdateZeroAndNegativeFlags(register_x);
        }
        private void LDA(AddressingMode mode)
        {
            ushort addr = GetOperandAddress(mode);
            register_a = MemRead(addr);
            UpdateZeroAndNegativeFlags(register_a);
        }
        private void STA(AddressingMode mode)
        {
            ushort addr = GetOperandAddress(mode);
            MemWrite(addr, register_a);
        }
        private void SetRegisterA(byte value)
        {
            register_a = value;
            UpdateZeroAndNegativeFlags(register_a);
        }
        private void AND(AddressingMode mode)
        {
            ushort addr = GetOperandAddress(mode);
            byte data = MemRead(addr);
            SetRegisterA((byte)(data & register_a));
        }
        private void EOR(AddressingMode mode)
        {
            ushort addr = GetOperandAddress(mode);
            byte data = MemRead(addr);
            SetRegisterA((byte)(data ^ register_a));
        }
        private void ORA(AddressingMode mode)
        {
            ushort addr = GetOperandAddress(mode);
            byte data = MemRead(addr);
            SetRegisterA((byte)(data | register_a));
        }
        private void TAX()
        {
            register_x = register_a;
            UpdateZeroAndNegativeFlags(register_x);
        }
        private void INX()
        {
            register_x++;
            UpdateZeroAndNegativeFlags(register_x);
        }
        private void INY()
        {
            register_y++;
            UpdateZeroAndNegativeFlags(register_y);
        }
        private void UpdateZeroAndNegativeFlags(byte result)
        {
            if (result == 0)
            {
                status |= CpuFlags.ZERO;
            }
            else
            {
                status &= ~CpuFlags.ZERO;
            }

            if (((CpuFlags)result & CpuFlags.NEGATIVE) != 0)
            {
                status |= CpuFlags.NEGATIVE;
            }
            else
            {
                status &= ~CpuFlags.NEGATIVE;
            }
        }
        public void LoadAndRun(byte[] program)
        {
            Load(program);
            Reset();
            Run();
        }
        public void Run()
        {
            RunWithCallback((_, _, _) => { });
        }
        public void RunWithCallback(Action<CPU, byte, OpCode> callback)
        {
            while (true)
            {
                byte code = MemRead(program_counter);
                program_counter++;
                ushort program_counter_state = program_counter;
                if (OpCodes.OPCODES_MAP.TryGetValue(code, out OpCode opcode))
                {
                    switch (code)
                    {
                        /* LDA */
                        case 0xa9:
                        case 0xa5:
                        case 0xb5:
                        case 0xad:
                        case 0xbd:
                        case 0xb9:
                        case 0xa1:
                        case 0xb1:
                            LDA(opcode.mode);
                            break;
                        /* STA */
                        case 0x85: case 0x95: case 0x8d: case 0x9d: case 0x99: case 0x81: case 0x91:
                            STA(opcode.mode);
                            break;
                        /* TAX */
                        case 0xaa:
                            TAX();
                            break;
                        /* INX */
                        case 0xe8:
                            INX();
                            break;
                        /* BRK */
                        case 0x00:
                            return;
                        /* CLD */
                        case 0xd8:
                            status &= ~CpuFlags.DECIMAL_MODE;
                            break;
                        /* CLI */
                        case 0x58:
                            status &= ~CpuFlags.INTERRUPT_DISABLE;
                            break;
                        /* CLV */
                        case 0xb8:
                            status &= ~CpuFlags.OVERFLOW;
                            break;
                        /* CLC */
                        case 0x18:
                            ClearCarryFlag();
                            break;
                        /* SEC */
                        case 0x38:
                            SetCarryFlag();
                            break;
                        /* SEI */
                        case 0x78:
                            status |= CpuFlags.INTERRUPT_DISABLE;
                            break;
                        /* SED */
                        case 0xf8:
                            status |= CpuFlags.DECIMAL_MODE;
                            break;
                        /* PHA */
                        case 0x48:
                            StackPush(register_a);
                            break;
                        /* PLA */
                        case 0x68:
                            PLA();
                            break;
                        /* PHP */
                        case 0x08:
                            PHP();
                            break;
                        /* PLP */
                        case 0x28:
                            PLP();
                            break;
                        /* ADC */
                        case 0x69: case 0x65: case 0x75: case 0x6d: case 0x7d: case 0x79:
                        case 0x61: case 0x71:
                            ADC(opcode.mode);
                            break;
                        /* SBC */
                        case 0xe9: case 0xe5: case 0xf5: case 0xed: case 0xfd: case 0xf9:
                        case 0xe1: case 0xf1:
                            SBC(opcode.mode);
                            break;
                        /* AND */
                        case 0x29:
                        case 0x25:
                        case 0x35:
                        case 0x2d:
                        case 0x3d:
                        case 0x39:
                        case 0x21:
                        case 0x31:
                            AND(opcode.mode);
                            break;
                        /* EOR */
                        case 0x49:
                        case 0x45:
                        case 0x55:
                        case 0x4d:
                        case 0x5d:
                        case 0x59:
                        case 0x41:
                        case 0x51:
                            EOR(opcode.mode);
                            break;
                        /* ORA */
                        case 0x09:
                        case 0x05:
                        case 0x15:
                        case 0x0d:
                        case 0x1d:
                        case 0x19:
                        case 0x01:
                        case 0x11:
                            ORA(opcode.mode);
                            break;
                        /* LSR */
                        case 0x4a:
                            LSRAccumulator();
                            break;
                        /* LSR */
                        case 0x46:
                        case 0x56:
                        case 0x4e:
                        case 0x5e:
                            LSR(opcode.mode);
                            break;
                        /* ASL */
                        case 0x0a:
                            ASLAccumulator();
                            break;
                        /* ASL */
                        case 0x06:
                        case 0x16:
                        case 0x0e:
                        case 0x1e:
                            ASL(opcode.mode);
                            break;
                        /* ROL */
                        case 0x2a:
                            ROLAccumulator();
                            break;
                        /* ROL */
                        case 0x26:
                        case 0x36:
                        case 0x2e:
                        case 0x3e:
                            ROL(opcode.mode);
                            break;
                        /* ROR */
                        case 0x6a:
                            RORAccumulator();
                            break;
                        /* ROR */
                        case 0x66:
                        case 0x76:
                        case 0x6e:
                        case 0x7e:
                            ROR(opcode.mode);
                            break;
                        /* INC */
                        case 0xe6:
                        case 0xf6:
                        case 0xee:
                        case 0xfe:
                            INC(opcode.mode);
                            break;
                        /* INY */
                        case 0xc8:
                            INY();
                            break;
                        /* DEC */
                        case 0xc6:
                        case 0xd6:
                        case 0xce:
                        case 0xde:
                            DEC(opcode.mode);
                            break;
                        /* DEX */
                        case 0xca:
                            DEX();
                            break;
                        /* DEY */
                        case 0x88:
                            DEY();
                            break;
                        /* CMP */
                        case 0xc9:
                        case 0xc5:
                        case 0xd5:
                        case 0xcd:
                        case 0xdd:
                        case 0xd9:
                        case 0xc1:
                        case 0xd1:
                            COMPARE(opcode.mode, register_a);
                            break;
                        /* CPY */
                        case 0xc0:
                        case 0xc4:
                        case 0xcc:
                            COMPARE(opcode.mode, register_y);
                            break;
                        /* CPX */
                        case 0xe0:
                        case 0xe4:
                        case 0xec:
                            COMPARE(opcode.mode, register_x);
                            break;
                        /* JMP Absolute */
                        case 0x4c:
                            {
                                ushort mem_addr = MemReadShort(program_counter);
                                program_counter = mem_addr;
                                break;
                            }
                        /* JMP Indirect */
                        case 0x6c:
                            {
                                ushort mem_addr = MemReadShort(program_counter);
                                ushort indirect_ref;
                                if ((mem_addr & 0x00ff) == 0x00ff)
                                {
                                    byte lo = MemRead(mem_addr);
                                    byte hi = MemRead((ushort)(mem_addr & 0xFF00));
                                    indirect_ref = (ushort)((hi << 8) | lo);
                                } else
                                {
                                    indirect_ref = MemReadShort(mem_addr);
                                }
                                program_counter = indirect_ref;
                                break;
                            }
                        /* JSR */
                        case 0x20:
                            {
                                StackPushShort((ushort)(program_counter + 2 - 1));
                                ushort target_addr = MemReadShort(program_counter);
                                program_counter = target_addr;
                                break;
                            }
                        /* RTS */
                        case 0x60:
                            program_counter = (ushort)(StackPopShort() + 1);
                            break;
                        /* RTI */
                        case 0x40:
                            {
                                status = (CpuFlags)StackPop();
                                status &= ~CpuFlags.BREAK;
                                status |= CpuFlags.BREAK2;

                                program_counter = StackPopShort();
                                break;
                            }
                        /* BNE */
                        case 0xd0:
                            BRANCH((status & CpuFlags.ZERO) == 0);
                            break;
                        /* BVS */
                        case 0x70:
                            BRANCH((status & CpuFlags.OVERFLOW) != 0);
                            break;
                        /* BVC */
                        case 0x50:
                            BRANCH((status & CpuFlags.OVERFLOW) == 0);
                            break;
                        /* BPL */
                        case 0x10:
                            BRANCH((status & CpuFlags.NEGATIVE) == 0);
                            break;
                        /* BMI */
                        case 0x30:
                            BRANCH((status & CpuFlags.NEGATIVE) != 0);
                            break;
                        /* BEQ */
                        case 0xf0:
                            BRANCH((status & CpuFlags.ZERO) != 0);
                            break;
                        /* BCS */
                        case 0xb0:
                            BRANCH((status & CpuFlags.CARRY) != 0);
                            break;
                        /* BCC */
                        case 0x90:
                            BRANCH((status & CpuFlags.CARRY) == 0);
                            break;
                        /* BIT */
                        case 0x24:
                        case 0x2c:
                            BIT(opcode.mode);
                            break;
                        /* STX */
                        case 0x86:
                        case 0x96:
                        case 0x8e:
                            {
                                ushort addr = GetOperandAddress(opcode.mode);
                                MemWrite(addr, register_x);
                                break;
                            }
                        /* STY */
                        case 0x84:
                        case 0x94:
                        case 0x8c:
                            {
                                ushort addr = GetOperandAddress(opcode.mode);
                                MemWrite(addr, register_y);
                                break;
                            }
                        /* LDX */
                        case 0xa2:
                        case 0xa6:
                        case 0xb6:
                        case 0xae:
                        case 0xbe:
                            LDX(opcode.mode);
                            break;
                        /* LDY */
                        case 0xa0:
                        case 0xa4:
                        case 0xb4:
                        case 0xac:
                        case 0xbc:
                            LDY(opcode.mode);
                            break;
                        /* NOP */
                        case 0xea:
                            break;
                        /* TAY */
                        case 0xa8:
                            {
                                register_y = register_a;
                                UpdateZeroAndNegativeFlags(register_y);
                                break;
                            }
                        /* TSX */
                        case 0xba:
                            {
                                register_x = stack_pointer;
                                UpdateZeroAndNegativeFlags(register_x);
                                break;
                            }
                        /* TXA */
                        case 0x8a:
                            {
                                register_a = register_x;
                                UpdateZeroAndNegativeFlags(register_a);
                                break;
                            }
                        /* TXS */
                        case 0x9a:
                            stack_pointer = register_x;
                            break;
                        /* TYA */
                        case 0x98:
                            {
                                register_a = register_y;
                                UpdateZeroAndNegativeFlags(register_a);
                                break;
                            }
                        default:
                            throw new Exception($"OpCode {code} {opcode.mnemonic} NYI");
                    }

                    if (program_counter_state == program_counter)
                    {
                        program_counter = (ushort)(program_counter + opcode.len - 1);
                    }

                    callback(this, code, opcode);
                }
                else
                {
                    throw new Exception($"OpCode {code} is not recognized.");
                }
            }
        }
    }
}

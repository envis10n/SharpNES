using System;
using System.Collections.Generic;

namespace SharpNES.Modules
{
    public static class TraceHelper
    {
        static string HexStr(params byte[] b) 
        {
            string tmp = "";
            foreach (byte v in b)
            {
                tmp += v.ToString("X2");
            }
            return tmp;
        }
        static string HexStr(params ushort[] b)
        {
            string tmp = "";
            foreach (byte v in b)
            {
                tmp += v.ToString("X4");
            }
            return tmp;
        }
        public static string Trace(CPU cpu)
        {
            ref Dictionary<byte, OpCode> opcodes = ref OpCodes.OPCODES_MAP;

            byte code = cpu.MemRead(cpu.program_counter);
            if(opcodes.TryGetValue(code, out OpCode ops)) 
            {
                ushort begin = cpu.program_counter;
                List<byte> hexdump = new List<byte>();
                hexdump.Add(code);

                ushort mem_addr;
                byte stored_value;
                if (ops.mode == AddressingMode.Immediate || ops.mode == AddressingMode.NoneAddressing)
                {
                    mem_addr = 0;
                    stored_value = 0;
                }
                else
                {
                    ushort addr = cpu.GetAbsoluteAddress(ops.mode, (ushort)(begin + 1));
                    mem_addr = addr;
                    stored_value = cpu.MemRead(addr);
                }

                string tmp;

                switch (ops.len)
                {
                    case 1:
                        {
                            switch (code)
                            {
                                case 0x0a:
                                case 0x4a:
                                case 0x2a:
                                case 0x6a:
                                    tmp = "A ";
                                    break;
                                default:
                                    tmp = "";
                                    break;
                            }
                            break;
                        }
                    case 2:
                        {
                            byte address = cpu.MemRead((ushort)(begin + 1));
                            hexdump.Add(address);

                            switch (ops.mode)
                            {
                                case AddressingMode.Immediate:
                                    tmp = string.Format("#${0:X2} = {1:X2}", mem_addr, stored_value);
                                    break;
                                case AddressingMode.ZeroPage:
                                    tmp = string.Format("${0:X2} = {1:X2}", mem_addr, stored_value);
                                    break;
                                case AddressingMode.ZeroPage_X:
                                    tmp = string.Format("${0:X2},X @ {1:X2} = {2:X2}", address, mem_addr, stored_value);
                                    break;
                                case AddressingMode.ZeroPage_Y:
                                    tmp = string.Format("${0:X2},Y @ {1:X2} = {2:X2}", address, mem_addr, stored_value);
                                    break;
                                case AddressingMode.Indirect_X:
                                    tmp = string.Format("(${0:X2},X) @ {1:X2} = {2:X4} = {3:X2}", address, (ushort)(address + cpu.register_x), mem_addr, stored_value);
                                    break;
                                case AddressingMode.Indirect_Y:
                                    tmp = string.Format("(${0:X2},Y) @ {1:X2} = {2:X4} = {3:X2}", address, (ushort)(address + cpu.register_y), mem_addr, stored_value);
                                    break;
                                case AddressingMode.NoneAddressing:
                                    ushort _address = (ushort)(begin + 2 + address);
                                    tmp = string.Format("${0:X4}", _address);
                                    break;
                                default:
                                    throw new Exception(string.Format("Unexpected addressing mode {0} has ops-len 2. code {1:X2}", ops.mode, code));
                            }
                            break;
                        }
                    case 3:
                        {
                            byte address_lo = cpu.MemRead((ushort)(begin + 1));
                            byte address_hi = cpu.MemRead((ushort)(begin + 2));
                            hexdump.Add(address_lo);
                            hexdump.Add(address_hi);

                            ushort address = cpu.MemReadShort((ushort)(begin + 1));

                            switch (ops.mode)
                            {
                                case AddressingMode.NoneAddressing:
                                    {
                                        if (code == 0x6c)
                                        {
                                            ushort jmp_addr;
                                            if ((ushort)(address & 0x00FF) == 0x00FF)
                                            {
                                                byte lo = cpu.MemRead(address);
                                                byte hi = cpu.MemRead((ushort)(address & 0xFF00));
                                                jmp_addr = (ushort)((ushort)(hi << 8) | lo);
                                            }
                                            else
                                            {
                                                jmp_addr = cpu.MemReadShort(address);
                                            }
                                            tmp = string.Format("(${0:X4}) = {1:X4}", address, jmp_addr);
                                        }
                                        else
                                        {
                                            tmp = string.Format("${0:X4}", address);
                                        }
                                        break;
                                    }
                                case AddressingMode.Absolute:
                                    tmp = string.Format("${0:X4} = {1:X2}", mem_addr, stored_value);
                                    break;
                                case AddressingMode.Absolute_X:
                                    tmp = string.Format("${0:X4},X @ {1:X4} = {2:X2}", address, mem_addr, stored_value);
                                    break;
                                case AddressingMode.Absolute_Y:
                                    tmp = string.Format("${0:X4},Y @ {1:X4} = {2:X2}", address, mem_addr, stored_value);
                                    break;
                                default:
                                    throw new Exception(string.Format("Unexpected addressing mode {0} has ops-len 3. code {1:X2}", ops.mode, code));
                            }
                            break;
                        }
                    default:
                        tmp = "";
                        break;
                }

                string hex_str = "";
                foreach (byte b in hexdump)
                {
                    hex_str += string.Format("{0:X2} ", b);
                }
                hex_str = hex_str.Trim();

                string asm_str = string.Format("{0:X4}  {1:8} {2: >4} {3}", begin, hex_str, ops.mnemonic, tmp).Trim().ToUpper();
                return asm_str;
            } else
            {
                return "UNKNOWN OP CODE";
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ExAssembler
    {
        private const int ARG_RCX = 0;
        private const int ARG_RDX = 1;
        private const int ARG_R8 = 2;
        private const int ARG_R9 = 3;

        public static byte[] CreateRemoteCall(PointerEx jumpLocation, PointerEx[] args, int pointerSize, PointerEx raxStorAddress, byte xmmMask_64 = 0, ExXMMReturnType xmmReturnType = ExXMMReturnType.XMMR_NONE)
        {
            if(pointerSize == 8)
            {
                return CreateRemoteCall64(jumpLocation, args, raxStorAddress, xmmMask_64, xmmReturnType);
            }
            return CreateRemoteCall32(jumpLocation, args, raxStorAddress);
        }

        private static byte[] CreateRemoteCall64(PointerEx jumpLocation, PointerEx[] args, PointerEx raxStorAddress, byte xmmMask, ExXMMReturnType xmmReturnType)
        {
            List<byte> data = new List<byte>();

            // sub rsp, 0x28
            data.AddRange(new byte[] { 0x48, 0x83, 0xEC, 0x28 });

            for(int i = args.Length - 1; i > -1; i--)
            {
                var arg = args[i];
                if(i < 4 && (PointerEx)(xmmMask & (1 << i)))
                {
                    bool is64xmm = (PointerEx)(xmmMask & (1 << i + 4));

                    // mov rax, arg
                    data.AddRange(new byte[] { 0x48, 0xb8 });
                    data.AddRange(BitConverter.GetBytes((long)arg));

                    if (is64xmm)
                    {
                        // movlpd xmm<?>, QWORD PTR [rax]
                        data.AddRange(new byte[] { 0x66, 0x0f, 0x12 });
                        data.Add((byte)(i * 8));
                    }
                    else
                    {
                        // movss xmm<?>, DWORD PTR [rax]
                        data.AddRange(new byte[] { 0xf3, 0x0f, 0x10 });
                        data.Add((byte)(i * 8));
                    }
                    continue;
                }

                switch (i)
                {
                    case ARG_RCX:
                        {
                            if (!arg)
                            {
                                // xor ecx, ecx
                                data.AddRange(new byte[] { 0x31, 0xC9 });
                                break;
                            }

                            if (arg <= (long)uint.MaxValue)
                            {
                                // mov ecx, arg
                                data.Add(0xB9);
                                data.AddRange(BitConverter.GetBytes((int)arg));
                                break;
                            }

                            // mov rcx, arg
                            data.AddRange(new byte[] { 0x48, 0xB9 });
                            data.AddRange(BitConverter.GetBytes((long)arg));
                        }
                        break;
                    case ARG_RDX:
                        {
                            if (!arg)
                            {
                                // xor edx, edx
                                data.AddRange(new byte[] { 0x31, 0xD2 });
                                break;
                            }

                            if (arg <= (long)uint.MaxValue)
                            {
                                // mov edx, arg
                                data.Add(0xBA);
                                data.AddRange(BitConverter.GetBytes((int)arg));
                                break;
                            }

                            // mov rdx, arg
                            data.AddRange(new byte[] { 0x48, 0xBA });
                            data.AddRange(BitConverter.GetBytes((long)arg));
                        }
                        break;
                    case ARG_R8:
                        {
                            if (!arg)
                            {
                                // xor r8d, r8d
                                data.AddRange(new byte[] { 0x45, 0x31, 0xC0 });
                                break;
                            }

                            if (arg <= (long)uint.MaxValue)
                            {
                                // mov r8d, arg
                                data.AddRange(new byte[] { 0x41, 0xB8 });
                                data.AddRange(BitConverter.GetBytes((int)arg));
                                break;
                            }

                            // mov r8, arg
                            data.AddRange(new byte[] { 0x49, 0xB8 });
                            data.AddRange(BitConverter.GetBytes((long)arg));
                        }
                        break;
                    case ARG_R9:
                        {
                            if (!arg)
                            {
                                // xor r9d, r8d
                                data.AddRange(new byte[] { 0x45, 0x31, 0xC9 });
                                break;
                            }

                            if (arg <= (long)uint.MaxValue)
                            {
                                // mov r9d, arg
                                data.AddRange(new byte[] { 0x41, 0xB9 });
                                data.AddRange(BitConverter.GetBytes((int)arg));
                                break;
                            }

                            // mov r9, arg
                            data.AddRange(new byte[] { 0x49, 0xB9 });
                            data.AddRange(BitConverter.GetBytes((long)arg));
                        }
                        break;
                    default:
                        {
                            if (!arg)
                            {
                                // push 0
                                data.AddRange(new byte[] { 0x6a, 0x00 });
                                break;
                            }

                            // mov rax, arg
                            data.AddRange(new byte[] { 0x48, 0xb8 });
                            data.AddRange(BitConverter.GetBytes((long)arg));

                            // push rax
                            data.Add(0x50);
                        }
                        break;
                }
            }

            // mov rax, jumploc
            data.AddRange(new byte[] { 0x48, 0xB8 });
            data.AddRange(BitConverter.GetBytes((long)jumpLocation));

            // call rax
            data.AddRange(new byte[] { 0xFF, 0xD0 });

            if (raxStorAddress)
            {
                if(xmmReturnType == ExXMMReturnType.XMMR_NONE)
                {
                    // mov ReturnAddress, rax
                    data.AddRange(new byte[] { 0x48, 0xA3 });
                    data.AddRange(BitConverter.GetBytes((long)raxStorAddress));
                }
                else
                {
                    // mov rax, ReturnAddress
                    data.AddRange(new byte[] { 0x48, 0xB8 });
                    data.AddRange(BitConverter.GetBytes((long)raxStorAddress));

                    if(xmmReturnType == ExXMMReturnType.XMMR_SINGLE)
                    {
                        // movss DWORD PTR [rax], xmm0
                        data.AddRange(new byte[] { 0xF3, 0x0F, 0x11, 0x00 });
                    }
                    else
                    {
                        // movlpd QWORD PTR [rax],xmm0
                        data.AddRange(new byte[] { 0x66, 0x0F, 0x13, 0x00 });
                    }
                }
            }

            // xor eax, eax
            data.AddRange(new byte[] { 0x31, 0xC0 });

            // add rsp, 0x28
            data.AddRange(new byte[] { 0x48, 0x83, 0xC4, 0x28 });

            // ret
            data.Add(0xC3);
            return data.ToArray();
        }

        private static byte[] CreateRemoteCall32(PointerEx jumpLocation, PointerEx[] args, PointerEx eaxStorAddress)
        {
            List<byte> data = new List<byte>();

            foreach (var arg in args.Reverse())
            {
                if (arg <= byte.MaxValue)
                {
                    // push byte
                    data.AddRange(new byte[] { 0x6A, arg });
                }
                else
                {
                    // push int32
                    data.Add(0x68);
                    data.AddRange(BitConverter.GetBytes((int)arg));
                }
            }

            // mov eax, jumpLoc
            data.Add(0xB8);
            data.AddRange(BitConverter.GetBytes((int)jumpLocation));

            // call eax
            data.AddRange(new byte[] { 0xFF, 0xD0 });

            if(eaxStorAddress)
            {
                // mov eaxStorAddress, eax
                data.Add(0xA3);
                data.AddRange(BitConverter.GetBytes((int)eaxStorAddress));
            }

            // xor eax, eax
            data.AddRange(new byte[] { 0x33, 0xC0 });

            // ret
            data.Add(0xC3);
            return data.ToArray();
        }
    }

    public enum ExXMMReturnType
    { 
        XMMR_NONE,
        XMMR_SINGLE,
        XMMR_DOUBLE
    }
}

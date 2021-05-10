using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.PEStructures;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.EnvironmentEx;
using static System.ModuleLoadOptions;
using static System.ModuleLoadType;
using static System.ExXMMReturnType;
using System.Threading;
using System.Runtime.CompilerServices;

namespace System
{
    public class ProcessEx
    {
        #region const
        public const int PROCESS_CREATE_THREAD = 0x02;
        public const int PROCESS_ACCESS = PROCESS_QUERY_INFORMATION | PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION | PROCESS_CREATE_THREAD;
        public const int PROCESS_QUERY_INFORMATION = 0x0400;
        public const int PAGE_READWRITE = 0x04;
        public const int PROCESS_VM_READ = 0x0010;
        public const int PROCESS_VM_WRITE = 0x0020;
        public const int PROCESS_VM_OPERATION = 0x0008;
        public const int MEM_DECOMMIT = 0x4000;
        public const int MEM_FREE = 0x10000;
        public const int MEM_COMMIT = 0x00001000;
        public const int MEM_RESERVE = 0x00002000;
        public const int MEM_PRIVATE = 0x20000;
        public const int MEM_IMAGE = 0x1000000;
        public const uint PAGE_GUARD = 0x100;
        public const uint PAGE_NOACCESS = 0x01;
        public const uint PAGE_READONLY = 0x02;
        public const uint PAGE_WRITECOPY = 0x08;
        public const uint PAGE_EXECUTE_READWRITE = 0x40;
        public const uint PAGE_EXECUTE_WRITECOPY = 0x80;
        public const uint PAGE_EXECUTE = 0x10;
        public const uint PAGE_EXECUTE_READ = 0x20;
        #endregion

        #region typedef
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            internal ushort wProcessorArchitecture;
            internal ushort wReserved;
            internal uint dwPageSize;
            internal PointerEx lpMinimumApplicationAddress;
            internal PointerEx lpMaximumApplicationAddress;
            internal PointerEx dwActiveProcessorMask;
            internal uint dwNumberOfProcessors;
            internal uint dwProcessorType;
            internal uint dwAllocationGranularity;
            internal ushort wProcessorLevel;
            internal ushort wProcessorRevision;
        }

        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        public struct MEMORY_BASIC_INFORMATION
        {
            public PointerEx BaseAddress;
            public PointerEx AllocationBase;
            public uint AllocationProtect;
            public uint __alignment1;
            public PointerEx RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
            public uint __alignment2;
        }
        #endregion

        #region pinvoke
        [DllImport("kernel32.dll")]
        public static extern PointerEx OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(PointerEx hProcess, PointerEx lpBaseAddress, [Out] byte[] lpBuffer, PointerEx dwSize, ref PointerEx lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeProcess(PointerEx hProcess, out uint ExitCode);

        [DllImport("kernel32.dll")]
        public static extern int GetProcessId(PointerEx handle);

        [DllImport("kernel32.dll")]
        public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern PointerEx VirtualQueryEx(PointerEx hProcess, PointerEx lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(PointerEx hProcess, PointerEx lpBaseAddress, byte[] lpBuffer, int dwSize, ref PointerEx lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualAlloc2(PointerEx hProcess, PointerEx lpBaseAddress, int RegionSize, ulong AllocType, ulong PageProtection);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern PointerEx VirtualAllocEx(PointerEx hProcess, PointerEx lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(PointerEx hProcess, PointerEx lpAddress, uint dwSize, int dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(PointerEx hHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool IsWow64Process(PointerEx processHandle, out bool isWow64Process);

        [DllImport("kernel32.dll")]
        internal static extern PointerEx CreateRemoteThread(PointerEx hProcess, PointerEx lpThreadAttributes, uint dwStackSize, PointerEx lpStartAddress, PointerEx lpParameter, uint dwCreationFlags, out PointerEx lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern PointerEx OpenThread(int dwDesiredAccess, bool bInheritHandle, int dwThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern PointerEx SuspendThread(PointerEx hThread);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool Wow64GetThreadContext(PointerEx hThread, ref CONTEXT lpContext);

        // Get context of thread x64, in x64 application
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetThreadContext(PointerEx hThread, ref CONTEXT64 lpContext);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetThreadContext(PointerEx hThread, ref CONTEXT lpContext);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool Wow64SetThreadContext(PointerEx hThread, CONTEXT lpContext);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetThreadContext(PointerEx hThread, CONTEXT lpContext);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetThreadContext(PointerEx hThread, CONTEXT64 lpContext);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint ResumeThread(PointerEx hThread);
        #endregion

        #region methods
        public ProcessEx(Process p, bool openHandle = false) 
        {
            if (p == null) throw new ArgumentException("Target process cannot be null");

            BaseProcess = p;
            p.EnableRaisingEvents = true;
            p.Exited += P_Exited;
            if(openHandle) OpenHandle();
        }

        private void P_Exited(object sender, EventArgs e) 
        {
            Handle = IntPtr.Zero;
        }

        public PointerEx OpenHandle(int dwDesiredAccess = PROCESS_ACCESS, bool newOnly = false) 
        {
            if (BaseProcess.HasExited) return IntPtr.Zero;
            if (Handle.IntPtr == IntPtr.Zero || newOnly) Handle = OpenProcess(dwDesiredAccess, false, BaseProcess.Id);
            return Handle;
        }

        public void CloseHandle()
        {
            if (!Handle) return;
            CloseHandle(Handle);
            Handle = 0;
        }

        public static ProcessEx FindProc(string name, bool OpenHandle = false)
        {
            var list = Process.GetProcessesByName(name);
            if (list.Length < 1) return null;
            return new ProcessEx(list[0], OpenHandle);
        }

        public T GetValue<T>(PointerEx absoluteAddress) where T : new()
        {
            if (!Handle) throw new InvalidOperationException("Tried to read from a memory region when a handle to the desired process doesn't exist");
            PointerEx size = Marshal.SizeOf(new T());
            byte[] data = GetBytes(absoluteAddress, size);
            if (typeof(T) == typeof(IntPtr) || typeof(T) == typeof(PointerEx))
            {
                IntPtr val = IntPtr.Size == sizeof(int) ? (IntPtr)BitConverter.ToInt32(data, 0) : (IntPtr)BitConverter.ToInt64(data, 0);
                if (typeof(T) == typeof(IntPtr)) return (dynamic)val;
                return (dynamic)new PointerEx(val);
            }
            if (typeof(T) == typeof(float)) return (dynamic)BitConverter.ToSingle(data, 0);
            if (typeof(T) == typeof(long)) return (dynamic)BitConverter.ToInt64(data, 0);
            if (typeof(T) == typeof(ulong)) return (dynamic)BitConverter.ToUInt64(data, 0);
            if (typeof(T) == typeof(int)) return (dynamic)BitConverter.ToInt32(data, 0);
            if (typeof(T) == typeof(uint)) return (dynamic)BitConverter.ToUInt32(data, 0);
            if (typeof(T) == typeof(short)) return (dynamic)BitConverter.ToInt16(data, 0);
            if (typeof(T) == typeof(ushort)) return (dynamic)BitConverter.ToUInt16(data, 0);
            if (typeof(T) == typeof(byte)) return (dynamic)data[0];
            if (typeof(T) == typeof(sbyte)) return (dynamic)data[0];
            throw new InvalidCastException($"Type {typeof(T)} is not a valid value type");
        }

        public void SetValue<T>(PointerEx absoluteAddress, T value) where T : new()
        {
            if (!Handle) throw new InvalidOperationException("Tried to write to a memory region when a handle to the desired process doesn't exist");
            byte[] data = Array.Empty<byte>();
            if (value is IntPtr ip) data = IntPtr.Size == sizeof(int) ? BitConverter.GetBytes(ip.ToInt32()) : BitConverter.GetBytes(ip.ToInt64());
            else if (value is PointerEx ipx) data = IntPtr.Size == sizeof(int) ? BitConverter.GetBytes(ipx.IntPtr.ToInt32()) : BitConverter.GetBytes(ipx.IntPtr.ToInt64());
            else if (value is float f) data = BitConverter.GetBytes(f);
            else if (value is long l) data = BitConverter.GetBytes(l);
            else if (value is ulong ul) data = BitConverter.GetBytes(ul);
            else if (value is int i) data = BitConverter.GetBytes(i);
            else if (value is uint ui) data = BitConverter.GetBytes(ui);
            else if (value is short s) data = BitConverter.GetBytes(s);
            else if (value is ushort u) data = BitConverter.GetBytes(u);
            else if (value is byte b) data = new byte[] { b };
            else if (value is sbyte sb) data = new byte[] { (byte)sb };
            else throw new InvalidCastException($"Cannot use type {typeof(T)} as a value type");
            SetBytes(absoluteAddress, data);
        }

        public byte[] GetBytes(PointerEx absoluteAddress, PointerEx NumBytes) 
        {
            if (!Handle) throw new InvalidOperationException("Tried to read from a memory region when a handle to the desired process doesn't exist");
            byte[] data = new byte[NumBytes];
            PointerEx bytesRead = IntPtr.Zero;
            ReadProcessMemory(Handle, absoluteAddress, data, NumBytes, ref bytesRead);
            if (bytesRead != NumBytes) throw new InvalidOperationException($"Failed to read data of size {NumBytes} from address 0x{absoluteAddress}");
            return data;
        }

        public void SetBytes(PointerEx absoluteAddress, byte[] data) 
        {
            if (!Handle) throw new InvalidOperationException("Tried to write to a memory region when a handle to the desired process doesn't exist");
            PointerEx bytesWritten = IntPtr.Zero;
            WriteProcessMemory(Handle, absoluteAddress, data, data.Length, ref bytesWritten);
            if (bytesWritten != data.Length) throw new InvalidOperationException($"Failed to write {data.Length} bytes to region 0x{absoluteAddress}");
        }

        public T GetStruct<T>(PointerEx absoluteAddress) where T : struct
        {
            return GetBytes(absoluteAddress, Marshal.SizeOf(typeof(T))).ToStruct<T>();
        }

        public void SetStruct<T>(PointerEx absoluteAddress, T s) where T : struct
        {
            SetBytes(absoluteAddress, s.ToByteArray());
        }

        public T[] GetArray<T>(PointerEx absoluteAddress, PointerEx numItems) where T : struct
        {
            T[] arr = new T[numItems];
            for(int i = 0; i < numItems; i++) arr[i] = GetStruct<T>(absoluteAddress + (i * Marshal.SizeOf(typeof(T))));
            return arr;
        }

        public void SetArray<T>(PointerEx absoluteAddress, T[] array) where T : struct
        {
            for (int i = 0; i < array.Length; i++) SetStruct(absoluteAddress + (i * Marshal.SizeOf(typeof(T))), array[i]);
        }

        public string GetString(PointerEx absoluteAddress, int MaxLength = 1023, int buffSize = 256) 
        {
            byte[] buffer;
            byte[] rawString = new byte[MaxLength + 1];
            int bytesRead = 0;
            while(bytesRead < MaxLength)
            {
                buffer = GetBytes(absoluteAddress + bytesRead, buffSize);
                for(int i = 0; i < buffer.Length && i + bytesRead < MaxLength; i++)
                {
                    if (buffer[i] == 0) return rawString.String();
                    rawString[bytesRead + i] = buffer[i];
                }
                bytesRead += buffSize;
            }
            return rawString.String();
        }

        public void SetString(PointerEx absoluteAddress, string Value) 
        {
            SetArray(absoluteAddress, Value.Bytes());
        }

        public Task<IEnumerable<PointerEx>> FindPattern(string query, PointerEx start, PointerEx end, MemorySearchFlags flags)
        {
            return new MemorySearcher(this).Search(query, start, end, flags);
        }

        public PointerEx LoadModule(Memory<byte> moduleData, ModuleLoadOptions loadOptions = MLO_None)
        {
            if (BaseProcess.HasExited) throw new InvalidOperationException("Cannot inject a dll to a process which has exited");
            if (moduleData.IsEmpty) throw new ArgumentException("Cannot inject an empty dll");
            if (!Environment.Is64BitProcess && (GetArchitecture() == Architecture.X64)) throw new InvalidOperationException("Cannot map to target; a 32-bit injector cannot inject to a 64 bit process.");
            var _pe = new PEImage(moduleData);

            return IntPtr.Zero; // failed to load
        }

        public Architecture GetArchitecture()
        {
            if (!Environment.Is64BitOperatingSystem || IsWow64Process()) return Architecture.X86;
            return Architecture.X64;
        }

        internal bool IsWow64Process()
        {
            if(!IsWow64Process(BaseProcess.Handle, out bool result)) throw new ComponentModel.Win32Exception();
            return result;
        }

        public int PointerSize()
        {
            return GetArchitecture() == Architecture.X86 ? sizeof(uint) : sizeof(ulong);
        }

        /// <summary>
        /// Call a remote procedure, with a return type. Arguments are not passed by reference, and may not be manipulated by the calling process. Structs are shallow copy. Will await return signal.
        /// </summary>
        /// <param name="absoluteAddress"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task<T> Call<T>(PointerEx absoluteAddress, params object[] args)
        {
            return Call<T>(absoluteAddress, DefaultRPCType, args);
        }

        /// <summary>
        /// Call a remote procedure, with a return type. Arguments are not passed by reference, and may not be manipulated by the calling process. Structs are shallow copy. Will await return signal.
        /// </summary>
        /// <param name="absoluteAddress"></param>
        /// <param name="callType">Type of call to initiate. Some call types must be initialized to be used.</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<T> Call<T>(PointerEx absoluteAddress, ExCallThreadType callType, params object[] args)
        {
            if (!RPCStackFrame.CanSerializeType(typeof(T)))
            {
                throw new InvalidCastException("Cannot cast type [" + typeof(T).GetType().Name + "] to a serializable type for RPC returns");
            }

            var pointerSize = PointerSize();
            var is64Call = pointerSize == 8;

            var xmmRetType = XMMR_NONE;
            if(typeof(T) == typeof(double))
            {
                xmmRetType = XMMR_DOUBLE;
            }
            if(typeof(T) == typeof(float))
            {
                xmmRetType = XMMR_SINGLE;
            }

            RPCStackFrame stackFrame = new RPCStackFrame(pointerSize);
            foreach (var arg in args)
            {
                stackFrame.PushArgument(arg);
            }

            PointerEx stackSize = stackFrame.Size();
            PointerEx hStack = QuickAlloc(stackSize);
            try
            {
                byte[] stackData = stackFrame.Build(hStack);
                var raxStorAddress = stackFrame.RAXStorOffset + hStack;
                var threadStateAddress = stackFrame.ThreadStateOffset + hStack;
                SetBytes(hStack, stackData);

                // Next, assemble the call code
                PointerEx[] ArgumentList = new PointerEx[args.Length];
                byte xmmArgMask = 0;

                for (int i = 0; i < args.Length; i++)
                {
                    ArgumentList[i] = stackFrame.GetArg(i);
                    if(is64Call && i < 4 && stackFrame.IsArgXMM(i))
                    {
                        xmmArgMask |= (byte)(1 << i);
                        if(stackFrame.IsArgXMM64(i))
                        {
                            xmmArgMask |= (byte)(1 << (i + 4));
                        }
                    }
                }

                // Write shellcode
                byte[] shellcode = ExAssembler.CreateRemoteCall(absoluteAddress, ArgumentList, PointerSize(), raxStorAddress, threadStateAddress, xmmArgMask, xmmRetType);
                PointerEx shellSize = shellcode.Length;
                PointerEx hShellcode = QuickAlloc(shellSize, true);

                try
                {
                    // Write the data for the shellcode
                    SetBytes(hShellcode, shellcode);
#if DEV
                    System.IO.File.AppendAllText("log.txt", $"hShellcode: 0x{hShellcode:X}, hStack: 0x{hStack:X}\n");
                    System.IO.File.AppendAllText("log.txt", $"shellcode: {shellcode.Hex()}\n");
                    System.IO.File.AppendAllText("log.txt", $"stack: {stackData.Hex()}\n");
                    //Environment.Exit(0);
#endif

                    switch(callType)
                    {
                        case ExCallThreadType.XCTT_NtCreateThreadEx:
                            {
                                // Start remote thread and await its exit.
                                StartThread(hShellcode, out SafeWaitHandle threadHandle);
                                AwaitThreadExit(ref threadHandle);
                            }
                            break;
                        case ExCallThreadType.XCTT_RIPHijack:
                            {
                                await CallRipHijack(hShellcode, threadStateAddress);
                            }
                            break;
                    }

                    if (!Handle) throw new Exception("Process exited unexpectedly...");

                    // read return value
                    PointerEx r_val = (PointerSize() == 4 ? GetValue<uint>(raxStorAddress) : GetValue<ulong>(raxStorAddress));

                    // deserialize return value to expected type

                    // if its a string...
                    if(typeof(T) == typeof(string))
                    {
                        return (T)(dynamic)(r_val ? GetString(r_val) : "");
                    }

                    // if its a value type that fits in a pointerex...
                    if(Marshal.SizeOf(default(T)) <= Marshal.SizeOf(default(PointerEx)))
                    {
                        try
                        {
                            return (T)(dynamic)r_val;
                        }
                        catch { }
                    }

                    if(r_val)
                    {
                        byte[] data = GetBytes(r_val, Marshal.SizeOf(default(T)));
                        return data.ToStructUnsafe<T>();
                    }
                }
                finally
                {
                    if (Handle)
                    {
                        VirtualFreeEx(Handle, hShellcode, shellSize, (int)FreeType.Release);
                    }
                }
            }
            finally
            {
                if(Handle)
                {
                    VirtualFreeEx(Handle, hStack, stackSize, (int)FreeType.Release);
                }
            }
            return default(T);
        }

        /// <summary>
        /// Call a remote procedure. Will not await a thread exit. If using an rpc method that is not thread safe, you may encounter a delayed execution of the thread, waiting for other RPC to continue.
        /// </summary>
        /// <param name="absoluteAddress"></param>
        /// <param name="args"></param>
        public void CallThreaded(PointerEx absoluteAddress, params object[] args)
        {
            Task.Run(() => { Call<int>(absoluteAddress, args); });
        }

        // TODO: CallRef

        private async Task CallRipHijack(PointerEx hShellcode, PointerEx threadStateAddress)
        {
            var targetThread = GetEarliestActiveThread();
            if(targetThread == null)
            {
                throw new Exception("Unable to find a thread which can be hijacked.");
            }

#if DEV
            System.IO.File.AppendAllText("log.txt", $"targetThreadID: {targetThread.Id}\n");
#endif

            var hThreadResume = QuickAlloc(4096, true);
            var hXmmSpace = QuickAlloc(256 * 2, true);
            try
            {
                if (!__hijackMutexTable.ContainsKey(BaseProcess.Id))
                {
                    __hijackMutexTable[BaseProcess.Id] = new object();
                }

                lock (__hijackMutexTable[BaseProcess.Id])
                {
                    PointerEx hThread = OpenThread((int)ThreadAccess.THREAD_HIJACK, false, targetThread.Id);

                    if (!hThread)
                    {
                        throw new Exception("Unable to open target thread for RPC...");
                    }

#if DEV
                    System.IO.File.AppendAllText("log.txt", $"hThread: {hThread}\n");
#endif

                    SuspendThread(hThread);

                    if(GetArchitecture() == Architecture.X86)
                    {
                        CONTEXT _32_context = new CONTEXT();
                        _32_context.ContextFlags = (uint)CONTEXT_FLAGS.CONTEXT_FULL;
                        bool wow64CtxGetResult;

                        if(Environment.Is64BitProcess)
                        {
                            wow64CtxGetResult = Wow64GetThreadContext(hThread, ref _32_context);
                        }
                        else
                        {
                            wow64CtxGetResult = GetThreadContext(hThread, ref _32_context);
                        }

                        if(!wow64CtxGetResult)
                        {
                            throw new Exception("Unable to get a thread context for the target process RPC.");
                        }

                        HijackRipInternal32(hThread, hShellcode, hThreadResume, ref _32_context);

                        if (Environment.Is64BitProcess)
                        {
                            Wow64SetThreadContext(hThread, _32_context);
                        }
                        else
                        {
                            SetThreadContext(hThread, _32_context);
                        }
                    }
                    else
                    {
                        CONTEXT64 _64_context = new CONTEXT64();
                        _64_context.ContextFlags = CONTEXT_FLAGS.CONTEXT_FULL;

                        if (!GetThreadContext(hThread, ref _64_context))
                        {
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                            throw new Exception("Unable to get a thread context for the target process RPC.");
                        }

#if DEV
                        System.IO.File.AppendAllText("log.txt", $"context obtained\n");
#endif

                        HijackRipInternal64(hThread, hShellcode, hThreadResume, hXmmSpace, ref _64_context);
                        SetThreadContext(hThread, _64_context);
                    }

                    ResumeThread(hThread);
                    CloseHandle(hThread);
                }

#if DEV
                System.IO.File.AppendAllText("log.txt", $"awaiting thread exit...\n");
#endif
                // await thread exit status
                while (Handle)
                {
                    if (GetValue<int>(threadStateAddress) != 0)
                    {
                        break;
                    }
                    await Task.Delay(1);
                }

#if DEV
                System.IO.File.AppendAllText("log.txt", $"good to go!\n");
#endif
            }
            finally
            {
                if(Handle)
                {
                    VirtualFreeEx(Handle, hThreadResume, 4096, (int)FreeType.Release);
                    VirtualFreeEx(Handle, hXmmSpace, 256 * 2, (int)FreeType.Release);
                }
            }
            
        }

        private void HijackRipInternal64(PointerEx hThread, PointerEx hShellcode, PointerEx hIntercept, PointerEx hXmmSpace, ref CONTEXT64 threadContext)
        {
            PointerEx originalIp = threadContext.Rip;
            byte[] data = ExAssembler.CreateThreadIntercept64(hShellcode, originalIp, hXmmSpace);
            SetBytes(hIntercept, data);
#if DEV
            System.IO.File.AppendAllText("log.txt", $"ripIntercept: {data.Hex()}\n");
#endif
            threadContext.Rip = hIntercept;
        }

        private void HijackRipInternal32(PointerEx hThread, PointerEx hShellcode, PointerEx hIntercept, ref CONTEXT threadContext)
        {
            PointerEx originalIp = threadContext.Eip;
            byte[] data = ExAssembler.CreateThreadIntercept32(hShellcode, originalIp);
            SetBytes(hIntercept, data);
            threadContext.Eip = hIntercept;
        }

        /// <summary>
        /// Allocate readable and writable memory in the target process. If executable is true, it will also be executable. Is not managed and can be leaked, so remember to free the memory when it is no longer needed.
        /// </summary>
        /// <param name="size_region"></param>
        /// <param name="Executable"></param>
        /// <returns></returns>
        public PointerEx QuickAlloc(PointerEx size_region, bool Executable = false)
        {
            if (!Handle) throw new InvalidOperationException("Tried to allocate a memory region when a handle to the desired process doesn't exist");
            return VirtualAllocEx(Handle, 0, size_region, AllocationType.Commit, Executable ? MemoryProtection.ExecuteReadWrite : MemoryProtection.ReadWrite);
        }

        /// <summary>
        /// Start a thread in this process, at the given address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public void StartThread(PointerEx address, out SafeWaitHandle threadHandle)
        {
            if (!Handle) throw new InvalidOperationException("Tried to create a thread in a process which has no open handle!");

            var status = NtCreateThreadEx(out threadHandle, AccessMask.SpecificRightsAll | AccessMask.StandardRightsAll, IntPtr.Zero, Handle, address, IntPtr.Zero, ThreadCreationFlags.HideFromDebugger | ThreadCreationFlags.SkipThreadAttach, 0, 0, 0, IntPtr.Zero);
            if (status != 0)
            {
                throw new Win32Exception(RtlNtStatusToDosError(status));
            }
        }

        /// <summary>
        /// Await the exit of a thread by its handle, optionally declaring the maximum time to wait.
        /// </summary>
        /// <param name="threadHandle"></param>
        /// <param name="MaxMSWait"></param>
        public void AwaitThreadExit(ref SafeWaitHandle threadHandle, int MaxMSWait = int.MaxValue)
        {
            if (threadHandle == null || threadHandle.IsClosed) return;
            using (threadHandle)
            {
                if (WaitForSingleObject(threadHandle, MaxMSWait) == -1)
                {
                    throw new Win32Exception();
                }
            }
        }

        /// <summary>
        /// Apply a new default calling type to RPC for this process. The type specified must be initialized prior to changing the default.
        /// </summary>
        /// <param name="type"></param>
        public void SetDefaultCallType(ExCallThreadType type)
        {
            if (!IsRPCTypeInitialized(type))
                throw new InvalidOperationException($"Cannot invoke rpc of type {type} because the rpc type has not been initialized.");
            DefaultRPCType = type;
        }

        /// <summary>
        /// Determines if an RPC type has been initialized properly
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsRPCTypeInitialized(ExCallThreadType type)
        {
            switch(type)
            {
                case ExCallThreadType.XCTT_RIPHijack:
                case ExCallThreadType.XCTT_NtCreateThreadEx:
                    return true;

                default:
                    throw new NotImplementedException($"Calltype {type} not implemented");
            }
        }

        /// <summary>
        /// Find the active thread with the earliest creation time
        /// </summary>
        /// <returns></returns>
        public ProcessThread GetEarliestActiveThread()
        {
            if (BaseProcess.HasExited) return null;
            ProcessThread earliest = null;
            foreach(ProcessThread thread in BaseProcess.Threads)
            {
                if (thread.ThreadState == Diagnostics.ThreadState.Terminated) continue;
                if (earliest == null)
                {
                    earliest = thread;
                    continue;
                }
                if (thread.StartTime < earliest.StartTime)
                {
                    earliest = thread;
                }
            }
            return earliest;
        }
        #endregion

        #region overrides
        public static implicit operator ProcessEx(Process p)
        {
            return new ProcessEx(p);
        }

        public static implicit operator Process(ProcessEx px)
        {
            return px.BaseProcess;
        }

        public static implicit operator ProcessEx(string name)
        {
            return FindProc(name);
        }

        public static implicit operator bool(ProcessEx px)
        {
            return px?.Handle ?? false;
        }

        public PointerEx this[PointerEx offset]
        {
            get
            {
                return BaseAddress + offset;
            }
        }

        public ProcessModuleEx this[string name]
        {
            get
            {
                foreach(var m in Modules)
                {
                    if (m.BaseModule.ModuleName.Equals(name, StringComparison.InvariantCultureIgnoreCase)) return m;
                }
                return null;
            }
        }
        #endregion

        #region Members
        public Process BaseProcess { get; private set; }

        /// <summary>
        /// The default code execution type for an RPC call with no thread type specifier
        /// </summary>
        public ExCallThreadType DefaultRPCType { get; private set; }

        public PointerEx BaseAddress
        { 
            get
            {
                return BaseProcess.MainModule.BaseAddress;
            }
        }

        private PointerEx __handle = IntPtr.Zero;
        public PointerEx Handle
        {
            get
            {
                if (BaseProcess.HasExited) return IntPtr.Zero;
                return __handle;
            }
            private set
            {
                __handle = value;
            }
        }

        public IEnumerable<ProcessModuleEx> Modules
        {
            get
            {
                foreach (ProcessModule p in BaseProcess.Modules) yield return p;
            }
        }

        #endregion

        #region static members
        private static Dictionary<int, object> __hijackMutexTable = new Dictionary<int, object>();
        #endregion
    }

    #region public Typedef
    #region enum
    public enum ModuleLoadType
    { 
        MLT_ManualMapped
    }

    public enum ModuleLoadOptions
    {
        MLO_None = 0
    }

    public enum ExCallThreadType
    {
        /// <summary>
        /// Start the call thread via NtCreateThreadEx (thread safe, default)
        /// </summary>
        XCTT_NtCreateThreadEx,

        /// <summary>
        /// Start a call via a thread hijacking procedure involving an RIP detour via the thread id specified (not thread safe)
        /// </summary>
        XCTT_RIPHijack,

        /// <summary>
        /// Start a call via a vectored exception (not thread safe)
        /// </summary>
        XCTT_VEH,

        /// <summary>
        /// Start a call via a VMT detour (not thread safe)
        /// </summary>
        XCTT_VMT_Detour,

        /// <summary>
        /// Start a call via a pointer replacement, typically in the data section
        /// </summary>
        XCTT_Custom_DS_Detour
    }

    #endregion
    #region struct
    [StructLayout(LayoutKind.Explicit, Size = 1152)]
    public readonly struct Peb32
    {
        [FieldOffset(0xC)]
        public readonly int Ldr;

        [FieldOffset(0x38)]
        public readonly int APISetMap;
    }

    [StructLayout(LayoutKind.Explicit, Size = 1992)]
    public readonly struct Peb64
    {
        [FieldOffset(0x18)]
        public readonly long Ldr;

        [FieldOffset(0x68)]
        public readonly long APISetMap;
    }

    // x86 float save
    [StructLayout(LayoutKind.Sequential)]
    public struct FLOATING_SAVE_AREA
    {
        public uint ControlWord;
        public uint StatusWord;
        public uint TagWord;
        public uint ErrorOffset;
        public uint ErrorSelector;
        public uint DataOffset;
        public uint DataSelector;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
        public byte[] RegisterArea;
        public uint Cr0NpxState;
    }

    // x86 context structure (not used in this example)
    [StructLayout(LayoutKind.Sequential)]
    public struct CONTEXT
    {
        public uint ContextFlags; //set this to an appropriate value 
                                  // Retrieved by CONTEXT_DEBUG_REGISTERS 
        public uint Dr0;
        public uint Dr1;
        public uint Dr2;
        public uint Dr3;
        public uint Dr6;
        public uint Dr7;
        // Retrieved by CONTEXT_FLOATING_POINT 
        public FLOATING_SAVE_AREA FloatSave;
        // Retrieved by CONTEXT_SEGMENTS 
        public uint SegGs;
        public uint SegFs;
        public uint SegEs;
        public uint SegDs;
        // Retrieved by CONTEXT_INTEGER 
        public uint Edi;
        public uint Esi;
        public uint Ebx;
        public uint Edx;
        public uint Ecx;
        public uint Eax;
        // Retrieved by CONTEXT_CONTROL 
        public uint Ebp;
        public uint Eip;
        public uint SegCs;
        public uint EFlags;
        public uint Esp;
        public uint SegSs;
        // Retrieved by CONTEXT_EXTENDED_REGISTERS 
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] ExtendedRegisters;
    }

    // x64 m128a
    [StructLayout(LayoutKind.Sequential)]
    public struct M128A
    {
        public ulong High;
        public long Low;

        public override string ToString()
        {
            return string.Format("High:{0}, Low:{1}", this.High, this.Low);
        }
    }

    // x64 save format
    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    public struct XSAVE_FORMAT64
    {
        public ushort ControlWord;
        public ushort StatusWord;
        public byte TagWord;
        public byte Reserved1;
        public ushort ErrorOpcode;
        public uint ErrorOffset;
        public ushort ErrorSelector;
        public ushort Reserved2;
        public uint DataOffset;
        public ushort DataSelector;
        public ushort Reserved3;
        public uint MxCsr;
        public uint MxCsr_Mask;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public M128A[] FloatRegisters;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public M128A[] XmmRegisters;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 96)]
        public byte[] Reserved4;
    }


    public enum CONTEXT_FLAGS : uint
    {
        CONTEXT_i386 = 0x10000,
        CONTEXT_i486 = 0x10000,   //  same as i386
        CONTEXT_CONTROL = CONTEXT_i386 | 0x01, // SS:SP, CS:IP, FLAGS, BP
        CONTEXT_INTEGER = CONTEXT_i386 | 0x02, // AX, BX, CX, DX, SI, DI
        CONTEXT_SEGMENTS = CONTEXT_i386 | 0x04, // DS, ES, FS, GS
        CONTEXT_FLOATING_POINT = CONTEXT_i386 | 0x08, // 387 state
        CONTEXT_DEBUG_REGISTERS = CONTEXT_i386 | 0x10, // DB 0-3,6,7
        CONTEXT_EXTENDED_REGISTERS = CONTEXT_i386 | 0x20, // cpu specific extensions
        CONTEXT_FULL = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_SEGMENTS,
        CONTEXT_ALL = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_SEGMENTS | CONTEXT_FLOATING_POINT | CONTEXT_DEBUG_REGISTERS | CONTEXT_EXTENDED_REGISTERS
    }

    // x64 context structure
    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    public struct CONTEXT64
    {
        public ulong P1Home;
        public ulong P2Home;
        public ulong P3Home;
        public ulong P4Home;
        public ulong P5Home;
        public ulong P6Home;

        public CONTEXT_FLAGS ContextFlags;
        public uint MxCsr;

        public ushort SegCs;
        public ushort SegDs;
        public ushort SegEs;
        public ushort SegFs;
        public ushort SegGs;
        public ushort SegSs;
        public uint EFlags;

        public ulong Dr0;
        public ulong Dr1;
        public ulong Dr2;
        public ulong Dr3;
        public ulong Dr6;
        public ulong Dr7;

        public ulong Rax;
        public ulong Rcx;
        public ulong Rdx;
        public ulong Rbx;
        public ulong Rsp;
        public ulong Rbp;
        public ulong Rsi;
        public ulong Rdi;
        public ulong R8;
        public ulong R9;
        public ulong R10;
        public ulong R11;
        public ulong R12;
        public ulong R13;
        public ulong R14;
        public ulong R15;
        public ulong Rip;

        public XSAVE_FORMAT64 DUMMYUNIONNAME;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
        public M128A[] VectorRegister;
        public ulong VectorControl;

        public ulong DebugControl;
        public ulong LastBranchToRip;
        public ulong LastBranchFromRip;
        public ulong LastExceptionToRip;
        public ulong LastExceptionFromRip;
    }

    public enum ThreadAccess : int
    {
        TERMINATE = (0x0001),
        SUSPEND_RESUME = (0x0002),
        GET_CONTEXT = (0x0008),
        SET_CONTEXT = (0x0010),
        SET_INFORMATION = (0x0020),
        QUERY_INFORMATION = (0x0040),
        SET_THREAD_TOKEN = (0x0080),
        IMPERSONATE = (0x0100),
        DIRECT_IMPERSONATION = (0x0200),
        THREAD_HIJACK = SUSPEND_RESUME | GET_CONTEXT | SET_CONTEXT,
        THREAD_ALL = TERMINATE | SUSPEND_RESUME | GET_CONTEXT | SET_CONTEXT | SET_INFORMATION | QUERY_INFORMATION | SET_THREAD_TOKEN | IMPERSONATE | DIRECT_IMPERSONATION
    }
    #endregion
    #endregion
}

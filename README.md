# External
Utility library for external cheats

In development, so many features are WIP (ie: TLB Parsing, QuartzASM (which is probably trashed), etc.)

This library aims to produce a utility to speed up workflow and provide additional functionality commonly needed for developing IPC-dependent tools.

# Current Features

## Memory Editing

- SetValue<type>(address, value), GetValue<type>(address); Supports all value types, including structs (shallow copy).
- SetArray<type>(address, array), GetArray<type>(address); Read an array of values, including structs (shallow copy).
- game["module"]; Gets the address of a module in memory
- game["module"]["function"]; Gets the address of a function in the module
- game["module"][offset]; Automatically offsets from the base of the module
- game[offset]; Automatically offsets from the game base.
- Simple memory searcher (copied from memory.dll)
- QuickAlloc(size, protections); Allocates memory as needed.
- OpenHandle/CloseHandle; Open or close a handle to the process as needed

## Built in RPC and Manual Mapping

- Game.Call<return_type>(address, params...); Call a remote procedure using the configured RPC type.
- Game.CallRef<return_type>(address, ref params...); Call a remote procedure and auto-marshal back any ref params
- Multiple execution methods supported: NtCreateThreadEx, Thread Hijacking, QUAPC2, and more in the future.
- LoadAndRegisterDllRemote(path); Remote call to LdrLoadDLL.
- MapModule(moduleData, loadOptions); Manual map a dll to memory. Still WIP but is functional for many simple DLLs.

## Utility Functions for Seamless C#/CPP Interop
- PointerEx; Value type struct wrapper for IntPtr which provides many overloaded methods and operators for more c-like numerical manipulation.
- byte[].ToStruct<type>(); Easily convert a byte array into a struct type.
- struct.ToByteArray(); Easily convert a value type into a byte array.
- string.Bytes(); Convert a string into a null-terminated c-str byte array.
- byte[].String(offset=0); Convert a null terminated c-str byte array into a c# string.


# Special Thanks
https://github.com/Dewera/Lunar \
https://github.com/cobbr/SharpSploit

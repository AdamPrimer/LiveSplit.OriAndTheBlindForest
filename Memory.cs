using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Devil
{
    class Memory
    {
        private static int counter;
        private static bool runCounter;

        const int MEM_COMMIT = 0x00001000;
        const int MEM_PRIVATE = 0x00020000;
        const int PAGE_READWRITE = 0x76;
        const int PAGE_EXECUTE_READWRITE = 0x40;

        public static int Counter(bool onoff) {
            runCounter = onoff;
            if (onoff) counter = 0;
            return counter;
        }

        public static int GetInt32(Process proc, int addr) {
            if (runCounter) counter++;

            byte[] buffer = new byte[4];
            int bytesRead;
            SafeNativeMethods.ReadProcessMemory(proc.Handle, (IntPtr)(addr), buffer, 4, out bytesRead);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static byte[] GetBytes(Process proc, int addr, int numBytes) {
            if (runCounter) counter++;

            byte[] buffer = new byte[numBytes];
            int bytesRead;
            SafeNativeMethods.ReadProcessMemory(proc.Handle, (IntPtr)(addr), buffer, numBytes, out bytesRead);
            return buffer;
        }

        public static bool GetBool(Process proc, int addr) {
            if (runCounter) counter++;

            byte[] buffer = new byte[4];
            int bytesRead;
            SafeNativeMethods.ReadProcessMemory(proc.Handle, (IntPtr)(addr), buffer, 1, out bytesRead);
            return BitConverter.ToBoolean(buffer, 0);
        }

        public static float GetFloat(Process proc, int addr) {
            if (runCounter) counter++;

            byte[] buffer = new byte[4];
            int bytesRead;
            SafeNativeMethods.ReadProcessMemory(proc.Handle, (IntPtr)(addr), buffer, 4, out bytesRead);
            return BitConverter.ToSingle(buffer, 0);
        }

        public static void PrintModuleAddresses(Process proc) {
            foreach (ProcessModule module in proc.Modules) {
                Console.WriteLine(module.ModuleName + " " + module.BaseAddress.ToInt32().ToString("X"));
            }
        }

        public static int GetModuleAddress(Process proc, string moduleName) {
            foreach (ProcessModule module in proc.Modules) {
                if (module.ModuleName.Equals(moduleName, StringComparison.OrdinalIgnoreCase)) {
                    return module.BaseAddress.ToInt32();
                }
            }
            return 0;
        }

        public static int[] FindMemorySignatures(Process targetProcess, params string[] searchStrings) {
            int[] returnAddresses = new int[searchStrings.Length];
            MemoryByteCode[] byteCodes = new MemoryByteCode[searchStrings.Length];
            for (int i = 0; i < searchStrings.Length; i++) {
                byteCodes[i] = GetByteCode(searchStrings[i]);
            }

            SystemInfo sysInfo;
            SafeNativeMethods.GetSystemInfo(out sysInfo);
            int minAddress = (int)sysInfo.minimumApplicationAddress;
            int maxAddress = (int)sysInfo.maximumApplicationAddress;
            MemoryInfo memInfo;

            int totalBytesRead = 0;
            int foundAddresses = 0;

            while (minAddress < maxAddress && foundAddresses < searchStrings.Length) {
                SafeNativeMethods.VirtualQueryEx(targetProcess.Handle, (IntPtr)minAddress, out memInfo, 28);

                // if this memory chunk is accessible
                if ((memInfo.AllocationProtect & PAGE_EXECUTE_READWRITE) != 0 && memInfo.lType == MEM_PRIVATE && memInfo.State == MEM_COMMIT) {
                    byte[] buffer = new byte[memInfo.RegionSize];

                    int bytesRead = 0;
                    // read everything in the buffer above
                    if (SafeNativeMethods.ReadProcessMemory(targetProcess.Handle, (IntPtr)memInfo.BaseAddress, buffer, memInfo.RegionSize, out bytesRead)) {
                        totalBytesRead += bytesRead;

                        for (int i = 0; i < searchStrings.Length; i++) {
                            if (returnAddresses[i] == 0) {
                                if (SearchMemory(buffer, byteCodes[i], minAddress, ref returnAddresses[i])) {
                                    foundAddresses++;
                                }
                            }
                        }
                    }
                }

                // move to the next memory chunk
                minAddress += memInfo.RegionSize;
            }

            return returnAddresses;
        }

        private static bool SearchMemory(byte[] buffer, MemoryByteCode byteCode, int currentAddress, ref int foundAddress) {
            byte[] bytes = byteCode.byteCode;
            byte[] wild = byteCode.wildCards;
            for (int i = 0, j = 0; i <= buffer.Length - bytes.Length; i++) {
                int k = i;
                while (j < bytes.Length && (wild[j] == 1 || buffer[k] == bytes[j])) {
                    k++; j++;
                }
                if (j == bytes.Length) {
                    foundAddress = currentAddress + i + bytes.Length + byteCode.offset;
                    return true;
                }
                j = 0;
            }
            return false;
        }

        private static MemoryByteCode GetByteCode(string searchString) {
            int offsetIndex = searchString.IndexOf("|");
            offsetIndex = offsetIndex < 0 ? searchString.Length : offsetIndex;

            if (offsetIndex % 2 != 0) {
                Console.WriteLine(searchString + " is of odd length.");
                return null;
            }

            byte[] byteCode = new byte[offsetIndex / 2];
            byte[] wildCards = new byte[offsetIndex / 2];
            for (int i = 0, j = 0; i < offsetIndex; i++) {
                byte temp = (byte)(((int)searchString[i] - 0x30) & 0x1F);
                byteCode[j] |= temp > 0x09 ? (byte)(temp - 7) : temp;
                if (searchString[i] == '?') {
                    wildCards[j] = 1;
                }
                if ((i & 1) == 1) {
                    j++;
                } else {
                    byteCode[j] <<= 4;
                }
            }
            int offset = 0;
            if (offsetIndex < searchString.Length) {
                int.TryParse(searchString.Substring(offsetIndex + 1), out offset);
            }
            return new MemoryByteCode(byteCode, wildCards, offset);
        }

        private class MemoryByteCode
        {
            public byte[] byteCode;
            public byte[] wildCards;
            public int offset;

            public MemoryByteCode(byte[] byteCode, byte[] wildCards, int offset) {
                this.byteCode = byteCode;
                this.wildCards = wildCards;
                this.offset = offset;
            }
        }

        public struct MemoryInfo
        {
            public int BaseAddress;
            public int AllocationBase;
            public int AllocationProtect;
            public int RegionSize;
            public int State;
            public int Protect;
            public int lType;
        }

        public struct SystemInfo
        {
            public ushort processorArchitecture;
            ushort reserved;
            public uint pageSize;
            public IntPtr minimumApplicationAddress;
            public IntPtr maximumApplicationAddress;
            public IntPtr activeProcessorMask;
            public uint numberOfProcessors;
            public uint processorType;
            public uint allocationGranularity;
            public ushort processorLevel;
            public ushort processorRevision;
        }

        internal static class SafeNativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);
            [DllImport("kernel32.dll")]
            public static extern void GetSystemInfo(out SystemInfo lpSystemInfo);
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MemoryInfo lpBuffer, uint dwLength);
        }
    }
}

using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using LiveSplit.OriAndTheBlindForest;
using LiveSplit.OriAndTheBlindForest.State;

namespace LiveSplit.OriAndTheBlindForest.Memory
{
    public class Memory
    {
        const int MEM_COMMIT = 0x00001000;
        const int MEM_PRIVATE = 0x00020000;
        const int PAGE_EXECUTE_READWRITE = 0x40;

        public static IntPtr GetForegroundWindow() {
            return SafeNativeMethods.GetForegroundWindow();
        }
        public static Vector4 GetProcessRect(Process targetProcess) {
            Rect rect = new Rect();
            Point ptUL = new Point();
            Point ptLR = new Point();

            try {
                SafeNativeMethods.GetClientRect(targetProcess.MainWindowHandle, ref rect);

                ptUL.X = rect.left;
                ptUL.Y = rect.top;
                ptLR.X = rect.right;
                ptLR.Y = rect.bottom;

                SafeNativeMethods.ClientToScreen(targetProcess.MainWindowHandle, ref ptUL);
                SafeNativeMethods.ClientToScreen(targetProcess.MainWindowHandle, ref ptLR);
            } catch { }
            return new Vector4(ptUL.X, ptUL.Y, ptLR.X - ptUL.X, ptLR.Y - ptUL.Y);
        }
        public static T ReadValue<T>(Process targetProcess, int address, params int[] offsets) {
            byte[] buffer = new byte[8];
            int bytesRead;

            try {
                for (int i = 0; i < offsets.Length - 1; i++) {
                    SafeNativeMethods.ReadProcessMemory(targetProcess.Handle, (IntPtr)(address + offsets[i]), buffer, 4, out bytesRead);
                    address = BitConverter.ToInt32(buffer, 0);
                }
                int last = offsets.Length > 0 ? offsets[offsets.Length - 1] : 0;
                if (typeof(T) == typeof(int)) {
                    SafeNativeMethods.ReadProcessMemory(targetProcess.Handle, (IntPtr)(address + last), buffer, 4, out bytesRead);
                    return (T)(object)BitConverter.ToInt32(buffer, 0);
                } else if (typeof(T) == typeof(long)) {
                    SafeNativeMethods.ReadProcessMemory(targetProcess.Handle, (IntPtr)(address + last), buffer, 8, out bytesRead);
                    return (T)(object)BitConverter.ToInt64(buffer, 0);
                } else if (typeof(T) == typeof(byte)) {
                    SafeNativeMethods.ReadProcessMemory(targetProcess.Handle, (IntPtr)(address + last), buffer, 1, out bytesRead);
                    buffer[1] = 0;
                    return (T)(object)(byte)BitConverter.ToInt16(buffer, 0);
                } else if (typeof(T) == typeof(short)) {
                    SafeNativeMethods.ReadProcessMemory(targetProcess.Handle, (IntPtr)(address + last), buffer, 2, out bytesRead);
                    return (T)(object)BitConverter.ToInt64(buffer, 0);
                } else if (typeof(T) == typeof(float)) {
                    SafeNativeMethods.ReadProcessMemory(targetProcess.Handle, (IntPtr)(address + last), buffer, 4, out bytesRead);
                    return (T)(object)BitConverter.ToSingle(buffer, 0);
                } else if (typeof(T) == typeof(double)) {
                    SafeNativeMethods.ReadProcessMemory(targetProcess.Handle, (IntPtr)(address + last), buffer, 8, out bytesRead);
                    return (T)(object)BitConverter.ToDouble(buffer, 0);
                } else if (typeof(T) == typeof(bool)) {
                    SafeNativeMethods.ReadProcessMemory(targetProcess.Handle, (IntPtr)(address + last), buffer, 1, out bytesRead);
                    return (T)(object)BitConverter.ToBoolean(buffer, 0);
                }
            } catch { }
            if (typeof(T) == typeof(int)) {
                return (T)(object)address;
            } else if (typeof(T) == typeof(long)) {
                return (T)(object)(long)address;
            } else if (typeof(T) == typeof(byte)) {
                return (T)(object)(byte)0;
            } else if (typeof(T) == typeof(short)) {
                return (T)(object)(short)0;
            } else if (typeof(T) == typeof(float)) {
                return (T)(object)(float)0;
            } else if (typeof(T) == typeof(double)) {
                return (T)(object)(double)0;
            } else {
                return (T)(object)false;
            }
        }
        public static byte[] GetBytes(Process proc, int addr, int numBytes) {
            byte[] buffer = new byte[numBytes];
            int bytesRead;
            SafeNativeMethods.ReadProcessMemory(proc.Handle, (IntPtr)addr, buffer, numBytes, out bytesRead);
            return buffer;
        }

        public static int[] FindMemorySignatures(Process targetProcess, params string[] searchStrings) {
            int[] returnAddresses = new int[searchStrings.Length];
            MemorySignature[] byteCodes = new MemorySignature[searchStrings.Length];
            for (int i = 0; i < searchStrings.Length; i++) {
                byteCodes[i] = GetSignature(searchStrings[i]);
            }

            try {
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
            } catch { }

            return returnAddresses;
        }

        private static bool SearchMemory(byte[] buffer, MemorySignature byteCode, int currentAddress, ref int foundAddress) {
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

        private static MemorySignature GetSignature(string searchString) {
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
            return new MemorySignature(byteCode, wildCards, offset);
        }

        private class MemorySignature
        {
            public byte[] byteCode;
            public byte[] wildCards;
            public int offset;

            public MemorySignature(byte[] byteCode, byte[] wildCards, int offset) {
                this.byteCode = byteCode;
                this.wildCards = wildCards;
                this.offset = offset;
            }
        }

        private struct MemoryInfo
        {
            public int BaseAddress;
            public int AllocationBase;
            public int AllocationProtect;
            public int RegionSize;
            public int State;
            public int Protect;
            public int lType;
        }
        private struct SystemInfo
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
        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private static class SafeNativeMethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);
            [DllImport("kernel32.dll")]
            public static extern void GetSystemInfo(out SystemInfo lpSystemInfo);
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MemoryInfo lpBuffer, uint dwLength);
            [DllImport("user32.dll")]
            public static extern IntPtr GetClientRect(IntPtr hWnd, ref Rect rect);
            [DllImport("user32.dll")]
            public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);
        }
    }
}
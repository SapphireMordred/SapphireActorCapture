using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SapphireActorCapture
{
    static class Memory
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        private const int PROCESS_WM_READ = 0x0010;

        public static int getZoneId()
        {
            return (int)readUInt32(0x0188F114);
        }

        private static UInt32 readUInt32(int offset)
        {
            Process process = Process.GetProcessById(Globals.ffxivPid);
            IntPtr processHandle = OpenProcess(PROCESS_WM_READ, false, process.Id);

            int bytesRead = 0;
            byte[] buffer = new byte[4];

            ReadProcessMemory((int)processHandle, offset, buffer, buffer.Length, ref bytesRead);

            return BitConverter.ToUInt32(buffer, 0);
        }
    }
}

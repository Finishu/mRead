using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace reader {

    class mReader
    {
        public static void Main(string[] Args)
        {
            Mem memLib = new Mem();
            memLib.OpenProcess("Tutorial-i386");
            byte[] buf = BitConverter.GetBytes((float)5000);
            byte[] buf2 = BitConverter.GetBytes((double)5000);
            if (memLib.WriteBytes(0x017A4510, buf) | memLib.WriteBytes(0x017A4518, buf2))
            {
                Console.WriteLine("Bytes escritos!");
            } else
            {
                Console.WriteLine("Não foi possível realizar esta ação! Erro: " + Marshal.GetLastWin32Error());
            }
            memLib.CloseHandle();
            Console.ReadKey();
        }
    }
    public class Mem
    {
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool ReadMemoryProcess(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UIntPtr nSize, IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);

        IntPtr hProc;

        public void OpenProcess(string process)
        {
            bool flag1 = process.Contains(".exe");
            if (flag1)
            {
                process = process.Replace(".exe", "");
            }
            Process[] p = Process.GetProcessesByName(process);
            hProc = OpenProcess(0x1F0FFF, false, p[0].Id);
        }

        public bool WriteBytes(int Address, byte[] buffer)
        {
            IntPtr Addr = (IntPtr)Address;
            UIntPtr size = (UIntPtr)buffer.Length;

            bool result = WriteProcessMemory(hProc, Addr, buffer, size, IntPtr.Zero);
            return result;
        }

        public byte[] ReadBytes(int Address, int Length)
        {
            IntPtr Addr = (IntPtr)Address;
            byte[] buffer = new byte[Length];
            uint size = (uint)Length;
            uint BytesRead = 0;
            ReadMemoryProcess(hProc, Addr, buffer, size, ref BytesRead);
            return buffer;
        }

        public void CloseHandle()
        {
            CloseHandle(hProc);
        }
    }
}
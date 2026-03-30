using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Chernobyl_Relay_Chat
{
    static class ProcessExtension
    {
        /// <summary>
        /// Returns the full path of the process executable, cross-platform.
        /// On Windows this uses QueryFullProcessImageName via kernel32 so it
        /// works even for elevated processes (Vista+).  On Linux/macOS .NET
        /// reads /proc/{pid}/exe (or the OS equivalent) through MainModule.
        /// </summary>
        public static string GetProcessPath(this Process process)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return GetProcessPathWindows(process.Id);

            return process.MainModule?.FileName ?? string.Empty;
        }

        // ── Windows-specific P/Invoke ────────────────────────────────────────

        [Flags]
        private enum ProcessAccessFlags : uint
        {
            QueryLimitedInformation = 0x00001000,
        }

        [DllImport("kernel32.dll")]
        private static extern bool QueryFullProcessImageName(IntPtr hprocess, int dwFlags, StringBuilder lpExeName, out int size);
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hHandle);

        private static string GetProcessPathWindows(int processId)
        {
            StringBuilder buffer = new StringBuilder(1024);
            IntPtr hprocess = OpenProcess(ProcessAccessFlags.QueryLimitedInformation, false, processId);
            if (hprocess != IntPtr.Zero)
            {
                try
                {
                    int size = buffer.Capacity;
                    if (QueryFullProcessImageName(hprocess, 0, buffer, out size))
                        return buffer.ToString();
                }
                finally
                {
                    CloseHandle(hprocess);
                }
            }
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}

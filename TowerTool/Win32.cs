using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TowerTool
{
    public class Win32
    {
        // Get a handle to an application window.
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        private static IntPtr FindWindowHandleByName(string name)
        {
            IntPtr hWnd = IntPtr.Zero;
            foreach (Process pList in Process.GetProcesses())
            {
                if (pList.MainWindowTitle.Contains(name))
                {
                    hWnd = pList.MainWindowHandle;
                }
            }
            return hWnd;
        }

        public static IntPtr FocusGame()
        {
            var handle = FindWindowHandleByName("Chrome");
            SwitchToThisWindow(handle, true);
            SetForegroundWindow(handle);
            return handle;
        }
    }
}

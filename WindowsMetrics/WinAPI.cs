using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using WindowsMetrics.Declarations;
using WindowsMetrics.Enums;
using WindowsMetrics.Exceptions;
using WindowsMetrics.Structures;
using WinEventDelegate = WindowsMetrics.Declarations.Delegates.WinEventDelegate;
using HookProc = WindowsMetrics.Declarations.Delegates.HookProc;

namespace WindowsMetrics
{
    public static class WinAPI
    {
        private const int DefaultBufferSize = 1024;

        /// <param name="window">Pointer to a window</param>
        /// <returns>Text of the title or NULL if there's no title (length == 0)</returns>
        /// <exception cref="">If pointer to a window is NULL</exception>
        private static string GetTextOfWindow(IntPtr window)
        {
            if (window != IntPtr.Zero)
            {
                int charsToObtain = 1024;
                StringBuilder buffer = new StringBuilder(DefaultBufferSize);
                WinAPIDeclarations.GetWindowText(window, buffer, charsToObtain);
                return buffer.Length != 0 ? buffer.ToString() : null;
            }
            else
            {
                //TODO solution
                //throw new ZeroHandleToWindowException();
                return null;
            }
        }

        public static string GetTextOfForegroundWindow()
        {
            IntPtr hwnd = WinAPIDeclarations.GetForegroundWindow();
            return GetTextOfWindow(hwnd);
        }

        public static string GetSystemUserName()
        {
            int size = 1024;
            StringBuilder buffer = new StringBuilder(DefaultBufferSize);
            bool success = WinAPIDeclarations.GetUserName(buffer, ref size);
            return buffer.ToString();
        }

        public static DateTime GetGMTDateTime()
        {
            SYSTEMTIME time;
            WinAPIDeclarations.GetSystemTime(out time);
            return new DateTime(time.Year, time.Month, time.Day, 
                time.Hour, time.Minute, time.Second, time.Milliseconds);
        }

        public static DateTime GetLocalDateTime()
        {
            SYSTEMTIME time;
            WinAPIDeclarations.GetLocalTime(out time);
            return new DateTime(time.Year, time.Month, time.Day,
                time.Hour, time.Minute, time.Second, time.Milliseconds);
        }

        public static IntPtr StartTrackingForegroundWindowChange(Action onWindowChangeAction, out GCHandle gcHandle)
        {
            WinEventDelegate action = (hook, type, hwnd, idObject, child, thread, time) =>
            {
                onWindowChangeAction.Invoke();
            };
            gcHandle = GCHandle.Alloc(action);
            return WinAPIDeclarations.SetWinEventHook((uint)Event.EVENT_SYSTEM_FOREGROUND, 
                (uint)Event.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, action, 0, 0, (uint)Event.WINEVENT_OUTOFCONTEXT);
        }

        public static bool StopTrackingForegroundWindowChange(IntPtr hook)
        {
            return WinAPIDeclarations.UnhookWinEvent(hook);
        }

        public static uint GetForegroundWindowProcessID()
        {
            uint pid;
            IntPtr fwin = WinAPIDeclarations.GetForegroundWindow();
            uint idCreatorThread = WinAPIDeclarations.GetWindowThreadProcessId(fwin, out pid); // identifier of the thread that created the window
            return pid;
        }

        /// <exception cref="">If the window process closing fails</exception>
        public static string GetForegroundWindowExeModulePath()
        {
            string result = null;
            uint processId = GetForegroundWindowProcessID();
            IntPtr processHandle = WinAPIDeclarations.OpenProcess(
                    ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VirtualMemoryRead, true, (int)processId);
            if (processHandle != IntPtr.Zero)
            {
                int size = 1024;
                StringBuilder buffer = new StringBuilder(DefaultBufferSize);
                uint length = WinAPIDeclarations.GetModuleFileNameEx(processHandle, IntPtr.Zero, buffer, size); // returns length of the string copied to the buffer (0 if failed)
                if (length > 0)
                    result = buffer.ToString();

                bool closed = WinAPIDeclarations.CloseHandle(processHandle);
                if (!closed)
                    throw new CloseProcessException();
            }
            return result;
        }

        public static IntPtr StartTrackingLeftClickEvent(Action onLeftClickAction, out GCHandle gcHandle)
        {
            HookProc action = (code, param, lParam) =>
            {
                if (param.ToInt32() == (int)WindowsMessageCode.WM_LBUTTONUP)
                    onLeftClickAction.Invoke();
                return IntPtr.Zero;
            };
            gcHandle = GCHandle.Alloc(action);
            return WinAPIDeclarations.SetWindowsHookEx(HookType.WH_MOUSE_LL, action, IntPtr.Zero, 0);
        }

        public static bool StopTrackingLeftClickEvent(IntPtr hook)
        {
            return WinAPIDeclarations.UnhookWindowsHookEx(hook);
        }

        public static Point GetMousePosition()
        {
            Point point = new Point();
            bool success = WinAPIDeclarations.GetCursorPos(ref point);
            return point;
        }

        public static IntPtr GetWindowUnderCursor()
        {
            Point point = new Point();
            bool success = WinAPIDeclarations.GetCursorPos(ref point);
            return WinAPIDeclarations.WindowFromPoint(point);
        }

        public static string GetTextOfWindowUnderCursor()
        {
            IntPtr windowUnderCursor = GetWindowUnderCursor();
            return GetTextOfWindow(windowUnderCursor);
        }

        public static string GetTopWindowText()
        {
            IntPtr desktop = WinAPIDeclarations.GetDesktopWindow();
            IntPtr topWindow = WinAPIDeclarations.GetTopWindow(desktop);
            return GetTextOfWindow(topWindow);
        }

        public static string GetForegroundWindowProcessName()
        {
            uint pid = GetForegroundWindowProcessID();
            Process p = Process.GetProcessById((int) pid);
            return p.ProcessName;
        }

        public static string GetChromeUrl(Process process)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));

            if (process.MainWindowHandle == IntPtr.Zero)
                return null;

            AutomationElement element = AutomationElement.FromHandle(process.MainWindowHandle);
            if (element == null)
                return null;

            AutomationElementCollection edits5 = element.FindAll(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
            AutomationElement edit = edits5[0];
            string vp = ((ValuePattern)edit.GetCurrentPattern(ValuePattern.Pattern)).Current.Value as string;
            Console.WriteLine(vp);
            return vp;
        }

        public static void GetAdapters()
        {
            long structSize = Marshal.SizeOf(typeof(IP_ADAPTER_INFO));
            IntPtr pArray = Marshal.AllocHGlobal(new IntPtr(structSize));

            int ret = WinAPIDeclarations.GetAdaptersInfo(pArray, ref structSize);

            if (ret == (int)AdaptersConsts.ERROR_BUFFER_OVERFLOW) // ERROR_BUFFER_OVERFLOW == 111
            {
                // Buffer was too small, reallocate the correct size for the buffer.
                pArray = Marshal.ReAllocHGlobal(pArray, new IntPtr(structSize));

                ret = WinAPIDeclarations.GetAdaptersInfo(pArray, ref structSize);
            }

            if (ret == 0)
            {
                // Call Succeeded
                IntPtr pEntry = pArray;

                do
                {
                    // Retrieve the adapter info from the memory address
                    IP_ADAPTER_INFO entry = (IP_ADAPTER_INFO)Marshal.PtrToStructure(pEntry, typeof(IP_ADAPTER_INFO));

                    // ***Do something with the data HERE!***
                    Console.WriteLine("\n");
                    Console.WriteLine("Index: {0}", entry.Index.ToString());

                    // Adapter Type
                    string tmpString = string.Empty;
                    switch (entry.Type)
                    {
                        case (uint)AdaptersConsts.MIB_IF_TYPE_ETHERNET: tmpString = "Ethernet"; break;
                        case (uint)AdaptersConsts.MIB_IF_TYPE_TOKENRING: tmpString = "Token Ring"; break;
                        case (uint)AdaptersConsts.MIB_IF_TYPE_FDDI: tmpString = "FDDI"; break;
                        case (uint)AdaptersConsts.MIB_IF_TYPE_PPP: tmpString = "PPP"; break;
                        case (uint)AdaptersConsts.MIB_IF_TYPE_LOOPBACK: tmpString = "Loopback"; break;
                        case (uint)AdaptersConsts.MIB_IF_TYPE_SLIP: tmpString = "Slip"; break;
                        default: tmpString = "Other/Unknown"; break;
                    } // switch
                    Console.WriteLine("Adapter Type: {0}", tmpString);

                    Console.WriteLine("Name: {0}", entry.AdapterName);
                    Console.WriteLine("Desc: {0}\n", entry.AdapterDescription);

                    Console.WriteLine("DHCP Enabled: {0}", (entry.DhcpEnabled == 1) ? "Yes" : "No");

                    if (entry.DhcpEnabled == 1)
                    {
                        Console.WriteLine("DHCP Server : {0}", entry.DhcpServer.IpAddress.Address);

                        // Lease Obtained (convert from "time_t" to C# DateTime)
                        DateTime pdatDate = new DateTime(1970, 1, 1).AddSeconds(entry.LeaseObtained).ToLocalTime();
                        Console.WriteLine("Lease Obtained: {0}", pdatDate.ToString());

                        // Lease Expires (convert from "time_t" to C# DateTime)
                        pdatDate = new DateTime(1970, 1, 1).AddSeconds(entry.LeaseExpires).ToLocalTime();
                        Console.WriteLine("Lease Expires : {0}\n", pdatDate.ToString());
                    } // if DhcpEnabled

                    Console.WriteLine("IP Address     : {0}", entry.IpAddressList.IpAddress.Address);
                    Console.WriteLine("Subnet Mask    : {0}", entry.IpAddressList.IpMask.Address);
                    Console.WriteLine("Default Gateway: {0}", entry.GatewayList.IpAddress.Address);

                    // MAC Address (data is in a byte[])
                    tmpString = string.Empty;
                    for (int i = 0; i < entry.AddressLength - 1; i++)
                    {
                        tmpString += string.Format("{0:X2}-", entry.Address[i]);
                    }
                    Console.WriteLine("MAC Address    : {0}{1:X2}\n", tmpString, entry.Address[entry.AddressLength - 1]);

                    Console.WriteLine("Has WINS: {0}", entry.HaveWins ? "Yes" : "No");
                    if (entry.HaveWins)
                    {
                        Console.WriteLine("Primary WINS Server  : {0}", entry.PrimaryWinsServer.IpAddress.Address);
                        Console.WriteLine("Secondary WINS Server: {0}", entry.SecondaryWinsServer.IpAddress.Address);
                    } // HaveWins

                    // Get next adapter (if any)
                    pEntry = entry.Next;

                }
                while (pEntry != IntPtr.Zero);

                Marshal.FreeHGlobal(pArray);

            }
            else
            {
                Marshal.FreeHGlobal(pArray);
                throw new InvalidOperationException("GetAdaptersInfo failed: " + ret);
            }

        }
    }
}

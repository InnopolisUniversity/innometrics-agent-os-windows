using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsMetrics.Declarations;
using WindowsMetrics.Enums;
using WindowsMetrics.Exceptions;
using WindowsMetrics.Structures;
using WinEventDelegate = WindowsMetrics.Declarations.Delegates.WinEventDelegate;

namespace WindowsMetrics
{
    public static class WinAPI
    {
        private const int DefaultBufferSize = 1024;

        public static string GetForegroundWindowText()
        {
            int charsToObtain = 1024;
            StringBuilder buffer = new StringBuilder(DefaultBufferSize);
            IntPtr hwnd = WinAPIDeclarations.GetForegroundWindow();
            if (hwnd != IntPtr.Zero)
            {
                WinAPIDeclarations.GetWindowText(hwnd, buffer, charsToObtain);
                return buffer.Length != 0 ? buffer.ToString() : null;
            }
            else
            {
               throw new ZeroHandleToWindowException();
            }
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
            WindowsMetrics.Structures.SYSTEMTIME time;
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

        public static IntPtr StartTrackingForegroundWindowChange(WinEventDelegate action)
        {
            return WinAPIDeclarations.SetWinEventHook((uint)Event.EVENT_SYSTEM_FOREGROUND, 
                (uint)Event.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, action, 0, 0, (uint)Event.WINEVENT_OUTOFCONTEXT);
        }

        public static IntPtr StartTrackingNamechange(WinEventDelegate action, uint idProcess)
        {
            return WinAPIDeclarations.SetWinEventHook((uint)Event.EVENT_OBJECT_NAMECHANGE,
                (uint)Event.EVENT_OBJECT_NAMECHANGE, IntPtr.Zero, action, idProcess, 0, (uint)Event.WINEVENT_OUTOFCONTEXT);
        }

        public static bool Stop(IntPtr hWinEventHook)
        {
            return WinAPIDeclarations.UnhookWinEvent(hWinEventHook);
        }

        public static string GetCurrentExeModulePath()
        {
            return Process.GetCurrentProcess().MainModule.FileName;
        }

        public static uint GetForegroundWindowProcessID()
        {
            IntPtr fwin = WinAPIDeclarations.GetForegroundWindow();
            return WinAPIDeclarations.GetProcessId(fwin);
        }
    }
}

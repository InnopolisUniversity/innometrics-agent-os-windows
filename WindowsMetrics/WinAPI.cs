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
    }
}

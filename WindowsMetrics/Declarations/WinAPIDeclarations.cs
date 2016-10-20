using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Interop;
using WindowsMetrics.Enums;
using WindowsMetrics.Structures;
using WinEventDelegate = WindowsMetrics.Declarations.Delegates.WinEventDelegate;
using HookProc = WindowsMetrics.Declarations.Delegates.HookProc;

namespace WindowsMetrics.Declarations
{
    public static class WinAPIDeclarations
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        ///     Copies the text of the specified window's title bar (if it has one) into a buffer. If the specified window is a
        ///     control, the text of the control is copied. However, GetWindowText cannot retrieve the text of a control in another
        ///     application.
        ///     <para>
        ///     Go to https://msdn.microsoft.com/en-us/library/windows/desktop/ms633520%28v=vs.85%29.aspx  for more
        ///     information
        ///     </para>
        /// </summary>
        /// <param name="hWnd">
        ///     C++ ( hWnd [in]. Type: HWND )<br />A <see cref="IntPtr" /> handle to the window or control containing the text.
        /// </param>
        /// <param name="lpString">
        ///     C++ ( lpString [out]. Type: LPTSTR )<br />The <see cref="StringBuilder" /> buffer that will receive the text. If
        ///     the string is as long or longer than the buffer, the string is truncated and terminated with a null character.
        /// </param>
        /// <param name="nMaxCount">
        ///     C++ ( nMaxCount [in]. Type: int )<br /> Should be equivalent to
        ///     <see cref="StringBuilder.Length" /> after call returns. The <see cref="int" /> maximum number of characters to copy
        ///     to the buffer, including the null character. If the text exceeds this limit, it is truncated.
        /// </param>
        /// <returns>
        ///     If the function succeeds, the return value is the length, in characters, of the copied string, not including
        ///     the terminating null character. If the window has no title bar or text, if the title bar is empty, or if the window
        ///     or control handle is invalid, the return value is zero. To get extended error information, call GetLastError.<br />
        ///     This function cannot retrieve the text of an edit control in another application.
        /// </returns>
        /// <remarks>
        ///     If the target window is owned by the current process, GetWindowText causes a WM_GETTEXT message to be sent to the
        ///     specified window or control. If the target window is owned by another process and has a caption, GetWindowText
        ///     retrieves the window caption text. If the window does not have a caption, the return value is a null string. This
        ///     behavior is by design. It allows applications to call GetWindowText without becoming unresponsive if the process
        ///     that owns the target window is not responding. However, if the target window is not responding and it belongs to
        ///     the calling application, GetWindowText will cause the calling application to become unresponsive. To retrieve the
        ///     text of a control in another process, send a WM_GETTEXT message directly instead of calling GetWindowText.<br />For
        ///     an example go to
        ///     <see cref="!:https://msdn.microsoft.com/en-us/library/windows/desktop/ms644928%28v=vs.85%29.aspx#sending">
        ///     Sending a
        ///     Message.
        ///     </see>
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool GetUserName(System.Text.StringBuilder sb, ref Int32 length);

        [DllImport("kernel32.dll")]
        public static extern void GetSystemTime(out SYSTEMTIME lpSystemTime);

        [DllImport("kernel32.dll")]
        public static extern void GetLocalTime(out SYSTEMTIME lpSystemTime);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, 
            IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("kernel32.dll", SetLastError = true)]
        [PreserveSig]
        public static extern uint GetModuleFileName
        (
            [In]
            IntPtr hModule,

            [Out]
            StringBuilder lpFilename,

            [In]
            [MarshalAs(UnmanagedType.U4)]
            int nSize
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint GetProcessId(IntPtr process);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        /// <summary>
        ///     Installs an application-defined hook procedure into a hook chain. You would install a hook procedure to monitor the
        ///     system for certain types of events. These events are associated either with a specific thread or with all threads
        ///     in the same desktop as the calling thread.
        ///     <para>See https://msdn.microsoft.com/en-us/library/windows/desktop/ms644990%28v=vs.85%29.aspx for more information</para>
        /// </summary>
        /// <param name="hookType">
        ///     C++ ( idHook [in]. Type: int )<br />The type of hook procedure to be installed. This parameter can be one of the
        ///     following values.
        ///     <list type="table">
        ///     <listheader>
        ///         <term>Possible Hook Types</term>
        ///     </listheader>
        ///     <item>
        ///         <term>WH_CALLWNDPROC (4)</term>
        ///         <description>
        ///         Installs a hook procedure that monitors messages before the system sends them to the
        ///         destination window procedure. For more information, see the CallWndProc hook procedure.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>WH_CALLWNDPROCRET (12)</term>
        ///         <description>
        ///         Installs a hook procedure that monitors messages after they have been processed by the
        ///         destination window procedure. For more information, see the CallWndRetProc hook procedure.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>WH_CBT (5)</term>
        ///         <description>
        ///         Installs a hook procedure that receives notifications useful to a CBT application. For more
        ///         information, see the CBTProc hook procedure.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>WH_DEBUG (9)</term>
        ///         <description>
        ///         Installs a hook procedure useful for debugging other hook procedures. For more information,
        ///         see the DebugProc hook procedure.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>WH_FOREGROUNDIDLE (11)</term>
        ///         <description>
        ///         Installs a hook procedure that will be called when the application's foreground thread is
        ///         about to become idle. This hook is useful for performing low priority tasks during idle time. For more
        ///         information, see the ForegroundIdleProc hook procedure.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>WH_GETMESSAGE (3)</term>
        ///         <description>
        ///         Installs a hook procedure that monitors messages posted to a message queue. For more
        ///         information, see the GetMsgProc hook procedure.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>WH_JOURNALPLAYBACK (1)</term>
        ///         <description>
        ///         Installs a hook procedure that posts messages previously recorded by a WH_JOURNALRECORD hook
        ///         procedure. For more information, see the JournalPlaybackProc hook procedure.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>WH_JOURNALRECORD (0)</term>
        ///         <description>
        ///         Installs a hook procedure that records input messages posted to the system message queue. This
        ///         hook is useful for recording macros. For more information, see the JournalRecordProc hook procedure.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>WH_KEYBOARD (2)</term>
        ///         <description>
        ///         Installs a hook procedure that monitors keystroke messages. For more information, see the
        ///         KeyboardProc hook procedure.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>WH_KEYBOARD_LL (13)</term>
        ///         <description>
        ///         Installs a hook procedure that monitors low-level keyboard input events. For more information,
        ///         see the LowLevelKeyboardProc hook procedure.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>WH_MOUSE (7)</term>
        ///         <description>
        ///         Installs a hook procedure that monitors mouse messages. For more information, see the
        ///         MouseProc hook procedure.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>WH_MOUSE_LL (14)</term>
        ///         <description>
        ///         Installs a hook procedure that monitors low-level mouse input events. For more information,
        ///         see the LowLevelMouseProc hook procedure.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>WH_MSGFILTER (-1)</term>
        ///         <description>
        ///         Installs a hook procedure that monitors messages generated as a result of an input event in a
        ///         dialog box, message box, menu, or scroll bar. For more information, see the MessageProc hook procedure.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>WH_SHELL (10)</term>
        ///         <description>
        ///         Installs a hook procedure that receives notifications useful to shell applications. For more
        ///         information, see the ShellProc hook procedure.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>WH_SYSMSGFILTER (6)</term><description></description>
        ///     </item>
        ///     </list>
        /// </param>
        /// <param name="lpfn">
        ///     C++ ( lpfn [in]. Type: HOOKPROC )<br />A pointer to the hook procedure. If the dwThreadId parameter
        ///     is zero or specifies the identifier of a thread created by a different process, the lpfn parameter must point to a
        ///     hook procedure in a DLL. Otherwise, lpfn can point to a hook procedure in the code associated with the current
        ///     process.
        /// </param>
        /// <param name="hMod">
        ///     C++ ( hMod [in]. Type: HINSTANCE )<br />A handle to the DLL containing the hook procedure pointed to
        ///     by the lpfn parameter. The hMod parameter must be set to NULL if the dwThreadId parameter specifies a thread
        ///     created by the current process and if the hook procedure is within the code associated with the current process.
        /// </param>
        /// <param name="dwThreadId">
        ///     C++ ( dwThreadId [in]. Type: DWORD )<br />The identifier of the thread with which the hook
        ///     procedure is to be associated. For desktop apps, if this parameter is zero, the hook procedure is associated with
        ///     all existing threads running in the same desktop as the calling thread. For Windows Store apps, see the Remarks
        ///     section.
        /// </param>
        /// <returns>
        ///     C++ ( Type: HHOOK )<br />If the function succeeds, the return value is the handle to the hook procedure. If
        ///     the function fails, the return value is NULL.
        ///     <para>To get extended error information, call GetLastError.</para>
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     SetWindowsHookEx can be used to inject a DLL into another process. A 32-bit DLL cannot be injected into a
        ///     64-bit process, and a 64-bit DLL cannot be injected into a 32-bit process. If an application requires the use
        ///     of hooks in other processes, it is required that a 32-bit application call SetWindowsHookEx to inject a 32-bit
        ///     DLL into 32-bit processes, and a 64-bit application call SetWindowsHookEx to inject a 64-bit DLL into 64-bit
        ///     processes. The 32-bit and 64-bit DLLs must have different names.
        ///     </para>
        ///     <para>
        ///     Because hooks run in the context of an application, they must match the "bitness" of the application. If a
        ///     32-bit application installs a global hook on 64-bit Windows, the 32-bit hook is injected into each 32-bit
        ///     process (the usual security boundaries apply). In a 64-bit process, the threads are still marked as "hooked."
        ///     However, because a 32-bit application must run the hook code, the system executes the hook in the hooking app's
        ///     context; specifically, on the thread that called SetWindowsHookEx. This means that the hooking application must
        ///     continue to pump messages or it might block the normal functioning of the 64-bit processes.
        ///     </para>
        ///     <para>
        ///     If a 64-bit application installs a global hook on 64-bit Windows, the 64-bit hook is injected into each
        ///     64-bit process, while all 32-bit processes use a callback to the hooking application.
        ///     </para>
        ///     <para>
        ///     To hook all applications on the desktop of a 64-bit Windows installation, install a 32-bit global hook and a
        ///     64-bit global hook, each from appropriate processes, and be sure to keep pumping messages in the hooking
        ///     application to avoid blocking normal functioning. If you already have a 32-bit global hooking application and
        ///     it doesn't need to run in each application's context, you may not need to create a 64-bit version.
        ///     </para>
        ///     <para>
        ///     An error may occur if the hMod parameter is NULL and the dwThreadId parameter is zero or specifies the
        ///     identifier of a thread created by another process.
        ///     </para>
        ///     <para>
        ///     Calling the CallNextHookEx function to chain to the next hook procedure is optional, but it is highly
        ///     recommended; otherwise, other applications that have installed hooks will not receive hook notifications and
        ///     may behave incorrectly as a result. You should call CallNextHookEx unless you absolutely need to prevent the
        ///     notification from being seen by other applications.
        ///     </para>
        ///     <para>
        ///     Before terminating, an application must call the UnhookWindowsHookEx function to free system resources
        ///     associated with the hook.
        ///     </para>
        ///     <para>
        ///     The scope of a hook depends on the hook type. Some hooks can be set only with global scope; others can also
        ///     be set for only a specific thread, as shown in the following table.
        ///     </para>
        ///     <list type="table">
        ///     <listheader>
        ///         <term>Possible Hook Types</term>
        ///     </listheader>
        ///     <item>
        ///         <term>WH_CALLWNDPROC (4)</term>
        ///         <description>Thread or global</description>
        ///     </item>
        ///     <item>
        ///         <term>WH_CALLWNDPROCRET (12)</term>
        ///         <description>Thread or global</description>
        ///     </item>
        ///     <item>
        ///         <term>WH_CBT (5)</term>
        ///         <description>Thread or global</description>
        ///     </item>
        ///     <item>
        ///         <term>WH_DEBUG (9)</term>
        ///         <description>Thread or global</description>
        ///     </item>
        ///     <item>
        ///         <term>WH_FOREGROUNDIDLE (11)</term>
        ///         <description>Thread or global</description>
        ///     </item>
        ///     <item>
        ///         <term>WH_GETMESSAGE (3)</term>
        ///         <description>Thread or global</description>
        ///     </item>
        ///     <item>
        ///         <term>WH_JOURNALPLAYBACK (1)</term>
        ///         <description>Global only</description>
        ///     </item>
        ///     <item>
        ///         <term>WH_JOURNALRECORD (0)</term>
        ///         <description>Global only</description>
        ///     </item>
        ///     <item>
        ///         <term>WH_KEYBOARD (2)</term>
        ///         <description>Thread or global</description>
        ///     </item>
        ///     <item>
        ///         <term>WH_KEYBOARD_LL (13)</term>
        ///         <description>Global only</description>
        ///     </item>
        ///     <item>
        ///         <term>WH_MOUSE (7)</term>
        ///         <description>Thread or global</description>
        ///     </item>
        ///     <item>
        ///         <term>WH_MOUSE_LL (14)</term>
        ///         <description>Global only</description>
        ///     </item>
        ///     <item>
        ///         <term>WH_MSGFILTER (-1)</term>
        ///         <description>Thread or global</description>
        ///     </item>
        ///     <item>
        ///         <term>WH_SHELL (10)</term>
        ///         <description>Thread or global</description>
        ///     </item>
        ///     <item>
        ///         <term>WH_SYSMSGFILTER (6)</term>
        ///         <description>Global only</description>
        ///     </item>
        ///     </list>
        ///     <para>
        ///     For a specified hook type, thread hooks are called first, then global hooks. Be aware that the WH_MOUSE,
        ///     WH_KEYBOARD, WH_JOURNAL*, WH_SHELL, and low-level hooks can be called on the thread that installed the hook
        ///     rather than the thread processing the hook. For these hooks, it is possible that both the 32-bit and 64-bit
        ///     hooks will be called if a 32-bit hook is ahead of a 64-bit hook in the hook chain.
        ///     </para>
        ///     <para>
        ///     The global hooks are a shared resource, and installing one affects all applications in the same desktop as
        ///     the calling thread. All global hook functions must be in libraries. Global hooks should be restricted to
        ///     special-purpose applications or to use as a development aid during application debugging. Libraries that no
        ///     longer need a hook should remove its hook procedure.
        ///     </para>
        ///     <para>
        ///     Windows Store app development If dwThreadId is zero, then window hook DLLs are not loaded in-process for the
        ///     Windows Store app processes and the Windows Runtime broker process unless they are installed by either UIAccess
        ///     processes (accessibility tools). The notification is delivered on the installer's thread for these hooks:
        ///     </para>
        ///     <list type="bullet">
        ///     <item>
        ///         <term>WH_JOURNALPLAYBACK</term>
        ///     </item>
        ///     <item>
        ///         <term>WH_JOURNALRECORD </term>
        ///     </item>
        ///     <item>
        ///         <term>WH_KEYBOARD </term>
        ///     </item>
        ///     <item>
        ///         <term>WH_KEYBOARD_LL </term>
        ///     </item>
        ///     <item>
        ///         <term>WH_MOUSE </term>
        ///     </item>
        ///     <item>
        ///         <term>WH_MOUSE_LL </term>
        ///     </item>
        ///     </list>
        ///     <para>
        ///     This behavior is similar to what happens when there is an architecture mismatch between the hook DLL and the
        ///     target application process, for example, when the hook DLL is 32-bit and the application process 64-bit.
        ///     </para>
        ///     <para>
        ///     For an example, see Installing and
        ///     <see
        ///         cref="!:https://msdn.microsoft.com/en-us/library/windows/desktop/ms644960%28v=vs.85%29.aspx#installing_releasing">
        ///         Releasing
        ///         Hook Procedures.
        ///     </see>
        ///     [
        ///     https://msdn.microsoft.com/en-us/library/windows/desktop/ms644960%28v=vs.85%29.aspx#installing_releasing ]
        ///     </para>
        /// </remarks>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        /// <summary>
        ///     Removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.
        ///     <para>
        ///     See [ https://msdn.microsoft.com/en-us/library/windows/desktop/ms644993%28v=vs.85%29.aspx ] for more
        ///     information
        ///     </para>
        /// </summary>
        /// <param name="hhk">
        ///     C++ ( hhk [in]. Type: HHOOK )<br />A handle to the hook to be removed. This parameter is a hook
        ///     handle obtained by a previous call to <see cref="SetWindowsHookEx" />.
        /// </param>
        /// <returns>
        ///     C++ ( Type: BOOL )
        ///     <c>true</c> or nonzero if the function succeeds, <c>false</c> or zero if the function fails.
        ///     <para>
        ///     To get extended error information, call
        ///     <see cref="!:https://msdn.microsoft.com/en-us/library/windows/desktop/ms679360%28v=vs.85%29.aspx">GetLastError</see>
        ///     .
        ///     </para>
        ///     <para>The return value is the calling thread's last-error code.</para>
        ///     The Return Value section of the documentation for each function that sets the last-error code notes the conditions
        ///     under which the function sets the last-error code. Most functions that set the thread's last-error code set it when
        ///     they fail. However, some functions also set the last-error code when they succeed. If the function is not
        ///     documented to set the last-error code, the value returned by this function is simply the most recent last-error
        ///     code to have been set; some functions set the last-error code to 0 on success and others do not.
        ///     <para></para>
        /// </returns>
        /// <remarks>
        ///     <para>
        ///     The hook procedure can be in the state of being called by another thread even after UnhookWindowsHookEx
        ///     returns. If the hook procedure is not being called concurrently, the hook procedure is removed immediately
        ///     before <see cref="UnhookWindowsHookEx" /> returns.
        ///     </para>
        ///     <para>
        ///     For an example, see
        ///     <see cref="!:https://msdn.microsoft.com/en-us/library/windows/desktop/ms644960%28v=vs.85%29.aspx#system_events">
        ///         Monitoring
        ///         System Events
        ///     </see>
        ///     .
        ///     </para>
        /// </remarks>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point p);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("user32.dll")]
        public static extern IntPtr GetTopWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);
        public static IntPtr OpenProcess(Process proc, ProcessAccessFlags flags)
        {
            return OpenProcess(flags, false, proc.Id);
        }

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);
        
        [DllImport("psapi.dll")]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess,
            IntPtr hModule, [Out] StringBuilder lpBaseName, [In] [MarshalAs(UnmanagedType.U4)] int nSize);
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
    [ExcludeFromCodeCoverage]
    public static class WinAPI
    {
        #region Private part

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
                return null;
            }
        }

        private static uint GetForegroundWindowProcessID()
        {
            uint pid;
            IntPtr fwin = WinAPIDeclarations.GetForegroundWindow();
            uint idCreatorThread = WinAPIDeclarations.GetWindowThreadProcessId(fwin, out pid);
                // identifier of the thread that created the window
            return pid;
        }

        private static IntPtr GetWindowUnderCursor()
        {
            Point point = new Point();
            bool success = WinAPIDeclarations.GetCursorPos(ref point);
            return WinAPIDeclarations.WindowFromPoint(point);
        }

        private static Point GetMousePosition()
        {
            Point point = new Point();
            bool success = WinAPIDeclarations.GetCursorPos(ref point);
            return point;
        }

        private static Process GetForegroundWindowProcess()
        {
            uint pid = GetForegroundWindowProcessID();
            Process p = Process.GetProcessById((int) pid);
            return p;
        }

        #endregion

        // ---------------

        #region TrackingForegroundWindowChange

        public static IntPtr StartTrackingForegroundWindowChange(Action onWindowChangeAction, out GCHandle gcHandle)
        {
            WinEventDelegate action = (hook, type, hwnd, idObject, child, thread, time) =>
            {
                onWindowChangeAction.Invoke();
            };
            gcHandle = GCHandle.Alloc(action);
            return WinAPIDeclarations.SetWinEventHook((uint) Event.EVENT_SYSTEM_FOREGROUND,
                (uint) Event.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, action, 0, 0, (uint) Event.WINEVENT_OUTOFCONTEXT);
        }

        public static bool StopTrackingForegroundWindowChange(IntPtr hook)
        {
            return WinAPIDeclarations.UnhookWinEvent(hook);
        }

        #endregion

        #region TrackingLeftClickEvent

        public static IntPtr StartTrackingLeftClickEvent(Action onLeftClickAction, out GCHandle gcHandle)
        {
            HookProc action = (code, param, lParam) =>
            {
                if (param.ToInt32() == (int) WindowsMessageCode.WM_LBUTTONUP)
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

        #endregion

        // ---------------

        public static IntPtr GetForegroundWindowId()
        {
            return WinAPIDeclarations.GetForegroundWindow();
        }

        public static string GetTextOfForegroundWindow()
        {
            IntPtr hwnd = WinAPIDeclarations.GetForegroundWindow();
            return GetTextOfWindow(hwnd);
        }

        public static string GetSystemUserName()
        {
            int size = DefaultBufferSize;
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

        /// <exception cref="CloseProcessException">If the window process closing fails</exception>
        public static string GetForegroundWindowExeModulePath()
        {
            string result = null;
            uint processId = GetForegroundWindowProcessID();
            IntPtr processHandle = WinAPIDeclarations.OpenProcess(
                ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VirtualMemoryRead, true, (int) processId);
            if (processHandle != IntPtr.Zero)
            {
                int size = 1024;
                StringBuilder buffer = new StringBuilder(DefaultBufferSize);
                uint length = WinAPIDeclarations.GetModuleFileNameEx(processHandle, IntPtr.Zero, buffer, size);
                    // returns length of the string copied to the buffer (0 if failed)
                if (length > 0)
                    result = buffer.ToString();

                bool closed = WinAPIDeclarations.CloseHandle(processHandle);
                if (!closed)
                    throw new CloseProcessException();
            }
            return result;
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

        public static void GetAdapters(out string IP, out string MAC)
        {
            StringBuilder s = new StringBuilder();

            long structSize = Marshal.SizeOf(typeof(IP_ADAPTER_INFO));
            IntPtr pArray = Marshal.AllocHGlobal(new IntPtr(structSize));

            int ret = WinAPIDeclarations.GetAdaptersInfo(pArray, ref structSize);

            if (ret == (int) AdaptersConsts.ERROR_BUFFER_OVERFLOW) // ERROR_BUFFER_OVERFLOW == 111
            {
                // Buffer was too small, reallocate the correct size for the buffer.
                pArray = Marshal.ReAllocHGlobal(pArray, new IntPtr(structSize));

                ret = WinAPIDeclarations.GetAdaptersInfo(pArray, ref structSize);
            }

            string ip = string.Empty, mac = string.Empty;
            if (ret == 0)
            {
                // Call Succeeded
                IntPtr pEntry = pArray;

                do
                {
                    // Retrieve the adapter info from the memory address
                    IP_ADAPTER_INFO entry = (IP_ADAPTER_INFO) Marshal.PtrToStructure(pEntry, typeof(IP_ADAPTER_INFO));

                    // ***Do something with the data HERE!***
                    //Console.WriteLine("\n");
                    //Console.WriteLine("Index: {0}", entry.Index.ToString());

                    // Adapter Type
                    string tmpString = string.Empty;
                    switch (entry.Type)
                    {
                        case (uint) AdaptersConsts.MIB_IF_TYPE_ETHERNET:
                            tmpString = "Ethernet";
                            break;
                        case (uint) AdaptersConsts.MIB_IF_TYPE_TOKENRING:
                            tmpString = "Token Ring";
                            break;
                        case (uint) AdaptersConsts.MIB_IF_TYPE_FDDI:
                            tmpString = "FDDI";
                            break;
                        case (uint) AdaptersConsts.MIB_IF_TYPE_PPP:
                            tmpString = "PPP";
                            break;
                        case (uint) AdaptersConsts.MIB_IF_TYPE_LOOPBACK:
                            tmpString = "Loopback";
                            break;
                        case (uint) AdaptersConsts.MIB_IF_TYPE_SLIP:
                            tmpString = "Slip";
                            break;
                        default:
                            tmpString = "Other/Unknown";
                            break;
                    } // switch
                    //Console.WriteLine("Adapter Type: {0}", tmpString);

                    //Console.WriteLine("Name: {0}", entry.AdapterName);
                    //Console.WriteLine("Desc: {0}\n", entry.AdapterDescription);

                    //Console.WriteLine("DHCP Enabled: {0}", (entry.DhcpEnabled == 1) ? "Yes" : "No");

                    if (entry.DhcpEnabled == 1)
                    {
                        //Console.WriteLine("DHCP Server : {0}", entry.DhcpServer.IpAddress.Address);

                        // Lease Obtained (convert from "time_t" to C# DateTime)
                        DateTime pdatDate = new DateTime(1970, 1, 1).AddSeconds(entry.LeaseObtained).ToLocalTime();
                        //Console.WriteLine("Lease Obtained: {0}", pdatDate.ToString());

                        // Lease Expires (convert from "time_t" to C# DateTime)
                        pdatDate = new DateTime(1970, 1, 1).AddSeconds(entry.LeaseExpires).ToLocalTime();
                        //Console.WriteLine("Lease Expires : {0}\n", pdatDate.ToString());
                    } // if DhcpEnabled

                    //Console.WriteLine("IP Address     : {0}", entry.IpAddressList.IpAddress.Address);
                    //Console.WriteLine("Subnet Mask    : {0}", entry.IpAddressList.IpMask.Address);
                    //Console.WriteLine("Default Gateway: {0}", entry.GatewayList.IpAddress.Address);

                    ip = entry.IpAddressList.IpAddress.Address;
                    if (ip != "0.0.0.0")
                    {
                        //s.Append(ip + " ");
                        tmpString = string.Empty;
                        for (int i = 0; i < entry.AddressLength - 1; i++)
                        {
                            tmpString += string.Format("{0:X2}-", entry.Address[i]);
                        }
                        //s.Append(string.Format("{0}{1:X2}", tmpString, entry.Address[entry.AddressLength - 1]));
                        mac = string.Format("{0}{1:X2}", tmpString, entry.Address[entry.AddressLength - 1]);
                        break;
                    }

                    // MAC Address (data is in a byte[])
                    //tmpString = string.Empty;
                    //for (int i = 0; i < entry.AddressLength - 1; i++)
                    //{
                    //    tmpString += string.Format("{0:X2}-", entry.Address[i]);
                    //}
                    //Console.WriteLine("MAC Address    : {0}{1:X2}\n", tmpString, entry.Address[entry.AddressLength - 1]);

                    //Console.WriteLine("Has WINS: {0}", entry.HaveWins ? "Yes" : "No");
                    //if (entry.HaveWins)
                    //{
                    //    Console.WriteLine("Primary WINS Server  : {0}", entry.PrimaryWinsServer.IpAddress.Address);
                    //    Console.WriteLine("Secondary WINS Server: {0}", entry.SecondaryWinsServer.IpAddress.Address);
                    //} // HaveWins

                    // Get next adapter (if any)
                    pEntry = entry.Next;

                } while (pEntry != IntPtr.Zero || (ip == string.Empty && mac == string.Empty));

                Marshal.FreeHGlobal(pArray);

            }
            else
            {
                Marshal.FreeHGlobal(pArray);
                throw new InvalidOperationException("GetAdaptersInfo failed: " + ret);
            }

            IP = ip;
            MAC = mac;
        }

        // ---------------

        #region Chrome

        public static string GetChormeUrl()
        {
            try
            {
                Process[] procs = Process.GetProcessesByName("chrome");

                foreach (Process proc in procs)
                {
                    // the chrome process must have a window
                    if (proc.MainWindowHandle == IntPtr.Zero)
                    {
                        continue;
                    }
                    //AutomationElement elm = AutomationElement.RootElement.FindFirst(TreeScope.Children,
                    //         new PropertyCondition(AutomationElement.ClassNameProperty, "Chrome_WidgetWin_1"));
                    // find the automation element
                    AutomationElement elm = AutomationElement.FromHandle(proc.MainWindowHandle);

                    // manually walk through the tree, searching using TreeScope.Descendants is too slow (even if it's more reliable)
                    AutomationElement elmUrlBar = null;
                    try
                    {
                        // walking path found using inspect.exe (Windows SDK) for Chrome 43.0.2357.81 m (currently the latest stable)
                        // Inspect.exe path - C://Program files (X86)/Windows Kits/10/bin/x64
                        var elm1 = elm.FindFirst(TreeScope.Children,
                            new PropertyCondition(AutomationElement.NameProperty, "Google Chrome"));
                        if (elm1 == null)
                        {
                            continue;
                        } // not the right chrome.exe
                        var elm2 = TreeWalker.RawViewWalker.GetLastChild(elm1);
                        // I don't know a Condition for this for finding
                        var elm3 = elm2.FindFirst(TreeScope.Children,
                            new PropertyCondition(AutomationElement.NameProperty, ""));
                        var elm4 = TreeWalker.RawViewWalker.GetNextSibling(elm3);
                        // I don't know a Condition for this for finding
                        var elm5 = elm4.FindFirst(TreeScope.Children,
                            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar));
                        var elm6 = elm5.FindFirst(TreeScope.Children,
                            new PropertyCondition(AutomationElement.NameProperty, ""));
                        elmUrlBar = elm6.FindFirst(TreeScope.Children,
                            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
                    }
                    catch
                    {
                        // Chrome has probably changed something, and above walking needs to be modified. :(
                        // put an assertion here or something to make sure you don't miss it
                        continue;
                    }

                    // make sure it's valid
                    if (elmUrlBar == null)
                    {
                        // it's not..
                        continue;
                    }

                    // elmUrlBar is now the URL bar element. we have to make sure that it's out of keyboard focus if we want to get a valid URL
                    if ((bool) elmUrlBar.GetCurrentPropertyValue(AutomationElement.HasKeyboardFocusProperty))
                    {
                        continue;
                    }

                    // there might not be a valid pattern to use, so we have to make sure we have one
                    AutomationPattern[] patterns = elmUrlBar.GetSupportedPatterns();
                    if (patterns.Length == 1)
                    {
                        string ret = "";
                        try
                        {
                            ret = ((ValuePattern) elmUrlBar.GetCurrentPattern(patterns[0])).Current.Value;
                        }
                        catch
                        {
                        }
                        if (ret != "")
                        {
                            // must match a domain name (and possibly "https://" in front)
                            if (Regex.IsMatch(ret, @"^(https:\/\/)?[a-zA-Z0-9\-\.]+(\.[a-zA-Z]{2,4}).*$"))
                            {
                                // prepend http:// to the url, because Chrome hides it if it's not SSL
                                if (!ret.StartsWith("http"))
                                {
                                    ret = "http://" + ret;
                                }
                                return ret;
                            }
                        }
                        continue;
                    }
                }
            }
            catch (Exception) { }
            return null;
        }



        public static void Tabs(out int x)
        {
            x = 0;
            List<string> s = new List<string>();
            Process[] procsChrome = Process.GetProcessesByName("chrome");
            if (procsChrome.Length <= 0)
            {
                Console.WriteLine("Chrome is not running");
                return;
            }
            foreach (Process proc in procsChrome)
            {
                // the chrome process must have a window 
                if (proc.MainWindowHandle == IntPtr.Zero)
                {
                    continue;
                }

                // to find the tabs we first need to locate something reliable - the 'New Tab' button 
                AutomationElement root = AutomationElement.FromHandle(proc.MainWindowHandle);
                Condition condNewTab = new PropertyCondition(AutomationElement.NameProperty, "Новая вкладка");
                AutomationElement elmNewTab = root.FindFirst(TreeScope.Descendants, condNewTab);
                if (elmNewTab == null)
                {
                    continue;
                }
                // get the tabstrip by getting the parent of the 'new tab' button 
                TreeWalker treewalker = TreeWalker.ControlViewWalker;
                AutomationElement elmTabStrip = treewalker.GetParent(elmNewTab);
                // loop through all the tabs and get the names which is the page title 
                Condition condTabItem = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem);

                var tabitems = elmTabStrip.FindAll(TreeScope.Children, condTabItem);
                x = tabitems.Count;
                foreach (AutomationElement elmUrlBar in tabitems)
                {
                    AutomationPattern[] patterns = elmUrlBar.GetSupportedPatterns();
                    if (patterns.Length == 1)
                    {
                        string ret = "";
                        try
                        {
                            ret = ((ValuePattern)elmUrlBar.GetCurrentPattern(patterns[0])).Current.Value;
                        }
                        catch { }
                        if (ret != "")
                        {
                            // must match a domain name (and possibly "https://" in front)
                            if (Regex.IsMatch(ret, @"^(https:\/\/)?[a-zA-Z0-9\-\.]+(\.[a-zA-Z]{2,4}).*$"))
                            {
                                // prepend http:// to the url, because Chrome hides it if it's not SSL
                                if (!ret.StartsWith("http"))
                                {
                                    ret = "http://" + ret;
                                }
                                s.Add(ret);
                            }
                        }
                    }
                }
            }
        }

        public static List<string> XXX()
        {
            List<string> listbox = new List<string>();

            Process[] procsChrome = Process.GetProcessesByName("chrome");
            foreach (Process chrome in procsChrome)
            {
                // the chrome process must have a window
                if (chrome.MainWindowHandle == IntPtr.Zero)
                {
                    continue;
                }

                // find the automation element
                AutomationElement elm = AutomationElement.FromHandle(chrome.MainWindowHandle);
                AutomationElement elmUrlBar = elm.FindFirst(TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));

                // if it can be found, get the value from the URL bar
                if (elmUrlBar != null)
                {
                    AutomationPattern[] patterns = elmUrlBar.GetSupportedPatterns();
                    if (patterns.Length > 0)
                    {
                        ValuePattern val = (ValuePattern) elmUrlBar.GetCurrentPattern(patterns[0]);
                        Console.WriteLine("Chrome URL found: " + val.Current.Value);
                        listbox.Add(val.Current.Value);
                    }
                }
            }
            return listbox;
        }

        //public static string GetChromeUrl() //Process process
        //{
        //    Process process = Process.GetProcessById(8144);

        //    if (process == null)
        //        throw new ArgumentNullException(nameof(process));

        //    if (process.MainWindowHandle == IntPtr.Zero)
        //        return null;

        //    AutomationElement element = AutomationElement.FromHandle(process.MainWindowHandle);
        //    if (element == null)
        //        return null;

        //    AutomationElementCollection edits5 = element.FindAll(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
        //    AutomationElement edit = edits5[0];
        //    string vp = ((ValuePattern)edit.GetCurrentPattern(ValuePattern.Pattern)).Current.Value as string;
        //    Console.WriteLine(vp);
        //    return vp;
        //}


        //public static string Gett()
        //{
        //    StringBuilder sb = new StringBuilder();

        //    // there are always multiple chrome processes, so we have to loop through all of them to find the
        //    // process with a Window Handle and an automation element of name "Address and search bar"
        //    Process[] procsChrome = Process.GetProcessesByName("chrome");
        //    foreach (Process chrome in procsChrome)
        //    {
        //        // the chrome process must have a window
        //        if (chrome.MainWindowHandle == IntPtr.Zero)
        //        {
        //            continue;
        //        }

        //        // find the automation element
        //        AutomationElement elm = AutomationElement.FromHandle(chrome.MainWindowHandle);
        //        AutomationElement elmUrlBar = elm.FindFirst(TreeScope.Descendants,
        //          new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));

        //        // if it can be found, get the value from the URL bar
        //        if (elmUrlBar != null)
        //        {
        //            AutomationPattern[] patterns = elmUrlBar.GetSupportedPatterns();
        //            if (patterns.Length > 0)
        //            {
        //                ValuePattern val = (ValuePattern)elmUrlBar.GetCurrentPattern(patterns[0]);
        //                //Console.WriteLine("Chrome URL found: " + val.Current.Value);
        //                sb.Append(val.Current.Value).Append("\n");
        //            }
        //        }
        //    }
        //    return sb.ToString();
        //}

        //public static string Gettt()
        //{
        //    StringBuilder sb = new StringBuilder();

        //    // there are always multiple chrome processes, so we have to loop through all of them to find the
        //    // process with a Window Handle and an automation element of name "Address and search bar"
        //    Process[] procsChrome = Process.GetProcessesByName("chrome");
        //    foreach (Process chrome in procsChrome)
        //    {
        //        // the chrome process must have a window
        //        if (chrome.MainWindowHandle == IntPtr.Zero)
        //        {
        //            continue;
        //        }

        //        // find the automation element
        //        AutomationElement elm = AutomationElement.FromHandle(chrome.MainWindowHandle);

        //        // manually walk through the tree, searching using TreeScope.Descendants is too slow (even if it's more reliable)
        //        AutomationElement elmUrlBar = null;
        //        try
        //        {
        //            // walking path found using inspect.exe (Windows SDK) for Chrome 31.0.1650.63 m (currently the latest stable)
        //            var elm1 = elm.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Google Chrome"));
        //            if (elm1 == null) { continue; } // not the right chrome.exe
        //                                            // here, you can optionally check if Incognito is enabled:
        //                                            //bool bIncognito = TreeWalker.RawViewWalker.GetFirstChild(TreeWalker.RawViewWalker.GetFirstChild(elm1)) != null;
        //            var elm2 = TreeWalker.RawViewWalker.GetLastChild(elm1); // I don't know a Condition for this for finding :(
        //            var elm3 = elm2.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, ""));
        //            var elm4 = elm3.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar));
        //            elmUrlBar = elm4.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Custom));
        //        }
        //        catch
        //        {
        //            // Chrome has probably changed something, and above walking needs to be modified. :(
        //            // put an assertion here or something to make sure you don't miss it
        //            continue;
        //        }

        //        // make sure it's valid
        //        if (elmUrlBar == null)
        //        {
        //            // it's not..
        //            continue;
        //        }

        //        // elmUrlBar is now the URL bar element. we have to make sure that it's out of keyboard focus if we want to get a valid URL
        //        if ((bool)elmUrlBar.GetCurrentPropertyValue(AutomationElement.HasKeyboardFocusProperty))
        //        {
        //            continue;
        //        }

        //        // there might not be a valid pattern to use, so we have to make sure we have one
        //        AutomationPattern[] patterns = elmUrlBar.GetSupportedPatterns();
        //        if (patterns.Length == 1)
        //        {
        //            string ret = "";
        //            try
        //            {
        //                ret = ((ValuePattern)elmUrlBar.GetCurrentPattern(patterns[0])).Current.Value;
        //            }
        //            catch { }
        //            if (ret != "")
        //            {
        //                // must match a domain name (and possibly "https://" in front)
        //                if (Regex.IsMatch(ret, @"^(https:\/\/)?[a-zA-Z0-9\-\.]+(\.[a-zA-Z]{2,4}).*$"))
        //                {
        //                    // prepend http:// to the url, because Chrome hides it if it's not SSL
        //                    if (!ret.StartsWith("http"))
        //                    {
        //                        ret = "http://" + ret;
        //                    }
        //                    //Console.WriteLine("Open Chrome URL found: '" + ret + "'");
        //                    sb.Append(ret).Append("\n");
        //                }
        //            }
        //            continue;
        //        }
        //    }
        //    return sb.ToString();
        //}


        //public static string GetCCC() //IntPtr hndl
        //{
        //    IntPtr hndl = GetWindowUnderCursor();
        //    if (hndl == IntPtr.Zero)
        //        hndl = GetWindowUnderCursor();

        //    if (hndl.ToInt32() > 0)
        //    {
        //        string url = "";
        //        AutomationElement elm = AutomationElement.FromHandle(hndl);
        //        AutomationElement elmUrlBar = elm.FindFirst(TreeScope.Descendants,
        //          new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));
        //        if (elmUrlBar != null)
        //        {
        //            AutomationPattern[] patterns = elmUrlBar.GetSupportedPatterns();
        //            if (patterns.Length > 0)
        //            {
        //                ValuePattern val = (ValuePattern)elmUrlBar.GetCurrentPattern(patterns[0]);
        //                url = val.Current.Value;
        //                return url;
        //            }
        //        }

        //    }
        //    return "";
        //}

        #endregion
    }
}

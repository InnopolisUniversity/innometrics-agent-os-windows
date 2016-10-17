using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsMetrics;
using WinEventDelegate = WindowsMetrics.Declarations.Delegates.WinEventDelegate;

namespace TestWindowsFormsApplication
{
    public partial class Form1 : Form
    {
        private IntPtr foregroundWindowHook;
        private IntPtr namechangeHook;

        private GCHandle h;
        private GCHandle h2;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            bool success = WinAPI.Stop(foregroundWindowHook);
            if (success)
            {
                richTextBox1.BackColor = Color.DarkRed;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            richTextBox1.BackColor = Color.White;
            WinEventDelegate action = (eventHook, type, hwnd, idObject, child, thread, time) =>
            {
                string foregroundWinTitle = WinAPI.GetForegroundWindowText();
                string path = WinAPI.GetCurrentExeModulePath();
                string user = WinAPI.GetSystemUserName();
                string currTime = WinAPI.GetLocalDateTime().ToString();
                richTextBox1.AppendText($"CHANGE {foregroundWinTitle}\n{path}\n{user}\n{currTime}\n***\n");
            };
            WinEventDelegate action2 = (eventHook, type, hwnd, idObject, child, thread, time) =>
            {
                string foregroundWinTitle = WinAPI.GetForegroundWindowText();
                string path = WinAPI.GetCurrentExeModulePath();
                string user = WinAPI.GetSystemUserName();
                string currTime = WinAPI.GetLocalDateTime().ToString();
                richTextBox1.AppendText($"SHOW {foregroundWinTitle}\n{path}\n{user}\n{currTime}\n***\n");
            };


            foregroundWindowHook = WinAPI.StartTrackingForegroundWindowChange(action);
            uint fwid = WinAPI.GetForegroundWindowProcessID();
            namechangeHook = WinAPI.StartTrackingNamechange(action2, fwid);
            h = GCHandle.Alloc(action);
            h2 = GCHandle.Alloc(action2);
        }
    }
}

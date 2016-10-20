using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsMetrics;
using WindowsMetrics.Helpers;

namespace TestWindowsFormsApplication
{
    public partial class Form1 : Form
    {
        private string _reportFilePath;
        StringBuilder report = new StringBuilder();

        private IntPtr foregroundWindowHook;
        private IntPtr mouseClickHook;

        private GCHandle foregroundWindowHandle;
        private GCHandle mouseClickHandle;

        private event Action StateScan;
        private event Action DataSaving;

        private Guard guardStateScanner;
        private Task taskForGuardStateScanner; // where guard works in

        private Guard guardDataSaver;
        private Task taskForGuardDataSaver; // where guard works in

        private const int StateScanIntervalSec = 5;
        private const int DataSavingIntervalSec = 30;

        private void OnForegroundWindowChange()
        {
            string foregroundWinTitle = WinAPI.GetTextOfForegroundWindow();
            string path = WinAPI.GetForegroundWindowExeModulePath();
            string user = WinAPI.GetSystemUserName();
            string currTime = DateTime.Now.ToString();
            string registry = $"WIN CHANGE {foregroundWinTitle}\n{path}\n{user}\n{currTime}\n***\n";
            report.Append(registry);
            richTextBox1.AppendText(registry);
            guardStateScanner.Reset();
        }
        private void OnLeftMouseClick()
        {
            string foregroundWinTitle = WinAPI.GetTextOfForegroundWindow();
            string path = WinAPI.GetForegroundWindowExeModulePath();
            string user = WinAPI.GetSystemUserName();
            string currTime = DateTime.Now.ToString();
            string registry = $"LEFT CLICK {foregroundWinTitle}\n{path}\n{user}\n{currTime}\n***\n";
            report.Append(registry);
            richTextBox1.AppendText(registry);
            guardStateScanner.Reset();
        }
        private void OnGuardStateScan()
        {
            string foregroundWinTitle = WinAPI.GetTextOfForegroundWindow();
            string path = WinAPI.GetForegroundWindowExeModulePath();
            string user = WinAPI.GetSystemUserName();
            string currTime = DateTime.Now.ToString();
            string registry = $"STATE SCAN {foregroundWinTitle}\n{path}\n{user}\n{currTime}\n***\n";
            report.Append(registry);
            richTextBox1.AppendText(registry);
        }
        private void OnGuardStateScanSafe()
        {
            this.SafeInvoke(OnGuardStateScan);
        }
        private void OnDataSaving()
        {
            string rep = report.ToString();
            report.Clear();
            FileWriter.Write(rep, _reportFilePath);
        }



        public Form1()
        {
            InitializeComponent();
            CreateReportFile();

            StateScan += OnGuardStateScanSafe;
            DataSaving += OnDataSaving;

            taskForGuardStateScanner = new Task(() =>
            {
                guardStateScanner = new Guard(
                    actionToDoEveryTick: (source, e) => StateScan?.Invoke(),
                    secondsToCountdown: StateScanIntervalSec
                    );
            });
            taskForGuardDataSaver = new Task(() =>
            {
                guardDataSaver = new Guard(
                    actionToDoEveryTick: (source, e) => DataSaving?.Invoke(),
                    secondsToCountdown: DataSavingIntervalSec
                    );
            });

            taskForGuardStateScanner.Start();
            taskForGuardDataSaver.Start();
        }

        private void CreateReportFile()
        {
            string currDir = Directory.GetCurrentDirectory();
            string reportDir = currDir + @"\Reports";
            if (!Directory.Exists(reportDir))
                Directory.CreateDirectory(reportDir);

            StringBuilder dateId = new StringBuilder();
            dateId.Append(DateTime.Now.Year)
                .Append(".")
                .Append(DateTime.Now.Month)
                .Append(".")
                .Append(DateTime.Now.Day)
                .Append("-")
                .Append(DateTime.Now.Hour)
                .Append(".")
                .Append(DateTime.Now.Minute)
                .Append(".")
                .Append(DateTime.Now.Second);

            string file = reportDir + $@"\Rep-{dateId}.txt";
            if (!File.Exists(file))
            {
                FileStream fs = File.Create(file);
                fs.Close();
            }
            _reportFilePath = file;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            bool success = WinAPI.StopTrackingForegroundWindowChange(foregroundWindowHook);
            bool success2 = WinAPI.StopTrackingLeftClickEvent(mouseClickHook);
            guardStateScanner.Stop();
            guardDataSaver.Stop();
            if (success && success2)
            {
                richTextBox1.BackColor = Color.LightSteelBlue;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            richTextBox1.BackColor = Color.White;

            foregroundWindowHook = WinAPI.StartTrackingForegroundWindowChange(OnForegroundWindowChange, out foregroundWindowHandle);
            mouseClickHook = WinAPI.StartTrackingLeftClickEvent(OnLeftMouseClick, out mouseClickHandle);
            guardStateScanner.Start();
            guardDataSaver.Start();
        }
    }
}

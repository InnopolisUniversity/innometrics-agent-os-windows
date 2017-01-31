using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsMetrics.Helpers;

namespace WindowsMetrics
{
    public class Writer : IDisposable
    {
        private string _reportFilePath;
        private readonly StringBuilder _report = new StringBuilder();

        private event Action DataSaving;

        private Guard _guardDataSaver;
        private Task _taskForGuardDataSaver; // where guard works in

        private const int DataSavingIntervalSec = 3;

        public Writer(string toDirectory)
        {
            CreateReportFile(toDirectory);
            DataSaving += OnDataSaving;

            _taskForGuardDataSaver = new Task(() =>
                {
                    _guardDataSaver = new Guard(
                        actionToDoEveryTick: () => DataSaving?.Invoke(),
                        secondsToCountdown: DataSavingIntervalSec
                    );
                }
            );

            _taskForGuardDataSaver.Start();
        }

        private void OnDataSaving()
        {
            string rep = _report.ToString();
            _report.Clear();
            FileWriteHelper.Write(rep, _reportFilePath);
        }

        public void Start()
        {
            _guardDataSaver.Start();
        }

        public void Stop()
        {
            _guardDataSaver.Stop();
        }
        
        public void Append(string s)
        {
            _report.Append(s);
        }

        private void CreateReportFile(string directory)
        {
            string reportDir = directory + @"\Reports";
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

        public void Dispose()
        {
            throw new NotImplementedException(); //TODO implement
        }
    }
}

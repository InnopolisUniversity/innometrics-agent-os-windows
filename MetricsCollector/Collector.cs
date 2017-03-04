using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsMetrics;
using WMCollector = WindowsMetrics.Collector;

namespace MetricsCollector
{
    public partial class Collector : ServiceBase
    {
        private CancellationTokenSource tokenSource;
        private Task collectorTask;
        private Task writerTask;

        private WMCollector collector; // TODO disposing
        private Writer writer;

        public Collector()
        {
            InitializeComponent();

            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string connectionString = config.ConnectionStrings.ConnectionStrings["DefaultConnection"].ConnectionString;
            int dataSavingIntervalSec = Convert.ToInt32(config.AppSettings.Settings["DataSavingIntervalSec"].Value);
            int stateScanIntervalSec = Convert.ToInt32(config.AppSettings.Settings["StateScanIntervalSec"].Value);
            bool enableForegroundWindowChangeTracking = Convert.ToBoolean(config.AppSettings.Settings["EnableForegroundWindowChangeTracking"].Value);
            bool enableLeftClickTracking = Convert.ToBoolean(config.AppSettings.Settings["EnableLeftClickTracking"].Value);
            bool enableStateScanning = Convert.ToBoolean(config.AppSettings.Settings["EnableStateScanning"].Value);

            writer = new Writer(connectionString, dataSavingIntervalSec);
            collector = new WMCollector(writer, stateScanIntervalSec,
                enableForegroundWindowChangeTracking, enableLeftClickTracking, enableStateScanning);
        }

        protected override void OnStart(string[] args)
        {
            tokenSource = new CancellationTokenSource();
            var cancellation = tokenSource.Token;

            writerTask =
                Task.Factory.StartNew(() => writer.Start(), cancellation,
                    TaskCreationOptions.LongRunning, TaskScheduler.Default);

            collectorTask =
                Task.Factory.StartNew(() => collector.Start(), cancellation,
                    TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        protected override void OnStop()
        {
            collector.Stop();
            writer.Stop();
            tokenSource.Cancel();
            writerTask.Wait();
            collectorTask.Wait();
        }










        //private static void Foo(CancellationToken token)
        //{
        //    while (true)
        //    {
        //        if (token.IsCancellationRequested)
        //        {
        //            break;
        //        }
        //        string path = "D:\\qwerf.txt";
        //        using (FileStream fs = File.Exists(path) ? File.Open(path, FileMode.Append) : File.Create(path))
        //        {
        //            using (StreamWriter writer = new StreamWriter(fs))
        //            {
        //                writer.Write("Success");
        //            }
        //        }
        //        Thread.Sleep(2000);
        //    }
        //}

        //private CancellationTokenSource tokenSource;
        //private Task backgroundTask;

        //public Collector()
        //{
        //    InitializeComponent();

        //    this.CanStop = true;
        //    this.CanPauseAndContinue = true;
        //    this.AutoLog = true;
        //}

        //protected override void OnStart(string[] args)
        //{
        //    tokenSource = new CancellationTokenSource();
        //    var cancellation = tokenSource.Token;

        //    backgroundTask =
        //        Task.Factory.StartNew(() => Foo(cancellation), cancellation,
        //            TaskCreationOptions.LongRunning, TaskScheduler.Default);
        //}

        //protected override void OnStop()
        //{
        //    tokenSource.Cancel();
        //    backgroundTask.Wait();
        //}
    }
}

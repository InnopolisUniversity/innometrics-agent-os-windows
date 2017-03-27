using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonModels;
using CommonModels.Helpers;
using MetricsCollectorApplication;

namespace MetricsCollectorApplicationTestConsole
{
    class Tester
    {
        private static readonly string ConfigFileName = "MetricsCollectorApplicationTestConsole.exe.config";
        private static readonly string ConnectionName = "DefaultConnection";
        private readonly int DataSavingIntervalSec;
        private static readonly int WaitingInterval = 1000;
        private static readonly DateTime MinDate = new DateTime(1800, 1, 1);

        public Tester()
        {
            DataSavingIntervalSec = int.Parse(ConfigHelper.GetAppSetting(ConfigFileName, "DataSavingIntervalSec")) * 1000;
        }

        private static void Write(string s)
        {
            Console.WriteLine(s);
        }

        private static void WriteSuccesss()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("SUCCESS");
            Console.ResetColor();
        }

        private static void WriteFail()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("FAIL");
            Console.ResetColor();
        }

        private static void ToGreen()
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }

        private static void ToRed()
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }

        private static void ResetColor()
        {
            Console.ResetColor();
        }


        #region Database Interaction

        private static MetricsCollectorApplicationMainForm CreateCollector()
        {
            MetricsCollectorApplicationMainForm collector = new MetricsCollectorApplicationMainForm();

            var cfn = typeof(MetricsCollectorApplicationMainForm).GetField("ConfigFileName",
                BindingFlags.Instance | BindingFlags.NonPublic);
            cfn.SetValue(collector, ConfigFileName);

            return collector;
        }

        private static void DeleteDatabase()
        {
            using (
                MetricsDataContext context =
                    new MetricsDataContext(ConfigHelper.GetConnectionString(ConfigFileName, ConnectionName)))
            {
                if (context.DatabaseExists())
                {
                    context.DeleteDatabase();
                    context.SubmitChanges();
                }
            }
        }

        private static bool DatabaseExists()
        {
            using (
                MetricsDataContext context =
                    new MetricsDataContext(ConfigHelper.GetConnectionString(ConfigFileName, ConnectionName)))
            {
                return context.DatabaseExists();
            }
        }

        private static DateTime GetLastRecordTime()
        {
            using (
                MetricsDataContext context =
                    new MetricsDataContext(ConfigHelper.GetConnectionString(ConfigFileName, ConnectionName)))
            {
                if (context.DatabaseExists())
                {
                    var x = context.Registries.OrderByDescending(r => r.Time).AsEnumerable().FirstOrDefault();
                    return x?.Time ?? MinDate;
                }
                return MinDate;
            }
        }

        private static bool AnyRecordsAfter(DateTime d)
        {
            using (
                MetricsDataContext context =
                    new MetricsDataContext(ConfigHelper.GetConnectionString(ConfigFileName, ConnectionName)))
            {
                return context.Registries.Any(r => r.Time > d);
            }
        }

        private static bool AllRecordsAfterOf(DateTime d, CollectionEvent @event)
        {
            using (
                MetricsDataContext context =
                    new MetricsDataContext(ConfigHelper.GetConnectionString(ConfigFileName, ConnectionName)))
            {
                return !context.Registries.Any(r => r.Time > d && r.Event != (uint) @event);
            }
        }

        private static void Load(MetricsCollectorApplicationMainForm collector)
        {
            var load = typeof(MetricsCollectorApplicationMainForm).GetMethod(
                "MetricsCollectorApplicationMainForm_Load", BindingFlags.Instance | BindingFlags.NonPublic);
            load.Invoke(collector, new object[] {null, null});
        }

        private static void Start(MetricsCollectorApplicationMainForm collector)
        {
            var start = typeof(MetricsCollectorApplicationMainForm).GetMethod("buttonStart_Click",
                BindingFlags.Instance | BindingFlags.NonPublic);
            start.Invoke(collector, new object[] {null, null});
        }

        private static void Stop(MetricsCollectorApplicationMainForm collector)
        {
            var stop = typeof(MetricsCollectorApplicationMainForm).GetMethod("buttonStop_Click",
                BindingFlags.Instance | BindingFlags.NonPublic);
            stop.Invoke(collector, new object[] {null, null});
        }

        #endregion








        public static void TestCreationDatabase()
        {
            MetricsCollectorApplicationMainForm collector = CreateCollector();
            Load(collector);
            
            Write("******TestCreationDatabase******");
            if (DatabaseExists())
                WriteSuccesss();
            else
                WriteFail();
        }


        public void TestStartRecording()
        {
            Write("******TestStartRecording******");

            var time = GetLastRecordTime();

            var tokenSource = new CancellationTokenSource();
            var cancellation = tokenSource.Token;
            Task task = new Task((token) =>
            {
                CancellationToken t = (CancellationToken)token;
                MetricsCollectorApplicationMainForm collector = CreateCollector();
                Load(collector);
                Start(collector);
            }, cancellation);
            task.Start();

            // TODO test buttons

            for (int i = 0; i < (DataSavingIntervalSec + WaitingInterval) / 1000; i++)
            {
                Write(i.ToString());
                Thread.Sleep(1000);
            }

            tokenSource.Cancel();
            task.Wait();
            task.Dispose();

            if (AnyRecordsAfter(time))
            {
                WriteSuccesss();
            }
            else
            {
                WriteFail();
            }
        }
    }
}

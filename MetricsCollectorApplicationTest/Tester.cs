using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonModels;
using CommonModels.Helpers;
using MetricsCollectorApplication;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetricsCollectorApplicationTest
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class Tester
    {
        private static readonly string ConfigFileName = "MetricsCollectorApplicationTest.dll.config";
        private static readonly string ConnectionName = "DefaultConnection";
        private static readonly int DataSavingIntervalSec = int.Parse(ConfigHelper.GetAppSetting(ConfigFileName, "DataSavingIntervalSec")) * 1000;
        private static readonly int WaitingInterval = 1000;
        private static readonly DateTime MinDate = new DateTime(1800, 1, 1);

        private static MetricsCollectorApplicationMainForm CreateCollector()
        {
            MetricsCollectorApplicationMainForm collector = new MetricsCollectorApplicationMainForm();

            var cfn = typeof(MetricsCollectorApplicationMainForm).GetField("ConfigFileName", BindingFlags.Instance | BindingFlags.NonPublic);
            cfn.SetValue(collector, ConfigFileName);

            return collector;
        }

        private static bool DatabaseExists()
        {
            using (MetricsDataContext context = new MetricsDataContext(ConfigHelper.GetConnectionString(ConfigFileName, ConnectionName)))
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
                if (context.DatabaseExists())
                    return context.Registries.Any(r => r.Time > d);

                return false;
            }
        }

        private static bool AllRecordsAfterOf(DateTime d, CollectionEvent @event)
        {
            using (
                MetricsDataContext context =
                    new MetricsDataContext(ConfigHelper.GetConnectionString(ConfigFileName, ConnectionName)))
            {
                if (context.DatabaseExists())
                    return !context.Registries.Any(r => r.Time > d && r.Event != (uint) @event);

                return false;
            }
        }

        private static void Load(MetricsCollectorApplicationMainForm collector)
        {
            var load = typeof(MetricsCollectorApplicationMainForm).GetMethod(
                "MetricsCollectorApplicationMainForm_Load", BindingFlags.Instance | BindingFlags.NonPublic);
            load.Invoke(collector, new object[] { null, null });
        }

        private static void Start(MetricsCollectorApplicationMainForm collector)
        {
            var start = typeof(MetricsCollectorApplicationMainForm).GetMethod("buttonStart_Click",
                BindingFlags.Instance | BindingFlags.NonPublic);
            start.Invoke(collector, new object[] { null, null });
        }

        private static void Stop(MetricsCollectorApplicationMainForm collector)
        {
            var stop = typeof(MetricsCollectorApplicationMainForm).GetMethod("buttonStop_Click",
                BindingFlags.Instance | BindingFlags.NonPublic);
            stop.Invoke(collector, new object[] { null, null });
        }







        //// TODO should run separately with no database on server
        //[TestMethod]
        //public void TestCreationDatabase()
        //{
        //    MetricsCollectorApplicationMainForm collector = CreateCollector();
        //    Load(collector);
        //    Assert.IsTrue(DatabaseExists());
        //}

        [TestMethod]
        public void TestStartWithNoChosenEvents()
        {
            MetricsCollectorApplicationMainForm collector = CreateCollector();
            Load(collector);

            var chbxs =
                typeof(MetricsCollectorApplicationMainForm).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Where(f => f.FieldType == typeof(CheckBox)).ToArray();
            foreach (var chbx in chbxs)
            {
                var c = chbx.GetValue(collector) as CheckBox;
                c.CheckState = CheckState.Unchecked;
            }

            Start(collector);

            var fCollector = typeof(MetricsCollectorApplicationMainForm).GetField("collector",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var fColVal = fCollector.GetValue(collector);
            Assert.IsNull(fColVal, "fColVal != null");

            var fStarted = typeof(MetricsCollectorApplicationMainForm).GetField("started",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var fStartedVal = (bool)fStarted.GetValue(collector);
            Assert.IsFalse(fStartedVal);
        }

        [TestMethod]
        public void TestStartRecording()
        {
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

            Thread.Sleep(DataSavingIntervalSec + WaitingInterval);

            tokenSource.Cancel();
            task.Wait();

            Assert.IsTrue(AnyRecordsAfter(time));
        }

        //[TestMethod]
        //public void TestStopRecording()
        //{
        //    MetricsCollectorApplicationMainForm collector = null;

        //    var tokenSource = new CancellationTokenSource();
        //    var cancellation = tokenSource.Token;
        //    Task task = new Task((token) =>
        //    {
        //        CancellationToken t = (CancellationToken)token;
        //        collector = CreateCollector();
        //        Load(collector);
        //        Start(collector);
        //    }, cancellation);
        //    task.Start();
        //    Thread.Sleep(WaitingInterval);

        //    var ffStarted = typeof(MetricsCollectorApplicationMainForm).GetField("started",
        //        BindingFlags.Instance | BindingFlags.NonPublic);
        //    var ffStartedVal = (bool)ffStarted.GetValue(collector);
        //    Assert.IsTrue(ffStartedVal, "ffStartedVal");


        //    Thread.Sleep(DataSavingIntervalSec + WaitingInterval);

        //    Stop(collector);

        //    Thread.Sleep(WaitingInterval * 5);

        //    var fStarted = typeof(MetricsCollectorApplicationMainForm).GetField("started",
        //        BindingFlags.Instance | BindingFlags.NonPublic);
        //    var fStartedVal = (bool)fStarted.GetValue(collector);
        //    Assert.IsFalse(fStartedVal, "fStartedVal");
        //}

        //[TestMethod]
        //public void TestRecordingStateScan()
        //{
        //    var time = GetLastRecordTime();

        //    var tokenSource = new CancellationTokenSource();
        //    var cancellation = tokenSource.Token;
        //    Task task = new Task((token) =>
        //    {
        //        CancellationToken t = (CancellationToken)token;
        //        MetricsCollectorApplicationMainForm collector = CreateCollector();

        //        var chbxs =
        //        typeof(MetricsCollectorApplicationMainForm).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
        //            .Where(f => f.FieldType == typeof(CheckBox)).ToArray();
        //        foreach (var chbx in chbxs)
        //        {
        //            var c = chbx.GetValue(collector) as CheckBox;
        //            if(!c.Name.Contains("StateScan"))
        //                c.CheckState = CheckState.Unchecked;
        //        }

        //        Load(collector);
        //        Start(collector);
        //    }, cancellation);
        //    task.Start();

        //    Thread.Sleep(DataSavingIntervalSec + WaitingInterval);

        //    tokenSource.Cancel();
        //    task.Wait();

        //    Assert.IsTrue(AllRecordsAfterOf(time, CollectionEvent.STATE_SCAN));
        //}

        //[TestMethod]
        //public void TestRecordingLeftClick()
        //{
        //    var time = GetLastRecordTime();

        //    var tokenSource = new CancellationTokenSource();
        //    var cancellation = tokenSource.Token;
        //    Task task = new Task((token) =>
        //    {
        //        CancellationToken t = (CancellationToken)token;
        //        MetricsCollectorApplicationMainForm collector = CreateCollector();

        //        var chbxs =
        //        typeof(MetricsCollectorApplicationMainForm).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
        //            .Where(f => f.FieldType == typeof(CheckBox)).ToArray();
        //        foreach (var chbx in chbxs)
        //        {
        //            var c = chbx.GetValue(collector) as CheckBox;
        //            if (!c.Name.Contains("LeftClick"))
        //                c.CheckState = CheckState.Unchecked;
        //        }

        //        Load(collector);
        //        Start(collector);
        //    }, cancellation);
        //    task.Start();

        //    Thread.Sleep(DataSavingIntervalSec + WaitingInterval);

        //    tokenSource.Cancel();
        //    task.Wait();

        //    Assert.IsTrue(AllRecordsAfterOf(time, CollectionEvent.LEFT_CLICK));
        //}
    }
}

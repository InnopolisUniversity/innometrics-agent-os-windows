using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsMetrics;
using CommonModels;
using CommonModels.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Timer = System.Timers.Timer;

namespace WindowsMetricsTest
{
    [TestClass]
    public class WriterTests
    {
        private static object GetInstanceField(Type type, object instance, string fieldName)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            return field?.GetValue(instance);
        }

        [TestMethod]
        public void TestAdd()
        {
            string connectionString = "some string";
            int interval = 100;
            Writer writer = new Writer(connectionString, interval);

            Registry registry = new Registry();
            writer.Add(registry);

            var report = GetInstanceField(typeof(Writer), writer, "_report") as IList<Registry>;
            Assert.IsNotNull(report, "report != null");
            Assert.IsTrue(report.Contains(registry), "report.Contains(registry)");
            Assert.IsTrue(report.Count == 1, "report.Count == 1");
        }

        [TestMethod]
        public void TestStart()
        {
            string connectionString = "some string";
            int interval = 100;
            Writer writer = new Writer(connectionString, interval);
            writer.Start();
            Thread.Sleep(1000);

            string connString = GetInstanceField(typeof(Writer), writer, "_connectionString") as string;
            Assert.IsTrue(connString == connectionString, "connString == connectionString");

            int interv = (int)GetInstanceField(typeof(Writer), writer, "_dataSavingIntervalSec");
            Assert.IsTrue(interval == interv, "interval == interv");

            var report = GetInstanceField(typeof(Writer), writer, "_report") as IList<Registry>;
            Assert.IsNotNull(report, "report != null");
            Assert.IsTrue(report.Count == 0, "report.Count == 0");

            var dataSavingEvent = GetInstanceField(typeof(Writer), writer, "DataSaving") as Action;
            Assert.IsNotNull(dataSavingEvent, "dataSavingEvent != null");

            var guard = GetInstanceField(typeof(Writer), writer, "_guardDataSaver") as Guard;
            Assert.IsNotNull(guard, "guard != null");
            var timer = GetInstanceField(typeof(Guard), guard, "_timer") as Timer;
            Assert.IsNotNull(timer, "timer != null");
            Assert.IsTrue(timer.Enabled, "timer.Enabled");
            Assert.IsTrue(timer.Interval == interval * 1000, "timer.Interval == interval");

            var task = GetInstanceField(typeof(Writer), writer, "_taskForGuardDataSaver") as Task;
            Assert.IsNotNull(task, "task != null");

            var token = GetInstanceField(typeof(Writer), writer, "_tokenSource") as CancellationTokenSource;
            Assert.IsNotNull(token, "token != null");
            Assert.IsFalse(token.IsCancellationRequested, "token.IsCancellationRequested");
        }

        [TestMethod]
        public void TestStop()
        {
            string connectionString = "some string";
            int interval = 100;
            Writer writer = new Writer(connectionString, interval);
            writer.Start();
            Thread.Sleep(1000);
            writer.Stop();
            Thread.Sleep(1000);

            var guard = GetInstanceField(typeof(Writer), writer, "_guardDataSaver") as Guard;
            Assert.IsNotNull(guard, "guard != null");
            var timer = GetInstanceField(typeof(Guard), guard, "_timer") as Timer;
            Assert.IsNotNull(timer, "timer != null");
            Assert.IsFalse(timer.Enabled, "timer.Enabled");

            var task = GetInstanceField(typeof(Writer), writer, "_taskForGuardDataSaver") as Task;
            Assert.IsNotNull(task, "task != null");
            Assert.IsFalse(task.Status == TaskStatus.Running);

            var token = GetInstanceField(typeof(Writer), writer, "_tokenSource") as CancellationTokenSource;
            Assert.IsNotNull(token, "token != null");
            Assert.IsTrue(token.IsCancellationRequested, "token.IsCancellationRequested");

            // TODO cover registry write
        }

        //private readonly string cString = ConfigHelper.GetConnectionString("WindowsMetricsTest.dll.config", "DefaultConnection");
        //[TestMethod]
        //public void TestCreateDatabaseIfNotExists()
        //{
        //    using (var context = new MetricsDataContext(cString))
        //    {
        //        if (context.DatabaseExists())
        //        {
        //            context.DeleteDatabase();
        //            context.SubmitChanges();
        //        }
        //    }

        //    int interval = 100;
        //    Writer writer = new Writer(cString, interval);
        //    writer.CreateDatabaseIfNotExists();

        //    using (var context = new MetricsDataContext(cString))
        //    {
        //        Assert.IsTrue(context.DatabaseExists());
        //        context.DeleteDatabase();
        //        context.SubmitChanges();
        //    }
        //}
    }
}

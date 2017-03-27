using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsMetrics;
using CommonModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WindowsMetricsTest
{
    [TestClass]
    public class CollectorTests
    {
        [TestMethod]
        public void TestMakeRegistry()
        {
            var method = typeof(Collector).GetMethod("MakeRegistry", BindingFlags.NonPublic | BindingFlags.Static);
            var result = method.Invoke(null, new object[] {CollectionEvent.WIN_CHANGE}) as Registry;
            Assert.IsNotNull(result, "result != null");
            Assert.IsTrue(result.Event == (ushort)CollectionEvent.WIN_CHANGE, "result.Event == (ushort)CollectionEvent.WIN_CHANGE");
            Assert.IsNotNull(result.Processed, "result.Processed != null");
            Assert.IsFalse(result.Processed.Value);
        }

        private static object GetInstanceField(Type type, object instance, string fieldName)
        {
            FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            return field?.GetValue(instance);
        }

        [TestMethod]
        public void CreateCollector()
        {
            Action<object> action = o => { };
            SynchronizationContext synchronizationContext = new SynchronizationContext();
            Writer writer = new Writer("", 1);
            int stateScanIntervalSec = 1;
            Collector collector = new Collector(
                writer: writer,
                stateScanIntervalSec: stateScanIntervalSec,
                enableForegroundWindowChangeTracking: true,
                enableLeftClickTracking: true,
                enableStateScanning: true,
                sync: synchronizationContext, 
                onForegroundWindowChangeAddon: action,
                onLeftMouseClickAddon: action,
                onGuardStateScanAddon: action
            );

            var WRITER = GetInstanceField(typeof(Collector), collector, "_writer") as Writer;
            Assert.IsNotNull(WRITER, "WRITER != null");
            Assert.IsTrue(WRITER == writer);

            var EnableForegroundWindowChangeTracking = (bool)GetInstanceField(typeof(Collector), collector, "_enableForegroundWindowChangeTracking");
            Assert.IsTrue(EnableForegroundWindowChangeTracking);

            var EnableLeftClickTracking = (bool)GetInstanceField(typeof(Collector), collector, "_enableLeftClickTracking");
            Assert.IsTrue(EnableLeftClickTracking);

            var EnableStateScanning = (bool)GetInstanceField(typeof(Collector), collector, "_enableStateScanning");
            Assert.IsTrue(EnableStateScanning);

            var StateScanIntervalSec = (int)GetInstanceField(typeof(Collector), collector, "_stateScanIntervalSec");
            Assert.IsTrue(StateScanIntervalSec == stateScanIntervalSec);

            var SYNC = GetInstanceField(typeof(Collector), collector, "_sync") as Writer;
            Assert.IsNotNull(WRITER, "WRITER != null");
            Assert.IsTrue(WRITER == writer);
        }
    }
}

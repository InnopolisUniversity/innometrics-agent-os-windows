using System;
using System.Collections.Generic;
using System.Reflection;
using CommonModels;
using CommonModels.Helpers;
using MetricsProcessing;
using MetricsProcessing.Straight;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetricsProcessingTest
{
    [TestClass]
    public class StraightRegistriesProcessorTests
    {
        private static readonly string ConfigFileName = "MetricsProcessingTest.dll.config";
        private static readonly string ConnectionName = "DefaultConnection";

        private static readonly DateTime Time = new DateTime(2000, 01, 01, 03, 00, 00);
        private static readonly int registriesInActivity = 10;
        private static RegistriesList GetActivityRegistries()
        {
            List<Registry> list = new List<Registry>();
            for (int j = 0; j < registriesInActivity; j++)
            {
                list.Add(new Registry()
                {
                    Id = j,
                    Event = (ushort)CollectionEvent.WIN_CHANGE,
                    WindowTitle = "Title",
                    ExeModulePath = "path",
                    ProcessName = "procname",
                    Time = Time + new TimeSpan(0, 0, j),
                    Username1 = new Username() { Value = "username" },
                    IpAddress = new IpAddress() { Value = "0.0.0.0" },
                    MacAddress = new MacAddress() { Value = "AAAA.AAAA.AAAA.AAAA" },
                    WindowId = "id",
                    Url = "url",
                    Processed = false
                });
            }
            return new RegistriesList(list);
        }

        [TestMethod]
        public void TestCreateActivity()
        {
            var reg = GetActivityRegistries();
            StraightRegistriesProcessor processor = new StraightRegistriesProcessor(ConfigHelper.GetConnectionString(ConfigFileName, ConnectionName));
            var ca = typeof(StraightRegistriesProcessor).GetMethod("CreateActivity",
                BindingFlags.Static | BindingFlags.NonPublic);
            if (ca != null)
            {
                var activity = ca.Invoke(processor, new object[] {reg}) as Activity;
                Assert.IsNotNull(activity, "activity != null");
            }
        }

        [TestMethod]
        public void TestExtractActivity()
        {
            var reg = GetActivityRegistries();
            StraightRegistriesProcessor processor = new StraightRegistriesProcessor(ConfigHelper.GetConnectionString(ConfigFileName, ConnectionName));
            var ca = typeof(StraightRegistriesProcessor).GetMethod("ExtractActivity",
                BindingFlags.Static | BindingFlags.NonPublic);
            if (ca != null)
            {
                var activity = ca.Invoke(processor, new object[] { reg, 5 }) as RegistriesList;
                Assert.IsNotNull(activity,"activity != null");
            }

            Assert.IsTrue(reg.Count == 5);
        }

        [TestMethod]
        public void TestDetectActivity()
        {
            var reg = GetActivityRegistries();
            StraightRegistriesProcessor processor = new StraightRegistriesProcessor(ConfigHelper.GetConnectionString(ConfigFileName, ConnectionName));
            var ca = typeof(StraightRegistriesProcessor).GetMethod("DetectActivity",
                BindingFlags.Static | BindingFlags.NonPublic);
            if (ca != null)
            {
                var num = (int)ca.Invoke(processor, new object[] { reg });
                Assert.IsTrue(num == 10);
            }
        }
    }
}

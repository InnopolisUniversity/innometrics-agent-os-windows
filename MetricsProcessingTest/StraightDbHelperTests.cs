using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommonModels;
using CommonModels.Helpers;
using MetricsProcessing;
using MetricsProcessing.Straight;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetricsProcessingTest
{
    [TestClass]
    public class StraightDbHelperTests
    {
        private static readonly string ConfigFileName = "MetricsProcessingTest.dll.config";
        private static readonly string ConnectionName = "DefaultConnection";

        private static readonly int numActivities = 4;
        private static readonly int registriesInActivity = 10;
        private static readonly string WindowTitle = "WindowTitle";
        private static readonly DateTime Time = new DateTime(2000, 01, 01, 03, 00, 00);
        private static List<Registry> GetRegistriesList()
        {
            List<Registry> list = new List<Registry>();

            for (int i = 0; i < numActivities; i++)
            {
                for (int j = 0; j < registriesInActivity; j++)
                {
                    int num = j + i * registriesInActivity;
                    string wt = num < (numActivities * registriesInActivity) - registriesInActivity ? WindowTitle + num / 10 : null;
                    list.Add(new Registry()
                    {
                        Id = num,
                        Event = (ushort)CollectionEvent.WIN_CHANGE,
                        WindowTitle = wt,
                        ExeModulePath = "path",
                        ProcessName = "procname",
                        Time = Time + new TimeSpan(0, 0, num),
                        Username1 = new Username() { Value = "username" },
                        IpAddress = new IpAddress() { Value = "0.0.0.0" },
                        MacAddress = new MacAddress() { Value = "AAAA.AAAA.AAAA.AAAA" },
                        WindowId = "id",
                        Url = "url",
                        Processed = false
                    });
                }
            }

            return list;
        }


        [TestMethod]
        public void TestGetRegistries()
        {
            var registries = StraightDbHelper.GetRegistries(ConfigHelper.GetConnectionString(ConfigFileName, ConnectionName));
            Assert.IsNotNull(registries, "registries != null");
            Assert.IsFalse(registries.IsEmpty, "registries.IsEmpty");
            Assert.IsNotNull(registries.First.IpAddress, "registries.First.IpAddress != null");
            Assert.IsNotNull(registries.First.MacAddress, "registries.First.MacAddress != null");
            Assert.IsNotNull(registries.First.Username1, "registries.First.Username1 != null");
        }

        [TestMethod]
        public void TestMakeRegistriesList()
        {
            var mrl = typeof(StraightDbHelper).GetMethod("MakeRegistriesList", BindingFlags.Static | BindingFlags.NonPublic);
            var registries = mrl.Invoke(null, new object[] {GetRegistriesList()}) as RegistriesList;
            Assert.IsNotNull(registries, "registries != null");
            Assert.IsFalse(registries.IsEmpty, "registries.IsEmpty");
            Assert.IsNotNull(registries.FilteredRegistries, "registries.FilteredRegistries != null");
            Assert.IsFalse(registries.FilteredRegistries.Any(), "registries.FilteredRegistries.Any()");
        }
    }
}

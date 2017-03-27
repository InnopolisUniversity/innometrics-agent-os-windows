using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonModels;
using MetricsProcessing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MetricsProcessingTest
{
    [TestClass]
    public class RegistriesListTests
    {
        private static readonly int numActivities = 4;
        private static readonly int registriesInActivity = 10;
        private static readonly string WindowTitle = "WindowTitle";
        private static readonly DateTime Time = new DateTime(2000, 01, 01, 03, 00, 00);
        private static RegistriesList GetRegistriesList()
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
                        Event = (ushort) CollectionEvent.WIN_CHANGE,
                        WindowTitle = wt,
                        ExeModulePath = "path",
                        ProcessName = "procname",
                        Time = Time + new TimeSpan(0, 0, num),
                        Username1 = new Username() {Value = "username"},
                        IpAddress = new IpAddress() {Value = "0.0.0.0"},
                        MacAddress = new MacAddress() {Value = "AAAA.AAAA.AAAA.AAAA"},
                        WindowId = "id",
                        Url = "url",
                        Processed = false
                    });
                }
            }

            return new RegistriesList(list);
        }

        [TestMethod]
        public void TestGetAllIds()
        {
            RegistriesList registries = GetRegistriesList();
            var ids = registries.GetAllIds().OrderBy(id => id).ToArray();
            Assert.IsTrue(ids.Length == numActivities * registriesInActivity, "ids.Length == numActivities * registriesInActivity");
            for (int i = 0; i < numActivities * registriesInActivity; i++)
            {
                Assert.IsTrue(ids[i] == i, $"ids[{i}] == {i}");
            }
        }

        [TestMethod]
        public void TestFilterNullTitles()
        {
            RegistriesList registries = GetRegistriesList();
            registries.Filter(null, false);
            Assert.IsFalse(registries.Any(r => r.WindowTitle == null));
        }

        [TestMethod]
        public void TestIncludeNullTitles()
        {
            RegistriesList registries = GetRegistriesList();
            registries.Filter(null, true);
            Assert.IsTrue(registries.Count(r => r.WindowTitle == null) == registriesInActivity);
        }

        [TestMethod]
        public void TestFilterNames()
        {
            RegistriesList registries = GetRegistriesList();
            List<string> filter = new List<string>() {$"{WindowTitle}1"};
            registries.Filter(filter, true);
            Assert.IsFalse(registries.Any(r => r.WindowTitle == $"{WindowTitle}1"));
        }

        [TestMethod]
        public void TestFilterDates()
        {
            RegistriesList registries = GetRegistriesList();
            registries.Filter(null, true, Time, Time + new TimeSpan(0, 0, 38));
            Assert.IsTrue(registries.Count == 39, $"registries.Count == {registries.Count}");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommonModels.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonModelsTest
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ConfigHelperTests
    {
        private const string configFileName = "CommonModelsTest.dll.config";

        [TestMethod]
        public void TestGetConfiguration()
        {
            var method = typeof(ConfigHelper).GetMethod("GetConfiguration", BindingFlags.NonPublic | BindingFlags.Static);
            var result = method.Invoke(null, new object[] {configFileName}) as Configuration;
            Assert.IsNotNull(result, "result != null");

            Assert.IsTrue(result.AppSettings.Settings.AllKeys.Contains("setting"), "result.AppSettings.Settings.AllKeys.Contains('setting')");
            Assert.IsTrue(result.AppSettings.Settings["setting"].Value == "0");

            Assert.IsTrue(result.ConnectionStrings.ConnectionStrings["DefaultConnection"].ConnectionString == "some string");
        }

        [TestMethod]
        public void TestUpdateConfig()
        {
            ConfigHelper.UpdateConfig(configFileName, "setting", "1");

            var method = typeof(ConfigHelper).GetMethod("GetConfiguration", BindingFlags.NonPublic | BindingFlags.Static);
            var result = method.Invoke(null, new object[] { configFileName }) as Configuration;
            Assert.IsNotNull(result, "result != null");

            Assert.IsTrue(result.AppSettings.Settings.AllKeys.Contains("setting"), "result.AppSettings.Settings.AllKeys.Contains('setting')");
            Assert.IsTrue(result.AppSettings.Settings["setting"].Value == "1");

            ConfigHelper.UpdateConfig(configFileName, "setting", "0");
        }

        [TestMethod]
        public void TestGetAppSettings()
        {
            var settings = ConfigHelper.GetAppSettings(configFileName);

            Assert.IsNotNull(settings, "settings != null");
            Assert.IsTrue(settings.ContainsKey("setting"));
            Assert.IsTrue(settings["setting"] == "0");
        }

        [TestMethod]
        public void TestGetConnectionString()
        {
            string cs = ConfigHelper.GetConnectionString(configFileName, "DefaultConnection");

            Assert.IsNotNull(cs, "cs != null");
            Assert.IsTrue(cs == "some string");
        }
    }
}

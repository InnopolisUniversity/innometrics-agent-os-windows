using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommonModels;
using CommonModels.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transmission;

namespace TransmissionTest
{
    [TestClass]
    public class SenderTests
    {
        private static readonly string ConfigFileName = "TransmissionTest.dll.config";
        private static string Login => ConfigHelper.GetAppSetting(ConfigFileName, "Login");
        private static string Password => ConfigHelper.GetAppSetting(ConfigFileName, "Password");
        private static string AuthorizationUri => ConfigHelper.GetAppSetting(ConfigFileName, "AuthorizationUri");
        private static string SendDataUri => ConfigHelper.GetAppSetting(ConfigFileName, "SendDataUri");

        [TestMethod]
        public void TestAuthorize()
        {
            Sender sender = new Sender(AuthorizationUri, SendDataUri);
            Assert.IsFalse(sender.Authorized, "sender.Authorized shouldnot");

            HttpStatusCode code;
            var success = sender.Authorize(Login, Password, out code);

            Assert.IsTrue(success, $"!success {code}");
            Assert.IsTrue(sender.Authorized, "sender.Authorized");
        }

        [TestMethod]
        public void TestSend()
        {
            Sender sender = new Sender(AuthorizationUri, SendDataUri);
            HttpStatusCode code;
            sender.Authorize(Login, Password, out code);

            Activity activity = new Activity()
            {
                Name = "activity",
            };
            activity.Measurements.Add(new Measurement() { Name = "measurement", Value = "value", Type = "type" });
            Report report = new Report() {Activities = new List<Activity>() {activity}};

            sender.SendActivities(report, out code);

            Assert.IsTrue(code < HttpStatusCode.BadRequest, $"code < HttpStatusCode.BadRequest {code}");
        }
    }
}

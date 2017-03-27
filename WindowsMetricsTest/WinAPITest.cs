using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WindowsMetrics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WindowsMetricsTest
{
    [TestClass]
    public class WinAPITest
    {
        [TestMethod]
        public void TestGetTextOfWindow()
        {
            var method = typeof(WinAPI).GetMethod("GetTextOfWindow", BindingFlags.NonPublic | BindingFlags.Static);
            var result = method.Invoke(null, new object[] {IntPtr.Zero});
            Assert.IsNull(result, "result should be null");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonModelsTest
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class GuardTests
    {
        [TestMethod]
        public void TestStart()
        {
            int i = 0;
            Action action = () => i++;
            Guard guard = new Guard(action, 1);
            guard.Start();
            Thread.Sleep(2000);
            Assert.IsTrue(i > 0);
        }

        [TestMethod]
        public void TestStop()
        {
            int i = 0;
            Action action = () => i++;
            Guard guard = new Guard(action, 1);
            guard.Start();
            Thread.Sleep(2000);

            if (i == 0)
                Assert.Fail("Guard wasn't started");

            guard.Stop();
            int I = i;
            Thread.Sleep(2000);

            Assert.IsTrue(i == I);
        }

        [TestMethod]
        public void TestReset()
        {
            int i = 0;
            Action action = () => i++;
            Guard guard = new Guard(action, 1);
            guard.Start();
            Thread.Sleep(600);
            guard.Reset();
            Thread.Sleep(600);
            guard.Reset();
            Assert.IsTrue(i == 0);
        }
    }
}

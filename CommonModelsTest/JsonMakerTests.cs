using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommonModels;
using CommonModels.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonModelsTest
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class JsonMakerTests
    {
        [TestMethod]
        public void TestSerialize()
        {
            int[] x = new[] {1, 2, 3};
            string json = JsonMaker.Serialize(x);
            json = Regex.Escape(json);
            string a = @"\[1,2,3]";
            Assert.IsTrue(json == a);
        }

        [TestMethod]
        public void TestDeserialize()
        {
            var text = new { Token = "tkn" };
            string json = JsonMaker.Serialize(text);
            string token = JsonMaker.DeserializeToken(json);

            Assert.IsNotNull(token, "token != null");
            Assert.IsTrue(token == "tkn");
        }
    }
}

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NZORConsumer.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAutoComplete()
        {
            NZORServiceMessage pm = null;
            string[] strList = NZORConsumer.ConsumerClient.GetAutoCompleteList("http://testserver05:8091/", "olea", 20, out pm);

            Assert.IsTrue(strList.Length > 0);
        }
    }
}

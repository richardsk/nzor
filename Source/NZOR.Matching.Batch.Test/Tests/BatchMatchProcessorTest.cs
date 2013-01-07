using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Configuration;
using System.IO;

namespace NZOR.Matching.Batch.Test.Tests
{
    [TestFixture]
    class BatchMatchProcessorTest
    {

        [Test]
        public void CanProcessBrokeredNames()
        {
            string cnnStr = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            NZOR.Matching.Batch.BrokeredNameRequestProcessor bnr = new NZOR.Matching.Batch.BrokeredNameRequestProcessor(cnnStr);
            bnr.ProcessBrokeredNameRequests("richardsk@landcareresearch.co.nz", "123", File.ReadAllText(@"C:\Users\richardsk\Downloads\nzorbrokerednames.csv"));
        }
    }
}

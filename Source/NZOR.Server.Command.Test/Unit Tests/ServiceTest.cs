using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NZOR.Admin.Data.Entities.Matching;
using NZOR.Admin.Data.Repositories.Matching;
using NZOR.Admin.Data.Sql.Repositories.Matching;
using NZOR.Publish.Model.Matching;

namespace NZOR.Command.Test.Unit_Tests
{
    [TestFixture]
    public class ServiceTest
    {
        [Test()]
        public void TestRun()
        {
            NZOR.Server.NZORProcessor.Start();

            while (!NZOR.Server.NZORProcessor.Complete)
            {
                System.Threading.Thread.Sleep(2000);
            }

            NZOR.Server.NZORProcessor.Stop();
        }

        [Test()]
        public void TestBatchMatch()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            MatchRepository mr = new MatchRepository(cnnStr);
            
            Match m = new Match();
            m.InputData = "ID,ScientificName" + Environment.NewLine + "1,Amanita muscaria" + Environment.NewLine + "2,Agaricus bisporus" + Environment.NewLine + "3,Charcoal flycap" +Environment.NewLine + "4,Agaricus sp." + Environment.NewLine + "5,\"Convallaria L., 1753\"" + Environment.NewLine + "6,Alternaria Nees" + Environment.NewLine + "7,Calocybe Kühner ex Donk, 1962";
            m.MatchId = Guid.NewGuid();
            m.ReceivedDate = DateTime.Now;
            m.Status = Match.Statuses.Pending;
            m.SubmitterEmail = "richardsk@landcareresearch.co.nz";
            m.State = Admin.Data.Entities.Entity.EntityState.Added;

            //mr.Save(m);

            NZOR.Server.BatchMatchProcessor bmp = new Server.BatchMatchProcessor();
            bmp.Run();

            System.Threading.Thread.Sleep(10000);

            bmp.Stop();
        }

        [Test()]
        public void TestBatchMatchNZBRN()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            MatchRepository mr = new MatchRepository(cnnStr);

            Match m = new Match();
            m.InputData = System.IO.File.ReadAllText(@"C:\Development\NZOR\NZBRNnamesShort.csv");
            m.MatchId = Guid.NewGuid();
            m.ReceivedDate = DateTime.Now;
            m.Status = Match.Statuses.Pending;
            m.SubmitterEmail = "richardsk@landcareresearch.co.nz";
            m.State = Admin.Data.Entities.Entity.EntityState.Added;

            //mr.Save(m);

            NZOR.Server.BatchMatchProcessor bmp = new Server.BatchMatchProcessor();
            bmp.ProcessBatchMatch(m);

            //bmp.Run();

            //System.Threading.Thread.Sleep(10000);

            //bmp.Stop();
        }

        [Test()]
        public void TestBatchMatches()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            MatchRepository mr = new MatchRepository(cnnStr);

            Match m = new Match();
            m.InputData = System.IO.File.ReadAllText(@"C:\Users\richardsk\Downloads\nzorbatchmatchsample2.csv");
            m.MatchId = Guid.NewGuid();
            m.ReceivedDate = DateTime.Now;
            m.Status = Match.Statuses.Pending;
            m.SubmitterEmail = "richardsk@landcareresearch.co.nz";
            m.State = Admin.Data.Entities.Entity.EntityState.Added;

            mr.Save(m);

            NZOR.Server.BatchMatchProcessor bmp = new Server.BatchMatchProcessor();
            //bmp.ProcessBatchMatch(m);

            bmp.Run();

            System.Threading.Thread.Sleep(10000);
            
            bmp.Stop();
        }
        
        //[Test()]
        //public void TestBrokerNames()
        //{
        //    ServiceClient client = new ServiceClient("123", "http://localhost:7783/"); //"http://test.data.nzor.org.nz");

        //    client.SubmitBrokeredNames(System.IO.File.ReadAllText(@"C:\Users\richardsk\Downloads\nzorbrokerednames.csv"), "richardsk@landcareresearch.co.nz", "123");
        //}

        //[Test()]
        //public void TestPollBatchMatch()
        //{
        //    ServiceClient client = new ServiceClient("123", "http://localhost:7783");
        //    AutoBatchMatchResponse resp = client.PollBatchMatch("8aa393c2-4bbb-425b-9a09-90dce0115ddb");
        //    Assert.That(resp.Status == Match.Statuses.Completed);            
        //}
    }
}

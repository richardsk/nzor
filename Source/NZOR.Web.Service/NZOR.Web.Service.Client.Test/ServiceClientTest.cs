using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NZOR.Publish.Model.Matching;
using System.IO;
using System.Xml.Serialization;
using NZOR.Publish.Model.Names;
using NZOR.Publish.Model.Search;
using NZOR.Publish.Model.Administration;
using NZOR.Web.Service.Client.Responses;

namespace NZOR.Web.Service.Client.Test
{
    [TestFixture] 
    public class ServiceClientTest
    {
        private ServiceClient _serviceClient;

        public ServiceClientTest()
        {
            //      _serviceClient = new ServiceClient("mykey", "http://nzor_dev:8081");
                 _serviceClient = new ServiceClient("mykey", "http://localhost:7783/");
            //      _serviceClient = new ServiceClient("mykey", "http://test.data.nzor.org.nz");
            //_serviceClient = new ServiceClient("mykey", "http://data.nzor.org.nz");
        }

        [Test]
        public void CanCreateListOfTestNames()
        {
            string namesXml = String.Empty;
            var nameIds = new List<string>();

            nameIds.Add("99846ED3-8A8C-4337-A792-A75699348A77");
            nameIds.Add("D5FD1FC5-8F0F-4973-A913-60847E0ECED0");
            nameIds.Add("7083F33B-58E2-41C8-BC7F-9C7F1151678B");
            nameIds.Add("0A951018-B122-4A19-B5B9-F4CEB867B6DC");
            nameIds.Add("73CDB3C3-A36C-4DAF-914B-B8305CC5128E");
            nameIds.Add("B46DF333-CBE8-4A05-A246-3CFB00D23B29");
            nameIds.Add("D9C9B9F7-3F67-49D0-9B9B-B49949FB44E4");
            nameIds.Add("61F4EFA4-40A2-41DB-A81C-B5FB0790B260");
            nameIds.Add("7A968377-8881-42FD-863D-9A710A003134");
            nameIds.Add("3AC35A97-9BD0-48FD-A597-62D941607BAE");
            nameIds.Add("CEDCC809-3A9B-4173-8A6F-835CE7387CA6");
            nameIds.Add("17AFE357-CEF5-4C88-9273-17BF436A052D");
            nameIds.Add("69B54B96-D234-42EC-9B00-AF7E113B7322");
            nameIds.Add("7D14ED31-EF14-4D2A-A486-0F18D04483A1");
            nameIds.Add("1D4411CF-1B3D-4EC1-A245-C1EF15541B53");

            var names = new List<Name>();

            foreach (string nameId in nameIds)
            {
                names.Add(_serviceClient.GetName(nameId));
            }

            using (var writer = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(List<Name>));

                serializer.Serialize(writer, names);
                namesXml = writer.ToString();
            }
        }

        [Test]
        public void CanGetNameById()
        {
            var name = _serviceClient.GetName("4b4512e6-6a2c-452d-a699-0001ac6c5c49");
        }

        [Test]
        public void CanGetNameByProviderId()
        {
            var name = _serviceClient.GetNameByProviderId("1CB17D41-36B9-11D5-9548-00D0592D548C");
            Assert.That(name != null);
        }

        [Test]
        public void CanSearch()
        {
            var request = new SearchRequest();

            request.Query = "*smith*";

            var response = _serviceClient.Search(request);

            Assert.That(response, Is.Not.Null);
        }

        [Test]
        public void CanSubmitSmallSetOfNamesForMatching()
        {
            string matchList = @"id, scientificname
                                2, lichen
                                3, acter";

            matchList = "id,scientificname\r\n2,lichen\r\n55,apoilo";

            _serviceClient = new ServiceClient("myKey", "http://localhost:7783");
            List<NameMatchResult> result = _serviceClient.SubmitMatchList(matchList);

            Assert.That(result, Has.Count.GreaterThan(0));
        }

        [Test]
        public void GetErrorFromNameMatching()
        {
            string matchList = @"id, 
                                2, lichen
                                3, acter";

            try
            {
                _serviceClient = new ServiceClient("myKey", "http://localhost:7783");
                List<NameMatchResult> result = _serviceClient.SubmitMatchList(matchList);
                Assert.That(result, Has.Count.GreaterThan(0));
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        [Test]
        public void CanSubmitFeedback()
        {
            Publish.Model.Common.Feedback fb = new Publish.Model.Common.Feedback();
            fb.Sender = "Kevin Richards";
            fb.SenderEmail = "richardsk@landcareresearch.co.nz";
            fb.NameId = new Guid("FBD47AF5-80FA-441B-B1C6-000FFB81549D");
            fb.Message = "test feedback message";
                                        
            try
            {
                _serviceClient = new ServiceClient("myKey", "http://localhost:7783");
                _serviceClient.SubmitFeedback(fb);                
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        [Test]
        public void CanGetStatistics()
        {
            var statistics = new StatisticsResponse();

            statistics = _serviceClient.GetStatistics(null, null, null);

            Assert.That(statistics, Is.Not.Null);
            Assert.That(statistics.Statistics, Has.Count.GreaterThan(0));

            statistics = _serviceClient.GetStatistics(null, 1, 15);

            Assert.That(statistics, Is.Not.Null);
            Assert.That(statistics.Statistics, Has.Count.EqualTo(15));
            Assert.That(statistics.PageSize, Is.EqualTo(15));
        }

        [Test]
        public void CanGetAndSetSetting()
        {
            _serviceClient.SetSetting("test", "1");
            _serviceClient.SetSetting("test", "2");

            SettingResponse sr = _serviceClient.GetSetting("test");

            Assert.That(sr.Value == "2");
        }
    
    }
}

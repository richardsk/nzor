using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NZOR.Matching.Batch.Matchers;
using NZOR.Matching.Batch.Test;
using System.Configuration;

namespace NZOR.Matching.Batch.Test
{
    [TestFixture]
    public class NameMatcherTest
    {
        public NameMatcher _matcher;

        [TestFixtureSetUp]
        public void Setup()
        {
            _matcher = new NameMatcher(@"\\testserver05\wwwroot\NZOR.Web.Service\App_Data\Indexes\Names", ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString);
        }

        [Test]
        [ExpectedException(ExpectedMessage = "The input data does not contain the headers specified in the template")]
        public void ErrorWhenIncorrectColumnHeaders()
        {
            Setup();
            string inputData = @"id,x";

            _matcher.Match(inputData);
        }

        [Test]
        [ExpectedException(ExpectedMessage = "The number of values does not match the number of headers (2)")]
        public void ErrorWhenValuesDontMatchHeaders()
        {
            Setup();
            string inputData = @"id,scientificname
                                1, abc
                                2
                                3, hij";

            _matcher.Match(inputData);
        }

        [Test]
        public void CanParseSimpleInputData()
        {
            Setup();
            string inputData = @"id,scientificname
                                1, lichen
                                2, asterice
                                3, fern";

            List<NameMatchResult> results = _matcher.Match(inputData);

            Assert.That(results, Has.Count.EqualTo(3));
            Assert.That(results.First(o => o.SubmittedId == "2").SubmittedScientificName, Is.EqualTo("asterice"));
        }

        [Test]
        public void CanParseSimpleInputDataAndOnlyProcessFirstTwo()
        {
            string inputData = @"id,scientificname
                                1, lichen
                                2, asterice
                                3, fern";

            List<NameMatchResult> results = _matcher.Match(inputData, 2);

            Assert.That(results, Has.Count.EqualTo(2));
            Assert.That(results.First(o => o.SubmittedId == "2").SubmittedScientificName, Is.EqualTo("asterice"));
        }


        [Test]
        public void DontProcessEmptyStings()
        {
            Setup();
            string inputData = @"id,scientificname
                                1, lichen
                                2, 
                                3, fern";

            List<NameMatchResult> results = _matcher.Match(inputData);

            Assert.That(results, Has.Count.EqualTo(3));
        }

        [Test]
        public void CanMatchDataWithHyphen()
        {
            Setup();
            string inputData = @"id,scientificname
                                1,Acaena novae-zelandiae";

            List<NameMatchResult> results = _matcher.Match(inputData);

            Assert.That(results, Has.Count.GreaterThan(0));

        }
        
        [Test]
        public void CanParseDataWithAndWithoutHyphens()
        {
            Setup();
            string inputData = @"id,scientificname
                                1,Acrogenospora novae-zelandiae";

            List<NameMatchResult> results = _matcher.Match(inputData);

            Assert.That(results, Has.Count.GreaterThan(0));

            inputData = @"id,scientificname
                                1,Acrogenospora novaezelandiae";

            results = _matcher.Match(inputData);

            Assert.That(results, Has.Count.GreaterThan(0));
        }


        [Test]
        public void CanParseEscapedInputData()
        {
            string inputData = "id,scientificname\r\n2,lichen\r\n55,apoilo";

            List<NameMatchResult> results = _matcher.Match(inputData);

            Assert.That(results, Has.Count.EqualTo(2));
            Assert.That(results.First(o => o.SubmittedId == "2").SubmittedScientificName, Is.EqualTo("lichen"));
        }

        [Test]
        public void CanTestWithABigFile()
        {
            string inputData = System.IO.File.ReadAllText(@"C:\Development\NZOR\Source\NZOR.Matching.Batch.Test\Resources\Test Data\Matching Name List.csv");

            List<NameMatchResult> results = _matcher.Match(inputData);

        }
    }
}

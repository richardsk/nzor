using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Xml.Linq;
using NZOR.Harvest.SchemaValidation;

namespace NZOR.Harvest.Test.UnitTests.ProviderXml
{
    [TestFixture]
    public class SchemaValidationTest
    {
        [Test]
        public void CanTrapValidationErrors()
        {
            XDocument document = XDocument.Load(@"C:\Development\NZOR\Source\NZOR.Harvest.Test\Resources\Test Provider Xml Data\NZOR-Test-Col2010-Invalid.xml");

            SchemaValidator.ValidationResult validationResult = SchemaValidator.Validate(document, Utility.ValidationSchemaUrl);

            Assert.That(validationResult.IsValid, Is.False);

            // Publications.
            Assert.That(validationResult.ValidationEvents.Any(o => o.LineNumber == 14 && o.ExceptionType == "The Enumeration constraint failed."), Is.True);

            // TaxonNames.
            Assert.That(validationResult.ValidationEvents.Any(o => o.LineNumber == 30 && o.Message == "The required attribute 'id' is missing."), Is.True);
        }

        [Test]
        public void CanTrapValidationErrorsOnLargeDataSet()
        {
            XDocument document = XDocument.Load(@"C:\Development\NZOR\Source\NZOR.Harvest.Test\Resources\Test Provider Xml Data\NZOR-Test-NZFLORA-InitialDataSet.xml");

            SchemaValidator.ValidationResult validationResult = SchemaValidator.Validate(document, Utility.ValidationSchemaUrl);

            Assert.That(validationResult.IsValid, Is.True);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace NZOR.Harvest.SchemaValidation
{
    public static class SchemaValidator
    {
        public class ValidationResult
        {
            public class ValidationEvent
            {
                public String ExceptionType { get; set; }
                public String Message { get; set; }
                public Int32 LineNumber { get; set; }
                public Int32 LinePosition { get; set; }
                public XmlSeverityType Severity { get; set; }

                public ValidationEvent()
                {
                    ExceptionType = String.Empty;
                    Message = String.Empty;
                    LineNumber = 0;
                    LinePosition = 0;
                    Severity = XmlSeverityType.Warning;
                }
            }

            public List<ValidationEvent> ValidationEvents { get; private set; }

            public ValidationResult()
            {
                ValidationEvents = new List<ValidationEvent>();
            }

            public Boolean IsValid
            {
                get { return !ValidationEvents.Any(o => o.Severity == XmlSeverityType.Error); }
            }
        }

        /// <summary>
        /// Validates an Xml document supplied in the NZOR provider format.
        /// </summary>
        /// <param name="document">The document to validate</param>
        /// <param name="schemaUrl">The location of the schema to use for the validation</param>
        /// <returns></returns>
        public static ValidationResult Validate(XDocument document, String schemaUrl)
        {
            ValidationResult validationResult = new ValidationResult();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.NameTable = new NameTable();
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationType |= ValidationType.Schema;
            settings.ValidationEventHandler += (o, e) =>
            {
                ValidationResult.ValidationEvent validationEvent = new ValidationResult.ValidationEvent();

                if (e.Exception != null)
                {
                    if (e.Exception.InnerException != null)
                    {
                        validationEvent.ExceptionType = e.Exception.InnerException.Message;
                    }
                    validationEvent.LineNumber = e.Exception.LineNumber;
                    validationEvent.LinePosition = e.Exception.LinePosition;
                }
                validationEvent.Message = e.Message;
                validationEvent.Severity = e.Severity;

                validationResult.ValidationEvents.Add(validationEvent);
            };

            XmlSchema schema = new XmlSchema();

            // Load validation schema from the specified url.
            //URL or file path?
            System.Uri url;
            if (System.Uri.TryCreate(schemaUrl, UriKind.Absolute, out url))
            {
                string proxyServer = System.Configuration.ConfigurationManager.AppSettings["proxyServer"];
                string proxyPort = System.Configuration.ConfigurationManager.AppSettings["proxyPort"];

                if (proxyServer != null && proxyPort != null && proxyServer.Length > 0) System.Net.WebRequest.DefaultWebProxy = new System.Net.WebProxy(proxyServer + ":" + proxyPort);

                System.Net.WebRequest req = System.Net.HttpWebRequest.Create(url);

                using (System.IO.Stream str = req.GetResponse().GetResponseStream())
                {
                    using (XmlReader reader = XmlReader.Create(str))
                    {
                        schema = XmlSchema.Read(reader, null);

                        settings.Schemas.Add(schema);
                    }
                }
            }
            else
            {
                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(schemaUrl))
                {
                    using (XmlReader reader = XmlReader.Create(streamReader))
                    {
                        schema = XmlSchema.Read(reader, null);

                        settings.Schemas.Add(schema);
                    }
                }
            }

            XmlNamespaceManager manager = new XmlNamespaceManager(settings.NameTable);

            manager.AddNamespace(String.Empty, schema.TargetNamespace);

            XmlParserContext parser = new XmlParserContext(settings.NameTable, manager, String.Empty, XmlSpace.Default);

            using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(document.ToString()), settings, parser))
            {
                while (reader.Read()) { }
            }

            return validationResult;
        }
    }
}

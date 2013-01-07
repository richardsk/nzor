using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NZOR.Data.Entities.Common;
using NZOR.Data.Entities.Provider;
using NZOR.Data.LookUps.Common;

namespace NZOR.Harvest.Parsers
{
    public class PublicationParser
    {
        private ReferenceTypeLookUp _referenceTypeLookUp;
        private ReferencePropertyTypeLookUp _referencePropertyTypeLookUp;

        private Guid _dataSourceId;

        public PublicationParser(Guid dataSourceId, ReferenceTypeLookUp referenceTypeLookUp, ReferencePropertyTypeLookUp referencePropertyTypeLookUp)
        {
            _dataSourceId = dataSourceId;
            _referenceTypeLookUp = referenceTypeLookUp;
            _referencePropertyTypeLookUp = referencePropertyTypeLookUp;
        }

        public Reference Parse(XElement publicationElement)
        {
            Reference reference = new Reference();

            reference.DataSourceId = _dataSourceId;

            ProcessAttributes(reference, publicationElement);
            ProcessCitation(reference, publicationElement);
            ProcessIdentifiers(reference, publicationElement);
            ProcessAuthors(reference, publicationElement);
            ProcessDates(reference, publicationElement);
            ProcessTitles(reference, publicationElement);
            ProcessEditors(reference, publicationElement);
            ProcessVolume(reference, publicationElement);
            ProcessIssue(reference, publicationElement);
            ProcessEdition(reference, publicationElement);
            ProcessPages(reference, publicationElement);
            ProcessPublisher(reference, publicationElement);
            ProcessKeywords(reference, publicationElement);
            ProcessLinks(reference, publicationElement);

            return reference;
        }

        private void ProcessAttributes(Reference reference, XElement publicationElement)
        {
            // id
            reference.ProviderRecordId = Utility.GetAttributeValue<String>(publicationElement, "id", String.Empty).ToUpper();

            //     _logger.Log(Logger.Level.Information, string.Format("Processing reference: {0}", reference.ProviderRecordId));

            // Parent reference Id
            String value = Utility.GetAttributeValue<String>(publicationElement, "parentPublicationRef", null);

            if (value != null)
            {
                ReferenceProperty referenceProperty = new ReferenceProperty();

                referenceProperty.ReferencePropertyTypeId = _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.ParentReferenceID).ReferencePropertyTypeId;

                referenceProperty.Value = value;

                reference.ReferenceProperties.Add(referenceProperty);
            }

            if (String.IsNullOrEmpty(reference.ProviderRecordId))
            {
                //          _logger.Log(Logger.Level.Warning, "No provider reference id", publicationElement.ToString());
            }

            // type
            ReferenceType referenceType = _referenceTypeLookUp.GetReferenceType(Utility.GetAttributeValue<String>(publicationElement, "type", String.Empty));

            if (referenceType == null)
            {
                //          _logger.Log(Logger.Level.Error, string.Format("No matching Publication@type for '{0}'", referenceType));

                //          return;
            }
            else
            {
                reference.ReferenceTypeId = referenceType.ReferenceTypeId;
            }

            // createdDate
            if (Utility.GetAttributeValue<String>(publicationElement, "createdDate", null) != null)
            {
                reference.ProviderCreatedDate = XmlConvert.ToDateTime(Utility.GetAttributeValue<String>(publicationElement, "createdDate", null), XmlDateTimeSerializationMode.Utc);
            }

            // modifiedDate
            if (Utility.GetAttributeValue<String>(publicationElement, "modifiedDate", null) != null)
            {
                reference.ProviderModifiedDate = XmlConvert.ToDateTime(Utility.GetAttributeValue<String>(publicationElement, "modifiedDate", null), XmlDateTimeSerializationMode.Utc);
            }
        }

        private void ProcessCitation(Reference reference, XElement publicationElement)
        {
            String value = Utility.GetElementValue<String>(publicationElement, null, "Citation", null);

            if (value != null)
            {
                ReferenceProperty referenceProperty = new ReferenceProperty();

                referenceProperty.ReferencePropertyTypeId = _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Citation).ReferencePropertyTypeId;

                referenceProperty.Value = value;

                reference.ReferenceProperties.Add(referenceProperty);
            }
        }

        private void ProcessIdentifiers(Reference reference, XElement publicationElement)
        {
            List<XElement> elements = publicationElement.Descendants("Identifier").ToList<XElement>();

            foreach (XElement element in elements)
            {
                ReferencePropertyType referencePropertyType = _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Identifier);

                if (referencePropertyType == null)
                {
                }
                else
                {
                    ReferenceProperty referenceProperty = new ReferenceProperty();

                    referenceProperty.ReferencePropertyTypeId = referencePropertyType.ReferencePropertyTypeId;

                    referenceProperty.SubType = Utility.GetAttributeValue<String>(element, "type", null);
                    referenceProperty.Level = Utility.GetAttributeValue<Int32?>(element, "level", null);
                    referenceProperty.Value = Utility.GetElementValue<String>(element, null, String.Empty, null);

                    reference.ReferenceProperties.Add(referenceProperty);
                }
            }
        }

        private void ProcessAuthors(Reference reference, XElement publicationElement)
        {
            XElement authorsElement = publicationElement.Element("Authors");

            if (authorsElement != null)
            {
                XElement simpleElement = authorsElement.Element("Simple");

                if (simpleElement != null)
                {
                    ReferenceProperty referenceProperty = new ReferenceProperty();

                    referenceProperty.ReferencePropertyTypeId = _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Author).ReferencePropertyTypeId;

                    referenceProperty.SubType = @"Simple";
                    referenceProperty.Value = Utility.GetElementValue<String>(simpleElement, null, String.Empty, null);

                    reference.ReferenceProperties.Add(referenceProperty);
                }
                else
                {
                    Int32 sequence = 1;

                    foreach (XElement element in authorsElement.Elements("Author"))
                    {
                        ReferenceProperty referenceProperty = new ReferenceProperty();

                        referenceProperty.ReferencePropertyTypeId = _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Author).ReferencePropertyTypeId;

                        referenceProperty.Sequence = sequence;
                        referenceProperty.SubType = Utility.GetAttributeValue<String>(element, "type", null);
                        referenceProperty.Value = Utility.GetElementValue<String>(element, null, String.Empty, String.Empty);

                        reference.ReferenceProperties.Add(referenceProperty);

                        sequence += 1;
                    }
                }
            }
        }

        private void ProcessDates(Reference reference, XElement publicationElement)
        {
            List<XElement> elements = publicationElement.Descendants("Date").ToList<XElement>();

            foreach (XElement element in elements)
            {
                ReferencePropertyType referencePropertyType = _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Date);

                if (referencePropertyType == null)
                {
                }
                else
                {
                    ReferenceProperty referenceProperty = new ReferenceProperty();

                    referenceProperty.ReferencePropertyTypeId = referencePropertyType.ReferencePropertyTypeId;

                    referenceProperty.SubType = Utility.GetAttributeValue<String>(element, "type", null);
                    referenceProperty.Value = Utility.GetElementValue<String>(element, null, String.Empty, null);

                    reference.ReferenceProperties.Add(referenceProperty);
                }
            }
        }

        private void ProcessTitles(Reference reference, XElement publicationElement)
        {
            List<XElement> elements = publicationElement.Descendants("Title").ToList<XElement>();

            foreach (XElement element in elements)
            {
                ReferencePropertyType referencePropertyType = _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Title);

                if (referencePropertyType == null)
                {
                }
                else
                {
                    ReferenceProperty referenceProperty = new ReferenceProperty();

                    referenceProperty.ReferencePropertyTypeId = referencePropertyType.ReferencePropertyTypeId;

                    referenceProperty.SubType = Utility.GetAttributeValue<String>(element, "type", null);
                    referenceProperty.Level = Utility.GetAttributeValue<Int32?>(element, "level", null);
                    referenceProperty.Value = Utility.GetElementValue<String>(element, null, String.Empty, null);

                    reference.ReferenceProperties.Add(referenceProperty);
                }
            }
        }

        private void ProcessEditors(Reference reference, XElement publicationElement)
        {
            XElement editorsElement = publicationElement.Element("Editors");

            if (editorsElement != null)
            {
                XElement simpleElement = editorsElement.Element("Simple");

                if (simpleElement != null)
                {
                    ReferenceProperty referenceProperty = new ReferenceProperty();

                    referenceProperty.ReferencePropertyTypeId = _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Editor).ReferencePropertyTypeId;

                    referenceProperty.SubType = @"Simple";
                    referenceProperty.Value = Utility.GetElementValue<String>(simpleElement, null, String.Empty, null);

                    reference.ReferenceProperties.Add(referenceProperty);
                }
                else
                {
                    Int32 sequence = 1;

                    foreach (XElement element in editorsElement.Elements("Editor"))
                    {
                        ReferenceProperty referenceProperty = new ReferenceProperty();

                        referenceProperty.ReferencePropertyTypeId = _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Editor).ReferencePropertyTypeId;

                        referenceProperty.Sequence = sequence;
                        referenceProperty.Value = Utility.GetElementValue<String>(element, null, String.Empty, String.Empty);

                        reference.ReferenceProperties.Add(referenceProperty);

                        sequence += 1;
                    }
                }
            }
        }

        private void ProcessVolume(Reference reference, XElement publicationElement)
        {
            String value = Utility.GetElementValue<String>(publicationElement, null, "Volume", null);

            if (value != null)
            {
                ReferenceProperty referenceProperty = new ReferenceProperty();

                referenceProperty.ReferencePropertyTypeId = _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Volume).ReferencePropertyTypeId;

                referenceProperty.Value = value;

                reference.ReferenceProperties.Add(referenceProperty);
            }
        }

        private void ProcessIssue(Reference reference, XElement publicationElement)
        {
            String value = Utility.GetElementValue<String>(publicationElement, null, "Issue", null);

            if (value != null)
            {
                ReferenceProperty referenceProperty = new ReferenceProperty();

                referenceProperty.ReferencePropertyTypeId = _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Issue).ReferencePropertyTypeId;

                referenceProperty.Value = value;

                reference.ReferenceProperties.Add(referenceProperty);
            }
        }

        private void ProcessEdition(Reference reference, XElement publicationElement)
        {
            String value = Utility.GetElementValue<String>(publicationElement, null, "Edition", null);

            if (value != null)
            {
                ReferenceProperty referenceProperty = new ReferenceProperty();

                referenceProperty.ReferencePropertyTypeId = _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Edition).ReferencePropertyTypeId;

                referenceProperty.Value = value;

                reference.ReferenceProperties.Add(referenceProperty);
            }
        }

        private void ProcessPages(Reference reference, XElement publicationElement)
        {
            List<XElement> elements = publicationElement.Descendants("Page").ToList<XElement>();

            foreach (XElement element in elements)
            {
                ReferencePropertyType referencePropertyType = _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Page);

                if (referencePropertyType == null)
                {
                }
                else
                {
                    ReferenceProperty referenceProperty = new ReferenceProperty();

                    referenceProperty.ReferencePropertyTypeId = referencePropertyType.ReferencePropertyTypeId;

                    referenceProperty.SubType = Utility.GetAttributeValue<String>(element, "type", null);
                    referenceProperty.Value = Utility.GetElementValue<String>(element, null, String.Empty, null);

                    reference.ReferenceProperties.Add(referenceProperty);
                }
            }
        }

        private void ProcessPublisher(Reference reference, XElement publicationElement)
        {
            XElement publisherElement = publicationElement.Element("Publisher");

            if (publisherElement != null)
            {
                String value = Utility.GetElementValue<String>(publisherElement, null, "Name", null);

                if (value != null)
                {
                    ReferenceProperty referenceProperty = new ReferenceProperty();

                    referenceProperty.ReferencePropertyTypeId = _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Publisher).ReferencePropertyTypeId;

                    referenceProperty.Value = value;

                    reference.ReferenceProperties.Add(referenceProperty);
                }

                value = Utility.GetElementValue<String>(publisherElement, null, "City", null);

                if (value != null)
                {
                    ReferenceProperty referenceProperty = new ReferenceProperty();

                    referenceProperty.ReferencePropertyTypeId = _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.PlaceOfPublication).ReferencePropertyTypeId;

                    referenceProperty.Value = value;

                    reference.ReferenceProperties.Add(referenceProperty);
                }
            }
        }

        private void ProcessKeywords(Reference reference, XElement publicationElement)
        {
            XElement keywordsElement = publicationElement.Element("Keywords");

            if (keywordsElement != null)
            {
                XElement simpleElement = keywordsElement.Element("Simple");

                if (simpleElement != null)
                {
                    ReferenceProperty referenceProperty = new ReferenceProperty();

                    referenceProperty.ReferencePropertyTypeId = _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Keyword).ReferencePropertyTypeId;

                    referenceProperty.SubType = @"Simple";
                    referenceProperty.Value = Utility.GetElementValue<String>(simpleElement, null, String.Empty, null);

                    reference.ReferenceProperties.Add(referenceProperty);
                }
                else
                {
                    Int32 sequence = 1;

                    foreach (XElement element in keywordsElement.Elements("Keyword"))
                    {
                        ReferenceProperty referenceProperty = new ReferenceProperty();

                        referenceProperty.ReferencePropertyTypeId = _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Keyword).ReferencePropertyTypeId;

                        referenceProperty.Sequence = sequence;
                        referenceProperty.Value = Utility.GetElementValue<String>(element, null, String.Empty, String.Empty);

                        reference.ReferenceProperties.Add(referenceProperty);

                        sequence += 1;
                    }
                }
            }
        }

        private void ProcessLinks(Reference reference, XElement publicationElement)
        {
            List<XElement> elements = publicationElement.Descendants("Link").ToList<XElement>();

            foreach (XElement element in elements)
            {
                ReferencePropertyType referencePropertyType = _referencePropertyTypeLookUp.GetReferencePropertyType(ReferencePropertyTypeLookUp.Link);

                if (referencePropertyType == null)
                {
                }
                else
                {
                    ReferenceProperty referenceProperty = new ReferenceProperty();

                    referenceProperty.ReferencePropertyTypeId = referencePropertyType.ReferencePropertyTypeId;

                    referenceProperty.SubType = Utility.GetAttributeValue<String>(element, "type", null);
                    referenceProperty.Value = Utility.GetElementValue<String>(element, null, String.Empty, null);

                    reference.ReferenceProperties.Add(referenceProperty);
                }
            }
        }
    }
}

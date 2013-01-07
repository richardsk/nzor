using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NZOR.Data.Entities.Common;
using NZOR.Data.Entities.Provider;
using NZOR.Data.LookUps.Common;

namespace NZOR.Harvest.Parsers
{
    public class TaxonNameParser
    {
        private NamePropertyTypeLookUp _namePropertyTypeLookUp;
        private TaxonRankLookUp _taxonRankLookUp;
        private NameClassLookUp _nameClassLookUp;

        private Guid _dataSourceId;

        public TaxonNameParser(Guid dataSourceId, NameClassLookUp nameClassLookUp, NamePropertyTypeLookUp namePropertyTypeLookUp, TaxonRankLookUp taxonRankLookUp)
        {
            _dataSourceId = dataSourceId;

            _nameClassLookUp = nameClassLookUp;
            _namePropertyTypeLookUp = namePropertyTypeLookUp;
            _taxonRankLookUp = taxonRankLookUp;
        }

        public Name Parse(XElement taxonNameElement)
        {
            Name name = new Name();

            name.DataSourceId = _dataSourceId;

            if (_nameClassLookUp == null || _nameClassLookUp.GetNameClass(NameClassLookUp.ScientificName) == null)
            {
            }
            else
            {
                name.NameClassId = _nameClassLookUp.GetNameClass(NameClassLookUp.ScientificName).NameClassId;
            }

            ProcessAttributes(name, taxonNameElement);

            name.GoverningCode = Utility.GetElementValue<String>(taxonNameElement, null, "NomenclaturalCode", null);

            name.FullName = Utility.GetElementValue(taxonNameElement, null, "FullName", String.Empty);

            ProcessRankElement(name, taxonNameElement);
            
            string basId = null;
            string authors = null;

            AddNameProperty(
                name: name,
                value: Utility.GetElementValue<String>(taxonNameElement, null, "CanonicalName", null),
                namePropertyType: _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, NamePropertyTypeLookUp.Canonical)
                );

            authors = Utility.GetElementValue<String>(taxonNameElement, null, "Authorship", null);
            if (name.GoverningCode == "ICZN") authors = authors.Replace("(", "").Replace(")", "");
            AddNameProperty(
                name: name,
                value: authors,
                namePropertyType: _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, NamePropertyTypeLookUp.Authors)
                );

            AddNameProperty(
                name: name,
                value: Utility.GetElementValue<String>(taxonNameElement, null, "BasionymAuthors", null),
                namePropertyType: _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, NamePropertyTypeLookUp.BasionymAuthors)
                );

            AddNameProperty(
                name: name,
                value: Utility.GetElementValue<String>(taxonNameElement, null, "CombiningAuthors", null),
                namePropertyType: _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, NamePropertyTypeLookUp.CombinationAuthors)
                );

            AddNameProperty(
                name: name,
                value: Utility.GetElementValue<String>(taxonNameElement.Element("PublishedIn"), null, String.Empty, null),
                providerRelatedId: Utility.GetAttributeValue<String>(taxonNameElement.Element("PublishedIn"), "ref", null),
                namePropertyType: _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, NamePropertyTypeLookUp.PublishedIn)
                );

            AddNameProperty(
                name: name,
                value: Utility.GetElementValue<String>(taxonNameElement, null, "Year", null),
                namePropertyType: _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, NamePropertyTypeLookUp.Year)
                );

            AddNameProperty(
                name: name,
                value: Utility.GetElementValue<String>(taxonNameElement, null, "MicroReference", null),
                namePropertyType: _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, NamePropertyTypeLookUp.MicroReference)
                );

            AddNameProperty(
                name: name,
                value: Utility.GetElementValue<String>(taxonNameElement.Element("TypeName"), null, String.Empty, null),
                providerRelatedId: Utility.GetAttributeValue<String>(taxonNameElement.Element("TypeName"), "ref", null),
                namePropertyType: _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, NamePropertyTypeLookUp.TypeName)
                );

            AddNameProperty(
                name: name,
                value: Utility.GetElementValue<String>(taxonNameElement, null, "Orthography", null),
                namePropertyType: _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, NamePropertyTypeLookUp.Orthography)
                );

            basId = Utility.GetAttributeValue<String>(taxonNameElement.Element("Basionym"), "ref", null);
            AddNameProperty(
                name: name,
                value: Utility.GetElementValue<String>(taxonNameElement.Element("Basionym"), null, String.Empty, null),
                providerRelatedId: basId,
                namePropertyType: _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, NamePropertyTypeLookUp.Basionym)
                );

            AddNameProperty(
                name: name,
                value: Utility.GetElementValue<String>(taxonNameElement.Element("LaterHomonymOf"), null, String.Empty, null),
                providerRelatedId: Utility.GetAttributeValue<String>(taxonNameElement.Element("LaterHomonymOf"), "ref", null),
                namePropertyType: _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, NamePropertyTypeLookUp.LaterHomonymOf)
                );

            XElement replacementForElement = taxonNameElement.Element("ReplacementFor");

            if (replacementForElement != null)
            {
                AddNameProperty(
                    name: name,
                    value: Utility.GetElementValue<String>(replacementForElement.Element("BlockedName"), null, String.Empty, null),
                    providerRelatedId: Utility.GetAttributeValue<String>(replacementForElement.Element("BlockedName"), "ref", null),
                    namePropertyType: _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, NamePropertyTypeLookUp.BlockedName)
                    );

                AddNameProperty(
                    name: name,
                    value: Utility.GetElementValue<String>(replacementForElement.Element("RecombinedName"), null, String.Empty, null),
                    providerRelatedId: Utility.GetAttributeValue<String>(replacementForElement.Element("RecombinedName"), "ref", null),
                    namePropertyType: _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, NamePropertyTypeLookUp.RecombinedName)
                    );
            }

            AddNameProperty(
                name: name,
                value: Utility.GetElementValue<String>(taxonNameElement, null, "NomenclaturalStatus", null),
                namePropertyType: _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, NamePropertyTypeLookUp.NomenclaturalStatus)
                );


            string isRecomb = Utility.GetElementValue<string>(taxonNameElement, null, "IsRecombination", null);
            if (isRecomb != null)
            {
                bool isR = false;
                if (bool.TryParse(isRecomb, out isR)) name.IsRecombination = isR;
            }
            else
            {
                if (basId != null && basId != name.ProviderRecordId) name.IsRecombination = true;
                if (authors != null && authors.IndexOfAny(new char[]{'(',')'}) != -1) name.IsRecombination = true;                
            }

            return name;
        }

        private void ProcessAttributes(Name name, XElement taxonNameElement)
        {
            // id
            name.ProviderRecordId = Utility.GetAttributeValue<String>(taxonNameElement, "id", String.Empty).ToUpper();

            // qualityCode
            AddNameProperty(name, Utility.GetAttributeValue<String>(taxonNameElement, "qualityCode", null), _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, NamePropertyTypeLookUp.QualityCode));

            // createdDate
            if (Utility.GetAttributeValue<String>(taxonNameElement, "createdDate", null) != null)
            {
                name.ProviderCreatedDate = XmlConvert.ToDateTime(Utility.GetAttributeValue<String>(taxonNameElement, "createdDate", null), XmlDateTimeSerializationMode.Utc);
            }

            // modifiedDate
            if (Utility.GetAttributeValue<String>(taxonNameElement, "modifiedDate", null) != null)
            {
                name.ProviderModifiedDate = XmlConvert.ToDateTime(Utility.GetAttributeValue<String>(taxonNameElement, "modifiedDate", null), XmlDateTimeSerializationMode.Utc);
            }
        }

        private void ProcessRankElement(Name name, XElement taxonNameElement)
        {
            String value = Utility.GetElementValue<String>(taxonNameElement, null, "Rank", null);

            if (value != null)
            {
                AddNameProperty(
                    name: name,
                    value: value,
                    namePropertyType: _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, NamePropertyTypeLookUp.Rank)
                    );

                TaxonRank taxonRank = _taxonRankLookUp.GetTaxonRank(value, name.GoverningCode);

                if (taxonRank == null)
                {
                    name.TaxonRankId = _taxonRankLookUp.GetTaxonRank("none", name.GoverningCode).TaxonRankId;
                }
                else
                {
                    name.TaxonRankId = taxonRank.TaxonRankId;
                }
            }
        }

        private void AddNameProperty(Name name, String value, String providerRelatedId, NamePropertyType namePropertyType)
        {
            if (!String.IsNullOrEmpty(value))
            {
                NameProperty nameProperty = new NameProperty();

                nameProperty.NamePropertyTypeId = namePropertyType.NamePropertyTypeId;
                nameProperty.NamePropertyType = namePropertyType.Name;

                nameProperty.ProviderRelatedId = providerRelatedId;
                if (nameProperty.ProviderRelatedId != null) nameProperty.ProviderRelatedId = nameProperty.ProviderRelatedId.ToUpper();
                nameProperty.Value = value;

                name.NameProperties.Add(nameProperty);
            }
        }

        private void AddNameProperty(Name name, String value, NamePropertyType namePropertyType)
        {
            AddNameProperty(name, value, null, namePropertyType);
        }
    }
}

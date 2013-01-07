using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NZOR.Data.Entities.Common;
using NZOR.Data.Entities.Provider;
using NZOR.Data.LookUps.Common;

namespace NZOR.Harvest.Parsers
{
    public class VernacularNameParser
    {
        private NamePropertyTypeLookUp _namePropertyTypeLookUp;
        private TaxonRankLookUp _taxonRankLookUp;
        private NameClassLookUp _nameClassLookUp;

        private Guid _dataSourceId;

        public VernacularNameParser(Guid dataSourceId, NameClassLookUp nameClassLookUp, NamePropertyTypeLookUp namePropertyTypeLookUp, TaxonRankLookUp taxonRankLookUp)
        {
            _dataSourceId = dataSourceId;

            _nameClassLookUp = nameClassLookUp;
            _namePropertyTypeLookUp = namePropertyTypeLookUp;
            _taxonRankLookUp = taxonRankLookUp;
        }

        public Name Parse(XElement vernacularNameElement)
        {
            Name name = new Name();

            name.DataSourceId = _dataSourceId;

            name.TaxonRankId = _taxonRankLookUp.GetTaxonRank("none", null).TaxonRankId;

            ProcessAttributes(name, vernacularNameElement);
            ProcessFullName(name, vernacularNameElement);
            ProcessPublishedIn(name, vernacularNameElement);
            ProcessLanguage(name, vernacularNameElement);
            ProcessCountry(name, vernacularNameElement);

            return name;
        }

        private void ProcessAttributes(Name name, XElement vernacularNameElement)
        {
            // id
            name.ProviderRecordId = Utility.GetAttributeValue<String>(vernacularNameElement, "id", String.Empty).ToUpper();

            // isTradeName
            if (Utility.GetAttributeValue<String>(vernacularNameElement, "isTradeName", null) != null && XmlConvert.ToBoolean(Utility.GetAttributeValue<String>(vernacularNameElement, "isTradeName", null)))
            {
                name.NameClassId = _nameClassLookUp.GetNameClass("Trade Name").NameClassId;
            }
            else
            {
                name.NameClassId = _nameClassLookUp.GetNameClass("Vernacular Name").NameClassId;
            }

            // createdDate
            if (Utility.GetAttributeValue<String>(vernacularNameElement, "createdDate", null) != null)
            {
                name.ProviderCreatedDate = XmlConvert.ToDateTime(Utility.GetAttributeValue<String>(vernacularNameElement, "createdDate", null), XmlDateTimeSerializationMode.Utc);
            }

            // modifiedDate
            if (Utility.GetAttributeValue<String>(vernacularNameElement, "modifiedDate", null) != null)
            {
                name.ProviderModifiedDate = XmlConvert.ToDateTime(Utility.GetAttributeValue<String>(vernacularNameElement, "modifiedDate", null), XmlDateTimeSerializationMode.Utc);
            }
        }

        private void ProcessFullName(Name name, XElement vernacularNameElement)
        {
            name.FullName = Utility.GetElementValue(vernacularNameElement, null, "FullName", String.Empty);
        }

        private void ProcessPublishedIn(Name name, XElement vernacularNameElement)
        {
            AddNameProperty(
                name: name,
                value: Utility.GetElementValue<String>(vernacularNameElement.Element("PublishedIn"), null, String.Empty, null),
                providerRelatedId: Utility.GetAttributeValue<String>(vernacularNameElement.Element("PublishedIn"), "ref", null),
                namePropertyType: _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, NamePropertyTypeLookUp.PublishedIn)
                );
        }

        private void ProcessLanguage(Name name, XElement vernacularNameElement)
        {
            AddNameProperty(
                name: name,
                value: Utility.GetElementValue<String>(vernacularNameElement, null, "Language", null),
                namePropertyType: _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, NamePropertyTypeLookUp.Language)
                );
        }

        private void ProcessCountry(Name name, XElement vernacularNameElement)
        {
            AddNameProperty(
                name: name,
                value: Utility.GetElementValue<String>(vernacularNameElement, null, "Country", null),
                namePropertyType: _namePropertyTypeLookUp.GetNamePropertyType(name.NameClassId, NamePropertyTypeLookUp.Country)
                );
        }

        private void AddNameProperty(Name name, String value, String providerRelatedId, NamePropertyType namePropertyType)
        {
            if (value != null)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using NZOR.Data.Entities.Common;
using NZOR.Data.Entities.Provider;
using NZOR.Data.LookUps.Common;

namespace NZOR.Harvest.Parsers
{
    public class TaxonPropertyParser
    {
        TaxonPropertyClassLookup _propertyClassLookup;
        TaxonPropertyTypeLookup _propertyTypeLookup;

        private Guid _dataSourceId;

        public TaxonPropertyParser(Guid dataSourceId, TaxonPropertyClassLookup propertyClassLookup, TaxonPropertyTypeLookup propertyTypeLookUp)
        {
            _dataSourceId = dataSourceId;
            _propertyClassLookup = propertyClassLookup;
            _propertyTypeLookup = propertyTypeLookUp;
        }

        public TaxonProperty Parse(XElement taxonPropertyElement)
        {
            TaxonProperty tp = new TaxonProperty();

            tp.DataSourceId = _dataSourceId;
            
            if (String.Compare(taxonPropertyElement.Name.LocalName, TaxonPropertyClassLookup.PropertyClassBiostatus, true) == 0)
            {
                TaxonPropertyClass cl = _propertyClassLookup.GetTaxonPropertyClass(TaxonPropertyClassLookup.PropertyClassBiostatus);
                tp.TaxonPropertyClassId = cl.TaxonPropertyClassId;
                tp.TaxonPropertyClass = cl.Name;
            }
            else if (string.Compare(taxonPropertyElement.Name.LocalName, TaxonPropertyClassLookup.PropertyClassManagementStatus, true) == 0)
            {
                TaxonPropertyClass cl = _propertyClassLookup.GetTaxonPropertyClass(TaxonPropertyClassLookup.PropertyClassManagementStatus);
                tp.TaxonPropertyClassId = cl.TaxonPropertyClassId;
                tp.TaxonPropertyClass = cl.Name;
            }
            else
            {
                throw new Exception("Unknown Taxon Property type : " + taxonPropertyElement.Name.LocalName);
            }

            tp.ProviderRecordId = Utility.GetAttributeValue<String>(taxonPropertyElement, "id", String.Empty).ToUpper();
            
            if (Utility.GetAttributeValue<String>(taxonPropertyElement, "createdDate", null) != null)
            {
                tp.ProviderCreatedDate = XmlConvert.ToDateTime(Utility.GetAttributeValue<String>(taxonPropertyElement, "createdDate", null), XmlDateTimeSerializationMode.Utc);
            }
            
            if (Utility.GetAttributeValue<String>(taxonPropertyElement, "modifiedDate", null) != null)
            {
                tp.ProviderModifiedDate = XmlConvert.ToDateTime(Utility.GetAttributeValue<String>(taxonPropertyElement, "modifiedDate", null), XmlDateTimeSerializationMode.Utc);
            }

            XElement taxonElement = taxonPropertyElement.Element("Taxon");
            tp.ProviderNameId = Utility.GetAttributeValue<String>(taxonElement, "nameRef", null);
            if (tp.ProviderNameId != null) tp.ProviderNameId = tp.ProviderNameId.ToUpper();
            tp.ProviderConceptId = Utility.GetAttributeValue<String>(taxonElement, "conceptRef", null);
            if (tp.ProviderConceptId != null) tp.ProviderConceptId = tp.ProviderConceptId.ToUpper();

            XElement accToElement = taxonPropertyElement.Element("AccordingTo");
            if (accToElement != null)
            {
                tp.ProviderReferenceId = Utility.GetAttributeValue<String>(accToElement, "ref", null);
                if (tp.ProviderReferenceId != null) tp.ProviderReferenceId = tp.ProviderReferenceId.ToUpper();
            }
            
            XElement geoElement = taxonPropertyElement.Element("Region");
            if (geoElement != null)
            {
                AddTaxonPropertyValue(
                    tp : tp,
                    value: geoElement.Value,
                    taxonPropertyType: _propertyTypeLookup.GetTaxonPropertyType(tp.TaxonPropertyClassId, TaxonPropertyTypeLookup.PropertyTypeGeoRegion)
                    );
            
                AddTaxonPropertyValue(
                    tp : tp,
                    value: Utility.GetAttributeValue<String>(geoElement, "geographicSchema", null),
                    taxonPropertyType: _propertyTypeLookup.GetTaxonPropertyType(tp.TaxonPropertyClassId, TaxonPropertyTypeLookup.PropertyTypeGeoSchema)
                    );
            }
            
            AddTaxonPropertyValue(
                tp : tp,
                value: Utility.GetElementValue<String>(taxonPropertyElement, null, "Biome", null),
                taxonPropertyType: _propertyTypeLookup.GetTaxonPropertyType(tp.TaxonPropertyClassId, TaxonPropertyTypeLookup.PropertyTypeBiome)
                );
            
            AddTaxonPropertyValue(
                tp : tp,
                value: Utility.GetElementValue<String>(taxonPropertyElement, null, "EnvironmentalContext", null),
                taxonPropertyType: _propertyTypeLookup.GetTaxonPropertyType(tp.TaxonPropertyClassId, TaxonPropertyTypeLookup.PropertyTypeEnvironmentalContext)
                );
            
            AddTaxonPropertyValue(
                tp : tp,
                value: Utility.GetElementValue<String>(taxonPropertyElement, null, "Origin", null),
                taxonPropertyType: _propertyTypeLookup.GetTaxonPropertyType(tp.TaxonPropertyClassId, TaxonPropertyTypeLookup.PropertyTypeOrigin)
                );
            
            AddTaxonPropertyValue(
                tp : tp,
                value: Utility.GetElementValue<String>(taxonPropertyElement, null, "Occurrence", null),
                taxonPropertyType: _propertyTypeLookup.GetTaxonPropertyType(tp.TaxonPropertyClassId, TaxonPropertyTypeLookup.PropertyTypeOccurrence)
                );
            
            AddTaxonPropertyValue(
                tp : tp,
                value: Utility.GetElementValue<String>(taxonPropertyElement, null, "Action", null),
                taxonPropertyType: _propertyTypeLookup.GetTaxonPropertyType(tp.TaxonPropertyClassId, TaxonPropertyTypeLookup.PropertyTypeAction)
                );

            AddTaxonPropertyValue(
                tp : tp,
                value: Utility.GetElementValue<String>(taxonPropertyElement, null, "Outcome", null),
                taxonPropertyType: _propertyTypeLookup.GetTaxonPropertyType(tp.TaxonPropertyClassId, TaxonPropertyTypeLookup.PropertyTypeOutcome)
                );
            
            AddTaxonPropertyValue(
                tp : tp,
                value: Utility.GetElementValue<String>(taxonPropertyElement, null, "ActionStatus", null),
                taxonPropertyType: _propertyTypeLookup.GetTaxonPropertyType(tp.TaxonPropertyClassId, TaxonPropertyTypeLookup.PropertyTypeActionStatus)
                );
            
            AddTaxonPropertyValue(
                tp : tp,
                value: Utility.GetElementValue<String>(taxonPropertyElement, null, "Status", null),
                taxonPropertyType: _propertyTypeLookup.GetTaxonPropertyType(tp.TaxonPropertyClassId, TaxonPropertyTypeLookup.PropertyTypeStatus)
                );

            return tp;
        }

        private void AddTaxonPropertyValue(TaxonProperty tp, String value, TaxonPropertyType taxonPropertyType)
        {
            if (!String.IsNullOrEmpty(value))
            {
                TaxonPropertyValue tpv = new TaxonPropertyValue();

                tpv.TaxonPropertyTypeId = taxonPropertyType.TaxonPropertyTypeId;
                tpv.TaxonPropertyId = tp.TaxonPropertyId;
                tpv.TaxonPropertyType = taxonPropertyType.Name;

                tpv.Value = value;

                tp.TaxonPropertyValues.Add(tpv);
            }
        }
    }
}

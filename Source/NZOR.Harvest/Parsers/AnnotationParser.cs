using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using NZOR.Data.Entities.Provider;
using System.Xml;

namespace NZOR.Harvest.Parsers
{
    public class AnnotationParser
    {
        private Guid _dataSourceId = Guid.Empty;

        public AnnotationParser(Guid dataSourceId)
        {
            _dataSourceId = dataSourceId;
        }

        public Annotation Parse(XElement annotationElement)
        {
            Annotation ann = new Annotation();

            ann.DataSourceId = _dataSourceId;
            
            ann.AnnotationText = Utility.GetElementValue<String>(annotationElement, null, "AnnotationText", null);

            ProcessAttributes(ann, annotationElement);

            XElement taxonElement = annotationElement.Element("Taxon");
            ann.ProviderNameId = Utility.GetAttributeValue<String>(taxonElement, "nameRef", null);
            if (ann.ProviderNameId != null) ann.ProviderNameId = ann.ProviderNameId.ToUpper();
            ann.ProviderConceptId = Utility.GetAttributeValue<String>(taxonElement, "conceptRef", null);
            if (ann.ProviderConceptId != null) ann.ProviderConceptId = ann.ProviderConceptId.ToUpper();

            XElement accToElement = annotationElement.Element("AccordingTo");
            if (accToElement != null)
            {
                ann.ProviderReferenceId = Utility.GetAttributeValue<String>(accToElement, "ref", null);
                if (ann.ProviderReferenceId != null) ann.ProviderReferenceId = ann.ProviderReferenceId.ToUpper();
            }

            return ann;
        }

        private void ProcessAttributes(Annotation ann, XElement annotationElement)
        {
            // id
            ann.ProviderRecordId = Utility.GetAttributeValue<String>(annotationElement, "id", String.Empty).ToUpper();

            //type
            ann.AnnotationType = Utility.GetAttributeValue<String>(annotationElement, "type", null);

            // createdDate
            if (Utility.GetAttributeValue<String>(annotationElement, "createdDate", null) != null)
            {
                ann.ProviderCreatedDate = XmlConvert.ToDateTime(Utility.GetAttributeValue<String>(annotationElement, "createdDate", null), XmlDateTimeSerializationMode.Utc);
            }

            // modifiedDate
            if (Utility.GetAttributeValue<String>(annotationElement, "modifiedDate", null) != null)
            {
                ann.ProviderModifiedDate = XmlConvert.ToDateTime(Utility.GetAttributeValue<String>(annotationElement, "modifiedDate", null), XmlDateTimeSerializationMode.Utc);
            }
        }
    }
}

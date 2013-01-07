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
    public class VernacularConceptParser
    {
        private ConceptApplicationTypeLookUp _conceptApplicationTypeLookUp;

        private Guid _dataSourceId;

        public VernacularConceptParser(Guid dataSourceId, ConceptApplicationTypeLookUp conceptApplicationTypeLookUp)
        {
            _dataSourceId = dataSourceId;

            _conceptApplicationTypeLookUp = conceptApplicationTypeLookUp;
        }

        public Concept Parse(XElement vernacularConceptElement)
        {
            Concept concept = new Concept();

            concept.DataSourceId = _dataSourceId;
            concept.Type = Concept.ConceptType.VernacularConcept;

            ProcessAttributes(concept, vernacularConceptElement);
            ProcessName(concept, vernacularConceptElement);
            ProcessAccordingTo(concept, vernacularConceptElement);
            ProcessRank(concept, vernacularConceptElement);
            ProcessApplications(concept, vernacularConceptElement);

            return concept;
        }

        private void ProcessAttributes(Concept concept, XElement vernacularConceptElement)
        {
            // id
            concept.ProviderRecordId = Utility.GetAttributeValue<String>(vernacularConceptElement, "id", String.Empty).ToUpper();

            // createdDate
            if (Utility.GetAttributeValue<String>(vernacularConceptElement, "createdDate", null) != null)
            {
                concept.ProviderCreatedDate = XmlConvert.ToDateTime(Utility.GetAttributeValue<String>(vernacularConceptElement, "createdDate", null), XmlDateTimeSerializationMode.Utc);
            }

            // modifiedDate
            if (Utility.GetAttributeValue<String>(vernacularConceptElement, "modifiedDate", null) != null)
            {
                concept.ProviderModifiedDate = XmlConvert.ToDateTime(Utility.GetAttributeValue<String>(vernacularConceptElement, "modifiedDate", null), XmlDateTimeSerializationMode.Utc);
            }
        }

        private void ProcessName(Concept concept, XElement vernacularConceptElement)
        {
            XElement nameElement = vernacularConceptElement.Descendants("VernacularName").SingleOrDefault<XElement>();

            if (nameElement == null)
            {
            }
            else
            {
                concept.ProviderNameId = Utility.GetAttributeValue<String>(nameElement, "ref", String.Empty).ToUpper();
            }
        }

        private void ProcessAccordingTo(Concept concept, XElement vernacularConceptElement)
        {
            XElement accordingToElement = vernacularConceptElement.Descendants("AccordingTo").SingleOrDefault<XElement>();

            if (accordingToElement == null)
            {
            }
            else
            {
                concept.ProviderReferenceId = Utility.GetAttributeValue<String>(accordingToElement, "ref", String.Empty).ToUpper();
            }
        }

        private void ProcessRank(Concept concept, XElement vernacularConceptElement)
        {
            concept.TaxonRank = Utility.GetElementValue<String>(vernacularConceptElement, null, "Rank", null);
        }

        private void ProcessApplications(Concept concept, XElement vernacularConceptElement)
        {
            XElement applicationsElement = vernacularConceptElement.Descendants("Applications").SingleOrDefault<XElement>();

            if (applicationsElement != null)
            {
                List<Concept> newConcepts = new List<Concept>();

                List<XElement> applicationElements = applicationsElement.Descendants("Application").ToList<XElement>();

                foreach (XElement applicationElement in applicationElements)
                {
                    String toId = Utility.GetAttributeValue<String>(applicationElement, "ref", String.Empty).ToUpper();
                    String type = Utility.GetAttributeValue<String>(applicationElement, "type", String.Empty);
                    ConceptApplicationType conceptApplicationType = _conceptApplicationTypeLookUp.GetConceptApplicationType(type);

                    if (conceptApplicationType == null)
                    {
                        conceptApplicationType = _conceptApplicationTypeLookUp.GetConceptApplicationType("is vernacular for");
                    }
                    
                    if (conceptApplicationType != null)
                    {
                        String gender = Utility.GetElementValue<String>(applicationElement, null, "Gender", null);
                        String partOfTaxon = Utility.GetElementValue<String>(applicationElement, null, "PartOfTaxon", null);
                        String lifeStage = Utility.GetElementValue<String>(applicationElement, null, "LifeStage", null);
                        String region = Utility.GetElementValue<String>(applicationElement, null, "GeoRegion", null);
                        String geoschema = Utility.GetAttributeValue<String>(applicationElement.Descendants("GeoRegion").SingleOrDefault<XElement>(), "geographicSchema", null);
                        String inUse = Utility.GetElementValue<String>(applicationElement, null, "InUse", null);

                        ConceptApplication conceptApplication = new ConceptApplication();

                        conceptApplication.ConceptApplicationTypeId = conceptApplicationType.ConceptApplicationTypeId;
                        conceptApplication.FromConceptId = concept.ConceptId;
                        conceptApplication.ProviderToRecordId = toId;
                        conceptApplication.Gender = gender;
                        conceptApplication.PartOfTaxon = partOfTaxon;
                        conceptApplication.LifeStage = lifeStage;
                        conceptApplication.GeoRegion = region;
                        conceptApplication.GeographicSchema = geoschema;
                        
                        concept.ConceptApplications.Add(conceptApplication);
                    }
                }
            }
        }
    }
}

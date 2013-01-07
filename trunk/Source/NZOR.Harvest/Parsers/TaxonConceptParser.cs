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
    public class TaxonConceptParser
    {
        private ConceptRelationshipTypeLookUp _conceptRelationshipTypeLookUp;

        private Guid _dataSourceId;

        public TaxonConceptParser(Guid dataSourceId, ConceptRelationshipTypeLookUp conceptRelationshipTypeLookUp)
        {
            _dataSourceId = dataSourceId;

            _conceptRelationshipTypeLookUp = conceptRelationshipTypeLookUp;
        }

        public Concept Parse(XElement taxonConceptElement)
        {
            Concept concept = new Concept();

            concept.DataSourceId = _dataSourceId;
            concept.Type = Concept.ConceptType.TaxonConcept;

            ProcessAttributes(concept, taxonConceptElement);
            ProcessName(concept, taxonConceptElement);
            ProcessAccordingTo(concept, taxonConceptElement);
            ProcessRank(concept, taxonConceptElement);
            ProcessRelationships(concept, taxonConceptElement);

            return concept;
        }

        private void ProcessAttributes(Concept concept, XElement taxonConceptElement)
        {
            // id
            concept.ProviderRecordId = Utility.GetAttributeValue<String>(taxonConceptElement, "id", String.Empty).ToUpper();

            // createdDate
            if (Utility.GetAttributeValue<String>(taxonConceptElement, "createdDate", null) != null)
            {
                concept.ProviderCreatedDate = XmlConvert.ToDateTime(Utility.GetAttributeValue<String>(taxonConceptElement, "createdDate", null), XmlDateTimeSerializationMode.Utc);
            }

            // modifiedDate
            if (Utility.GetAttributeValue<String>(taxonConceptElement, "modifiedDate", null) != null)
            {
                concept.ProviderModifiedDate = XmlConvert.ToDateTime(Utility.GetAttributeValue<String>(taxonConceptElement, "modifiedDate", null), XmlDateTimeSerializationMode.Utc);
            }
        }

        private void ProcessName(Concept concept, XElement taxonConceptElement)
        {
            XElement nameElement = taxonConceptElement.Descendants("TaxonName").SingleOrDefault<XElement>();

            if (nameElement == null)
            {
            }
            else
            {
                concept.ProviderNameId = Utility.GetAttributeValue<String>(nameElement, "ref", String.Empty).ToUpper();
            }
        }

        private void ProcessAccordingTo(Concept concept, XElement taxonConceptElement)
        {
            XElement accordingToElement = taxonConceptElement.Descendants("AccordingTo").SingleOrDefault<XElement>();

            if (accordingToElement == null)
            {
            }
            else
            {
                concept.ProviderReferenceId = Utility.GetAttributeValue<String>(accordingToElement, "ref", String.Empty).ToUpper();
            }
        }

        private void ProcessRank(Concept concept, XElement taxonConceptElement)
        {
            concept.TaxonRank = Utility.GetElementValue<String>(taxonConceptElement, null, "Rank", null);
        }

        private void ProcessRelationships(Concept concept, XElement taxonConceptElement)
        {
            XElement relationshipsElement = taxonConceptElement.Descendants("Relationships").SingleOrDefault<XElement>();

            if (relationshipsElement != null)
            {
                List<Concept> newConcepts = new List<Concept>();

                XElement acceptedConceptElement = relationshipsElement.Element("AcceptedConcept");

                if (acceptedConceptElement != null)
                {
                    ConceptRelationship conceptRelationship = new ConceptRelationship();

                    conceptRelationship.ProviderToRecordId = Utility.GetAttributeValue(acceptedConceptElement, "ref", String.Empty).ToUpper();
                    
                    conceptRelationship.ConceptRelationshipTypeId = _conceptRelationshipTypeLookUp.GetConceptRelationshipType(ConceptRelationshipTypeLookUp.IsSynonymOf).ConceptRelationshipTypeId;
                    if (Utility.GetAttributeValue<String>(acceptedConceptElement, "inUse", null) != null)
                    {
                        conceptRelationship.InUse = XmlConvert.ToBoolean(Utility.GetAttributeValue<String>(acceptedConceptElement, "inUse", null));
                    }

                    concept.ConceptRelationships.Add(conceptRelationship);
                }

                XElement parentConceptElement = relationshipsElement.Element("ParentConcept");

                if (parentConceptElement != null)
                {
                    ConceptRelationship conceptRelationship = new ConceptRelationship();

                    conceptRelationship.ProviderToRecordId = Utility.GetAttributeValue(parentConceptElement, "ref", String.Empty).ToUpper();
                    
                    conceptRelationship.ConceptRelationshipTypeId = _conceptRelationshipTypeLookUp.GetConceptRelationshipType("is child of").ConceptRelationshipTypeId;
                    if (Utility.GetAttributeValue<String>(parentConceptElement, "inUse", null) != null)
                    {
                        conceptRelationship.InUse = XmlConvert.ToBoolean(Utility.GetAttributeValue<String>(parentConceptElement, "inUse", null));
                    }

                    concept.ConceptRelationships.Add(conceptRelationship);
                }

                List<XElement> relatedConceptElements = relationshipsElement.Descendants("RelatedConcept").ToList<XElement>();

                foreach (XElement relatedConceptElement in relatedConceptElements)
                {
                    ConceptRelationshipType conceptRelationshipType = _conceptRelationshipTypeLookUp.GetConceptRelationshipType(Utility.GetAttributeValue(relatedConceptElement, "type", String.Empty));

                    if (conceptRelationshipType == null)
                    {
                        //todo alert
                    }
                    else
                    {
                        ConceptRelationship conceptRelationship = new ConceptRelationship();

                        conceptRelationship.ProviderToRecordId = Utility.GetAttributeValue(relatedConceptElement, "ref", String.Empty).ToUpper();
                        
                        conceptRelationship.ConceptRelationshipTypeId = conceptRelationshipType.ConceptRelationshipTypeId;

                        //  conceptRelationship.R = Utility.GetAttributeValue<String>(relatedConceptElement, "rank", null);
                        conceptRelationship.InUse = XmlConvert.ToBoolean(Utility.GetAttributeValue<String>(relatedConceptElement, "inUse", null));

                        concept.ConceptRelationships.Add(conceptRelationship);
                    }
                }
            }
        }
    }
}

using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NZOR.Data.Entities.Common;
using NZOR.Data.Entities.Provider;
using NZOR.Data.LookUps.Common;
using System.Collections.Generic;

namespace NZOR.Harvest.Parsers
{
    public class TaxonNameUseParser
    {
        private ConceptRelationshipTypeLookUp _conceptRelationshipTypeLookUp;

        private Guid _dataSourceId;

        public TaxonNameUseParser(Guid dataSourceId, ConceptRelationshipTypeLookUp conceptRelationshipTypeLookUp)
        {
            _dataSourceId = dataSourceId;

            _conceptRelationshipTypeLookUp = conceptRelationshipTypeLookUp;
        }

        public List<Concept> Parse(XElement taxonNameUseElement, List<Concept> currentConcepts)
        {
            List<Concept> concepts = new List<Concept>();

            //Name Based Concepts dont necessarily have a Provider Name Id, so any Concepts with the same name and reference is the same name based concept

            String nameId = GetProviderNameId(taxonNameUseElement);
            String referenceId = GetProviderReferenceId(taxonNameUseElement);

            Concept nameConcept = currentConcepts.SingleOrDefault(o => (o.ProviderNameId.Equals(nameId, StringComparison.OrdinalIgnoreCase) 
                && (o.ProviderReferenceId ?? "").Equals(referenceId, StringComparison.OrdinalIgnoreCase)
                && (o.Type == Concept.ConceptType.TaxonNameUse || o.Type == Concept.ConceptType.VernacularUse)));

            if (nameConcept == null)
            {
                nameConcept = new Concept();

                nameConcept.DataSourceId = _dataSourceId;

                if (String.IsNullOrEmpty(Utility.GetAttributeValue<String>(taxonNameUseElement, "id", String.Empty)))
                {
                    nameConcept.ProviderRecordId = Guid.NewGuid().ToString().ToUpper();
                }
                else
                {
                    nameConcept.ProviderRecordId = Utility.GetAttributeValue<String>(taxonNameUseElement, "id", String.Empty).ToUpper();
                }
                
                concepts.Add(nameConcept);
            }

            nameConcept.Type = Concept.ConceptType.TaxonNameUse;
            nameConcept.ProviderNameId = nameId;
            nameConcept.ProviderReferenceId = referenceId;

            String acceptedNameId = GetAcceptedProviderNameId(taxonNameUseElement);

            Guid accRelId = _conceptRelationshipTypeLookUp.GetConceptRelationshipType(ConceptRelationshipTypeLookUp.IsSynonymOf).ConceptRelationshipTypeId;
            Guid parRelId = _conceptRelationshipTypeLookUp.GetConceptRelationshipType(ConceptRelationshipTypeLookUp.IsChildOf).ConceptRelationshipTypeId;

            if (!String.IsNullOrWhiteSpace(acceptedNameId))
            {
                if (nameId.Equals(acceptedNameId, StringComparison.OrdinalIgnoreCase))
                {
                    ConceptRelationship conceptRelationship = nameConcept.ConceptRelationships.Where(r => r.ConceptRelationshipTypeId == accRelId).FirstOrDefault();
                    if (conceptRelationship == null) conceptRelationship = new ConceptRelationship();

                    conceptRelationship.ProviderToRecordId = nameConcept.ProviderRecordId;
                    conceptRelationship.ConceptRelationshipTypeId = accRelId;

                    XElement acceptedNameElement = taxonNameUseElement.Element("AcceptedName");

                    if (Utility.GetAttributeValue<String>(acceptedNameElement, "inUse", null) != null)
                    {
                        conceptRelationship.InUse = XmlConvert.ToBoolean(Utility.GetAttributeValue<String>(acceptedNameElement, "inUse", null));
                    }

                    nameConcept.ConceptRelationships.Add(conceptRelationship);
                }
                else
                {
                    Concept acceptedNameConcept = currentConcepts.Union(concepts).SingleOrDefault(
                        o => o.ProviderNameId.Equals(acceptedNameId, StringComparison.OrdinalIgnoreCase)                    
                        && (o.ProviderReferenceId ?? "").Equals(referenceId, StringComparison.OrdinalIgnoreCase)
                        && o.DataSourceId == nameConcept.DataSourceId);
                    
                    if (acceptedNameConcept == null)
                    {
                        acceptedNameConcept = new Concept();

                        acceptedNameConcept.ConceptId = Guid.NewGuid();
                        acceptedNameConcept.DataSourceId = _dataSourceId;
                        acceptedNameConcept.Type = Concept.ConceptType.TaxonNameUse;
                        acceptedNameConcept.ProviderRecordId = Guid.NewGuid().ToString().ToUpper();
                        acceptedNameConcept.ProviderNameId = acceptedNameId;
                        acceptedNameConcept.ProviderReferenceId = referenceId;

                        concepts.Add(acceptedNameConcept);
                    }
                    
                    ConceptRelationship conceptRelationship = nameConcept.ConceptRelationships.Where(r => r.ConceptRelationshipTypeId == accRelId).FirstOrDefault();                        
                    if (conceptRelationship == null) conceptRelationship = new ConceptRelationship();

                    conceptRelationship.ProviderToRecordId = acceptedNameConcept.ProviderRecordId;
                    conceptRelationship.ConceptRelationshipTypeId = accRelId;

                    XElement acceptedNameElement = taxonNameUseElement.Element("AcceptedName");

                    if (Utility.GetAttributeValue<String>(acceptedNameElement, "inUse", null) != null)
                    {
                        conceptRelationship.InUse = XmlConvert.ToBoolean(Utility.GetAttributeValue<String>(acceptedNameElement, "inUse", null));
                    }

                    nameConcept.ConceptRelationships.Add(conceptRelationship);
                }
            }

            String parentNameId = GetParentProviderNameId(taxonNameUseElement);

            if (!String.IsNullOrWhiteSpace(parentNameId))
            {
                Concept parentNameConcept = currentConcepts.Union(concepts).SingleOrDefault(
                        o => o.ProviderNameId.Equals(parentNameId, StringComparison.OrdinalIgnoreCase)
                        && (o.ProviderReferenceId ?? "").Equals(referenceId, StringComparison.OrdinalIgnoreCase)
                        && o.DataSourceId == nameConcept.DataSourceId);
                    
                if (parentNameConcept == null)
                {
                    parentNameConcept = new Concept();

                    parentNameConcept.ConceptId = Guid.NewGuid();
                    parentNameConcept.DataSourceId = _dataSourceId;
                    parentNameConcept.Type = Concept.ConceptType.TaxonNameUse;
                    parentNameConcept.ProviderRecordId = Guid.NewGuid().ToString().ToUpper();
                    parentNameConcept.ProviderNameId = parentNameId;
                    parentNameConcept.ProviderReferenceId = referenceId;

                    concepts.Add(parentNameConcept);
                }

                ConceptRelationship conceptRelationship = nameConcept.ConceptRelationships.Where(r => r.ConceptRelationshipTypeId == parRelId).FirstOrDefault();
                if (conceptRelationship == null) conceptRelationship = new ConceptRelationship();

                conceptRelationship.ProviderToRecordId = parentNameConcept.ProviderRecordId;
                conceptRelationship.ConceptRelationshipTypeId = parRelId;

                XElement parentNameElement = taxonNameUseElement.Element("ParentName");

                if (Utility.GetAttributeValue<String>(parentNameElement, "inUse", null) != null)
                {
                    conceptRelationship.InUse = XmlConvert.ToBoolean(Utility.GetAttributeValue<String>(parentNameElement, "inUse", null));
                }

                nameConcept.ConceptRelationships.Add(conceptRelationship);
            }

            return concepts;
        }

        private String GetProviderNameId(XElement taxonNameUseElement)
        {
            XElement nameElement = taxonNameUseElement.Descendants("Name").SingleOrDefault<XElement>();

            if (nameElement == null)
            {
                return String.Empty;
            }
            else
            {
                return Utility.GetAttributeValue<String>(nameElement, "ref", String.Empty).ToUpper();
            }
        }

        private String GetProviderReferenceId(XElement taxonNameUseElement)
        {
            XElement accordingToElement = taxonNameUseElement.Descendants("AccordingTo").SingleOrDefault<XElement>();

            if (accordingToElement == null)
            {
                return String.Empty;
            }
            else
            {
                return Utility.GetAttributeValue<String>(accordingToElement, "ref", String.Empty).ToUpper();
            }
        }

        private String GetAcceptedProviderNameId(XElement taxonNameUseElement)
        {
            XElement nameElement = taxonNameUseElement.Descendants("AcceptedName").SingleOrDefault<XElement>();

            if (nameElement == null)
            {
                return String.Empty;
            }
            else
            {
                return Utility.GetAttributeValue<String>(nameElement, "ref", String.Empty).ToUpper();
            }
        }

        private String GetParentProviderNameId(XElement taxonNameUseElement)
        {
            XElement nameElement = taxonNameUseElement.Descendants("ParentName").SingleOrDefault<XElement>();

            if (nameElement == null)
            {
                return String.Empty;
            }
            else
            {
                return Utility.GetAttributeValue<String>(nameElement, "ref", String.Empty).ToUpper();
            }
        }
    }
}
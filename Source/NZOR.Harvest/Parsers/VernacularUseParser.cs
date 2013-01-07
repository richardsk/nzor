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
    public class VernacularUseParser
    {
        private ConceptApplicationTypeLookUp _conceptApplTypeLookUp;

        private Guid _dataSourceId;

        public VernacularUseParser(Guid dataSourceId, ConceptApplicationTypeLookUp conceptApplTypeLookUp)
        {
            _dataSourceId = dataSourceId;

            _conceptApplTypeLookUp = conceptApplTypeLookUp;
        }

        public List<Concept> Parse(XElement vernacularUseElement, List<Concept> currentConcepts)
        {
            List<Concept> concepts = new List<Concept>();

            //Name Based Concepts dont necessarily have a Provider Name Id, so any Concepts with the same name and reference is the same name based concept
            String nameId = GetProviderNameId(vernacularUseElement);
            String referenceId = GetProviderReferenceId(vernacularUseElement);

            Concept nameConcept = currentConcepts.SingleOrDefault(o => (o.ProviderNameId.Equals(nameId, StringComparison.OrdinalIgnoreCase)
                && (o.ProviderReferenceId ?? "").Equals(referenceId, StringComparison.OrdinalIgnoreCase)
                && (o.Type == Concept.ConceptType.TaxonNameUse || o.Type == Concept.ConceptType.VernacularUse)));

            if (nameConcept == null)
            {
                nameConcept = new Concept();

                nameConcept.DataSourceId = _dataSourceId;

                if (String.IsNullOrEmpty(Utility.GetAttributeValue<String>(vernacularUseElement, "id", String.Empty)))
                {
                    nameConcept.ProviderRecordId = Guid.NewGuid().ToString().ToUpper();
                }
                else
                {
                    nameConcept.ProviderRecordId = Utility.GetAttributeValue<String>(vernacularUseElement, "id", String.Empty).ToUpper();
                }

                concepts.Add(nameConcept);
            }

            nameConcept.Type = Concept.ConceptType.VernacularUse;
            nameConcept.ProviderNameId = nameId;
            nameConcept.ProviderReferenceId = referenceId;

            XElement rankElement = vernacularUseElement.Descendants("Rank").SingleOrDefault<XElement>();
            if (rankElement != null)
            {
                nameConcept.TaxonRank = rankElement.Value;
            }

            IEnumerable<XElement> applicationElements = vernacularUseElement.Descendants("Application");

            foreach (XElement appEl in applicationElements)
            {
                String taxonNameId = GetProviderTaxonNameId(appEl);

                if (!String.IsNullOrWhiteSpace(taxonNameId))
                {
                    Concept taxonNameConcept = currentConcepts.Union(concepts).SingleOrDefault(
                        o => o.ProviderNameId.Equals(taxonNameId, StringComparison.OrdinalIgnoreCase)
                        && (o.ProviderReferenceId ?? "").Equals(referenceId, StringComparison.OrdinalIgnoreCase)
                        && o.DataSourceId == nameConcept.DataSourceId);

                    if (taxonNameConcept == null)
                    {
                        taxonNameConcept = new Concept();

                        taxonNameConcept.DataSourceId = _dataSourceId;
                        taxonNameConcept.Type = Concept.ConceptType.VernacularUse;
                        taxonNameConcept.ProviderRecordId = Guid.NewGuid().ToString().ToUpper();
                        taxonNameConcept.ProviderNameId = taxonNameId;
                        taxonNameConcept.ProviderReferenceId = referenceId;

                        concepts.Add(taxonNameConcept);
                    }

                    ConceptApplication app = new ConceptApplication();

                    app.ProviderToRecordId = taxonNameConcept.ProviderRecordId;
                    app.ConceptApplicationTypeId = _conceptApplTypeLookUp.GetConceptApplicationType(ConceptApplicationTypeLookUp.IsVernacularFor).ConceptApplicationTypeId;

                    if (Utility.GetAttributeValue<String>(appEl, "inUse", null) != null)
                    {
                        app.InUse = XmlConvert.ToBoolean(Utility.GetAttributeValue<String>(appEl, "inUse", null));
                    }

                    XElement genderElement = appEl.Descendants("Gender").SingleOrDefault<XElement>();
                    if (genderElement != null)
                    {
                        app.Gender = genderElement.Value;
                    }

                    XElement partOfTaxonElement = appEl.Descendants("PartOfTaxon").SingleOrDefault<XElement>();
                    if (partOfTaxonElement != null)
                    {
                        app.PartOfTaxon = partOfTaxonElement.Value;
                    }

                    XElement lifeStageElement = appEl.Descendants("LifeStage").SingleOrDefault<XElement>();
                    if (lifeStageElement != null)
                    {
                        app.LifeStage = lifeStageElement.Value;
                    }

                    XElement geoRegionElement = appEl.Descendants("GeoRegion").SingleOrDefault<XElement>();
                    if (geoRegionElement != null)
                    {
                        app.GeoRegion = geoRegionElement.Value;

                        if (Utility.GetAttributeValue<String>(geoRegionElement, "geographicSchema", null) != null)
                        {
                            app.GeographicSchema = Utility.GetAttributeValue<String>(geoRegionElement, "geographicSchema", null);
                        }
                    }

                    nameConcept.ConceptApplications.Add(app);
                }
            }

            return concepts;
        }

        private String GetProviderNameId(XElement vernacularElement)
        {
            XElement nameElement = vernacularElement.Descendants("VernacularName").SingleOrDefault<XElement>();

            if (nameElement == null)
            {
                return String.Empty;
            }
            else
            {
                return Utility.GetAttributeValue<String>(nameElement, "ref", String.Empty);
            }
        }

        private String GetProviderReferenceId(XElement vernacularElement)
        {
            XElement accordingToElement = vernacularElement.Descendants("AccordingTo").SingleOrDefault<XElement>();

            if (accordingToElement == null)
            {
                return String.Empty;
            }
            else
            {
                return Utility.GetAttributeValue<String>(accordingToElement, "ref", String.Empty);
            }
        }

        private String GetProviderTaxonNameId(XElement appElement)
        {
            XElement nameElement = appElement.Descendants("TaxonName").SingleOrDefault<XElement>();

            if (nameElement == null)
            {
                return String.Empty;
            }
            else
            {
                return Utility.GetAttributeValue<String>(nameElement, "ref", String.Empty);
            }
        }
    }
}
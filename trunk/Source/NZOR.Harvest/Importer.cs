using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;
using NZOR.Data.Entities.Common;
using NZOR.Data.Entities.Provider;
using NZOR.Data.LookUps.Common;
using NZOR.Data.Repositories.Common;
using NZOR.Data.Repositories.Provider;
using NZOR.Harvest.Parsers;
using NZOR.Harvest.SchemaValidation;
using NZOR.Admin.Data.Repositories;
using NZOR.Admin.Data.Lookups;

namespace NZOR.Harvest
{
    public class Importer
    {
        INameRepository _nameRepository;
        IReferenceRepository _referenceRepository;
        IConceptRepository _conceptRepository;
        ITaxonPropertyRepository _taxonPropertyRepository;
        IAnnotationRepository _annotationRepository;
        IProviderRepository _providerRepository;
        ILookUpRepository _lookUpRepository;

        String _validationSchemaUrl;

        List<DataSource> _dataSources;
        
        public class DataSource
        {
            public Guid DataSourceId { get; set; }

            public String DataSourceCode { get; set; }

            public SortedList<String, Name> Names { get; set; }
            public SortedList<String, Reference> References { get; set; }
            public SortedList<String, Concept> Concepts { get; set; }
            public SortedList<String, TaxonProperty> TaxonProperties { get; set; }
            public SortedList<String, Annotation> Annotations { get; set; }

            public DataSource()
            {
                DataSourceId = Guid.Empty;

                DataSourceCode = String.Empty;

                Names = new SortedList<String, Name>(10000, StringComparer.OrdinalIgnoreCase);
                References = new SortedList<String, Reference>(10000, StringComparer.OrdinalIgnoreCase);
                Concepts = new SortedList<String, Concept>(10000, StringComparer.OrdinalIgnoreCase);
                TaxonProperties = new SortedList<string, TaxonProperty>(10000, StringComparer.OrdinalIgnoreCase);
                Annotations = new SortedList<string, Annotation>(10000, StringComparer.OrdinalIgnoreCase);
            }
        }

        public Importer(INameRepository nameRepository,
            IReferenceRepository referenceRepository,
            IConceptRepository conceptRespository,
            ILookUpRepository lookUpRepository,
            ITaxonPropertyRepository taxonPropertyRepository,
            IAnnotationRepository annRepository,
            IProviderRepository provRepository,
            String validationSchemaUrl)
        {
            _nameRepository = nameRepository;
            _referenceRepository = referenceRepository;
            _conceptRepository = conceptRespository;
            _taxonPropertyRepository = taxonPropertyRepository;
            _annotationRepository = annRepository;
            _providerRepository = provRepository;
            _lookUpRepository = lookUpRepository;

            _validationSchemaUrl = validationSchemaUrl;

            _dataSources = new List<DataSource>();
        }

        public DataSource DataSources(String dataSourceCode)
        {
            return _dataSources.SingleOrDefault<DataSource>(o => o.DataSourceCode == dataSourceCode);
        }

        public int TotalHarvested { get; set; }
        
        public void Import(XDocument document)
        {
            _dataSources.Clear();

            SchemaValidator.ValidationResult validationResult = SchemaValidator.Validate(document, _validationSchemaUrl);
            
            XElement metadataElement = document.Descendants("Metadata").SingleOrDefault();
            XElement providerElement = metadataElement.Descendants("Provider").SingleOrDefault();
            String providerCode = Utility.GetAttributeValue<String>(providerElement, "id", String.Empty);

            if (validationResult.IsValid)
            { 
                ProviderLookUp providerLookUp = new ProviderLookUp(_providerRepository.GetProviders());

                Admin.Data.Entities.Provider provider = providerLookUp.GetProvider(providerCode);

                List<XElement> dataSourceElements = document.Descendants("DataSource").ToList<XElement>();

                foreach (XElement dataSourceElement in dataSourceElements)
                {
                    String dataSourceCode = Utility.GetAttributeValue<String>(dataSourceElement, "id", String.Empty);
                    Guid dataSourceId = provider.DataSources.Where(o => o.Code == dataSourceCode).SingleOrDefault().DataSourceId;

                    DataSource dataSource = ProcessDataSourceElement(dataSourceId, dataSourceCode, dataSourceElement);

                    LinkTaxonConcepts(dataSource);
                    
                    LinkPropertyRelatedIds(dataSource);

                    LinkTaxonProperties(dataSource);

                    LinkAnnotations(dataSource);

                    _dataSources.Add(dataSource);

                }
            }
            else
            {
                string valString = "";
                foreach (SchemaValidator.ValidationResult.ValidationEvent ve in validationResult.ValidationEvents)
                {
                    valString += "Line " + ve.LineNumber.ToString() + ", Line Position " + ve.LinePosition.ToString() + ", " + ve.Message + Environment.NewLine;
                }

                throw new Exception("Harvested XML is not valid for provider " + providerCode + ": " + Environment.NewLine + valString);
            }
        }

        public void Save()
        {
            foreach (DataSource dataSource in _dataSources)
            {
                _nameRepository.Names.AddRange(dataSource.Names.Values.Where(o => o.State != Data.Entities.Entity.EntityState.Unchanged));
                _referenceRepository.References.AddRange(dataSource.References.Values.Where(o => o.State != Data.Entities.Entity.EntityState.Unchanged));
                _conceptRepository.Concepts.AddRange(dataSource.Concepts.Values.Where(o => o.State != Data.Entities.Entity.EntityState.Unchanged));
                _taxonPropertyRepository.TaxonProperties.AddRange(dataSource.TaxonProperties.Values.Where(o => o.State != Data.Entities.Entity.EntityState.Unchanged));
                _annotationRepository.Annotations.AddRange(dataSource.Annotations.Values.Where(o => o.State != Data.Entities.Entity.EntityState.Unchanged));

                IEnumerable<Concept> n = dataSource.Concepts.Values.Where(o => o.NameId == Guid.Empty);
                if (n.Count() > 0)
                {
                    string etrr = "err";
                }
            }

            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(1, 0, 0, 0, 0)))
                {
                    _nameRepository.Save();
                    _referenceRepository.Save();
                    _conceptRepository.Save();
                    _taxonPropertyRepository.Save();
                    _annotationRepository.Save();

                    scope.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private DataSource ProcessDataSourceElement(Guid dataSourceId, String dataSourceCode, XElement dataSourceElement)
        {
            DataSource dataSource = new DataSource();

            dataSource.DataSourceId = dataSourceId;
            dataSource.DataSourceCode = dataSourceCode;

            _nameRepository.GetNames(dataSourceId).ForEach(o => dataSource.Names.Add(o.ProviderRecordId, o));
            _referenceRepository.GetReferences(dataSourceId).ForEach(o => dataSource.References.Add(o.ProviderRecordId, o));
            _conceptRepository.GetConcepts(dataSourceId).ForEach(o => dataSource.Concepts.Add(o.ProviderRecordId, o));
            _taxonPropertyRepository.GetTaxonProperties(dataSourceId).ForEach(o => dataSource.TaxonProperties.Add(o.ProviderRecordId, o));
            _annotationRepository.GetAnnotations(dataSourceId).ForEach(o => dataSource.Annotations.Add(o.ProviderRecordId, o));

            TotalHarvested = 0;

            List<XElement> taxonNameElements = dataSourceElement.Descendants().Where(o => o.Name == "TaxonName" && o.Parent.Name == "TaxonNames").ToList<XElement>();
            TotalHarvested += taxonNameElements.Count;
            ProcessTaxonNames(dataSource, taxonNameElements);

            List<XElement> vernacularNameElements = dataSourceElement.Descendants().Where(o => o.Name == "VernacularName" && o.Parent.Name == "TaxonNames").ToList<XElement>();
            TotalHarvested += vernacularNameElements.Count;
            ProcessVernacularNames(dataSource, vernacularNameElements);

            List<XElement> publicationElements = dataSourceElement.Descendants().Where(o => o.Name == "Publication" && o.Parent.Name == "Publications").ToList<XElement>();
            TotalHarvested += publicationElements.Count;
            ProcessPublications(dataSource, publicationElements);

            List<XElement> taxonConceptElements = dataSourceElement.Descendants().Where(o => o.Name == "TaxonConcept" && o.Parent.Name == "TaxonConcepts").ToList<XElement>();
            TotalHarvested += taxonConceptElements.Count;
            ProcessTaxonConcepts(dataSource, taxonConceptElements);

            List<XElement> vernacularConceptElements = dataSourceElement.Descendants().Where(o => o.Name == "VernacularConcept" && o.Parent.Name == "TaxonConcepts").ToList<XElement>();
            TotalHarvested += vernacularConceptElements.Count;
            ProcessVernacularConcepts(dataSource, vernacularConceptElements);

            List<XElement> taxonNameUseElements = dataSourceElement.Descendants().Where(o => o.Name == "TaxonNameUse" && o.Parent.Name == "NameBasedConcept").ToList<XElement>();
            TotalHarvested += taxonNameUseElements.Count;
            ProcessTaxonNameUses(dataSource, taxonNameUseElements);

            List<XElement> vernacularUseElements = dataSourceElement.Descendants().Where(o => o.Name == "VernacularUse" && o.Parent.Name == "NameBasedConcept").ToList<XElement>();
            TotalHarvested += vernacularUseElements.Count;
            ProcessVernacularUses(dataSource, vernacularUseElements);

            List<XElement> taxonPropertyElements = dataSourceElement.Descendants().Where(o => o.Name == "Biostatus" && o.Parent.Name == "BiostatusValues").ToList<XElement>();
            TotalHarvested += taxonPropertyElements.Count;
            ProcessTaxonProperties(dataSource, taxonPropertyElements);

            taxonPropertyElements = dataSourceElement.Descendants().Where(o => o.Name == "ManagementStatus" && o.Parent.Name == "ManagementStatusValues").ToList<XElement>();
            TotalHarvested += taxonPropertyElements.Count;
            ProcessTaxonProperties(dataSource, taxonPropertyElements);

            List<XElement> annotationElements = dataSourceElement.Descendants().Where(o => o.Name == "Annotation" && o.Parent.Name == "Annotations").ToList<XElement>();
            TotalHarvested += annotationElements.Count;
            ProcessAnnotations(dataSource, annotationElements);

            return dataSource;
        }

        private void ProcessTaxonNames(DataSource dataSource, List<XElement> taxonNameElements)
        {
            TaxonRankLookUp taxonRankLookUp = new TaxonRankLookUp(_lookUpRepository.GetTaxonRanks());
            NamePropertyTypeLookUp namePropertyTypeLookUp = new NamePropertyTypeLookUp(_lookUpRepository.GetNamePropertyTypes());
            NameClassLookUp nameClassLookUp = new NameClassLookUp(_lookUpRepository.GetNameClasses());

            TaxonNameParser parser = new TaxonNameParser(dataSource.DataSourceId, nameClassLookUp, namePropertyTypeLookUp, taxonRankLookUp);

            List<Name> parsedNames = new List<Name>();

            foreach (XElement taxonNameElement in taxonNameElements)
            {
                Name parsedName = parser.Parse(taxonNameElement);

                parsedNames.Add(parsedName);
            }

            UpdateDataSourceNames(dataSource, parsedNames);
        }

        private void ProcessTaxonProperties(DataSource dataSource, List<XElement> taxonPropertyElements)
        {
            TaxonPropertyClassLookup propClassLookup = new TaxonPropertyClassLookup(_lookUpRepository.GetTaxonPropertyClasses());
            TaxonPropertyTypeLookup propTypeLookup = new TaxonPropertyTypeLookup(_lookUpRepository.GetTaxonPropertyTypes());

            TaxonPropertyParser parser = new TaxonPropertyParser(dataSource.DataSourceId, propClassLookup, propTypeLookup);

            List<TaxonProperty> parsedProperties = new List<TaxonProperty>();

            foreach (XElement taxonPropElement in taxonPropertyElements)
            {
                TaxonProperty prop = parser.Parse(taxonPropElement);

                parsedProperties.Add(prop);
            }

            UpdateDataSourceTaxonProperties(dataSource, parsedProperties);
        }

        private void ProcessVernacularNames(DataSource dataSource, List<XElement> vernacularNameElements)
        {
            TaxonRankLookUp taxonRankLookUp = new TaxonRankLookUp(_lookUpRepository.GetTaxonRanks());
            NamePropertyTypeLookUp namePropertyTypeLookUp = new NamePropertyTypeLookUp(_lookUpRepository.GetNamePropertyTypes());
            NameClassLookUp nameClassLookUp = new NameClassLookUp(_lookUpRepository.GetNameClasses());

            VernacularNameParser parser = new VernacularNameParser(dataSource.DataSourceId, nameClassLookUp, namePropertyTypeLookUp, taxonRankLookUp);

            List<Name> parsedNames = new List<Name>();

            foreach (XElement vernacularNameElement in vernacularNameElements)
            {
                Name parsedName = parser.Parse(vernacularNameElement);

                parsedNames.Add(parsedName);
            }

            UpdateDataSourceNames(dataSource, parsedNames);
        }

        private void ProcessAnnotations(DataSource dataSource, List<XElement> annotationElements)
        {
            AnnotationParser parser = new AnnotationParser(dataSource.DataSourceId);

            List<Annotation> parsedAnnotations = new List<Annotation>();

            foreach (XElement annElement in annotationElements)
            {
                Annotation ann = parser.Parse(annElement);

                parsedAnnotations.Add(ann);
            }

            UpdateDataSourceAnnotations(dataSource, parsedAnnotations);
        }

        private void UpdateDataSourceNames(DataSource dataSource, List<Name> parsedNames)
        {
            Name existingName = null;

            foreach (Name parsedName in parsedNames)
            {
                if (dataSource.Names.ContainsKey(parsedName.ProviderRecordId))
                {
                    existingName = dataSource.Names[parsedName.ProviderRecordId];

                    parsedName.NameId = existingName.NameId;

                    parsedName.State = existingName.State;
                    if (existingName.State != Data.Entities.Entity.EntityState.Added)
                    {
                        parsedName.State = Data.Entities.Entity.EntityState.Modified;
                        parsedName.ModifiedDate = DateTime.Now;
                    }

                    //use existing name values for non provided stuff
                    parsedName.AddedDate = existingName.AddedDate;
                    parsedName.ConsensusNameId = existingName.ConsensusNameId;
                    parsedName.LinkStatus = existingName.LinkStatus;
                    parsedName.MatchPath = existingName.MatchPath;
                    parsedName.MatchScore = existingName.MatchScore;
                    
                    dataSource.Names.Remove(existingName.ProviderRecordId);
                }
                else
                {
                    parsedName.NameId = Guid.NewGuid();
                    parsedName.AddedDate = DateTime.Now;

                    parsedName.State = Data.Entities.Entity.EntityState.Added;
                }

                dataSource.Names.Add(parsedName.ProviderRecordId, parsedName);
            }
        }

        private void UpdateDataSourceTaxonProperties(DataSource dataSource, List<TaxonProperty> parsedProps)
        {
            TaxonProperty existingProp = null;

            foreach (TaxonProperty tp in parsedProps)
            {
                if (dataSource.TaxonProperties.ContainsKey(tp.ProviderRecordId))
                {
                    existingProp = dataSource.TaxonProperties[tp.ProviderRecordId];

                    tp.TaxonPropertyId = existingProp.TaxonPropertyId;

                    tp.State = existingProp.State;
                    if (existingProp.State != Data.Entities.Entity.EntityState.Added)
                    {
                        tp.State = Data.Entities.Entity.EntityState.Modified;
                        tp.ModifiedDate = DateTime.Now;
                    }

                    //use existing tp values for non provided stuff
                    tp.AddedDate = existingProp.AddedDate;
                    tp.ConsensusTaxonPropertyId = existingProp.ConsensusTaxonPropertyId;
                    tp.LinkStatus = existingProp.LinkStatus;                    
                    tp.MatchScore = existingProp.MatchScore;
                    
                    dataSource.TaxonProperties.Remove(tp.ProviderRecordId);
                }
                else
                {
                    bool done = false;

                    if (tp.ProviderRecordId == string.Empty)
                    {   
                        //prov record id not provided, so find based on tpye, name and concept                        
                        KeyValuePair<string, TaxonProperty> item = dataSource.TaxonProperties.FirstOrDefault(o => o.Value.ProviderConceptId == tp.ProviderConceptId && o.Value.TaxonPropertyClassId == tp.TaxonPropertyClassId && o.Value.ProviderNameId == tp.ProviderNameId);
                        
                        if (item.Key != null)
                        {
                            tp.TaxonPropertyId = item.Value.TaxonPropertyId;

                            tp.State = existingProp.State;
                            if (existingProp.State != Data.Entities.Entity.EntityState.Added)
                            {
                                tp.State = Data.Entities.Entity.EntityState.Modified;
                                tp.ModifiedDate = DateTime.Now;
                            }

                            //use existing tp values for non provided stuff
                            tp.AddedDate = item.Value.AddedDate;
                            tp.ConsensusTaxonPropertyId = item.Value.ConsensusTaxonPropertyId;
                            tp.LinkStatus = item.Value.LinkStatus;
                            tp.MatchScore = item.Value.MatchScore;

                            dataSource.TaxonProperties.Remove(item.Key);

                            done = true;
                        }
                    }

                    if (!done)
                    {
                        tp.TaxonPropertyId = Guid.NewGuid();
                        tp.AddedDate = DateTime.Now;

                        tp.State = Data.Entities.Entity.EntityState.Added;
                    }
                }

                dataSource.TaxonProperties.Add(tp.ProviderRecordId, tp);
            }
        }


        private void UpdateDataSourceAnnotations(DataSource dataSource, List<Annotation> parsedAnnotations)
        {
            Annotation existingAnn = null;

            foreach (Annotation ann in parsedAnnotations)
            {
                if (dataSource.Annotations.ContainsKey(ann.ProviderRecordId))
                {
                    existingAnn = dataSource.Annotations[ann.ProviderRecordId];

                    ann.AnnotationId = ann.AnnotationId;

                    ann.State = existingAnn.State;
                    if (existingAnn.State != Data.Entities.Entity.EntityState.Added)
                    {
                        ann.State = Data.Entities.Entity.EntityState.Modified;
                        ann.ModifiedDate = DateTime.Now;
                    }

                    //use existing tp values for non provided stuff
                    ann.AddedDate = existingAnn.AddedDate;
                    ann.ConsensusAnnotationId = existingAnn.ConsensusAnnotationId;

                    dataSource.Annotations.Remove(ann.ProviderRecordId);
                }
                else
                {
                    if (ann.ProviderRecordId == string.Empty) ann.ProviderRecordId = Guid.NewGuid().ToString();

                    ann.AnnotationId = Guid.NewGuid();
                    ann.AddedDate = DateTime.Now;

                    ann.State = Data.Entities.Entity.EntityState.Added;
                }

                dataSource.Annotations.Add(ann.ProviderRecordId, ann);
            }
        }


        private void ProcessPublications(DataSource dataSource, List<XElement> publicationElements)
        {
            ReferenceTypeLookUp referenceTypeLookUp = new ReferenceTypeLookUp(_lookUpRepository.GetReferenceTypes());
            ReferencePropertyTypeLookUp referencePropertyTypeLookUp = new ReferencePropertyTypeLookUp(_lookUpRepository.GetReferencePropertyTypes());

            PublicationParser parser = new PublicationParser(dataSource.DataSourceId, referenceTypeLookUp, referencePropertyTypeLookUp);

            Reference existingReference = null;

            foreach (XElement publicationElement in publicationElements)
            {
                Reference parsedReference = parser.Parse(publicationElement);

                if (dataSource.References.ContainsKey(parsedReference.ProviderRecordId))
                {
                    existingReference = dataSource.References[parsedReference.ProviderRecordId];
                    parsedReference.ReferenceId = existingReference.ReferenceId;

                    parsedReference.State = existingReference.State;
                    if (existingReference.State != Data.Entities.Entity.EntityState.Added)
                    {
                        parsedReference.State = Data.Entities.Entity.EntityState.Modified;
                        parsedReference.ModifiedDate = DateTime.Now;
                    }

                    //use existing ref values for non provided stuff
                    parsedReference.AddedDate = existingReference.AddedDate;
                    parsedReference.ConsensusReferenceId = existingReference.ConsensusReferenceId;
                    parsedReference.LinkStatus = existingReference.LinkStatus;
                    parsedReference.MatchPath = existingReference.MatchPath;
                    parsedReference.MatchScore = existingReference.MatchScore;

                    dataSource.References.Remove(existingReference.ProviderRecordId);
                }
                else
                {
                    parsedReference.ReferenceId = Guid.NewGuid();
                    parsedReference.AddedDate = DateTime.Now;

                    parsedReference.State = Data.Entities.Entity.EntityState.Added;
                }

                dataSource.References.Add(parsedReference.ProviderRecordId, parsedReference);
            }
        }

        private void ProcessTaxonConcepts(DataSource dataSource, List<XElement> taxonConceptElements)
        {
            ConceptRelationshipTypeLookUp conceptRelationshipTypeLookUp = new ConceptRelationshipTypeLookUp(_lookUpRepository.GetConceptRelationshipTypes());
            TaxonConceptParser parser = new TaxonConceptParser(dataSource.DataSourceId, conceptRelationshipTypeLookUp);
            List<Concept> parsedConcepts = new List<Concept>();

            foreach (XElement taxonConceptElement in taxonConceptElements)
            {
                Concept parsedConcept = parser.Parse(taxonConceptElement);

                parsedConcepts.Add(parsedConcept);
            }

            UpdateDataSourceConcepts(dataSource, parsedConcepts);
        }

        private void ProcessVernacularConcepts(DataSource dataSource, List<XElement> vernacularConceptElements)
        {
            ConceptApplicationTypeLookUp conceptApplicationTypeLookUp = new ConceptApplicationTypeLookUp(_lookUpRepository.GetConceptApplicationTypes());
            VernacularConceptParser parser = new VernacularConceptParser(dataSource.DataSourceId, conceptApplicationTypeLookUp);
            List<Concept> parsedConcepts = new List<Concept>();

            foreach (XElement vernacularConceptElement in vernacularConceptElements)
            {
                Concept parsedConcept = parser.Parse(vernacularConceptElement);

                parsedConcepts.Add(parsedConcept);
            }

            UpdateDataSourceConcepts(dataSource, parsedConcepts);
        }

        private void ProcessTaxonNameUses(DataSource dataSource, List<XElement> taxonNameUses)
        {
            ConceptRelationshipTypeLookUp conceptRelationshipTypeLookUp = new ConceptRelationshipTypeLookUp(_lookUpRepository.GetConceptRelationshipTypes());
            TaxonNameUseParser parser = new TaxonNameUseParser(dataSource.DataSourceId, conceptRelationshipTypeLookUp);
            List<Concept> parsedConcepts = new List<Concept>();

            foreach (XElement tnuElement in taxonNameUses)
            {
                List<Concept> newConcepts = parser.Parse(tnuElement, new List<Concept>(dataSource.Concepts.Values.Union(parsedConcepts)));

                parsedConcepts.AddRange(newConcepts);
            }

            UpdateDataSourceConcepts(dataSource, parsedConcepts);
        }

        private void ProcessVernacularUses(DataSource dataSource, List<XElement> vernacularUses)
        {
            ConceptApplicationTypeLookUp conceptAppTypeLookUp = new ConceptApplicationTypeLookUp(_lookUpRepository.GetConceptApplicationTypes());
            VernacularUseParser parser = new VernacularUseParser(dataSource.DataSourceId, conceptAppTypeLookUp);
            List<Concept> parsedConcepts = new List<Concept>();

            foreach (XElement vernElement in vernacularUses)
            {
                List<Concept> newConcepts = parser.Parse(vernElement, new List<Concept>(dataSource.Concepts.Values.Union(parsedConcepts)));

                parsedConcepts.AddRange(newConcepts);
            }

            UpdateDataSourceConcepts(dataSource, parsedConcepts);
        }

        private void UpdateDataSourceConcepts(DataSource dataSource, List<Concept> parsedConcepts)
        {
            Concept existingConcept = null;

            foreach (Concept parsedConcept in parsedConcepts)
            {
                if (parsedConcept.Type == Concept.ConceptType.TaxonNameUse || parsedConcept.Type == Concept.ConceptType.VernacularUse)
                {
                    existingConcept = dataSource.Concepts.Values.SingleOrDefault(o => (o.ProviderNameId.Equals(parsedConcept.ProviderNameId, StringComparison.OrdinalIgnoreCase) 
                        && o.ProviderReferenceId.Equals(parsedConcept.ProviderReferenceId, StringComparison.OrdinalIgnoreCase)
                        && (o.Type == Concept.ConceptType.TaxonNameUse || o.Type == Concept.ConceptType.VernacularUse))
                        && o.DataSourceId == parsedConcept.DataSourceId);

                    if (existingConcept != null && existingConcept.State != Data.Entities.Entity.EntityState.Added)//if not added during this import
                    {
                        parsedConcept.ConceptId = existingConcept.ConceptId;

                        parsedConcept.State = existingConcept.State;
                        if (existingConcept.State != Data.Entities.Entity.EntityState.Added)
                        {
                            parsedConcept.State = Data.Entities.Entity.EntityState.Modified;
                            parsedConcept.ModifiedDate = DateTime.Now;
                        }

                        //use existing concept values for non provided stuff
                        parsedConcept.AddedDate = existingConcept.AddedDate;
                        parsedConcept.ConsensusConceptId = existingConcept.ConsensusConceptId;
                        parsedConcept.LinkStatus = existingConcept.LinkStatus;
                        parsedConcept.MatchPath = existingConcept.MatchPath;
                        parsedConcept.MatchScore = existingConcept.MatchScore;

                        parsedConcept.ProviderRecordId = existingConcept.ProviderRecordId;

                        foreach (ConceptApplication ca in parsedConcept.ConceptApplications)
                        {
                            ca.AddedDate = parsedConcept.AddedDate;
                            ca.ModifiedDate = DateTime.Now;                            
                        }
                        
                        dataSource.Concepts.Remove(existingConcept.ProviderRecordId);
                    }
                    else
                    {
                        parsedConcept.ConceptId = Guid.NewGuid();
                        parsedConcept.AddedDate = DateTime.Now;

                        parsedConcept.State = Data.Entities.Entity.EntityState.Added;
                    }
                }
                else
                {
                    if (dataSource.Concepts.ContainsKey(parsedConcept.ProviderRecordId))
                    {
                        existingConcept = dataSource.Concepts[parsedConcept.ProviderRecordId];
                        parsedConcept.ConceptId = existingConcept.ConceptId;

                        parsedConcept.State = existingConcept.State;
                        if (existingConcept.State != Data.Entities.Entity.EntityState.Added)
                        {
                            parsedConcept.State = Data.Entities.Entity.EntityState.Modified;
                            parsedConcept.ModifiedDate = DateTime.Now;
                        }

                        //use existing concept values for non provided stuff
                        parsedConcept.AddedDate = existingConcept.AddedDate;
                        parsedConcept.ConsensusConceptId = existingConcept.ConsensusConceptId;
                        parsedConcept.LinkStatus = existingConcept.LinkStatus;
                        parsedConcept.MatchPath = existingConcept.MatchPath;
                        parsedConcept.MatchScore = existingConcept.MatchScore;

                        dataSource.Concepts.Remove(existingConcept.ProviderRecordId);
                    }
                    else
                    {
                        parsedConcept.ConceptId = Guid.NewGuid();
                        parsedConcept.AddedDate = DateTime.Now;

                        parsedConcept.State = Data.Entities.Entity.EntityState.Added;
                    }
                }
                dataSource.Concepts.Add(parsedConcept.ProviderRecordId, parsedConcept);
            }
        }

        private void LinkPropertyRelatedIds(DataSource dataSource)
        {
            Parallel.ForEach(dataSource.Names.Values.Where(
                n => n.State == Data.Entities.Entity.EntityState.Added || n.State == Data.Entities.Entity.EntityState.Modified),
                name => LinkPropertyRelatedIds(dataSource, name)
                );
        }

        private void LinkPropertyRelatedIds(DataSource dataSource, Name name)
        {
            foreach (NameProperty np in name.NameProperties)
            {
                if (np.ProviderRelatedId != null)
                {
                    if (np.NamePropertyType == Data.LookUps.Common.NamePropertyTypeLookUp.PublishedIn)
                    {
                        if (dataSource.References.ContainsKey(np.ProviderRelatedId))
                        {
                            np.RelatedId = dataSource.References[np.ProviderRelatedId].ReferenceId;
                        }
                    }
                    else
                    {
                        if (dataSource.Names.ContainsKey(np.ProviderRelatedId))
                        {
                            np.RelatedId = dataSource.Names[np.ProviderRelatedId].NameId;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Links taxon name concepts to associated objects.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <remarks>
        /// Associated objects includes:
        /// - Names
        /// - References
        /// - Also links relationships and applications to other concepts.
        /// </remarks>
        private void LinkTaxonConcepts(DataSource dataSource)
        {
            IEnumerable<Concept> cList = dataSource.Concepts.Values.Where(
                c => c.State == Data.Entities.Entity.EntityState.Added || c.State == Data.Entities.Entity.EntityState.Modified);

            List<Concept> newConcepts = new List<Concept>();

            for (int i = cList.Count() - 1; i >= 0; i--)
            {
                Concept c = cList.ElementAt(i);
                LinkTaxonConcept(dataSource, c, newConcepts);
            }

            foreach (Concept c in newConcepts)
            {
                dataSource.Concepts.Add(c.ProviderRecordId, c);
            }
        }

        private void LinkTaxonConcept(DataSource dataSource, Concept concept, List<Concept> newConcepts)
        {
            if (dataSource.Names.ContainsKey(concept.ProviderNameId))
            {
                concept.NameId = dataSource.Names[concept.ProviderNameId].NameId;
            }
            else
            {
                //ERROR
                string err = "Provider Concept (ProviderRecordId='" + concept.ProviderRecordId + "') points to a Name that does not exist";
                //TODO log error?  fail?
            }

            if (dataSource.References.ContainsKey(concept.ProviderReferenceId))
            {
                concept.AccordingToReferenceId = dataSource.References[concept.ProviderReferenceId].ReferenceId;
            }
            
            foreach (ConceptRelationship conceptRelationship in concept.ConceptRelationships)
            {
                if (dataSource.Concepts.ContainsKey(conceptRelationship.ProviderToRecordId))
                {
                    conceptRelationship.FromConceptId = concept.ConceptId;
                    conceptRelationship.ToConceptId = dataSource.Concepts[conceptRelationship.ProviderToRecordId].ConceptId;
                }
                else
                {
                    //not pointing to a concept.  must be a name
                    Concept taxonNameConcept = dataSource.Concepts.Values.Union(newConcepts).SingleOrDefault(
                        o => o.ProviderNameId.Equals(conceptRelationship.ProviderToRecordId, StringComparison.OrdinalIgnoreCase)
                            && (o.ProviderReferenceId == null ? "" : o.ProviderReferenceId).Equals((concept.ProviderReferenceId == null ? "" : concept.ProviderReferenceId), StringComparison.OrdinalIgnoreCase)
                            && o.DataSourceId == concept.DataSourceId);

                    if (taxonNameConcept == null)
                    {
                        //name based concept
                        taxonNameConcept = new Concept();

                        taxonNameConcept.State = Data.Entities.Entity.EntityState.Added;
                        taxonNameConcept.ConceptId = Guid.NewGuid();
                        taxonNameConcept.DataSourceId = dataSource.DataSourceId;
                        taxonNameConcept.Type = Concept.ConceptType.TaxonNameUse;
                        taxonNameConcept.ProviderRecordId = Guid.NewGuid().ToString().ToUpper();
                        taxonNameConcept.ProviderNameId = conceptRelationship.ProviderToRecordId;
                        taxonNameConcept.ProviderReferenceId = concept.ProviderReferenceId;
                        
                        //link it
                        Name pn = dataSource.Names.Values.SingleOrDefault(o => o.ProviderRecordId == taxonNameConcept.ProviderNameId);
                        if (pn != null)
                        {
                            taxonNameConcept.NameId = pn.NameId;
                        }
                        else
                        {
                            //TODO error
                            string msg = "error";
                        }

                        Reference pr = dataSource.References.Values.SingleOrDefault(o => o.ProviderRecordId.Equals(taxonNameConcept.ProviderReferenceId, StringComparison.OrdinalIgnoreCase));
                        if (pr != null) taxonNameConcept.AccordingToReferenceId = pr.ReferenceId;

                        newConcepts.Add(taxonNameConcept);
                    }

                    conceptRelationship.FromConceptId = concept.ConceptId;
                    conceptRelationship.ToConceptId = taxonNameConcept.ConceptId;
                }
            }

            foreach (ConceptApplication conceptApplication in concept.ConceptApplications)
            {
                if (dataSource.Concepts.ContainsKey(conceptApplication.ProviderToRecordId))
                {
                    conceptApplication.FromConceptId = concept.ConceptId;
                    conceptApplication.ToConceptId = dataSource.Concepts[conceptApplication.ProviderToRecordId].ConceptId;
                }
                else
                {
                    //not pointing to a concept.  must be a name
                    Concept taxonNameConcept = dataSource.Concepts.Values.Union(newConcepts).SingleOrDefault(
                        o => o.ProviderNameId.Equals(conceptApplication.ProviderToRecordId, StringComparison.OrdinalIgnoreCase)
                            && (o.ProviderReferenceId == null ? "" : o.ProviderReferenceId).Equals((concept.ProviderReferenceId == null ? "" : concept.ProviderReferenceId), StringComparison.OrdinalIgnoreCase)                            
                            && o.DataSourceId == concept.DataSourceId);

                    if (taxonNameConcept == null)
                    {
                        //name based concept
                        taxonNameConcept = new Concept();

                        taxonNameConcept.State = Data.Entities.Entity.EntityState.Added;
                        taxonNameConcept.ConceptId = Guid.NewGuid();
                        taxonNameConcept.DataSourceId = dataSource.DataSourceId;
                        taxonNameConcept.Type = Concept.ConceptType.TaxonNameUse;
                        taxonNameConcept.ProviderRecordId = Guid.NewGuid().ToString().ToUpper();
                        taxonNameConcept.ProviderNameId = conceptApplication.ProviderToRecordId;
                        taxonNameConcept.ProviderReferenceId = concept.ProviderReferenceId;
                        
                        //link it
                        Name pn = dataSource.Names.Values.SingleOrDefault(o => o.ProviderRecordId == taxonNameConcept.ProviderNameId);
                        
                        if (pn != null)
                        {
                            taxonNameConcept.NameId = pn.NameId;
                        }
                        else
                        {
                            //TODO error
                            string msg = "error";
                        }

                        Reference pr = dataSource.References.Values.SingleOrDefault(o => o.ProviderRecordId.Equals(taxonNameConcept.ProviderReferenceId, StringComparison.OrdinalIgnoreCase));
                        if (pr != null) taxonNameConcept.AccordingToReferenceId = pr.ReferenceId;

                        newConcepts.Add(taxonNameConcept);
                    }

                    conceptApplication.FromConceptId = concept.ConceptId;
                    conceptApplication.ToConceptId = taxonNameConcept.ConceptId;
                }
            }
        }

        private void LinkTaxonProperties(DataSource dataSource)
        {
            Parallel.ForEach(dataSource.TaxonProperties.Values.Where(
                p => p.State == Data.Entities.Entity.EntityState.Added || p.State == Data.Entities.Entity.EntityState.Modified),
                prop => LinkTaxonProperty(dataSource, prop)
                );
        }

        private void LinkTaxonProperty(DataSource dataSource, TaxonProperty prop)
        {
            if (prop.ProviderNameId != null && dataSource.Names.ContainsKey(prop.ProviderNameId))
            {
                prop.NameId = dataSource.Names[prop.ProviderNameId].NameId;
            }

            if (prop.ProviderReferenceId != null && dataSource.References.ContainsKey(prop.ProviderReferenceId))
            {
                prop.ReferenceId = dataSource.References[prop.ProviderReferenceId].ReferenceId;
            }

            if (prop.ProviderConceptId != null && dataSource.Concepts.ContainsKey(prop.ProviderConceptId))
            {
                prop.ConceptId = dataSource.Concepts[prop.ProviderConceptId].ConceptId;
            }

        }

        private void LinkAnnotations(DataSource dataSource)
        {
            Parallel.ForEach(dataSource.Annotations.Values.Where(
                p => p.State == Data.Entities.Entity.EntityState.Added || p.State == Data.Entities.Entity.EntityState.Modified),
                ann => LinkAnnotation(dataSource, ann)
                );
        }

        private void LinkAnnotation(DataSource dataSource, Annotation ann)
        {
            if (ann.ProviderNameId != null && dataSource.Names.ContainsKey(ann.ProviderNameId))
            {
                ann.NameId = dataSource.Names[ann.ProviderNameId].NameId;
            }

            if (ann.ProviderReferenceId != null && dataSource.References.ContainsKey(ann.ProviderReferenceId))
            {
                ann.ReferenceId = dataSource.References[ann.ProviderReferenceId].ReferenceId;
            }

            if (ann.ProviderConceptId != null && dataSource.Concepts.ContainsKey(ann.ProviderConceptId))
            {
                ann.ConceptId = dataSource.Concepts[ann.ProviderConceptId].ConceptId;
            }

        }
    }
}

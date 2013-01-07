using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

using NZOR.Data;
using NZOR.Matching;
using NZOR.Data.Entities.Common;
using NZOR.Validation;
using NZOR.Data.DataSets;
using NZOR.Data.Entities.Integration;

namespace NZOR.Integration
{    
    public class IntegrationProcessor
    {
        public List<MatchResult> Results = new List<MatchResult>();
        public Dictionary<Guid, RecordForRefresh> RefreshedRecords = new Dictionary<Guid, RecordForRefresh>();
        public string ConnectionString = null;

        public string StatusText = "";
        public int Progress = 0;

        public Guid IntegrationBatchID = Guid.Empty;

        private int _maxToProcess = -1;
        private int _namesToProcessCount = 0;
        private int _refsToProcessCount = 0;
        private int _conceptsToProcessCount = 0;
        private int _recordsToRefreshCount = 0;
        private int _processedNamesCount = 0;
        private int _processedRefsCount = 0;
        private int _processedConceptsCount = 0;
        private int _refreshedCount = 0;
        private List<NZOR.Admin.Data.Entities.AttachmentPoint> _attachmentPoints = null;
        private Data.Entities.Integration.IntegrationState _integrationState = Data.Entities.Integration.IntegrationState.None;
        
        public event EventHandler ProcessingComplete;
        public event EventHandler<StatusEventArgs> StatusUpdate;

        /// <summary>
        /// </summary>
        public void RunIntegration(XmlDocument config, int maxRecords)
        {
            ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            
            if (IntegrationBatchID == Guid.Empty) IntegrationBatchID = Guid.NewGuid();

            _recordsToRefreshCount = GetRecordsForRefreshCount();
            _namesToProcessCount = GetNamesForIntegrationCount();
            _refsToProcessCount = GetReferencesForIntegrationCount();
            _conceptsToProcessCount = GetConceptsForIntegrationCount();
            Progress = 1; //started

            MatchProcessor.LoadConfig(config);

            NZOR.Admin.Data.Repositories.IProviderRepository pr = new NZOR.Admin.Data.Sql.Repositories.ProviderRepository(ConnectionString);
            _attachmentPoints = pr.GetAllAttachmentPoints();

            _maxToProcess = _recordsToRefreshCount + _namesToProcessCount + _refsToProcessCount + _conceptsToProcessCount;

            //max records?
            if (maxRecords > 0 && maxRecords < _maxToProcess) _maxToProcess = maxRecords;


            //any records to refresh (updates from providers)
            _integrationState = IntegrationState.Refreshing;

            while (_integrationState != IntegrationState.Completed && ProcessNextRefreshRecord())
            {
                _refreshedCount++;
                StatusText = "Refreshed " + _refreshedCount.ToString() + " of " + _recordsToRefreshCount.ToString() + " records.";
                if (StatusUpdate != null) StatusUpdate(null, new StatusEventArgs(StatusText));
                Progress = ((RefreshedRecords.Count + Results.Count) * 100 / (_recordsToRefreshCount + _namesToProcessCount + _refsToProcessCount + _conceptsToProcessCount));

                if (CheckForCompletion())
                {
                    _integrationState = Data.Entities.Integration.IntegrationState.Completed;
                    Progress = 100;
                }
            }

            if (_integrationState != IntegrationState.Completed)
            {
                ProcessReferences();
            }
            
            if (_integrationState != Data.Entities.Integration.IntegrationState.Completed)
            {
                _integrationState = Data.Entities.Integration.IntegrationState.Names;

                while (_integrationState == Data.Entities.Integration.IntegrationState.Names && ProcessNextName())
                {
                    _processedNamesCount += 1;
                    StatusText = "Processed " + _processedNamesCount.ToString() + " of " + _namesToProcessCount.ToString() + " names.";
                    //IntegratorThread.Log.Add(DateTime.Now.ToString() + " : Processing name ID=" + nextName.ToString());
                    if (StatusUpdate != null) StatusUpdate(null, new StatusEventArgs(StatusText));
                    Progress = ((RefreshedRecords.Count + Results.Count) * 100 / (_recordsToRefreshCount + _namesToProcessCount + _refsToProcessCount + _conceptsToProcessCount));

                    if (CheckForCompletion())
                    {
                        _integrationState = Data.Entities.Integration.IntegrationState.Completed;
                        Progress = 100;
                    }
                }
            }
            
            if (_integrationState != Data.Entities.Integration.IntegrationState.Completed)
            {
                _integrationState = Data.Entities.Integration.IntegrationState.Concepts;

                while (_integrationState == Data.Entities.Integration.IntegrationState.Concepts && ProcessNextConcept())
                {
                    _processedConceptsCount += 1;
                    StatusText = "Processed " + _processedConceptsCount.ToString() + " of " + _conceptsToProcessCount.ToString() + " concepts.";
                    if (StatusUpdate != null) StatusUpdate(null, new StatusEventArgs(StatusText));
                    Progress = ((RefreshedRecords.Count + Results.Count) * 100 / (_recordsToRefreshCount + _namesToProcessCount + _refsToProcessCount + _conceptsToProcessCount));

                    if (CheckForCompletion())
                    {
                        _integrationState = Data.Entities.Integration.IntegrationState.Completed;
                        Progress = 100;
                    }
                }
            }

            _integrationState = IntegrationState.Completed;
            Progress = 100;
        }

        public void ProcessReferences()
        {
            _integrationState = Data.Entities.Integration.IntegrationState.References;

            if (_refsToProcessCount == 0) _refsToProcessCount = GetReferencesForIntegrationCount();
            if (_maxToProcess == -1) _maxToProcess = _refsToProcessCount;
            
            while (_integrationState == Data.Entities.Integration.IntegrationState.References && ProcessNextReference())
            {
                _processedRefsCount++;
                StatusText = "Processed " + _processedRefsCount.ToString() + " of " + _refsToProcessCount.ToString() + " references.";
                if (StatusUpdate != null) StatusUpdate(null, new StatusEventArgs(StatusText));
                Progress = ((RefreshedRecords.Count + Results.Count) * 100 / (_recordsToRefreshCount + _namesToProcessCount + _refsToProcessCount + _conceptsToProcessCount));

                if (CheckForCompletion())
                {
                    _integrationState = Data.Entities.Integration.IntegrationState.Completed;
                    Progress = 100;
                }
            }
        }

        private bool ProcessNextRefreshRecord()
        {
            RecordForRefresh rfr = GetNextRecordForRefresh();
            if (rfr != null)
            {
                try
                {
                    if (rfr.RecordType == NZOR.Admin.Data.Entities.NZORRecordType.Reference)
                    {
                        NZOR.Data.Sql.Integration.RefreshConsensusReference(rfr.Id, ConnectionString);
                    }
                    else if (rfr.RecordType == Admin.Data.Entities.NZORRecordType.Name)
                    {
                        Data.Sql.Integration.RefreshConsensusName(rfr.Id, ConnectionString, _attachmentPoints);
                    }
                    else if (rfr.RecordType == Admin.Data.Entities.NZORRecordType.Concept)
                    {
                        NZOR.Data.Repositories.Provider.IConceptRepository pcr = new NZOR.Data.Sql.Repositories.Provider.ConceptRepository(ConnectionString);
                        NZOR.Data.Repositories.Consensus.IConceptRepository ccr = new NZOR.Data.Sql.Repositories.Consensus.ConceptRepository(ConnectionString);

                        List<Data.Entities.Provider.Concept> pcList = pcr.GetProviderConcepts(rfr.Id);
                        Data.Entities.Consensus.Concept cc = ccr.GetConcept(rfr.Id);


                        //is there a parent relationship at all - if not we need to know this further down the line
                        bool hasParentRel = false;
                        Data.Sql.Repositories.Common.LookUpRepository lr = new Data.Sql.Repositories.Common.LookUpRepository(ConnectionString);
                        Data.LookUps.Common.ConceptRelationshipTypeLookUp crtl = new Data.LookUps.Common.ConceptRelationshipTypeLookUp(lr.GetConceptRelationshipTypes());
                        ConceptRelationshipType parRelType = crtl.GetConceptRelationshipType(Data.LookUps.Common.ConceptRelationshipTypeLookUp.IsChildOf);
                        List<Data.Entities.Provider.Concept> provConcepts = pcr.GetProviderConceptsByName(cc.NameId);
                        foreach (Data.Entities.Provider.Concept pc in provConcepts)
                        {
                            foreach (Data.Entities.Provider.ConceptRelationship pccr in pc.ConceptRelationships)
                            {
                                if (pccr.ConceptRelationshipTypeId == parRelType.ConceptRelationshipTypeId && (!pccr.InUse.HasValue || pccr.InUse.Value))
                                {
                                    hasParentRel = true;
                                    break;
                                }
                            }
                        }

                        Data.Sql.Integration.RefreshConsensusConcept(cc, pcList, hasParentRel, ConnectionString, _attachmentPoints);
                        if (cc.State != Data.Entities.Entity.EntityState.Unchanged)
                        {
                            ccr.Save(cc, true);
                        }
                        Data.Sql.Integration.UpdateRelatedConcepts(cc, pcList, ConnectionString, _attachmentPoints);

                        //update the relationships as obtained from the refresh
                        if (cc.ConceptRelationships.Count > 0 || cc.ConceptApplications.Count > 0)
                        {
                            ccr.SaveRelationships(cc);
                        }
                    }
                }
                catch (Exception ex)
                {
                    rfr.Error = "ERROR : Failed to refresh record with ID " + rfr.Id.ToString() + " : " + ex.Message + " : " + ex.StackTrace;                    
                }

                RefreshedRecords.Add(rfr.Id, rfr);
                    
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ProcessNextReference()
        {
            DsIntegrationReference.ProviderReferenceRow nextRef = GetNextReferenceForIntegration();
            if (nextRef != null)
            {
                MatchResult res = MatchProcessor.DoMatchReference(null, nextRef, true, ConnectionString);
                if (res.Matches.Count == 0)
                {
                    //insert
                    res.Status = LinkStatus.Inserted;
                    
                    DataSet newRef = Data.Sql.Integration.AddConsensusReference(ConnectionString, nextRef);

                    nextRef.ConsensusReferenceID = (Guid)newRef.Tables[0].Rows[0]["ReferenceID"];
                    nextRef.MatchPath = res.MatchPath;
                    nextRef.LinkStatus = LinkStatus.Inserted.ToString();

                    res.MatchedReference = nextRef.Citation;
                    res.MatchedId = nextRef.ConsensusReferenceID.ToString();

                    NZOR.Data.Sql.Integration.UpdateProviderReferenceLink(ConnectionString, nextRef, NZOR.Data.LinkStatus.Inserted, nextRef.ConsensusReferenceID, 100, res.MatchPath, IntegrationBatchID);
                }
                else if (res.Matches.Count == 1)
                {                    
                    NZOR.Data.Sql.Integration.UpdateProviderReferenceLink(ConnectionString, nextRef, NZOR.Data.LinkStatus.Matched, res.Matches[0].Id, res.Matches[0].MatchScore, res.MatchPath, IntegrationBatchID);
                    res.Status = NZOR.Data.LinkStatus.Matched;
                    res.MatchedId = res.Matches[0].Id.ToString();
                    res.MatchedReference = nextRef.Citation;

                    NZOR.Data.Sql.Integration.RefreshConsensusReference(res.Matches[0].Id.Value, ConnectionString);
                }
                else
                {
                    res.Status = LinkStatus.Multiple;
                    
                    NZOR.Data.Sql.Integration.UpdateProviderReferenceLink(ConnectionString, nextRef, NZOR.Data.LinkStatus.Multiple, null, 0, res.MatchPath, IntegrationBatchID);
                }
                 
                Results.Add(res);
                
                return true;
            }

            return false;
        }

        private bool ProcessNextConcept()
        {
            DsIntegrationConcept.ProviderConceptRow nextConc = GetNextConceptForIntegration();
            if (nextConc != null)
            {
                MatchResult res = new MatchResult();
                if (nextConc.IsConsensusNameIDNull())
                {
                    res.ProviderRecordId = nextConc.ProviderRecordID;
                    res.Status = LinkStatus.DataFail;
                    res.MatchPath = "ERROR : Concept (id = " + nextConc.ConceptID.ToString() + ") cannot be integrated because the name for this concept is not integrated.";
                    
                    using (SqlConnection cnn = new SqlConnection(ConnectionString))
                    {
                        cnn.Open();
                        Data.Sql.Integration.UpdateProviderConceptLink(cnn, nextConc.ConceptID, NZOR.Data.LinkStatus.DataFail, null, 0, res.MatchPath, IntegrationBatchID);
                    }
                }
                else
                {
                    NZOR.Data.Repositories.Provider.IConceptRepository pcr = new NZOR.Data.Sql.Repositories.Provider.ConceptRepository(ConnectionString);
                    List<NZOR.Data.Entities.Provider.Concept> pcList = pcr.GetConceptsByName(nextConc.NameID);
                    NZOR.Data.Entities.Provider.Concept pc = pcr.GetConcept(nextConc.ConceptID);

                    ProcessConcept(pc, nextConc.ConsensusNameID, pcList);

                    res.MatchedId = pc.ConsensusConceptId.ToString();
                    res.ProviderRecordId = pc.ProviderRecordId;
                    res.Status = (LinkStatus)Enum.Parse(typeof(LinkStatus), pc.LinkStatus);
                }

                Results.Add(res);

                return true;
            }

            return false;
        }

        private bool ProcessNextName()
        {
            bool more = true;

            string fullName = "";
            List<Guid> parConsNameIDs = new List<Guid>();
            int setId = -1;
            Guid nextName = GetNextNameForIntegration(ref fullName, ref parConsNameIDs, ref setId);
            if (nextName != Guid.Empty)
            {
                ConfigSet cs = MatchProcessor.GetMatchSet(setId);
                                
                //validate
                //get the parent name information
                bool fail = false;
                string msg = "";
                DsIntegrationName.ProviderNameRow provName = null;

                try
                {
                    if (setId == -1)
                    {
                        fail = true;
                        msg = "SYSTEM ERROR : No match rule set defined for taxon rank of name " + nextName.ToString();
                    }
                    else
                    {
                        provName = Data.Sql.Integration.GetNameMatchData(ConnectionString, nextName, _attachmentPoints);

                        if (!provName.IsParentNamesNull() && Data.Sql.Integration.GetParentConsensusNameIDs(provName).Count == 0 && provName.TaxonRankSort >= NZOR.Data.LookUps.Common.TaxonRankLookUp.SortOrderGenus)
                        {
                            fail = true;
                            msg = "ERROR : Parent of provider name (id = " + nextName.ToString() + ") has not been integrated.";
                        }
                        else
                        {
                            //validate provider data
                            Integration.IntegrationValidation iv = new IntegrationValidation();
                            ValidationResultLookup valRes = iv.ValidateProviderData(provName);

                            if (!valRes.AllClear())
                            {
                                fail = true;
                                msg = "ERROR : Validation of provider name (id = " + nextName.ToString() + ") failed." + valRes.ErrorMessages();
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    fail = true;
                    msg = ex.Message;
                }

                if (fail)
                {
                    using (SqlConnection cnn = new SqlConnection(ConnectionString))
                    {
                        cnn.Open();
                        Data.Sql.Integration.UpdateProviderNameLink(cnn, nextName, NZOR.Data.LinkStatus.DataFail, null, 0, msg, IntegrationBatchID);
                    }

                    IntegratorThread.Log.Add(DateTime.Now.ToString() + " : " + msg);
                    IntegratorThread.Log.Add(DateTime.Now.ToString() + ": ERROR : Integration failed for name '" + nextName.ToString() + "'.");
                    MatchResult mr = new MatchResult();
                    if (provName != null) mr.ProviderRecordId = provName.ProviderRecordID;
                    mr.MatchPath = msg;
                    mr.Status = LinkStatus.DataFail;

                    Results.Add(mr);
                }
                else
                {
                    SqlConnection cnn = new SqlConnection(ConnectionString);
                    cnn.Open();

                    MatchResult res = MatchProcessor.DoMatch(null, provName, cs.Routines, true, ConnectionString);

                    if (res.Matches.Count == 0)
                    {
                        //insert
                        DataSet newName = NZOR.Data.Sql.Integration.AddConsensusName(ConnectionString, provName);
                        DataRow nameRow = newName.Tables[0].Rows[0];
                        Data.Sql.Integration.UpdateProviderNameLink(cnn, provName.NameID, NZOR.Data.LinkStatus.Inserted, (Guid?)nameRow["NameID"], 100, res.MatchPath, IntegrationBatchID);

                        res.MatchedId = nameRow["NameID"].ToString();
                        res.MatchedName = nameRow["FullName"].ToString();
                        res.Status = NZOR.Data.LinkStatus.Inserted;
                    }
                    else if (res.Matches.Count == 1)
                    {
                        //link 
                        NZOR.Data.Sql.Integration.UpdateProviderNameLink(cnn, provName.NameID, NZOR.Data.LinkStatus.Matched, res.Matches[0].Id, res.Matches[0].MatchScore, res.MatchPath, IntegrationBatchID);
                        res.MatchedId = res.Matches[0].Id.ToString();
                        res.MatchedName = res.Matches[0].DisplayText;
                        res.Status = NZOR.Data.LinkStatus.Matched;
                    }
                    else
                    {
                        //multiple matches
                        NZOR.Data.Sql.Integration.UpdateProviderNameLink(cnn, provName.NameID, NZOR.Data.LinkStatus.Multiple, null, 0, res.MatchPath, IntegrationBatchID);
                        res.Status = NZOR.Data.LinkStatus.Multiple;
                    }
                    Results.Add(res);
                                        
                    cnn.Close();

                    if (res.Matches.Count == 1) //name needs refresh
                    {
                        Data.Sql.Integration.RefreshConsensusName(res.Matches[0].Id.Value, ConnectionString, _attachmentPoints);
                    }

                    if (res.Matches.Count <= 1)
                    {
                        NZOR.Data.Repositories.Provider.INameRepository pnr = new NZOR.Data.Sql.Repositories.Provider.NameRepository(ConnectionString);

                        //do concepts 
                        ProcessNameConcepts(pnr.GetName(provName.NameID), res);

                        //stacked name (after parent cpncepts have been added
                        Data.Sql.Integration.UpdateConsensusStackedNameData(ConnectionString, new Guid(res.MatchedId));

                        //do taxon properties
                        ProcessTaxonProperties(provName.NameID, res);
                    }
                }
            }
            else
            {
                more = false;
            }

            return more;
        }

        public void ProcessTaxonProperties(Guid nameId, MatchResult nameRes)
        {
            NZOR.Data.Repositories.Provider.ITaxonPropertyRepository tpr = new NZOR.Data.Sql.Repositories.Provider.TaxonPropertyRepository(ConnectionString);
            NZOR.Data.Repositories.Consensus.ITaxonPropertyRepository ctpr = new NZOR.Data.Sql.Repositories.Consensus.TaxonPropertyRepository(ConnectionString);

            List<NZOR.Data.Entities.Provider.TaxonProperty> provProps = tpr.GetTaxonPropertiesByName(nameId);

            List<NZOR.Data.Entities.Consensus.TaxonProperty> consProps = NZOR.Data.Sql.Integration.GetConsensusTaxonProperties(provProps, ConnectionString);
            ctpr.TaxonProperties.AddRange(consProps);
            ctpr.Save();
        }

        public void ProcessNameConcepts(Data.Entities.Provider.Name provName, MatchResult nameRes)
        {
            NZOR.Data.Repositories.Consensus.IConceptRepository ccr = new NZOR.Data.Sql.Repositories.Consensus.ConceptRepository(ConnectionString);
            NZOR.Data.Repositories.Provider.IConceptRepository pcr = new NZOR.Data.Sql.Repositories.Provider.ConceptRepository(ConnectionString);
            NZOR.Data.Sql.Repositories.Common.LookUpRepository lr = new Data.Sql.Repositories.Common.LookUpRepository(ConnectionString);
            NZOR.Data.LookUps.Common.ConceptRelationshipTypeLookUp crtl = new Data.LookUps.Common.ConceptRelationshipTypeLookUp(lr.GetConceptRelationshipTypes());
            NZOR.Data.LookUps.Common.NameClassLookUp ncl = new Data.LookUps.Common.NameClassLookUp(lr.GetNameClasses());

            bool parentDefined = false;
            List<Data.Entities.Provider.Concept> allpcList = pcr.GetProviderConceptsByName(new Guid(nameRes.MatchedId));
            foreach (Data.Entities.Provider.Concept pc in allpcList)
            {
                if (pc.GetRelationships(crtl.GetConceptRelationshipType(Data.LookUps.Common.ConceptRelationshipTypeLookUp.IsChildOf).ConceptRelationshipTypeId).Count > 0)
                {
                    parentDefined = true;
                    break;
                }
            }

            List<NZOR.Data.Entities.Provider.Concept> pcList = pcr.GetConceptsByName(provName.NameId);

            if (ncl.GetNameClassById(provName.NameClassId).Name == NZOR.Data.LookUps.Common.NameClassLookUp.ScientificName)
            {
                if (!parentDefined)
                {
                    //need to at least add one for the parent
                    Admin.Data.Sql.Repositories.ProviderRepository pr = new Admin.Data.Sql.Repositories.ProviderRepository(ConnectionString);
                    List<Admin.Data.Entities.AttachmentPoint> attPoints = pr.GetAllAttachmentPoints();

                    NZOR.Data.LookUps.Common.TaxonRankLookUp rl = new Data.LookUps.Common.TaxonRankLookUp(lr.GetTaxonRanks());

                    CalculateNameParentData calcData = new CalculateNameParentData();
                    calcData.ProviderNameRankSort = rl.GetTaxonRank(provName.TaxonRankId).SortOrder.Value;
                    calcData.ProviderNameID = provName.NameId;
                    calcData.AttachmentPoints = attPoints;
                    calcData.DataSourceID = provName.DataSourceId;
                    calcData.ProviderNameParents = new List<Guid>(); //no parents have been defined by prov records
                    calcData.ProviderRecordID = provName.ProviderRecordId;
                    calcData.GoverningCode = provName.GoverningCode;
                    calcData.ProviderNameCanonical = provName.GetNameProperty(Data.LookUps.Common.NamePropertyTypeLookUp.Canonical).Value;
                    calcData.ProviderNameFullName = provName.FullName;
                    calcData.ProviderNameRankID = provName.TaxonRankId;

                    CalculateNameParentResult calcRes = Data.Sql.Integration.CalculateNameParent(ConnectionString, calcData);

                    if (calcRes.ParentID != Guid.Empty)
                    {
                        Data.Entities.Consensus.Concept cc = new Data.Entities.Consensus.Concept();
                        cc.ConceptId = Guid.NewGuid();
                        cc.NameId = new Guid(nameRes.MatchedId);
                        cc.AddedDate = DateTime.Now;
                        cc.State = Data.Entities.Entity.EntityState.Added;

                        //get TO concept
                        Data.Entities.Consensus.Concept toConcept = ccr.GetConcept(calcRes.ParentID, null);

                        if (toConcept == null)
                        {
                            toConcept = new Data.Entities.Consensus.Concept();
                            toConcept.NameId = calcRes.ParentID;
                            toConcept.ConceptId = Guid.NewGuid();
                            toConcept.AddedDate = DateTime.Now;
                            toConcept.State = Data.Entities.Entity.EntityState.Added;

                            ccr.Save(toConcept, true);
                        }

                        Data.Entities.Consensus.ConceptRelationship rel = new Data.Entities.Consensus.ConceptRelationship();
                        rel.ConceptRelationshipId = Guid.NewGuid();
                        rel.AddedDate = DateTime.Now;
                        rel.ConceptRelationshipTypeId = crtl.GetConceptRelationshipType(Data.LookUps.Common.ConceptRelationshipTypeLookUp.IsChildOf).ConceptRelationshipTypeId;
                        rel.FromConceptId = cc.ConceptId;
                        rel.ToConceptId = toConcept.ConceptId;
                        rel.IsActive = true;

                        cc.ConceptRelationships.Add(rel);

                        ccr.Save(cc, true);
                    }
                }
            }

            foreach (NZOR.Data.Entities.Provider.Concept pc in pcList)
            {
                ProcessConcept(pc, new Guid(nameRes.MatchedId), pcList);
            }

        }

        private void ProcessConcept(NZOR.Data.Entities.Provider.Concept pc, Guid consensusNameId, List<NZOR.Data.Entities.Provider.Concept> providerNameConcepts)
        {
            NZOR.Data.Repositories.Consensus.IConceptRepository ccr = new NZOR.Data.Sql.Repositories.Consensus.ConceptRepository(ConnectionString);
            NZOR.Data.Repositories.Provider.IConceptRepository pcr = new NZOR.Data.Sql.Repositories.Provider.ConceptRepository(ConnectionString);
            NZOR.Data.Repositories.Provider.IReferenceRepository prr = new NZOR.Data.Sql.Repositories.Provider.ReferenceRepository(ConnectionString);

            Guid? refId = null;
            if (pc.AccordingToReferenceId.HasValue)
            {
                NZOR.Data.Entities.Provider.Reference provRef = prr.GetReference(pc.AccordingToReferenceId.Value);
                if (!provRef.ConsensusReferenceId.HasValue)
                {
                    pc.LinkStatus = LinkStatus.DataFail.ToString();
                    pc.MatchPath = "ERROR : Failed to integrate concept (id = " + pc.ConceptId.ToString() + ").  Reference for this concept has not been integrated.";
                    pc.ConsensusConceptId = null;
                    return;
                }
                refId = provRef.ConsensusReferenceId.Value;
            }
            NZOR.Data.Entities.Consensus.Concept cc = ccr.GetConcept(consensusNameId, refId);

            if (cc != null)
            {
                pc.ConsensusConceptId = cc.ConceptId;
                pc.LinkStatus = LinkStatus.Matched.ToString();
                pc.MatchScore = 100;
            }
            else
            {
                cc = new Data.Entities.Consensus.Concept();
                cc.ConceptId = Guid.NewGuid();
                cc.NameId = consensusNameId;
                cc.AccordingToReferenceId = refId;
                cc.AddedDate = DateTime.Now;
                cc.State = Data.Entities.Entity.EntityState.Added;

                ccr.Save(cc, true);

                pc.ConsensusConceptId = cc.ConceptId;
                pc.LinkStatus = LinkStatus.Inserted.ToString();                
            }

            pc.ModifiedDate = DateTime.Now;
            pc.State = Data.Entities.Entity.EntityState.Modified;
            pcr.Save(pc, true);

            //get consensus relationships and values
            Data.Sql.Integration.RefreshConsensusConcept(cc, providerNameConcepts, true, ConnectionString, _attachmentPoints);
            Data.Sql.Integration.UpdateRelatedConcepts(cc, providerNameConcepts, ConnectionString, _attachmentPoints);

            //update the relationships as obtained from the refresh
            if (cc.ConceptRelationships.Count > 0 || cc.ConceptApplications.Count > 0)
            {
                ccr.SaveRelationships(cc);
            }
        }

        private bool CheckForCompletion()
        {
            if (_integrationState == Data.Entities.Integration.IntegrationState.Completed)
            {
                return true;
            }

            int prog = 1;
            prog = ((RefreshedRecords.Count + Results.Count * 100) / (_recordsToRefreshCount + _namesToProcessCount + _refsToProcessCount + _conceptsToProcessCount));

            if (prog == 0) prog = 1; //at least to indicate we have started
            if (prog >= 100 || (RefreshedRecords.Count + Results.Count) >= _maxToProcess)
            {
                if (_maxToProcess > (RefreshedRecords.Count + Results.Count))
                {
                    prog = 99; //not 100 % complete until ALL names are done
                }
                else
                {
                    _integrationState = Data.Entities.Integration.IntegrationState.Completed;
                    prog = 100;
                }
            }

            Progress = prog;

            if (_integrationState == Data.Entities.Integration.IntegrationState.Completed && ProcessingComplete != null) ProcessingComplete(null, EventArgs.Empty);

            return (_integrationState == Data.Entities.Integration.IntegrationState.Completed);
        }
        
        private Guid GetNextNameForIntegration(ref string fullName, ref List<Guid> parentConsNameIDs, ref int matchRuleSetId)
        {
            Guid id = Guid.Empty;

            if (_integrationState == Data.Entities.Integration.IntegrationState.Completed) return id;

            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    //cmd.CommandText = "select top 1 n.NameID, n.FullName, pc.ConsensusNameToID, tr.MatchRuleSetID " +
                    //        "from provider.Name n " +
                    //        "inner join dbo.TaxonRank tr on tr.TaxonRankID = n.TaxonRankID " +
                    //        "left join provider.vwConcepts pc on pc.NameID = n.NameID and pc.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' and InUse = 1 " +
                    //        "where n.ConsensusNameID is null and (n.IntegrationBatchID is null or n.IntegrationBatchID <> '" + _thisBatchID.ToString() + "') and " +
                    //        " (n.LinkStatus is null or n.LinkStatus <> 'Integrating') " +
                    //        "order by tr.SortOrder ";

                    cmd.CommandTimeout = 120;
                    cmd.CommandText = @"set concat_null_yields_null off

                        select top 1 pn.NameID, pn.FullName, tr.MatchRuleSetID,
                            c.par as ParentNames 
                        from provider.Name pn
                        inner join dbo.TaxonRank tr on tr.TaxonRankID = pn.TaxonRankID
                        inner join dbo.NameClass nc on nc.NameClassID = pn.NameClassID
                        left join provider.vwConcepts par on par.NameID = pn.NameID and par.ConsensusNameToID is null and par.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155'
                        CROSS APPLY 
                        ( 
                            select (SELECT distinct CONVERT(VARCHAR(38), pc.ConsensusNameToID) + '|' AS [text()] 
                            FROM provider.vwConcepts pc 
                            where pc.NameID = pn.NameID 
		                        and pc.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155'
                            FOR XML PATH(''))
                        ) C (par)
                        where pn.ConsensusNameID is null and (pn.IntegrationBatchID is null or pn.IntegrationBatchID <> '" + IntegrationBatchID.ToString() + @"')  
                        order by nc.Name, tr.SortOrder;";
                    //and (par.NameID is null or par.ConsensusNameToID is not null)
                    DataSet res = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(res);
                    if (res != null && res.Tables.Count > 0 && res.Tables[0].Rows.Count > 0)
                    {
                        id = (Guid)res.Tables[0].Rows[0]["NameId"];
                        fullName = res.Tables[0].Rows[0]["FullName"].ToString();
                        matchRuleSetId = (int)res.Tables[0].Rows[0]["MatchRuleSetID"];

                        if (!res.Tables[0].Rows[0].IsNull("ParentNames"))
                        {
                            string[] parents = res.Tables[0].Rows[0]["ParentNames"].ToString().Split('|');
                            foreach (string par in parents)
                            {
                                if (par != string.Empty) parentConsNameIDs.Add(new Guid(par));
                            }
                        }

                        //if (!res.Tables[0].Rows[0].IsNull("ConsensusNameToID")) parentConsNameIDs.Add((Guid)res.Tables[0].Rows[0]["ConsensusNameToID"]);
                    }

                }

                if (id != Guid.Empty)
                {
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = "update provider.Name set IntegrationBatchId = '" + IntegrationBatchID.ToString() + "' where NameID = '" + id.ToString() + "'";
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            return id;
        }

        private RecordForRefresh GetNextRecordForRefresh()
        {
            RecordForRefresh rfr = null;

            if (_integrationState == Data.Entities.Integration.IntegrationState.Completed) return rfr;

            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = @"select top 1 r.ReferenceID
                                from provider.Reference pr 
                                inner join consensus.Reference r on r.ReferenceID = pr.ConsensusReferenceID
                                where ISNULL(pr.ProviderModifiedDate, pr.ProviderCreatedDate) > ISNULL(r.ModifiedDate, r.AddedDate)";

                    object val = cmd.ExecuteScalar();
                    if (val != DBNull.Value && val != null)
                    {
                        Guid id = (Guid)val;
                        rfr = new RecordForRefresh();
                        rfr.Id = id;
                        rfr.RecordType = Admin.Data.Entities.NZORRecordType.Reference;
                    }
                }

                if (rfr == null)
                {
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = @"select top 1 n.NameID
                                from provider.Name pn
                                inner join consensus.Name n on n.NameID = pn.ConsensusNameID
                                where isnull(pn.ProviderModifiedDate, pn.ProviderCreatedDate) > ISNULL(n.ModifiedDate, n.AddedDate)";

                        object val = cmd.ExecuteScalar();
                        if (val != DBNull.Value && val != null)
                        {
                            Guid id = (Guid)val;
                            rfr = new RecordForRefresh();
                            rfr.Id = id;
                            rfr.RecordType = Admin.Data.Entities.NZORRecordType.Name;
                        }
                    }
                }

                if (rfr == null)
                {
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = @"select top 1 c.conceptid
                                from provider.Concept pc 
                                inner join consensus.Concept c on c.ConceptID = pc.ConsensusConceptID
                                where ISNULL(pc.ProviderModifiedDate, pc.ProviderCreatedDate) > ISNULL(c.ModifiedDate, c.AddedDate)";

                        object val = cmd.ExecuteScalar();
                        if (val != DBNull.Value && val != null)
                        {
                            Guid id = (Guid)val;
                            rfr = new RecordForRefresh();
                            rfr.Id = id;
                            rfr.RecordType = Admin.Data.Entities.NZORRecordType.Concept;
                        }
                    }
                }
            }

            return rfr;
        }

        private DsIntegrationReference.ProviderReferenceRow GetNextReferenceForIntegration()
        {
            DsIntegrationReference.ProviderReferenceRow pr = null;

            if (_integrationState == Data.Entities.Integration.IntegrationState.Completed) return pr;

            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = @"select top 1 r.ReferenceID,
	                        r.ReferenceTypeID,
	                        r.DataSourceID,
	                        r.ConsensusReferenceID,
	                        r.LinkStatus,
	                        r.MatchScore,
	                        rp.Value as Citation,
	                        r.ProviderRecordID,
	                        props.val as Properties
                        from provider.Reference r
                        left join provider.ReferenceProperty rp on rp.ReferenceID = r.ReferenceID and rp.ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07' --citation
                        cross apply
                        (
                            select (SELECT distinct '[' + CONVERT(VARCHAR(38), rp.ReferencePropertyTypeID) + ':' + rp.Value + '],' AS [text()] 
                            FROM provider.ReferenceProperty rp 
                            where rp.ReferenceID = r.ReferenceID
                            FOR XML PATH(''))
                        ) props(val)
                        where r.ConsensusReferenceID is null and (r.IntegrationBatchId is null or r.IntegrationBatchId <> '" + IntegrationBatchID.ToString() + "')";

                    DsIntegrationReference ds = new DsIntegrationReference();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.TableMappings.Add("Table", ds.ProviderReference.TableName);
                    da.Fill(ds);
                    if (ds != null && ds.ProviderReference.Count > 0)
                    {
                        pr = ds.ProviderReference[0];
                    }

                }

                if (pr != null)
                {
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = "update provider.Reference set IntegrationBatchId = '" + IntegrationBatchID.ToString() + "' where ReferenceID = '" + pr.ReferenceID.ToString() + "'";
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            return pr;
        }

        private DsIntegrationConcept.ProviderConceptRow GetNextConceptForIntegration()
        {
            DsIntegrationConcept.ProviderConceptRow pc = null;

            if (_integrationState == Data.Entities.Integration.IntegrationState.Completed) return pc;

            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = @"select top 1 c.ConceptID,
	                        c.NameID,
                            c.AccordingToReferenceID,
                            n.ConsensusNameID,
                            c.ProviderRecordID
                        from provider.Concept c
                        inner join provider.Name n on n.NameID = c.NameID
                        where c.ConsensusConceptID is null 
                            and (c.IntegrationBatchId is null or c.IntegrationBatchID <> '" + IntegrationBatchID.ToString() + "')";

                    DsIntegrationConcept ds = new DsIntegrationConcept();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.TableMappings.Add("Table", ds.ProviderConcept.TableName);
                    da.Fill(ds);
                    if (ds != null && ds.ProviderConcept.Count > 0)
                    {
                        pc = ds.ProviderConcept[0];
                    }
                }

                if (pc != null)
                {
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = "update provider.Concept set IntegrationBatchId = '" + IntegrationBatchID.ToString() + "' where ConceptID = '" + pc.ConceptID.ToString() + "'";
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            return pc;
        }



        private int GetRecordsForRefreshCount()
        {
            int cnt = 0;

            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandTimeout = 1000;
                    cmd.CommandText = @"
                            select count(distinct n.NameID)
                            from provider.Name pn
                            inner join consensus.Name n on n.NameID = pn.ConsensusNameID
                            where isnull(pn.ProviderModifiedDate, pn.ProviderCreatedDate) > ISNULL(n.ModifiedDate, n.AddedDate);

                            select count(distinct c.conceptid)
                            from provider.Concept pc 
                            inner join consensus.Concept c on c.ConceptID = pc.ConsensusConceptID
                            where ISNULL(pc.ProviderModifiedDate, pc.ProviderCreatedDate) > ISNULL(c.ModifiedDate, c.AddedDate);

                            select count(distinct r.referenceid)
                            from provider.Reference pr 
                            inner join consensus.Reference r on r.ReferenceID = pr.ConsensusReferenceID
                            where ISNULL(pr.ProviderModifiedDate, pr.ProviderCreatedDate) > ISNULL(r.ModifiedDate, r.AddedDate);";

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    if (ds != null && ds.Tables.Count == 3)
                    {
                        cnt = (int)ds.Tables[0].Rows[0][0];
                        cnt += (int)ds.Tables[1].Rows[0][0];
                        cnt += (int)ds.Tables[2].Rows[0][0];
                    }
                }
            }

            return cnt;
        }

        private int GetNamesForIntegrationCount()
        {
            int cnt = 0;

            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "select count(nameid) from provider.Name where ConsensusNameId is null";

                    object val = cmd.ExecuteScalar();
                    if (val != DBNull.Value) cnt = (int)val;
                }
            }

            return cnt;
        }
        
        private int GetConceptsForIntegrationCount()
        {
            int cnt = 0;

            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "select count(conceptid) from provider.Concept where consensusconceptid is null";
                    
                    object val = cmd.ExecuteScalar();
                    if (val != DBNull.Value) cnt = (int)val;
                }
            }

            return cnt;
        }

        private int GetReferencesForIntegrationCount()
        {
            int cnt = 0;

            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "select count(referenceid) from provider.Reference where ConsensusReferenceId is null";

                    object val = cmd.ExecuteScalar();
                    if (val != DBNull.Value) cnt = (int)val;
                }
            }

            return cnt;
        }

        
    }
}

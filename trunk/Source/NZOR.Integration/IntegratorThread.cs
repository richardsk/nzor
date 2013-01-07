using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using System.Threading;

using NZOR.Data;
using NZOR.Data.DataSets;
using NZOR.Data.Entities.Common;
using NZOR.Data.Entities.Integration;
using NZOR.Matching;
using NZOR.Validation;

namespace NZOR.Integration
{
    public class IntegratorThread
    {
        public enum ProcessingState
        {
            None,
            References,
            Names,
            Finished
        }

        private SqlConnection _cnn = null;
        private List<IntegrationData> _data = new List<IntegrationData>();
        private Dictionary<Guid, MatchResult> _nameResults = new Dictionary<Guid, MatchResult>();
        private Dictionary<Guid, MatchResult> _refResults = new Dictionary<Guid, MatchResult>();
        private Dictionary<Guid, IntegrationData> _processedNames = new Dictionary<Guid, IntegrationData>();
        private bool _requireMutex = false;
        private Dictionary<Guid, ValidationResultLookup> _nameValidations = new Dictionary<Guid, ValidationResultLookup>();
        private Guid _integrationBatchId = Guid.Empty;
        private int _maxRecords = -1;

        public delegate void NameProcessed(IntegratorThread it, IntegrationData intData, MatchResult result);
        public NameProcessed NameProcessedCallback;

        public delegate void ReferenceProcessed(IntegratorThread it, MatchResult result);
        public ReferenceProcessed ReferenceProcessedCallback;

        public static List<String> Log = new List<string>();

        public Mutex ThreadDataMutex = new Mutex();
        public MatchData MatchData = null;
        public ProcessingState State = ProcessingState.None;

        public bool StopThread = false;
        
        public IntegratorThread(MatchData data, bool requireMutex, Guid integrationBatchId, int maxRecords)
        {
            this.MatchData = data;
            _integrationBatchId = integrationBatchId;
            _requireMutex = requireMutex;
           _maxRecords = maxRecords;
            
        }

        public List<IntegrationData> NameData
        {
            get
            {
                return _data;
            }
        }
        
        public void AddNameData(IntegrationData data)
        {
            if (_requireMutex) ThreadDataMutex.WaitOne();

            if (State == ProcessingState.Finished) throw new Exception("Thread is complete");

            if (!_processedNames.ContainsKey(data.NameID)) //already processed ???
                _data.Add(data);
            else
            {
                int i = 0;
                i += 1;
            }

            if (_requireMutex) ThreadDataMutex.ReleaseMutex();
        }

        public MatchResult NameResult(Guid provNameId)
        {
            MatchResult res = _nameResults[provNameId];
            return res;
        }

        public MatchResult ReferenceResult(Guid provRefId)
        {
            MatchResult res = _refResults[provRefId];
            return res;
        }

        public IntegrationData GetProcessedNameData(Guid provNameID)
        {
            IntegrationData id = _processedNames[provNameID];
            return id;
        }
        
        public void ProcessReferences()
        {            
            if (MatchData.ReferencesIntegrationSet == null) return;

            State = ProcessingState.References;

            foreach (DsIntegrationReference.ProviderReferenceRow nextRef in MatchData.ReferencesIntegrationSet.ProviderReference)
            {
                if (nextRef.IsConsensusReferenceIDNull() && nextRef["IntegrationBatchID"].ToString() != _integrationBatchId.ToString())
                {
                    if (nextRef.IsLinkStatusNull() || (
                        nextRef.LinkStatus != Data.LinkStatus.Integrating.ToString() &&
                        nextRef.LinkStatus != Data.LinkStatus.Discarded.ToString() &&
                        nextRef.LinkStatus != Data.LinkStatus.Matched.ToString() &&
                        nextRef.LinkStatus != Data.LinkStatus.Inserted.ToString()))
                    {
                        nextRef.LinkStatus = LinkStatus.Integrating.ToString();
                        nextRef.IntegrationBatchID = _integrationBatchId;

                        MatchResult res = MatchProcessor.DoMatchReference(MatchData, nextRef, false, string.Empty);
                        if (res.Matches.Count == 0)
                        {
                            //insert
                            res.Status = LinkStatus.Inserted;

                            Guid newId = Guid.NewGuid();

                            MatchData.GetProviderDataLock();

                            nextRef.ConsensusReferenceID = newId;
                            nextRef.MatchPath = res.MatchPath;
                            nextRef.LinkStatus = LinkStatus.Inserted.ToString();

                            MatchData.ReleaseProviderDataLock();

                            MatchData.GetConsensusDataLock();

                            MatchData.AllData.ConsensusData.ConsensusReference.AddConsensusReferenceRow(newId, nextRef.ReferenceTypeID, nextRef["Citation"].ToString(), nextRef["Properties"].ToString());

                            NZOR.Data.Sql.Integration.RefreshConsensusReferenceData(newId, MatchData.ReferencesIntegrationSet, MatchData.AllData.ConsensusData);

                            res.MatchedReference = nextRef.Citation;
                            res.MatchedId = newId.ToString();

                            MatchData.ReleaseConsensusDataLock();
                        }
                        else if (res.Matches.Count == 1)
                        {
                            res.Status = LinkStatus.Matched;
                            res.MatchedId = res.Matches[0].Id.Value.ToString();
                            res.MatchedReference = res.Matches[0].DisplayText;

                            MatchData.GetProviderDataLock();

                            nextRef.ConsensusReferenceID = res.Matches[0].Id.Value;
                            nextRef.MatchPath = res.MatchPath;
                            nextRef.LinkStatus = LinkStatus.Matched.ToString();
                            nextRef.MatchScore = res.Matches[0].MatchScore;

                            MatchData.ReleaseProviderDataLock();

                            MatchData.GetConsensusDataLock();

                            NZOR.Data.Sql.Integration.RefreshConsensusReferenceData(nextRef.ConsensusReferenceID, MatchData.ReferencesIntegrationSet, MatchData.AllData.ConsensusData);

                            MatchData.ReleaseConsensusDataLock();
                        }
                        else
                        {
                            res.Status = LinkStatus.Multiple;

                            MatchData.GetProviderDataLock();

                            nextRef.LinkStatus = LinkStatus.Multiple.ToString();
                            nextRef["ConsensusReferenceID"] = DBNull.Value;
                            nextRef.MatchPath = res.MatchPath;
                            nextRef["MatchScore"] = DBNull.Value;

                            MatchData.ReleaseProviderDataLock();
                        }

                        _refResults.Add(nextRef.ReferenceID, res);

                        if (_maxRecords != -1 && _refResults.Count > _maxRecords)
                        {
                            State = ProcessingState.Finished; 
                        }

                        if (ReferenceProcessedCallback != null)
                        {
                            ReferenceProcessedCallback(this, res);
                        }

                        if (State != ProcessingState.Finished && StopThread) State = ProcessingState.Finished;
                    }

                    if (State == ProcessingState.Finished) break;
                }
            }
        }

        /// <summary>
        /// Should be called to process all references and names consecutively.
        /// </summary>
        /// <param name="stateInfo"></param>
        public void ProcessAllData(Object stateInfo)
        {
            ProcessReferences();

            if (State != ProcessingState.Finished) ProcessAllNames(null);

            State = ProcessingState.Finished;
        }
                
        /// <summary>
        /// Should be called when all names can be processed singly, consecutively.
        /// </summary>
        public void ProcessAllNames(Object stateInfo)
        {
            State = ProcessingState.Names;

            int index = 0;

            DsIntegrationName.ProviderNameRow nextName = GetNextNameForProcessing();

            while (nextName != null && State != ProcessingState.Finished)
            {
                IntegratorThread.Log.Add(DateTime.Now.ToString() + " : Processing name ID=" + nextName.NameID.ToString());

                index++;

                MatchResult mr = new MatchResult();

                ConfigSet cs = MatchProcessor.GetMatchSet(nextName.MatchRuleSetID);
                List<Guid> parentConsNameIDs = new List<Guid>();

                if (nextName.HasClassification)
                {
                    Data.Sql.Integration.GetParentData(nextName, MatchData.AllData);
                    parentConsNameIDs = Data.Sql.Integration.GetParentConsensusNameIDs(nextName);
                }

                
                IntegrationData data = new IntegrationData(nextName.NameID, nextName.FullName, parentConsNameIDs, cs, false, null, _integrationBatchId, null);

                //validate
                //get the parent name information
                bool fail = false;
                string msg = "";

                MatchData.GetAllDataLock();
                try
                {
                    if (nextName.IsMatchRuleSetIDNull())
                    {
                        fail = true;
                        msg = "SYSTEM ERROR : No match rule set defined for taxon rank '" + nextName.TaxonRank + "', " + nextName.TaxonRankID.ToString();
                    }
                    else
                    {                        
                        //validate provider data
                        ValidationResultLookup valRes = null;
                        if (_nameValidations.ContainsKey(nextName.NameID)) valRes = _nameValidations[nextName.NameID];
                        if (valRes == null)
                        {
                            Integration.IntegrationValidation iv = new IntegrationValidation();
                            valRes = iv.ValidateProviderData(nextName);
                        }
                        if (!valRes.AllClear())
                        {
                            fail = true;
                            msg = "ERROR : Validation of provider name (id = " + nextName["NameID"].ToString() + ") failed." + valRes.ErrorMessages();
                        }

                    }
                }
                catch (Exception ex)
                {
                    fail = true;
                    msg = ex.Message;
                }
                MatchData.ReleaseAllDataLock();

                if (fail)
                {
                    MatchData.GetProviderDataLock();

                    nextName.LinkStatus = LinkStatus.DataFail.ToString();
                    nextName.IntegrationBatchID = _integrationBatchId;
                    nextName.MatchPath = msg;
                    IntegratorThread.Log.Add(DateTime.Now.ToString() + " : " + msg);
                    IntegratorThread.Log.Add(DateTime.Now.ToString() + ": ERROR : Integration failed for name '" + nextName.NameID.ToString() + "', " + nextName.FullName + ".");

                    mr.ProviderRecordId = nextName.ProviderRecordID;
                    mr.MatchPath = msg;
                    mr.Status = LinkStatus.DataFail;
                    _nameResults.Add(nextName.NameID, mr);

                    MatchData.ReleaseProviderDataLock();
                }
                else
                {
                    mr = ProcessName(data);
                }

                nextName = GetNextNameForProcessing();
                if (nextName == null)
                {
                    State = ProcessingState.Finished;
                }

                if (_maxRecords != -1 && _nameResults.Count + _refResults.Count > _maxRecords)
                {
                    State = ProcessingState.Finished;
                }

                if (NameProcessedCallback != null)
                {
                    NameProcessedCallback(this, data, mr);
                }

                if (State != ProcessingState.Finished && StopThread) State = ProcessingState.Finished;

                if (State != ProcessingState.Finished) State = ProcessingState.None;
            }
        }

        private DsIntegrationName.ProviderNameRow GetNextNameForProcessing()
        {
            DsIntegrationName.ProviderNameRow pn = null;

            foreach (DsIntegrationName.ProviderNameRow nm in MatchData.NameIntegrationSet.ProviderName)
            {
                if (nm.IsConsensusNameIDNull() && nm["IntegrationBatchID"].ToString() != _integrationBatchId.ToString())
                {
                    if (nm.IsLinkStatusNull() || (
                        nm.LinkStatus != Data.LinkStatus.Integrating.ToString() &&
                        nm.LinkStatus != Data.LinkStatus.Discarded.ToString() &&
                        nm.LinkStatus != Data.LinkStatus.Matched.ToString() &&
                        nm.LinkStatus != Data.LinkStatus.Inserted.ToString()))
                    {
                        pn = nm;
                        pn.LinkStatus = "Integrating";
                        break;
                    }
                }
            }

            return pn;
        }

        public void ProcessNameProc(Object stateInfo)
        {
            State = ProcessingState.Names;

            try
            {
                //get next data to use
                while (_data.Count > 0)
                {
                    IntegrationData data = null;
                    if (_requireMutex) ThreadDataMutex.WaitOne();

                    data = _data[0];
                    _processedNames.Add(data.NameID, data);
                    _data.Remove(data);

                    if (_requireMutex) ThreadDataMutex.ReleaseMutex();

                    MatchResult result = ProcessName(data);
                    
                    if (_data.Count == 0) State = ProcessingState.Finished;
                    
                    if (NameProcessedCallback != null)
                    {
                        NameProcessedCallback(this, data, result);
                    }
                }
            }
            catch (Exception ex)
            {
                MatchResult res = new MatchResult();
                res.Error = DateTime.Now.ToString() + ": ERROR : Integration failed : " + ex.Message + " : " + ex.StackTrace;

                if (NameProcessedCallback != null)
                {
                    NameProcessedCallback(this, null, res);
                }
            }

            if (State != ProcessingState.Finished) State = ProcessingState.None;
        }

        private MatchResult ProcessName(IntegrationData data)
        {
            MatchResult result = new MatchResult();

            if (data.Config != null)
            {

                if (data.UseDB)
                {
                    _cnn = new SqlConnection(data.DBCnnStr);
                    _cnn.Open();

                    MatchResult res = MatchProcessor.DoMatch(MatchData, data.ProviderName, data.Config.Routines, data.UseDB, data.DBCnnStr);

                    if (res.Matches.Count == 0)
                    {
                        //insert
                        DataSet newName = NZOR.Data.Sql.Integration.AddConsensusName(data.DBCnnStr, data.ProviderName);
                        DataRow nameRow = newName.Tables[0].Rows[0];
                        Data.Sql.Integration.UpdateProviderNameLink(_cnn, data.NameID, NZOR.Data.LinkStatus.Inserted, (Guid?)nameRow["NameID"], 100, res.MatchPath, data.IntegrationBatchID);

                        res.MatchedId = nameRow["NameID"].ToString();
                        res.MatchedName = nameRow["FullName"].ToString();
                        res.Status = NZOR.Data.LinkStatus.Inserted;
                    }
                    else if (res.Matches.Count == 1)
                    {
                        //link 
                        NZOR.Data.Sql.Integration.UpdateProviderNameLink(_cnn, data.NameID, NZOR.Data.LinkStatus.Matched, res.Matches[0].Id, res.Matches[0].MatchScore, res.MatchPath, data.IntegrationBatchID);
                        res.MatchedId = res.Matches[0].Id.ToString();
                        res.MatchedName = res.Matches[0].DisplayText;
                        res.Status = NZOR.Data.LinkStatus.Matched;
                    }
                    else
                    {
                        //multiple matches
                        NZOR.Data.Sql.Integration.UpdateProviderNameLink(_cnn, data.NameID, NZOR.Data.LinkStatus.Multiple, null, 0, res.MatchPath, data.IntegrationBatchID);
                        res.Status = NZOR.Data.LinkStatus.Multiple;
                    }

                    result = res;
                    _nameResults.Add(data.NameID, res);

                    _cnn.Close();

                    if (res.Matches.Count == 1) //name needs refresh
                    {
                        Data.Sql.Integration.RefreshConsensusName(res.Matches[0].Id.Value, data.DBCnnStr, data.AttachmentPoints);
                    }
                    
                    if (res.Matches.Count <= 1)
                    {
                        //do concepts 
                        ProcessNameConcepts(data, res);

                        //stacked name (after parent cpncepts have been added
                        Data.Sql.Integration.UpdateConsensusStackedNameData(data.DBCnnStr, new Guid(res.MatchedId));

                        //do taxon properties
                        ProcessTaxonProperties(data, res);
                    }

                }
                else
                {
                    //non DB version 
                    DsIntegrationName.ProviderNameRow provName = null;

                    MatchData.GetProviderDataLock();
                    provName = MatchData.NameIntegrationSet.ProviderName.FindByNameID(data.NameID);
                    if (provName != null) provName.IntegrationBatchID = data.IntegrationBatchID; //processed
                    MatchData.ReleaseProviderDataLock();

                    if (provName != null)
                    {
                        MatchResult res = MatchProcessor.DoMatch(MatchData, provName, data.Config.Routines, false, null);

                        if (res.Matches.Count == 0)
                        {
                            //insert
                            List<Guid> parConsNameIds = Data.Sql.Integration.GetParentConsensusNameIDs(provName);
                            //List<Guid> parentIds = Data.Sql.Integration.GetParentNameIDs(provName);

                            if (parConsNameIds.Count == 0 && provName.HasClassification) //names with no classification (vernaculars?) do not need a parent
                            {
                                //dont know where to put it
                                res.Status = LinkStatus.DataFail;

                                MatchData.GetProviderDataLock();

                                provName.LinkStatus = LinkStatus.DataFail.ToString();
                                provName["ConsensusNameID"] = DBNull.Value;
                                provName.MatchPath = res.MatchPath;
                                provName["MatchScore"] = DBNull.Value;

                                MatchData.ReleaseProviderDataLock();

                                Log.Add(DateTime.Now.ToString() + ": ERROR : Integration failed for name '" + provName.NameID.ToString() + "', " + provName.FullName + ".  Not enough parent taxon information.");
                            }
                            else if (parConsNameIds.Count > 1)
                            {
                                //too many parent concepts and none are "inUse"
                                res.Status = LinkStatus.DataFail;

                                MatchData.GetProviderDataLock();

                                provName.LinkStatus = LinkStatus.DataFail.ToString();
                                provName["ConsensusNameID"] = DBNull.Value;
                                provName.MatchPath = res.MatchPath;
                                provName["MatchScore"] = DBNull.Value;

                                MatchData.ReleaseProviderDataLock();

                                Log.Add(DateTime.Now.ToString() + ": ERROR : Integration failed for name '" + provName.NameID.ToString() + "', " + provName.FullName + ".  Not possible to determine parent classification due to multiple parent concepts.");
                            }
                            else
                            {
                                res.Status = LinkStatus.Inserted;

                                Guid newId = Guid.NewGuid();
                                
                                MatchData.GetAllDataLock();

                                //basionym
                                object basID = DBNull.Value;
                                if (!provName.IsBasionymIDNull())
                                {
                                    DsIntegrationName.ProviderNameRow pr = MatchData.NameIntegrationSet.ProviderName.FindByNameID(provName.BasionymID);
                                    if (pr != null) basID = pr["ConsensusNameID"];
                                }

                                String pref = Data.Sql.Integration.GetPreferredConsensusName(provName);
                                Guid prefId = Data.Sql.Integration.GetPreferredConsensusNameID(provName);

                                //parent details
                                string parent = "";
                                string parentIds = "";
                                Guid parentId = Guid.Empty;
                                if (provName.HasClassification)
                                {
                                    DsConsensusData.ConsensusNameRow parRow = MatchData.AllData.ConsensusData.ConsensusName.FindByNameID(parConsNameIds[0]);
                                    parent = parRow.FullName;
                                    parentIds = "[" + parConsNameIds[0].ToString() + ":" + parRow.TaxonRank + "]";
                                    parentId = parConsNameIds[0];
                                }

                                provName.ConsensusNameID = newId;
                                provName.MatchPath = res.MatchPath;
                                provName.LinkStatus = LinkStatus.Inserted.ToString();


                                MatchData.AllData.ConsensusData.ConsensusName.Rows.Add(newId, provName.FullName, provName.NameClassID, provName.NameClass, provName.TaxonRankID, provName.TaxonRank,
                                    provName.TaxonRankSort, provName["Authors"], provName["GoverningCode"], provName["Canonical"], provName["YearOfPublication"], basID, provName["Basionym"], provName["BasionymAuthors"],
                                    provName["CombinationAuthors"], provName["MicroReference"], provName["PublishedIn"], parentId, parentIds, parent,
                                    prefId, pref, provName["Country"], provName["Language"], provName["Region"]);

                                NZOR.Data.Sql.Integration.RefreshConsensusNameData(newId, new List<DsIntegrationName.ProviderNameRow>(){provName}, MatchData.AllData.ConsensusData);
                                
                                //update children names to reflect this linkage
                                if (provName.HasClassification) NZOR.Data.Sql.Integration.UpdateChildrenParentLinks(provName, MatchData.AllData);

                                MatchData.ReleaseAllDataLock();

                                res.MatchedName = provName.FullName;
                                res.MatchedId = newId.ToString();

                            }
                        }
                        else if (res.Matches.Count == 1)
                        {
                            res.Status = LinkStatus.Matched;
                            res.MatchedId = res.Matches[0].Id.Value.ToString();
                            res.MatchedName = res.Matches[0].DisplayText;

                            MatchData.GetAllDataLock();

                            provName.ConsensusNameID = res.Matches[0].Id.Value;
                            provName.MatchPath = res.MatchPath;
                            provName.LinkStatus = LinkStatus.Matched.ToString();
                            provName.MatchScore = res.Matches[0].MatchScore;
                            
                            List<DsIntegrationName.ProviderNameRow> pnList = MatchData.AllData.GetProviderNames(provName.ConsensusNameID);
                            NZOR.Data.Sql.Integration.RefreshConsensusNameData(provName.ConsensusNameID, pnList, MatchData.AllData.ConsensusData);

                            //update children names to reflect this linkage
                            NZOR.Data.Sql.Integration.UpdateChildrenParentLinks(provName, MatchData.AllData);

                            MatchData.ReleaseAllDataLock();
                        }
                        else
                        {
                            res.Status = LinkStatus.Multiple;

                            MatchData.GetProviderDataLock();

                            provName.LinkStatus = LinkStatus.Multiple.ToString();
                            provName["ConsensusNameID"] = DBNull.Value;
                            provName.MatchPath = res.MatchPath;
                            provName["MatchScore"] = DBNull.Value;

                            MatchData.ReleaseProviderDataLock();
                        }

                        result = res;
                        _nameResults.Add(data.NameID, res);

                    }
                }
            }

            return result;
        }

        public void ProcessTaxonProperties(IntegrationData data, MatchResult nameRes)
        {
            NZOR.Data.Repositories.Provider.ITaxonPropertyRepository tpr = new NZOR.Data.Sql.Repositories.Provider.TaxonPropertyRepository(data.DBCnnStr);
            NZOR.Data.Repositories.Consensus.ITaxonPropertyRepository ctpr = new NZOR.Data.Sql.Repositories.Consensus.TaxonPropertyRepository(data.DBCnnStr);

            List<NZOR.Data.Entities.Provider.TaxonProperty> provProps = tpr.GetTaxonPropertiesByName(data.NameID);

            List<NZOR.Data.Entities.Consensus.TaxonProperty> consProps = NZOR.Data.Sql.Integration.GetConsensusTaxonProperties(provProps, data.DBCnnStr);
            ctpr.TaxonProperties.AddRange(consProps);
            ctpr.Save();
        }

        public void ProcessNameConcepts(IntegrationData data, MatchResult nameRes)
        {
            NZOR.Data.Repositories.Provider.IConceptRepository pcr = new NZOR.Data.Sql.Repositories.Provider.ConceptRepository(data.DBCnnStr);
            NZOR.Data.Repositories.Provider.IReferenceRepository prr = new NZOR.Data.Sql.Repositories.Provider.ReferenceRepository(data.DBCnnStr);
            NZOR.Data.Repositories.Consensus.IConceptRepository ccr = new NZOR.Data.Sql.Repositories.Consensus.ConceptRepository(data.DBCnnStr);

            List<NZOR.Data.Entities.Provider.Concept> pcList = pcr.GetConceptsByName(data.NameID);
            
            if (pcList.Count == 0)
            {
                //need to at least add one for the parent
                Admin.Data.Sql.Repositories.ProviderRepository pr = new Admin.Data.Sql.Repositories.ProviderRepository(data.DBCnnStr);
                List<Admin.Data.Entities.AttachmentPoint> attPoints = pr.GetAllAttachmentPoints();
                
                NZOR.Data.Sql.Repositories.Common.LookUpRepository lr = new Data.Sql.Repositories.Common.LookUpRepository(data.DBCnnStr);
                NZOR.Data.LookUps.Common.TaxonRankLookUp rl = new Data.LookUps.Common.TaxonRankLookUp(lr.GetTaxonRanks());

                CalculateNameParentData calcData = new CalculateNameParentData();
                calcData.ProviderNameRankSort = rl.GetTaxonRank(data.ProviderName.TaxonRankID).SortOrder.Value;
                calcData.ProviderNameID = data.ProviderName.NameID;
                calcData.AttachmentPoints = attPoints;
                calcData.DataSourceID = data.ProviderName.DataSourceID;
                calcData.ProviderNameParents = new List<Guid>(); //no parents have been defined by prov records
                calcData.ProviderRecordID = data.ProviderName.ProviderRecordID;
                calcData.GoverningCode = data.ProviderName.GoverningCode;
                calcData.ProviderNameCanonical = data.ProviderName.Canonical;
                calcData.ProviderNameFullName = data.ProviderName.FullName;
                calcData.ProviderNameRankID = data.ProviderName.TaxonRankID;

                CalculateNameParentResult calcRes = Data.Sql.Integration.CalculateNameParent(data.DBCnnStr, calcData);

                if (calcRes.ParentID != Guid.Empty)
                {
                    Data.Entities.Consensus.Concept cc = new Data.Entities.Consensus.Concept();
                    cc.ConceptId = Guid.NewGuid();
                    cc.NameId = data.NameID;
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

                    Data.LookUps.Common.ConceptRelationshipTypeLookUp crtl = new Data.LookUps.Common.ConceptRelationshipTypeLookUp(lr.GetConceptRelationshipTypes());

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

            foreach (NZOR.Data.Entities.Provider.Concept pc in pcList)
            {
                Guid? refId = null;
                if (pc.AccordingToReferenceId.HasValue)
                {
                    NZOR.Data.Entities.Provider.Reference provRef = prr.GetReference(pc.AccordingToReferenceId.Value);
                    if (!provRef.ConsensusReferenceId.HasValue) continue;
                    refId = provRef.ConsensusReferenceId.Value;
                }
                NZOR.Data.Entities.Consensus.Concept cc = ccr.GetConcept(new Guid(nameRes.MatchedId), refId);

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
                    cc.NameId = new Guid(nameRes.MatchedId);
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
                Data.Sql.Integration.RefreshConsensusConcept(cc, pcList, true, data.DBCnnStr, data.AttachmentPoints);
                Data.Sql.Integration.UpdateRelatedConcepts(cc, pcList, data.DBCnnStr, data.AttachmentPoints);

                //update the relationships as obtained from the refresh
                if (cc.ConceptRelationships.Count > 0 || cc.ConceptApplications.Count > 0)
                {
                    ccr.SaveRelationships(cc);
                }
            }
            
        }

    }
}

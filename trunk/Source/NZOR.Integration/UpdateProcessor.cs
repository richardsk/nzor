using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using NZOR.Matching;


namespace NZOR.Integration
{
    public class UpdateProcessor
    {
        public List<string> Messages = new List<string>();

        private static System.Threading.Semaphore _sem = new System.Threading.Semaphore(1, 1);

        /// <summary>
        /// Process all provider data records that have been updated via a recent import.
        /// The more tricky stuff is with names and concepts.
        /// Need to check if the record is currently integrated.  If not attempt to integrate it.
        /// If the record is integrated, check it still matches the same consensus record.  If not then reintegrate to the other consensus record, and deprecate name if no other provider names for this name.  
        /// If the record still matches the same consensus record then the data will be updated during the integration step.
        /// For other record types, just need to do a refresh of the consensus record.
        /// </summary>
        /// <param name="fromDate"></param>
        public void ProcessUpdatedProviderData(DateTime fromDate, XmlDocument config, List<Data.Entities.Integration.DeprecatedRecord> deprecatedRecords)
        {
            _sem.WaitOne();

            try
            {
                String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

                Data.Repositories.Consensus.INameRepository nr = new Data.Sql.Repositories.Consensus.NameRepository(connectionString);
                Data.Repositories.Consensus.IConceptRepository cr = new Data.Sql.Repositories.Consensus.ConceptRepository(connectionString);
                Data.Repositories.Provider.INameRepository pnr = new Data.Sql.Repositories.Provider.NameRepository(connectionString);
                Data.Repositories.Provider.IConceptRepository pcr = new Data.Sql.Repositories.Provider.ConceptRepository(connectionString);
                Data.Repositories.Provider.IReferenceRepository prr = new Data.Sql.Repositories.Provider.ReferenceRepository(connectionString);

                Data.Sql.Repositories.Common.LookUpRepository lr = new Data.Sql.Repositories.Common.LookUpRepository(connectionString);
                Data.LookUps.Common.ConceptRelationshipTypeLookUp crtl = new Data.LookUps.Common.ConceptRelationshipTypeLookUp(lr.GetConceptRelationshipTypes());
                NZOR.Data.Entities.Common.ConceptRelationshipType parRelType = crtl.GetConceptRelationshipType(Data.LookUps.Common.ConceptRelationshipTypeLookUp.IsChildOf);

                List<NZOR.Data.Entities.Provider.Name> provNamesForUpdate = pnr.GetNamesModifiedSince(fromDate);

                NZOR.Admin.Data.Repositories.IProviderRepository pr = new NZOR.Admin.Data.Sql.Repositories.ProviderRepository(connectionString);
                List<Admin.Data.Entities.AttachmentPoint> attPoints = pr.GetAllAttachmentPoints();

                NZOR.Integration.MatchProcessor.LoadConfig(config);

                foreach (NZOR.Data.Entities.Provider.Name pn in provNamesForUpdate)
                {
                    if (pn.ConsensusNameId.HasValue)
                    {
                        //find match
                        NZOR.Data.DataSets.DsIntegrationName.ProviderNameRow pnRow = Data.Sql.Integration.GetNameMatchData(connectionString, pn.NameId, attPoints);
                        Matching.MatchData md = new Matching.MatchData();

                        ConfigSet cs = NZOR.Integration.MatchProcessor.GetMatchSet(pnRow.MatchRuleSetID);

                        MatchResult mr = MatchProcessor.DoMatch(new MatchData(), pnRow, cs.Routines, true, connectionString);

                        Guid oldNameId = pn.ConsensusNameId.Value;
                        Guid? newNameId = null;
                        int score = 0;
                        if (mr.Matches.Count == 1)
                        {
                            newNameId = mr.Matches[0].Id;
                            score = mr.Matches[0].MatchScore;
                        }
                        else if (mr.Matches.Count > 1)
                        {
                            //issue to be resolved by admin?
                            Messages.Add("Error updating provider name (ProviderRecordId=" + pnRow.ProviderRecordID + "), now has multiple matching NZOR names.");

                            //unlink
                            NZOR.Data.Sql.Integration.UpdateProviderNameLink(connectionString, pn.NameId, NZOR.Data.LinkStatus.Unmatched, null, 0, mr.MatchPath, null);

                            Data.Sql.Integration.RefreshConsensusName(oldNameId, connectionString, attPoints);

                            //fix/refresh any names connecting to this name
                            List<NZOR.Data.Entities.Consensus.Name> names = nr.GetRelatedNames(oldNameId);
                            foreach (NZOR.Data.Entities.Consensus.Name n in names)
                            {
                                Data.Sql.Integration.RefreshConsensusName(n.NameId, connectionString, attPoints);
                            }
                        }

                        //check it matches same consensus name
                        if (oldNameId != newNameId && newNameId != null)
                        {
                            List<Data.Entities.Provider.Name> pnList = pnr.GetNamesForConsensusName(oldNameId);

                            //move provider name and concepts                        
                            NZOR.Data.Sql.Integration.UpdateProviderNameLink(connectionString, pn.NameId, NZOR.Data.LinkStatus.Matched, newNameId, score, mr.MatchPath, null);

                            List<NZOR.Data.Entities.Provider.Concept> pcList = pcr.GetConceptsByName(pn.NameId);
                            foreach (NZOR.Data.Entities.Provider.Concept pConcept in pcList)
                            {
                                Guid? accToId = null;
                                if (pConcept.AccordingToReferenceId.HasValue)
                                {
                                    NZOR.Data.Entities.Provider.Reference pRef = prr.GetReference(pConcept.AccordingToReferenceId.Value);
                                    if (pRef.ConsensusReferenceId.HasValue) accToId = pRef.ConsensusReferenceId.Value;
                                }

                                NZOR.Data.Entities.Consensus.Concept newConcept = cr.GetConcept(newNameId.Value, accToId);
                                Guid? cId = null;
                                if (newConcept != null) cId = newConcept.ConceptId;

                                Guid? batchId = null;

                                NZOR.Data.Sql.Integration.UpdateProviderConceptLink(connectionString, pConcept.ConceptId, (cId.HasValue ? Data.LinkStatus.Unmatched : Data.LinkStatus.Matched), cId, 0, null, batchId);

                                //check if the old concept needs deprecating
                                if (pConcept.ConsensusConceptId.HasValue)
                                {
                                    List<NZOR.Data.Entities.Provider.Concept> provPCList = pcr.GetProviderConcepts(pConcept.ConsensusConceptId.Value);
                                    if (provPCList.Count == 0) //no providers left for this concept
                                    {
                                        NZOR.Data.Entities.Consensus.Concept oldConcept = cr.GetConcept(pConcept.ConsensusConceptId.Value);
                                        cr.DeleteConcept(oldConcept, null);
                                    }
                                }
                            }

                            //refresh new name
                            Data.Sql.Integration.RefreshConsensusName(newNameId.Value, connectionString, attPoints);

                            if (pnList.Count == 1) //no other prov names exists for the old name
                            {
                                //delete old consensus name
                                nr.DeleteName(oldNameId, newNameId);
                            }
                            else
                            {
                                //otherwise refresh old name
                                Data.Sql.Integration.RefreshConsensusName(oldNameId, connectionString, attPoints);
                            }

                            //fix/refresh any names connecting to this name
                            List<NZOR.Data.Entities.Consensus.Name> names = nr.GetRelatedNames(oldNameId);
                            foreach (NZOR.Data.Entities.Consensus.Name n in names)
                            {
                                Data.Sql.Integration.RefreshConsensusName(n.NameId, connectionString, attPoints);
                            }
                        }
                        else
                        {
                            Data.Sql.Integration.RefreshConsensusName(oldNameId, connectionString, attPoints);
                        }
                    }
                }

                //concepts
                List<NZOR.Data.Entities.Provider.Concept> provConcepts = pcr.GetConceptsModifiedSince(fromDate);

                List<Guid> refreshedConcepts = new List<Guid>();

                foreach (NZOR.Data.Entities.Provider.Concept pConcept in provConcepts)
                {
                    Guid? accToId = null;
                    if (pConcept.AccordingToReferenceId.HasValue)
                    {
                        NZOR.Data.Entities.Provider.Reference pRef = prr.GetReference(pConcept.AccordingToReferenceId.Value);
                        if (pRef.ConsensusReferenceId.HasValue) accToId = pRef.ConsensusReferenceId.Value;
                    }

                    NZOR.Data.Entities.Provider.Name newName = pnr.GetName(pConcept.NameId);

                    NZOR.Data.Entities.Consensus.Concept newConcept = null;
                    if (newName.ConsensusNameId.HasValue) newConcept = cr.GetConcept(newName.ConsensusNameId.Value, accToId);
                    Guid? cId = null;
                    if (newConcept != null)
                    {
                        cId = newConcept.ConceptId;

                        NZOR.Data.Entities.Consensus.Concept oldConcept = null;
                        if (pConcept.ConsensusConceptId.HasValue) oldConcept = cr.GetConcept(pConcept.ConsensusConceptId.Value);

                        if (!pConcept.ConsensusConceptId.HasValue || cId != pConcept.ConsensusConceptId.Value)
                        {
                            Guid? batchId = null;

                            NZOR.Data.Sql.Integration.UpdateProviderConceptLink(connectionString, pConcept.ConceptId, (cId.HasValue ? Data.LinkStatus.Unmatched : Data.LinkStatus.Matched), cId, 0, null, batchId);

                            NZOR.Data.Sql.Integration.RefreshConsensusConcept(connectionString, newConcept, null, null, attPoints);

                            if (!refreshedConcepts.Contains(newConcept.ConceptId)) refreshedConcepts.Add(newConcept.ConceptId);

                            //check if the old concept needs deprecating
                            if (pConcept.ConsensusConceptId.HasValue)
                            {
                                List<NZOR.Data.Entities.Provider.Concept> provPCList = pcr.GetProviderConcepts(pConcept.ConsensusConceptId.Value);
                                if (provPCList.Count == 0) //no providers left for this concept
                                {
                                    if (oldConcept != null) cr.DeleteConcept(oldConcept, null);
                                }
                                else
                                {
                                    if (oldConcept != null && !refreshedConcepts.Contains(oldConcept.ConceptId))
                                    {
                                        NZOR.Data.Sql.Integration.RefreshConsensusConcept(connectionString, oldConcept, null, null, attPoints);
                                        refreshedConcepts.Add(newConcept.ConceptId);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (oldConcept != null && !refreshedConcepts.Contains(oldConcept.ConceptId))
                            {
                                NZOR.Data.Sql.Integration.RefreshConsensusConcept(connectionString, oldConcept, null, null, attPoints);
                                refreshedConcepts.Add(newConcept.ConceptId);
                            }
                        }
                    }
                }

                //taxonproperties?


                if (deprecatedRecords != null)
                {
                    foreach (Data.Entities.Integration.DeprecatedRecord dr in deprecatedRecords)
                    {
                        if (dr.RecordType == Admin.Data.Entities.NZORRecordType.Name)
                        {
                            Data.Entities.Provider.Name delPn = pnr.GetNameByProviderId(dr.DataSourceCode, dr.ProviderRecordId);
                            if (delPn != null)
                            {
                                UnintegrateAndRemoveProviderName(delPn, connectionString, attPoints);
                            }
                        }

                        if (dr.RecordType == Admin.Data.Entities.NZORRecordType.Concept)
                        {
                            Admin.Data.Entities.Provider delProv = pr.GetProviders().Where(p => p.Code == dr.ProviderCode).FirstOrDefault();
                            if (delProv != null)
                            {
                                Data.Entities.Provider.Concept delPc = pcr.GetConceptByProviderId(dr.ProviderRecordId, delProv.ProviderId);
                                if (delPc != null) pcr.DeleteConcept(delPc.ConceptId);
                            }
                        }

                        //TODO refereences and other data types?
                    }
                }
            }
            catch (Exception ex)
            {                
                Log.LogError(ex);
                throw ex;
            }
            finally
            {
                _sem.Release();
            }
        }

        public void UnintegrateAndRemoveProviderName(NZOR.Data.Entities.Provider.Name provName, string connectionString, List<Admin.Data.Entities.AttachmentPoint> attPoints)
        {
            Data.Repositories.Consensus.INameRepository nr = new Data.Sql.Repositories.Consensus.NameRepository(connectionString);
            Data.Repositories.Provider.INameRepository pnr = new Data.Sql.Repositories.Provider.NameRepository(connectionString);
            
            List<NZOR.Data.Entities.Consensus.Name> relatedNames = null;
            if (provName.ConsensusNameId.HasValue) relatedNames = nr.GetRelatedNames(provName.ConsensusNameId.Value);

            UnIntegrator.UnIntegrateName(connectionString, provName.NameId); 

            pnr.DeleteName(provName.NameId);

            if (provName.ConsensusNameId.HasValue)
            {
                List<Data.Entities.Provider.Name> pnList = pnr.GetNamesForConsensusName(provName.ConsensusNameId.Value);
                if (pnList.Count == 0) //no other prov names exists for the old name
                {
                    //delete old consensus name
                    nr.DeleteName(provName.ConsensusNameId.Value, null);

                    //fix/refresh any names connecting to this name
                    foreach (NZOR.Data.Entities.Consensus.Name n in relatedNames)
                    {
                        Data.Sql.Integration.RefreshConsensusName(n.NameId, connectionString, attPoints);
                    }
                }
            }
        }

        public void RemoveBrokeredName(NZOR.Admin.Data.Entities.BrokeredName brokeredName, string connectionString)
        {
            NZOR.Admin.Data.Repositories.IAdminRepository ar = new NZOR.Admin.Data.Sql.Repositories.AdminRepository(connectionString);

            NZOR.Admin.Data.Repositories.IProviderRepository pr = new NZOR.Admin.Data.Sql.Repositories.ProviderRepository(connectionString);
            List<Admin.Data.Entities.AttachmentPoint> attPoints = pr.GetAllAttachmentPoints();

            NZOR.Data.Repositories.Provider.INameRepository pnr = new NZOR.Data.Sql.Repositories.Provider.NameRepository(connectionString);
            List<NZOR.Data.Entities.Provider.Name> provNames = pnr.GetNamesForBrokeredName(brokeredName.BrokeredNameId);

            foreach (NZOR.Data.Entities.Provider.Name pn in provNames)
            {
                UnintegrateAndRemoveProviderName(pn, connectionString, attPoints);
            }

            ar.DeleteBrokeredName(brokeredName.BrokeredNameId);
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Integration
{
    public class UnIntegrator
    {

        /// <summary>
        /// Unlink a provider name and all related data (name properties, concepts, applications, etc).  
        /// Ensure that the integrity of the consensus data is not jeopradised (particularly breakages in parentage chains - ie dont remove Concepts that define parents, leaving hanging names).
        /// Remove consensus data if it is the only provider name for this record and integrity is ok.
        /// </summary>
        /// <param name="cnnStr"></param>
        /// <param name="consensusNameId"></param>
        /// <returns>List of error strings</returns>
        public static List<String> UnIntegrateName(string cnnStr, Guid providerNameId)
        {
            List<string> errors = new List<string>();

            try
            {
                NZOR.Data.Sql.Repositories.Provider.NameRepository pnr = new Data.Sql.Repositories.Provider.NameRepository(cnnStr);
                NZOR.Data.Sql.Repositories.Provider.ConceptRepository pcr = new Data.Sql.Repositories.Provider.ConceptRepository(cnnStr);
                NZOR.Data.Sql.Repositories.Consensus.NameRepository nr = new Data.Sql.Repositories.Consensus.NameRepository(cnnStr);
                Admin.Data.Sql.Repositories.ProviderRepository pRep = new Admin.Data.Sql.Repositories.ProviderRepository(cnnStr);
                
                List<Admin.Data.Entities.AttachmentPoint> attPoints = pRep.GetAllAttachmentPoints();

                NZOR.Data.Entities.Provider.Name pn = pnr.GetName(providerNameId);

                if (pn.ConsensusNameId.HasValue)
                {
                    //unlink all provider concepts and relationships and applications, and all related concepts and applications
                    //check that this is not the only concept and provider concept that defines a praentage relationship - if so, leave a SYSTEM concept in place
                    if (!pnr.CanUnintegrateName(pn))
                    {
                        //TODO alert to provider
                        errors.Add("Provider name (ID=" + pn.ProviderRecordId + ") has been removed from NZOR, but leaves hanging concepts.");
                    }

                    //unlink provider name, concepts, etc
                    NZOR.Data.Entities.Consensus.Name cn = nr.GetName(pn.ConsensusNameId.Value);
                    List<Data.Entities.Provider.Concept> pcList = pcr.GetConceptsByName(pn.NameId);

                    using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, new TimeSpan(1, 0, 0, 0, 0)))
                    {
                        pn.ConsensusNameId = null;
                        pn.LinkStatus = Data.LinkStatus.Unmatched.ToString();
                        pn.MatchPath = null;
                        pn.MatchScore = null;
                        pn.ModifiedDate = DateTime.Now;
                        pn.State = Data.Entities.Entity.EntityState.Modified;

                        foreach (Data.Entities.Provider.Concept pc in pcList)
                        {
                            pc.ConsensusConceptId = null;
                            pc.LinkStatus = Data.LinkStatus.Unmatched.ToString();
                            pc.MatchScore = null;
                            pc.ModifiedDate = DateTime.Now;
                            pc.State = Data.Entities.Entity.EntityState.Modified;

                            pcr.Save(pc, true);
                        }

                        pnr.Save(pn);


                        //refresh concesnsus data
                        Data.Sql.Integration.RefreshConsensusName(cn.NameId, cnnStr, attPoints);

                        scope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add("Error un-integrating name : " + ex.Message + " : " + ex.StackTrace);
            }

            return errors;
        }

        public static void UnIntegrateDataSource(string cnnStr, Guid dataSourceId)
        {
        }

    }
}

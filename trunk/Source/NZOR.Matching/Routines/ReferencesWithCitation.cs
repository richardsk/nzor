using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using NZOR.Data;
using NZOR.Data.DataSets;

namespace NZOR.Matching.Routines
{
    public class ReferencesWithCitation : BaseMatcher
    {
        public DsReferenceMatch GetMatchingReferences(DsIntegrationReference.ProviderReferenceRow pr, ref string matchComments)
        {            
            DsReferenceMatch matches = new DsReferenceMatch();
            if (pr == null || pr.IsCitationNull() || pr.Citation.Length == 0) return matches;

            if (UseDBConnection)
            {
                using (SqlConnection cnn = new SqlConnection(DBConnectionString))
                {
                    cnn.Open();
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = @"select distinct r.ReferenceID, rp.Value as Citation, 100 as PercentMatch from consensus.Reference r " +
                            " inner join consensus.ReferenceProperty rp on rp.ReferenceId = r.ReferenceId and rp.ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07' " +
                            " where lower(rp.Value) = '" + pr.Citation.Replace("'", "''").ToLower() + "'";

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.TableMappings.Add("Table", matches.Reference.TableName);
                        da.Fill(matches);
                    }
                }
            }
            else
            {
                foreach (DsConsensusData.ConsensusReferenceRow cr in MatchData.AllData.ConsensusData.ConsensusReference)
                {
                    if (cr["Citation"].ToString().ToLower() == pr["Citation"].ToString().ToLower())
                    {
                        matches.Reference.AddReferenceRow(cr.ReferenceID, cr.Citation, 100);
                    }
                }
            }
            return matches;
        }

        public void RemoveNonMatches(DsIntegrationReference.ProviderReferenceRow pr, ref DsReferenceMatch refs, ref string matchComments)
        {
            for (int i = refs.Reference.Count - 1; i >= 0; i--)
            {
                DsReferenceMatch.ReferenceRow rr = refs.Reference[i];
                if (rr["Citation"].ToString().ToLower() != pr["Citation"].ToString().ToLower())
                {
                    rr.Delete();
                }
            }
            refs.AcceptChanges();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.Data.SqlClient;

using NZOR.Data.Entities.Common;
using NZOR.Data.DataSets;

namespace NZOR.Integration.Other
{

    public class NameDataRowComparer : IEqualityComparer<DataRow>
    {
        public bool Equals(DataRow name1, DataRow name2)
        {
            if (name1.RowState == DataRowState.Deleted || name2.RowState == DataRowState.Deleted) return false;

            string rank1 = name1["TaxonRankID"].ToString();
            string canonical1 = name1["Canonical"].ToString().ToLower();
            string authors1 = name1["Authors"].ToString().Replace(" et ", " & ").Replace(" and ", " & ").ToLower();
            if (authors1.IndexOf(" ex ") != -1) authors1 = authors1.Substring(authors1.IndexOf(" ex ") + 4);
            string year1 = name1["Year"].ToString().ToLower();
            string genus1 = name1["Genus"].ToString().ToLower();
            string species1 = name1["Species"].ToString().ToLower();
            string govCode1 = name1["GoverningCode"].ToString();

            object ruleSetId = name1["MatchRuleSetId"];

            Matching.ConfigSet ruleSet = MatchProcessor.GetMatchSet(ruleSetId == DBNull.Value ? 1 : (int)ruleSetId);

            string rank2 = name2["TaxonRankID"].ToString();
            string name2Id = name2["NameID"].ToString().ToLower();
            string canonical2 = name2["Canonical"].ToString().ToLower();
            string authors2 = name2["Authors"].ToString().Replace(" et ", " & ").Replace(" and ", " & ").ToLower();
            if (authors2.IndexOf(" ex ") != -1) authors2 = authors2.Substring(authors2.IndexOf(" ex ") + 4);
            string year2 = name2["Year"].ToString().ToLower();
            string genus2 = name2["Genus"].ToString().ToLower();
            string species2 = name2["Species"].ToString().ToLower();
            string govCode2 = name2["GoverningCode"].ToString();
            int sortOrder = (int)name2["SortOrder"];

            //must be same rank below genus
            if (sortOrder >= Data.LookUps.Common.TaxonRankLookUp.SortOrderGenus && rank1 != rank2)
            {
                name1["MatchScore"] = 0;
            }


            //if dont start with same letter, then not the same
            if (canonical1[0] != 'x' && canonical1[0] != '×' && canonical2[0] != 'x' && canonical2[0] != '×' && canonical1[0] != canonical2[0])
            {
                name1["MatchScore"] = 0;
                return false;
            }

            if (canonical1 == canonical2 && authors1 == authors2 && year1 == year2 && genus1 == genus2 && species1 == species2 && govCode1 == govCode2)
            {
                name1["MatchScore"] = 100;
                return true;
            }
            
            //use levenshtein to check for equivalences
            if (genus1 == genus2 && species1 == species2 && govCode1 == govCode2)
            {
                double canPerc =  Matching.Utility.LevenshteinPercent(canonical1, canonical2);
                if (canPerc > ruleSet.GetRoutine(typeof(Matching.NamesWithPartialCanonical).ToString()).Threshold)
                {
                    double authPerc = Matching.Utility.LevenshteinPercent(authors1, authors2);
                    if (authPerc > ruleSet.GetRoutine(typeof(Matching.NamesWithPartialAuthors).ToString()).Threshold)
                    {
                        double yearPerc = Matching.Utility.LevenshteinPercent(year1, year2);
                        if (yearPerc > ruleSet.GetRoutine(typeof(Matching.NamesWithPartialYear).ToString()).Threshold)
                        {
                            double[] vals = {canPerc, authPerc, yearPerc};
                            name1["MatchScore"] = vals.Average();
                            return true;
                        }
                    }
                }
            }

            name1["MatchScore"] = 0;
            return false;
        }
        
        public int GetHashCode(DataRow obj)
        {
            return DataRowComparer.Default.GetHashCode(obj);
        }
    }

    /// <summary>
    /// This version first selects all distinct provider names with minimum data (canonical and authors)
    /// Then runs through these names to see if any are close matches to any other selected names, removes them, or one of them, if there is
    /// Then uses another processor to finish the job for all other names and concepts.
    /// </summary>
    public class IntegrationProcessor5
    {
        public string ConnectionString = null;

        public int Progress = 0;
        public String Status = "";

        public void RunIntegration(XmlDocument config)
        {
            Guid integrationBatchId = Guid.NewGuid();

            ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            NZOR.Admin.Data.Repositories.IProviderRepository pr = new NZOR.Admin.Data.Sql.Repositories.ProviderRepository(ConnectionString);
            List<Admin.Data.Entities.AttachmentPoint> attPoints = pr.GetAllAttachmentPoints();

            Data.Sql.Integration.UpdateProviderStackedNameData(ConnectionString);

            Integration.IntegrationProcessor nextProc = new Integration.IntegrationProcessor();
            nextProc.IntegrationBatchID = integrationBatchId;
            nextProc.ConnectionString = ConnectionString;

            Progress = 0;
                        
            MatchProcessor.LoadConfig(config);

            Status = "Processing references...";

            //references
            nextProc.ProcessReferences();

            Status = "Loading data";

            //get distinct names from provider and consensus data (that have full data = canonical and author)
            DsDistinctName distNames = GetDistinctNames(false);

            int totalProvNames = GetNamesForIntegrationCount();

            //int total = totalProvNames;
            int done = 0;

            //var query = (from n in names.Tables[0].AsEnumerable()
            //             orderby n.Field<int>("SortOrder")
            //             select n).Distinct(new NameDataRowComparer());

            //DsDistinctName distNames = new DsDistinctName();


            //foreach (DataRow dn in query.AsEnumerable())
            //{
            //    distNames.Name.ImportRow(dn);
                
            //    done++;
            //    Progress = done * 100 / total;
            //}

            //List<DistinctName> results = new List<DistinctName>();

            //foreach (DataRow r in query.AsEnumerable())
            //{
            //    DistinctName dn = new DistinctName();
            //    dn.NameId = r["NameID"].ToString();
            //    dn.Canonical = r["Canonical"].ToString();
            //    dn.Authors = r["Authors"].ToString();
            //    dn.Year = r["Year"].ToString();
            //    dn.Genus = r["Genus"].ToString();
            //    dn.Species = r["Species"].ToString();
            //    dn.TaxonRankId = r.IsNull("TaxonRankID") ? Guid.Empty : (Guid)r["TaxonRankID"];
            //    dn.SortOrder = r.IsNull("SortOrder") ? -1 : (int)r["SortOrder"];
            //    dn.MatchRuleSetId = r.IsNull("MatchRuleSetId") ? -1 : (int)r["MatchRuleSetID"];
            //    dn.GoverningCode = r["GoverningCode"].ToString();

            //    results.Add(dn);
            //}
            
                        
            List<Matching.MatchResult> results = new List<Matching.MatchResult>();

            Data.Sql.Repositories.Provider.NameRepository pnr = new Data.Sql.Repositories.Provider.NameRepository(ConnectionString);

            Status = "Processing provider names";

            //get provider names with full details, canonical and authors
            DsDistinctName provNames = GetProviderNamesForIntegration();

            //find matches for prov names
            foreach (DsDistinctName.NameRow provName in provNames.Name)
            {
                Matching.MatchResult res = new Matching.MatchResult();
                res.ProviderRecordId = provName["ProviderRecordID"].ToString();

                try
                {
                    using (SqlConnection cnn = new SqlConnection(ConnectionString))
                    {
                        cnn.Open();

                        Data.Entities.Provider.Name pn = pnr.GetName(provName.NameID);
                     
                        //TODO - do we care if there is no parent concept and this name may end up with no consensus parent concept?

                        //find any prov names that match this name and insert/match it
                        var matches = (from n in distNames.Name.AsEnumerable()
                                       select n).Intersect(new DataRow[] { provName }, new NameDataRowComparer());

                        if (matches.Count() == 0 || matches.First().IsNull("NameID"))
                        {
                            //insert
                            DataSet newName = NZOR.Data.Sql.Integration.AddConsensusName(ConnectionString, pn);
                            DataRow nameRow = newName.Tables[0].Rows[0];
                            Data.Sql.Integration.UpdateProviderNameLink(cnn, pn.NameId, NZOR.Data.LinkStatus.Inserted, (Guid?)nameRow["NameID"], 100, "Distinct name matching", integrationBatchId);

                            res.MatchedId = nameRow["NameID"].ToString();
                            res.MatchedName = nameRow["FullName"].ToString();
                            res.Status = NZOR.Data.LinkStatus.Inserted;

                            if (matches.Count() > 0)
                            {
                                matches.First()["NameID"] = (Guid)nameRow["NameID"];
                            }
                            else
                            {
                                //add new name to distinct names set
                                object[] vals = provName.ItemArray;
                                vals[0] = nameRow["NameID"];
                                vals[0] = DBNull.Value;
                                distNames.Name.LoadDataRow(vals, true);
                            }
                        }
                        else if (matches.Count() == 1)
                        {
                            //link 
                            NZOR.Data.Sql.Integration.UpdateProviderNameLink(cnn, pn.NameId, NZOR.Data.LinkStatus.Matched, (Guid?)matches.First()["NameID"], (int)matches.First()["MatchScore"], "Distinct name matching", integrationBatchId);
                            res.MatchedId = matches.First()["NameID"].ToString();
                            res.MatchedName = matches.First()["Canonical"].ToString();
                            res.Status = NZOR.Data.LinkStatus.Matched;

                            matches.First()["NameID"] = (Guid)matches.First()["NameID"];
                        }
                        else
                        {
                            res.Status = Data.LinkStatus.Multiple;
                            NZOR.Data.Sql.Integration.UpdateProviderNameLink(cnn, pn.NameId, NZOR.Data.LinkStatus.Multiple, null, 0, "Distinct name matching", integrationBatchId);
                        }
                        
                        if (matches.Count() == 1)
                        {
                            Data.Sql.Integration.RefreshConsensusName(new Guid(res.MatchedId), ConnectionString, attPoints);
                        }

                        if (matches.Count() <= 1)
                        {
                            //do concepts 
                            nextProc.ProcessNameConcepts(pn, res);

                            //stacked name (after parent cpncepts have been added
                            Data.Sql.Integration.UpdateConsensusStackedNameData(ConnectionString, new Guid(res.MatchedId));

                            //do taxon properties
                            nextProc.ProcessTaxonProperties(pn.NameId, res);
                        }

                    }
                }
                catch (Exception ex)
                {
                    res.Error = "Failed to integrate provider name, ProviderRecordID = '" + res.ProviderRecordId + "'. " + ex.Message + " : " + ex.StackTrace;
                    res.Status = Data.LinkStatus.DataFail;
                }

                results.Add(res);

                done++;
                Progress = done * 100 / totalProvNames;
            }


            //process remaining "naked" names
            nextProc.RunIntegration(config, -1);

        }
              
        public DsDistinctName GetDistinctNames(bool allProviderNames)
        {
            DsDistinctName ds = new DsDistinctName();
            
            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                cnn.Open();

                //existing consensus names
                if (!allProviderNames)
                {
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = @"select n.nameid, n.TaxonRankID, cnp.Value as Canonical, anp.value as Authors, ynp.Value as Year, gn.CanonicalName as Genus, sn.CanonicalName as Species,
	                        n.GoverningCode, tr.SortOrder, tr.MatchRuleSetId
                        from consensus.Name n
                        inner join consensus.NameProperty cnp on cnp.NameID = n.NameID and cnp.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
                        inner join TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
                        inner join consensus.NameProperty anp on anp.NameID = n.NameID and anp.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
                        left join consensus.NameProperty ynp on ynp.NameID = n.NameID and ynp.NamePropertyTypeID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'
                        left join consensus.StackedName gn on gn.SeedNameID = n.nameid and gn.Depth > 0
	                        and gn.TaxonRankID = '20552EB6-1BF0-4073-A021-A6C7A89B7F14'
                        left join consensus.StackedName sn on sn.SeedNameID = n.nameid and sn.Depth > 0
	                        and sn.TaxonRankID = 'C21BB221-5291-4540-94D1-55A12D1BD0AD'
                        order by tr.SortOrder, Genus, Species, Canonical";

                        cmd.CommandTimeout = 5000;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.TableMappings.Add("Table", "Name");
                        da.Fill(ds);
                    }
                }

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = @"select distinct n.TaxonRankID, cnp.Value as Canonical, anp.value as Authors, ynp.Value as Year, gn.CanonicalName as Genus, sn.CanonicalName as Species,
	                        n.GoverningCode, tr.SortOrder, tr.MatchRuleSetId
                        from provider.Name n
                        inner join provider.NameProperty cnp on cnp.NameID = n.NameID and cnp.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
                        inner join TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
                        inner join provider.NameProperty anp on anp.NameID = n.NameID and anp.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
                        left join provider.NameProperty ynp on ynp.NameID = n.NameID and ynp.NamePropertyTypeID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'
                        left join provider.StackedName gn on gn.SeedNameID = n.nameid and gn.Depth > 0
	                        and gn.TaxonRankID = '20552EB6-1BF0-4073-A021-A6C7A89B7F14'
                        left join provider.StackedName sn on sn.SeedNameID = n.nameid and sn.Depth > 0
	                        and sn.TaxonRankID = 'C21BB221-5291-4540-94D1-55A12D1BD0AD' ";

                    if (!allProviderNames)
                    {
                        cmd.CommandText += @"where not exists(select cn.nameid from consensus.name cn
                                inner join consensus.NameProperty cnp2 on cnp2.NameID = cn.NameID and cnp2.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
                                inner join TaxonRank tr on tr.TaxonRankID = cn.TaxonRankID
                                inner join consensus.NameProperty anp2 on anp2.NameID = cn.NameID and anp2.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
                                left join consensus.NameProperty ynp2 on ynp2.NameID = cn.NameID and ynp2.NamePropertyTypeID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'
                                left join consensus.StackedName gn2 on gn2.SeedNameID = cn.nameid and gn2.Depth > 0
                                    and gn2.TaxonRankID = '20552EB6-1BF0-4073-A021-A6C7A89B7F14'
                                left join consensus.StackedName sn2 on sn2.SeedNameID = cn.nameid and sn2.Depth > 0
                                    and sn2.TaxonRankID = 'C21BB221-5291-4540-94D1-55A12D1BD0AD'
                                where cnp2.Value = cnp.Value and anp2.Value = anp.Value and isnull(ynp2.value,'') = isnull(ynp.value,'') 
                                    and isnull(gn2.CanonicalName,'') = isnull(gn.canonicalname,'') and isnull(sn2.canonicalname,'') = isnull(sn.canonicalname,'') 
                                    and cn.governingcode = n.governingcode)";
                    }

                    cmd.CommandText += @"group by n.TaxonRankID, cnp.Value, anp.Value, ynp.Value, gn.CanonicalName, sn.CanonicalName, n.GoverningCode, tr.SortOrder, tr.MatchRuleSetId
                                    order by tr.SortOrder, Genus, Species, Canonical";

                    cmd.CommandTimeout = 5000;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.TableMappings.Add("Table", "Name");
                    da.Fill(ds);
                }
            }

            return ds;
        }

        private DsDistinctName GetProviderNamesForIntegration()
        {
            DsDistinctName ds = new DsDistinctName();

            using (SqlConnection cnn = new SqlConnection(ConnectionString))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = @"select n.NameId, n.providerrecordid, n.TaxonRankID, cnp.Value as Canonical, anp.value as Authors, ynp.Value as Year, gn.CanonicalName as Genus, sn.CanonicalName as Species,
	                        n.GoverningCode, tr.SortOrder, tr.MatchRuleSetId
                        from provider.Name n
                        inner join provider.NameProperty cnp on cnp.NameID = n.NameID and cnp.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
                        inner join TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
                        left join provider.NameProperty anp on anp.NameID = n.NameID and anp.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
                        left join provider.NameProperty ynp on ynp.NameID = n.NameID and ynp.NamePropertyTypeID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'
                        left join provider.StackedName gn on gn.SeedNameID = n.nameid and gn.Depth > 0
	                        and gn.TaxonRankID = '20552EB6-1BF0-4073-A021-A6C7A89B7F14'
                        left join provider.StackedName sn on sn.SeedNameID = n.nameid and sn.Depth > 0
	                        and sn.TaxonRankID = 'C21BB221-5291-4540-94D1-55A12D1BD0AD' 
                        where n.ConsensusNameID is null
                        order by tr.SortOrder";

                    cmd.CommandTimeout = 5000;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.TableMappings.Add("Table", "Name");
                    da.Fill(ds);
                }
            }

            return ds;
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

    }
}

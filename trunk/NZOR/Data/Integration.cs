using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Data;

namespace NZOR.Data
{
    public class IntegrationSaveResult
    {
        public int NumberInserted = 0;
        public int NumberUpdated = 0;
        public int ProviderNamesIntegrated = 0;
        public int ProviderNamesWithErrors = 0;
        public List<String> Errors = new List<string>();
    }

    public class Integration
    {
        public static int Progress = 0;

        /// <summary>
        /// Dataset file in binary format
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DsIntegrationName LoadDataFile(string filePath)
        {
            DsIntegrationName data = new DsIntegrationName();

            data.RemotingFormat = SerializationFormat.Binary;

            IFormatter fmt = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.Stream str = new System.IO.FileStream(filePath, System.IO.FileMode.Open);
            data = (DsIntegrationName)fmt.Deserialize(str);
            str.Close();

            return data;
        }

        public static void SaveDataFile(DsIntegrationName data, string filePath)
        {
            data.RemotingFormat = SerializationFormat.Binary;
            IFormatter fmt = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.Stream str = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
            fmt.Serialize(str, data);
            str.Close();
        }

        public static IntegrationSaveResult SaveIntegrationData(SqlConnection cnn, DsIntegrationName data)
        {
            Progress = 0;
            int total = data.ProviderName.Count + data.ConsensusName.Count;
            int done = 0;

            IntegrationSaveResult isr = new IntegrationSaveResult();

            foreach (DsIntegrationName.ConsensusNameRow cnRow in data.ConsensusName)
            {
                if (cnRow.RowState == System.Data.DataRowState.Added)
                {
                    ConsensusName.AddConsensusName(cnn, cnRow);
                    isr.NumberInserted++;
                }
                else if (cnRow.RowState == System.Data.DataRowState.Modified)
                {
                    ConsensusName.UpdateConsensusName(cnn, cnRow);
                    isr.NumberUpdated++;
                }

                done++;
                Progress = (done * 100 / total);
            }

            foreach (DsIntegrationName.ProviderNameRow pnRow in data.ProviderName)
            {
                if (pnRow.RowState == System.Data.DataRowState.Modified)
                {
                    ProviderName.UpdateProviderName(cnn, pnRow);
                    if (pnRow.LinkStatus == LinkStatus.DataFail.ToString() || pnRow.LinkStatus == LinkStatus.Multiple.ToString() || pnRow.LinkStatus == LinkStatus.MultipleParent.ToString() ||
                        pnRow.LinkStatus == LinkStatus.ParentMissing.ToString() || pnRow.LinkStatus == LinkStatus.ParentNotIntegrated.ToString())
                    {
                        isr.ProviderNamesWithErrors++;
                    }
                    else
                    {
                        isr.ProviderNamesIntegrated++;
                    }
                }

                done++;
                Progress = (done * 100 / total);
            }


            //TODO - any more clean up?

            return isr;
        }


        public static DsIntegrationName GetAllDataForIntegration(SqlConnection cnn)
        {
            DsIntegrationName ds = new DsIntegrationName();

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"
                    select distinct pn.NameID,
	                pn.ConsensusNameID,
	                pn.LinkStatus,
	                pn.MatchScore,
	                pn.MatchPath,
	                pn.FullName,
	                pn.NameClassID,
	                nc.Name as NameClass,
                    tr.TaxonRankID,
	                tr.Name as TaxonRank,
	                tr.SortOrder as TaxonRankSort,
                    tr.MatchRuleSetID,
                    ap.Value as Authors,
	                pn.GoverningCode,	
	                pn.DataSourceID,
	                cp.Value as Canonical, --canonical
	                yp.Value as YearOnPublication, --year on pub
	                bp.RelatedID as BasionymID, --basionym
	                bp.Value as Basionym, 
	                bap.Value as BasionymAuthors, --basionym authors
	                cap.Value as CombinationAuthors, --comb authors
	                mrp.Value as MicroReference, --micro ref
	                pip.Value as PublishedIn, --published in
	                (select top 1 pc.NameToID) as ParentID, --parent name
                    (select top 1 pc.ConsensusNameToID) as ParentConsensusNameID,
	                (select top 1 pc.NameToFull) as Parent,
	                (select top 1 prc.NameToID) as PreferredNameID, --pref name
                    (select top 1 prc.ConsensusNameToID) as PreferredConsensusNameID,
	                (select top 1 prc.NameToFull) as PreferredName
                from provider.Name pn
                inner join dbo.TaxonRank tr on tr.TaxonRankID = pn.TaxonRankID
                inner join dbo.NameClass nc on nc.NameClassID = pn.NameClassID
                inner join provider.NameProperty cp on cp.NameID = pn.NameID and cp.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
                left join provider.NameProperty yp on yp.NameID = pn.NameID and yp.NamePropertyTypeID = '4EC79307-E41A-4540-8647-03EF48795435'
                left join provider.NameProperty bp on bp.NameID = pn.NameID and bp.NamePropertyTypeID = 'F496FBCC-8DA6-4CA1-9884-11BD9B5DF63B'
                left join provider.NameProperty ap on ap.NameID = pn.NameID and ap.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
                left join provider.NameProperty bap on bap.NameID = pn.NameID and bap.NamePropertyTypeID = '6272B3D0-C91B-4FD4-A714-662B10FA6E68'
                left join provider.NameProperty cap on cap.NameID = pn.NameID and cap.NamePropertyTypeID = '6196CDC4-BACB-4172-8186-14BA494621A7'
                left join provider.NameProperty mrp on mrp.NameID = pn.NameID and mrp.NamePropertyTypeID = '4A344D40-7448-49D6-956B-4392B33A749F'
                left join provider.NameProperty pip on pip.NameID = pn.NameID and pip.NamePropertyTypeID = 'DEDC63F0-FB2A-420B-9932-786B4347DA45'
                left join (select NameID, NameToID, NameToFull, ConsensusNameToID from vwProviderConcepts where ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' and InUse = 1) pc 
	                on pc.NameID = pn.NameID
                left join (select NameID, NameToID, NameToFull, ConsensusNameToID from vwProviderConcepts where ConceptRelationshipTypeID = '0CA79AB3-E213-4F51-88B9-4CE01F735A1D' and InUse = 1) prc
	                on prc.NameID = pn.NameId
                order by tr.SortOrder;

                
                select cn.NameID,
                cn.FullName,
                cn.NameClassID,
                nc.Name as NameClass,
                tr.TaxonRankID,
                tr.Name as TaxonRank,
                tr.SortOrder as TaxonRankSort,
                ap.Value as Authors,
                cn.GoverningCode,	
                cp.Value as Canonical, --canonical
                yp.Value as YearOnPublication, --year on pub
                bp.RelatedID as BasionymID, --basionym
                bp.Value as Basionym, 
                bap.Value as BasionymAuthors, --basionym authors
                cap.Value as CombinationAuthors, --comb authors
                mrp.Value as MicroReference, --micro ref
                pip.Value as PublishedIn, --published in
                (select top 1 pc.NameToID) as ParentID, --parent name
                ids.list as ParentIDsToRoot,
                (select top 1 pc.NameToFull) as Parent,
                (select top 1 prc.NameToID) as PreferredNameID, --pref name
                (select top 1 prc.NameToFull) as PreferredName
            from consensus.Name cn                
            left join dbo.TaxonRank tr on tr.TaxonRankID = cn.TaxonRankID
            left join dbo.NameClass nc on nc.NameClassID = cn.NameClassID
            left join consensus.NameProperty cp on cp.NameID = cn.NameID and cp.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
            left join consensus.NameProperty yp on yp.NameID = cn.NameID and yp.NamePropertyTypeID = '4EC79307-E41A-4540-8647-03EF48795435'
            left join consensus.NameProperty bp on bp.NameID = cn.NameID and bp.NamePropertyTypeID = 'F496FBCC-8DA6-4CA1-9884-11BD9B5DF63B'
            left join consensus.NameProperty ap on ap.NameID = cn.NameID and ap.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
            left join consensus.NameProperty bap on bap.NameID = cn.NameID and bap.NamePropertyTypeID = '6272B3D0-C91B-4FD4-A714-662B10FA6E68'
            left join consensus.NameProperty cap on cap.NameID = cn.NameID and cap.NamePropertyTypeID = '6196CDC4-BACB-4172-8186-14BA494621A7'
            left join consensus.NameProperty mrp on mrp.NameID = cn.NameID and mrp.NamePropertyTypeID = '4A344D40-7448-49D6-956B-4392B33A749F'
            left join consensus.NameProperty pip on pip.NameID = cn.NameID and pip.NamePropertyTypeID = 'DEDC63F0-FB2A-420B-9932-786B4347DA45'
            left join (select NameID, NameToID, NameToFull from vwconsensusConcepts where ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155') pc 
                on pc.NameID = cn.NameID
            left join (select NameID, NameToID, NameToFull from vwconsensusConcepts where ConceptRelationshipTypeID = '0CA79AB3-E213-4F51-88B9-4CE01F735A1D' ) prc
                on prc.NameID = cn.NameId
                CROSS APPLY 
	            ( 
		            SELECT '[' + CONVERT(VARCHAR(38), fn.NameID) + ':' + convert(varchar(38), fn.TaxonRankID) + '],' AS [text()] 
		            FROM cons.FlatName fn
		            WHERE fn.SeedNameID = cn.NameID 
		            FOR XML PATH('') 
	            ) ids (list); "; //parent names [Parent Guid:Rank Guid],[Parent Guid:Rank Guid] ...

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.TableMappings.Add("Table", "ProviderName");
                da.TableMappings.Add("Table1", "ConsensusName");

                da.Fill(ds);

                //if no parent concept, get "fuzzy" match parents
                // -- do during integration
                //GetParentDataAll(cnn, ds);
            }

            return ds;
        }

        public static void GetConsensusDataForIntegration(SqlConnection cnn, ref DsIntegrationName ds)
        {
            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"
                    select cn.NameID,
                    cn.FullName,
                    cn.NameClassID,
                    nc.Name as NameClass,
                    tr.TaxonRankID,
                    tr.Name as TaxonRank,
                    tr.SortOrder as TaxonRankSort,
                    ap.Value as Authors,
                    cn.GoverningCode,	
                    cp.Value as Canonical, --canonical
                    yp.Value as YearOnPublication, --year on pub
                    bp.RelatedID as BasionymID, --basionym
                    bp.Value as Basionym, 
                    bap.Value as BasionymAuthors, --basionym authors
                    cap.Value as CombinationAuthors, --comb authors
                    mrp.Value as MicroReference, --micro ref
                    pip.Value as PublishedIn, --published in
                    (select top 1 pc.NameToID) as ParentID, --parent name
                    ids.list as ParentIDsToRoot,
                    (select top 1 pc.NameToFull) as Parent,
                    (select top 1 prc.NameToID) as PreferredNameID, --pref name
                    (select top 1 prc.NameToFull) as PreferredName
                from consensus.Name cn                
                left join dbo.TaxonRank tr on tr.TaxonRankID = cn.TaxonRankID
                left join dbo.NameClass nc on nc.NameClassID = cn.NameClassID
                left join consensus.NameProperty cp on cp.NameID = cn.NameID and cp.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
                left join consensus.NameProperty yp on yp.NameID = cn.NameID and yp.NamePropertyTypeID = '4EC79307-E41A-4540-8647-03EF48795435'
                left join consensus.NameProperty bp on bp.NameID = cn.NameID and bp.NamePropertyTypeID = 'F496FBCC-8DA6-4CA1-9884-11BD9B5DF63B'
                left join consensus.NameProperty ap on ap.NameID = cn.NameID and ap.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
                left join consensus.NameProperty bap on bap.NameID = cn.NameID and bap.NamePropertyTypeID = '6272B3D0-C91B-4FD4-A714-662B10FA6E68'
                left join consensus.NameProperty cap on cap.NameID = cn.NameID and cap.NamePropertyTypeID = '6196CDC4-BACB-4172-8186-14BA494621A7'
                left join consensus.NameProperty mrp on mrp.NameID = cn.NameID and mrp.NamePropertyTypeID = '4A344D40-7448-49D6-956B-4392B33A749F'
                left join consensus.NameProperty pip on pip.NameID = cn.NameID and pip.NamePropertyTypeID = 'DEDC63F0-FB2A-420B-9932-786B4347DA45'
                left join (select NameID, NameToID, NameToFull from vwconsensusConcepts where ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155') pc 
                    on pc.NameID = cn.NameID
                left join (select NameID, NameToID, NameToFull from vwconsensusConcepts where ConceptRelationshipTypeID = '0CA79AB3-E213-4F51-88B9-4CE01F735A1D' ) prc
                    on prc.NameID = cn.NameId
                    CROSS APPLY 
	                ( 
		                SELECT '[' + CONVERT(VARCHAR(38), fn.NameID) + ':' + convert(varchar(38), fn.TaxonRankID) + '],' AS [text()] 
		                FROM cons.FlatName fn
		                WHERE fn.SeedNameID = cn.NameID 
		                FOR XML PATH('') 
	                ) ids (list); "; //parent names [Parent Guid:Rank Guid],[Parent Guid:Rank Guid] ...

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.TableMappings.Add("Table", "ConsensusName");

                da.Fill(ds);

                //if no parent concept, get "fuzzy" match parents
                // -- do during integration
                //GetParentDataAll(cnn, ds);
            }
        }


        public static void PostIntegrationCleanup(SqlConnection cnn)
        {
            //reset all provider records with status "Integrating", to "Unmatched"
            //TODO other stuff?

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "update provider.Name set LinkStatus = 'Unmatched', MatchScore = null, MatchPath = null, ConsensusNameID = null " +
                    "where LinkStatus = 'Integrating'";

                cmd.ExecuteNonQuery();

                cmd.CommandText = "update provider.Concept set LinkStatus = 'Unmatched', MatchScore = null, ConsensusConceptID = null " +
                    "where LinkStatus = 'Integrating'";

                cmd.ExecuteNonQuery();

                cmd.CommandText = "update provider.Reference set LinkStatus = 'Unmatched', MatchScore = null, ConsensusReferenceID = null " +
                    "where LinkStatus = 'Integrating'";

                cmd.ExecuteNonQuery();

                cmd.CommandText = "update prov.TaxonProperty set LinkStatus = 'Unmatched', MatchScore = null, ConsensusTaxonPropertyID = null " +
                    "where LinkStatus = 'Integrating'";

                cmd.ExecuteNonQuery();
            }
        }
 
    }
}

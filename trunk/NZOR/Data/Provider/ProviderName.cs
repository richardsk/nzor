using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.EntityClient;

namespace NZOR.Data
{
    public class ProviderName
    {
        public static int Progress = 0; //for async processes


        public static void UpdateProviderName(SqlConnection cnn, DsIntegrationName.ProviderNameRow provName)
        {
            //update parts of provider name that can be updated
            string sql = "update provider.Name set " +
                "ConsensusNameID = " + (provName.IsConsensusNameIDNull() ? "null, " : "'" + provName.ConsensusNameID.ToString() + "', ") +
                "LinkStatus = " + (provName.IsLinkStatusNull() ? "null, " : "'" + provName.LinkStatus + "', ") +
                "MatchScore = " + (provName.IsMatchScoreNull() ? "null, " : provName.MatchScore.ToString() + ", ") +
                "MatchPath = " + (provName.IsMatchPathNull() ? "null, " : "'" + provName.MatchPath + "', ") +
                "ModifiedDate = getdate() " +
                "where NameID = '" + provName.NameID.ToString() + "'";
                
            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }

            //properties   - Note can not update properties - must be imported
            
            //Update Flat Name data
            UpdateFlatNameData(cnn, provName.NameID);
        }

        public static void UpdateFlatNameData(SqlConnection cnn, Guid nameID)
        {
            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "delete prov.FlatName where SeedNameID = '" + nameID.ToString() + "'";
                cmd.ExecuteNonQuery();
            }

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "INSERT prov.FlatName(ParentNameID, NameID, Canonical, TaxonRankID, RankName, SortOrder, Depth, SeedNameID) " +
                    "EXEC sprSelect_ProvFlatNameToRoot '" + nameID.ToString() + "'";
                cmd.ExecuteNonQuery();
            }
        }

        public static DataSet GetName(SqlConnection cnn, Guid provNameId)
        {
            DataSet ds = new DataSet();

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"
	                        select * 
	                        from provider.Name pn
	                        inner join TaxonRank tr on tr.TaxonRankID = pn.TaxonRankID
	                        where NameID = '" + provNameId.ToString() + @"'
                        	
	                        select * 
	                        from provider.NameProperty np
	                        inner join NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID
	                        where NameID = '" + provNameId.ToString() + @"'
                        	
	                        select * 
	                        from vwProviderConcepts
	                        where NameID = '" + provNameId.ToString() + @"'";

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                
                ds.Tables[0].TableName = "Name";
                ds.Tables[1].TableName = "NameProperty";
                ds.Tables[2].TableName = "Concepts";
            }
                
            return ds;
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
	                pn.SubDataSetID,
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
                inner join provider.NameProperty cp on cp.NameID = pn.NameID and cp.NameClassPropertyID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
                left join provider.NameProperty yp on yp.NameID = pn.NameID and yp.NameClassPropertyID = '4EC79307-E41A-4540-8647-03EF48795435'
                left join provider.NameProperty bp on bp.NameID = pn.NameID and bp.NameClassPropertyID = 'F496FBCC-8DA6-4CA1-9884-11BD9B5DF63B'
                left join provider.NameProperty ap on ap.NameID = pn.NameID and ap.NameClassPropertyID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
                left join provider.NameProperty bap on bap.NameID = pn.NameID and bap.NameClassPropertyID = '6272B3D0-C91B-4FD4-A714-662B10FA6E68'
                left join provider.NameProperty cap on cap.NameID = pn.NameID and cap.NameClassPropertyID = '6196CDC4-BACB-4172-8186-14BA494621A7'
                left join provider.NameProperty mrp on mrp.NameID = pn.NameID and mrp.NameClassPropertyID = '4A344D40-7448-49D6-956B-4392B33A749F'
                left join provider.NameProperty pip on pip.NameID = pn.NameID and pip.NameClassPropertyID = 'DEDC63F0-FB2A-420B-9932-786B4347DA45'
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
            from cons.Name cn                
            left join dbo.TaxonRank tr on tr.TaxonRankID = cn.TaxonRankID
            left join dbo.NameClass nc on nc.NameClassID = cn.NameClassID
            left join cons.NameProperty cp on cp.NameID = cn.NameID and cp.NameClassPropertyID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
            left join cons.NameProperty yp on yp.NameID = cn.NameID and yp.NameClassPropertyID = '4EC79307-E41A-4540-8647-03EF48795435'
            left join cons.NameProperty bp on bp.NameID = cn.NameID and bp.NameClassPropertyID = 'F496FBCC-8DA6-4CA1-9884-11BD9B5DF63B'
            left join cons.NameProperty ap on ap.NameID = cn.NameID and ap.NameClassPropertyID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
            left join cons.NameProperty bap on bap.NameID = cn.NameID and bap.NameClassPropertyID = '6272B3D0-C91B-4FD4-A714-662B10FA6E68'
            left join cons.NameProperty cap on cap.NameID = cn.NameID and cap.NameClassPropertyID = '6196CDC4-BACB-4172-8186-14BA494621A7'
            left join cons.NameProperty mrp on mrp.NameID = cn.NameID and mrp.NameClassPropertyID = '4A344D40-7448-49D6-956B-4392B33A749F'
            left join cons.NameProperty pip on pip.NameID = cn.NameID and pip.NameClassPropertyID = 'DEDC63F0-FB2A-420B-9932-786B4347DA45'
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
	            ) ids (list); ";

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

        private static void GetParentDataAll(SqlConnection cnn, NZOR.Data.DsIntegrationName pn)
        {
            Progress = 0;
            int total = 0;
            foreach (DsIntegrationName.ProviderNameRow row in pn.ProviderName)
            {
                GetParentData(cnn, row);
                total++;
                Progress = total * 100 / pn.ProviderName.Count;
            }
            Progress = 100;
        }

        public static DsIntegrationName.ProviderNameRow GetNameMatchData(SqlConnection cnn, Guid provNameId)
        {
            //TODO SQL Transaction to prevent deadlocks!!

            DsIntegrationName ds = new DsIntegrationName();

            using (SqlCommand cmd = cnn.CreateCommand())
            {
//                cmd.CommandText = @"
//	                        select * 
//	                        from provider.Name pn
//	                        inner join TaxonRank tr on tr.TaxonRankID = pn.TaxonRankID
//	                        where NameID = '" + provNameId.ToString() + @"'
//                        	
//	                        select * 
//	                        from provider.NameProperty np
//	                        inner join NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID
//	                        where NameID = '" + provNameId.ToString() + @"'
//                        	
//	                        select * 
//	                        from vwProviderConcepts
////	                        where NameID = '" + provNameId.ToString() + @"'";

//                SqlDataAdapter da = new SqlDataAdapter(cmd);
//                da.Fill(ds);

//                ds.Tables[0].TableName = "Name";
//                ds.Tables[1].TableName = "NameProperty";
//                ds.Tables[2].TableName = "Concepts";

                cmd.CommandText = @"
                    select pn.NameID,
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
	                pn.SubDataSetID,
	                cp.Value as Canonical, --canonical
	                yp.Value as YearOnPublication, --year on pub
	                bp.RelatedID as BasionymID, --basionym
	                bp.Value as Basionym, 
	                bap.Value as BasionymAuthors, --basionym authors
	                cap.Value as CombinationAuthors, --comb authors
	                mrp.Value as MicroReference, --micro ref
	                pip.Value as PublishedIn, --published in
	                pc.NameToID as ParentID, --parent name
                    pc.ConsensusNameToID as ParentConsensusNameID,
	                pc.NameToFull as Parent,
	                prc.NameToID as PreferredNameID, --pref name
                    prc.ConsensusNameToID as PreferredConsensusNameID,
	                prc.NameToFull as PreferredName
                from provider.Name pn
                inner join dbo.TaxonRank tr on tr.TaxonRankID = pn.TaxonRankID
                inner join dbo.NameClass nc on nc.NameClassID = pn.NameClassID
                inner join provider.NameProperty cp on cp.NameID = pn.NameID and cp.NameClassPropertyID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
                left join provider.NameProperty yp on yp.NameID = pn.NameID and yp.NameClassPropertyID = '4EC79307-E41A-4540-8647-03EF48795435'
                left join provider.NameProperty bp on bp.NameID = pn.NameID and bp.NameClassPropertyID = 'F496FBCC-8DA6-4CA1-9884-11BD9B5DF63B'
                left join provider.NameProperty ap on ap.NameID = pn.NameID and ap.NameClassPropertyID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
                left join provider.NameProperty bap on bap.NameID = pn.NameID and bap.NameClassPropertyID = '6272B3D0-C91B-4FD4-A714-662B10FA6E68'
                left join provider.NameProperty cap on cap.NameID = pn.NameID and cap.NameClassPropertyID = '6196CDC4-BACB-4172-8186-14BA494621A7'
                left join provider.NameProperty mrp on mrp.NameID = pn.NameID and mrp.NameClassPropertyID = '4A344D40-7448-49D6-956B-4392B33A749F'
                left join provider.NameProperty pip on pip.NameID = pn.NameID and pip.NameClassPropertyID = 'DEDC63F0-FB2A-420B-9932-786B4347DA45'
                left join (select top 1 NameID, NameToID, NameToFull, ConsensusNameToID from vwProviderConcepts where NameID = '" + provNameId.ToString() + "' " +
                    @" and ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' and InUse = 1) pc 
	                on pc.NameID = pn.NameID
                left join (select top 1 NameID, NameToID, NameToFull, ConsensusNameToID from vwProviderConcepts where NameID = '" + provNameId.ToString() + "' " +
                    @" and ConceptRelationshipTypeID = '0CA79AB3-E213-4F51-88B9-4CE01F735A1D' and InUse = 1) prc
	                on prc.NameID = pn.NameId
                where pn.NameID = '" + provNameId.ToString() + "'";

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.TableMappings.Add("Table", "ProviderName");

                da.Fill(ds);

                //if no parent concept, get "fuzzy" match parents
                GetParentData(cnn, ds.ProviderName[0]);
            }

            if (ds.ProviderName.Count == 0) return null;

            return ds.ProviderName[0];
        }

        public static Object GetNamePropertyValue(System.Data.DataTable namePropDt, String field)
        {
            Object val = DBNull.Value;

            foreach (System.Data.DataRow dr in namePropDt.Rows)
            {
                if (dr["Name"].ToString() == field)
                {
                    val = dr["Value"];
                    break;
                }
            }

            return val;
        }

        public static System.Data.DataRow GetNameConcept(System.Data.DataTable conceptsDt, String conceptType)
        {
            System.Data.DataRow r = null;

            foreach (System.Data.DataRow dr in conceptsDt.Rows)
            {
                if (dr["Relationship"].ToString() == conceptType)
                {
                    r = dr;
                    break;
                }
            }

            return r;
        }

        public static void UpdateProviderNameLink(SqlConnection cnn, DsIntegrationName.ProviderNameRow provName, LinkStatus status, Guid? nameId, int matchScore, string matchPath, Guid integrationBatchId)
        {
            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "update provider.Name set LinkStatus = '" + status.ToString() + "', MatchScore = " + matchScore.ToString() + ", MatchPath = '" + matchPath +
                    "', ConsensusNameID = " + (nameId.HasValue ? "'" + nameId.Value.ToString() + "', " : "null, ") +
                    "IntegrationBatchID = '" + integrationBatchId.ToString() + "', ModifiedDate = getdate() " + 
                    "where NameID = '" + provName.NameID.ToString() + "'";

                cmd.ExecuteNonQuery();
            }
        }

        //public static void GetParentData(SqlConnection cnn, System.Data.DataSet pn)
        //{
        //    //Check we have a parent concept.  If not get fuzzy matches for use in matching process
        //    //This routine makes sure that all names that have been selected for possible match are at the correct rank.
        //    //  Names that may be selected could be children of the same parent as the matching name, but this does not mean the names will be the 
        //    //  same rank - eg matching name "Aus bus var. dus" - may select a name in the DB "Aus bus subsp. cus" as "cus" is a child of the naem "Aus bus"            

        //    System.Data.DataRow parRow = GetNameConcept(pn.Tables["Concepts"], NZOR.Data.ConceptProperties.ParentRelationshipType);
        //    string fullName = pn.Tables["Name"].Rows[0]["FullName"].ToString();

        //    DataSet ds = new DataSet();
        //    Guid parentNameID = Guid.Empty;
        //    string parFullName = "";
        //    Guid parRank = Guid.Empty;

        //    if (parRow == null || parRow.IsNull("ConsensusNameToID"))
        //    {
        //        //NO Parent CONCEPT - check for higher ranks
        //        System.String pnCanonical = NZOR.Data.ProviderName.GetNamePropertyValue(pn.Tables["NameProperty"], NZOR.Data.NameProperties.Canonical).ToString();
        //        Guid rankId = (Guid)pn.Tables["Name"].Rows[0]["TaxonRankID"];
        //        string govCode = pn.Tables["Name"].Rows[0]["GoverningCode"].ToString();
        //        NZOR.Data.SystemData.TaxonRank tr = Data.SystemData.TaxonRankData.GetTaxonRank(cnn, rankId);

        //        //TODO - CHECK THIS !  - do we need to allow for Provider/Dataset preferences - ie provider specifies the location in the taxon hierarchy where names should fit
        //        //ORDER and above - just match canonical and rank 
        //        if (tr.SortOrder <= 1600)
        //        {
        //            using (SqlCommand cmd = cnn.CreateCommand())
        //            {
        //                cmd.CommandText = "select distinct fn.ParentNameID, n.FullName, n.TaxonRankID from cons.Name n inner join cons.nameproperty np on np.nameid = n.nameid "
        //                    + " inner join dbo.nameclassproperty ncp on ncp.nameclasspropertyid = np.nameclasspropertyid "
        //                    + " inner join cons.FlatName fn on fn.NameID = n.NameID where n.TaxonRankID = '"
        //                    + tr.TaxonRankId.ToString() + "' and np.Value = '" + pnCanonical + "' and ncp.name = '"
        //                    + NZOR.Data.NameProperties.Canonical + "' and n.GoverningCode = '" + govCode + "'";

        //                DataSet pds = new DataSet();
        //                SqlDataAdapter da = new SqlDataAdapter(cmd);
        //                da.Fill(pds);

        //                if (pds.Tables.Count > 0 && pds.Tables[0].Rows.Count == 1)
        //                {
        //                    parentNameID = (Guid)pds.Tables[0].Rows[0]["ParentNameID"];
        //                    parFullName = pds.Tables[0].Rows[0]["FullName"].ToString();
        //                    parRank = (Guid)pds.Tables[0].Rows[0]["TaxonRankID"];
        //                }
        //            }
        //        }

        //        //Below GENUS - use the Genus (first word of the full name)
        //        if (tr.SortOrder > 3000)
        //        {
        //            if (fullName.IndexOf(" ") != -1)
        //            {
        //                String parent = fullName.Substring(0, fullName.IndexOf(" "));

        //                using (SqlCommand cmd = cnn.CreateCommand())
        //                {
        //                    cmd.CommandText = "select n.NameID from cons.Name n inner join cons.nameproperty np on np.nameid = n.nameid "
        //                        + " inner join dbo.nameclassproperty ncp on ncp.nameclasspropertyid = np.nameclasspropertyid where TaxonRankID = '"
        //                        + NZOR.Data.SystemData.TaxonRankData.GenusRank(cnn).TaxonRankId.ToString() + "' and np.Value = '" + parent + "' and ncp.name = '"
        //                        + NZOR.Data.NameProperties.Canonical + "' and n.GoverningCode = '" + govCode + "'";

        //                    DataSet pds = new DataSet();
        //                    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //                    da.Fill(pds);

        //                    if (pds.Tables.Count > 0 && pds.Tables[0].Rows.Count == 1)
        //                    {
        //                        parentNameID = (Guid)pds.Tables[0].Rows[0]["NameID"];
        //                    }
        //                }
        //            }
        //        }

        //        if (parentNameID != Guid.Empty)
        //        {
        //            //add parent concept
        //            //copy from name details
        //            DataRow cRow = pn.Tables["Concepts"].NewRow();
        //            cRow["NameID"] = pn.Tables[0].Rows[0]["NameID"];
        //            cRow["ConsensusNameID"] = pn.Tables[0].Rows[0]["ConsensusNameID"];
        //            cRow["FullName"] = pn.Tables[0].Rows[0]["FullName"];
        //            cRow["TaxonRankID"] = pn.Tables[0].Rows[0]["TaxonRankID"];
        //            //cRow["RankName"] = pn.Tables[0].Rows[0]["RankName"];
        //            cRow["SortOrder"] = pn.Tables[0].Rows[0]["SortOrder"];
        //            cRow["NameClassID"] = pn.Tables[0].Rows[0]["NameClassID"];
        //            cRow["LinkStatus"] = pn.Tables[0].Rows[0]["LinkStatus"];
        //            cRow["GoverningCode"] = pn.Tables[0].Rows[0]["GoverningCode"];
        //            cRow["SubDataSetID"] = pn.Tables[0].Rows[0]["SubDataSetID"];
        //            cRow["ProviderRecordID"] = pn.Tables[0].Rows[0]["ProviderRecordID"];
        //            cRow["ProviderModifiedDate"] = pn.Tables[0].Rows[0]["ProviderModifiedDate"];
        //            cRow["AddedDate"] = DateTime.Now;

        //            //add parent concept to parent name id
        //            cRow["ConceptID"] = Guid.Empty; //not a real concept
        //            cRow["ConceptRelationshipTypeID"] = ConceptRelationshipType.ParentRelationshipTypeID(cnn);
        //            cRow["Relationship"] = ConceptProperties.ParentRelationshipType;
        //            cRow["NameToID"] = Guid.Empty; //dummy
        //            cRow["ConsensusNameToID"] = parentNameID;
        //            cRow["NameToFull"] = parFullName;
        //            cRow["TaxonRankToID"] = parRank;

        //            pn.Tables["Concepts"].Rows.Add(cRow);
        //        }
        //    }
        //}

        public static void GetParentData(SqlConnection cnn, NZOR.Data.DsIntegrationName.ProviderNameRow pn)
        {
            //Check we have a parent concept.  If not get fuzzy matches for use in matching process
            //This routine makes sure that all names that have been selected for possible match are at the correct rank.
            //  Names that may be selected could be children of the same parent as the matching name, but this does not mean the names will be the 
            //  same rank - eg matching name "Aus bus var. dus" - may select a name in the DB "Aus bus subsp. cus" as "cus" is a child of the naem "Aus bus"            

            if (pn.IsParentIDNull() || pn.IsParentConsensusNameIDNull())
            {
                Guid parentConsNameID = Guid.Empty;
                Guid parRank = Guid.Empty;
                String parFullName = "";

                //NO Parent CONCEPT - check for higher ranks
                int trSort = pn.TaxonRankSort;

                //TODO - CHECK THIS !  - do we need to allow for Provider/Dataset preferences - ie provider specifies the location in the taxon hierarchy where names should fit
                //ORDER and above - just match canonical and rank 
                if (trSort <= 1600)
                {
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = "select distinct fn.ParentNameID, n.FullName, n.TaxonRankID from cons.Name n inner join cons.nameproperty np on np.nameid = n.nameid "
                            + " inner join dbo.nameclassproperty ncp on ncp.nameclasspropertyid = np.nameclasspropertyid "
                            + " inner join cons.FlatName fn on fn.NameID = n.NameID where n.TaxonRankID = '"
                            + pn.TaxonRankID.ToString() + "' and np.Value = '" + pn.Canonical + "' and ncp.name = 'Canonical' and n.GoverningCode = '" 
                            + pn.GoverningCode + "'";

                        DataSet pds = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(pds);

                        if (pds.Tables.Count > 0 && pds.Tables[0].Rows.Count == 1)
                        {
                            parentConsNameID = (Guid)pds.Tables[0].Rows[0]["ParentNameID"];
                            parFullName = pds.Tables[0].Rows[0]["FullName"].ToString();
                            parRank = (Guid)pds.Tables[0].Rows[0]["TaxonRankID"];
                        }
                    }
                }

                //Below GENUS - use the Genus (first word of the full name)
                if (trSort > 3000)
                {
                    if (pn.FullName.IndexOf(" ") != -1)
                    {
                        parFullName = pn.FullName.Substring(0, pn.FullName.IndexOf(" "));

                        using (SqlCommand cmd = cnn.CreateCommand())
                        {
                            cmd.CommandText = "select n.NameID from cons.Name n inner join cons.nameproperty np on np.nameid = n.nameid "
                                + " inner join dbo.nameclassproperty ncp on ncp.nameclasspropertyid = np.nameclasspropertyid where TaxonRankID = '"
                                + NZOR.Data.SystemData.TaxonRankData.GenusRank(cnn).TaxonRankId.ToString() + "' and np.Value = '" + parFullName + "' and ncp.name = '"
                                + NZOR.Data.NameProperties.Canonical + "' and n.GoverningCode = '" + pn.GoverningCode + "'";

                            DataSet pds = new DataSet();
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(pds);

                            if (pds.Tables.Count > 0 && pds.Tables[0].Rows.Count == 1)
                            {
                                parentConsNameID = (Guid)pds.Tables[0].Rows[0]["NameID"];
                            }
                        }
                    }
                }

                if (parentConsNameID != Guid.Empty)
                {
                    //add parent concept
                    pn.ParentConsensusNameID = parentConsNameID;
                    pn.Parent = parFullName;
                    pn.ParentID = Guid.Empty; //not a real provider name
                }
            }
        }

        public static void GetParentData(DsIntegrationName.ProviderNameRow pn, DsIntegrationName allData)
        {
            //Same as abovwe, but no DB connection
            //Check we have a parent concept.  If not get fuzzy matches for use in matching process
            //This routine makes sure that all names that have been selected for possible match are at the correct rank.
            //  Names that may be selected could be children of the same parent as the matching name, but this does not mean the names will be the 
            //  same rank - eg matching name "Aus bus var. dus" - may select a name in the DB "Aus bus subsp. cus" as "cus" is a child of the naem "Aus bus"            

            if (pn.IsParentIDNull() || pn.IsParentConsensusNameIDNull())
            {
                Guid parentConsNameID = Guid.Empty;
                String parFullName = "";

                //check for parent concept
                if (!pn.IsParentIDNull() && pn.ParentID != Guid.Empty)
                {
                    DsIntegrationName.ProviderNameRow parRow = (DsIntegrationName.ProviderNameRow)(allData.ProviderName.Select("NameID = '" + pn.ParentID.ToString() + "'")[0]);
                    if (!parRow.IsConsensusNameIDNull())
                    {
                        parentConsNameID = parRow.PreferredConsensusNameID;
                        parFullName = parRow.FullName;
                    }
                }

                if (parentConsNameID == Guid.Empty)
                {
                    //NO Parent CONCEPT - check for higher ranks
                    int trSort = pn.TaxonRankSort;

                    //TODO - CHECK THIS !  - do we need to allow for Provider/Dataset preferences - ie provider specifies the location in the taxon hierarchy where names should fit
                    //ORDER and above - just match canonical and rank 
                    if (trSort <= 1600)
                    {
                        DataRow[] parents = allData.ConsensusName.Select("TaxonRankID = '" + pn.TaxonRankID.ToString() + "' and Canonical = '" + pn.Canonical + "' and GoverningCode = '" + pn.GoverningCode + "'");
                        if (parents.Length > 0 && !parents[0].IsNull("ParentID"))
                        {
                            parentConsNameID = (Guid)parents[0]["ParentID"];
                            parFullName = parents[0]["Parent"].ToString();
                        }
                    }

                    //Below GENUS - use the Genus (first word of the full name)
                    if (trSort > 3000)
                    {
                        if (pn.FullName.IndexOf(" ") != -1)
                        {
                            parFullName = pn.FullName.Substring(0, pn.FullName.IndexOf(" "));

                            DataRow[] parents = allData.ConsensusName.Select("TaxonRank = 'genus' and Canonical = '" + parFullName + "' and GoverningCode = '" + pn.GoverningCode + "'");

                            if (parents.Length > 0)
                            {
                                parentConsNameID = (Guid)parents[0]["NameID"];
                            }
                        }
                    }
                }

                if (parentConsNameID != Guid.Empty)
                {
                    //add parent concept
                    pn.ParentConsensusNameID = parentConsNameID;
                    pn.Parent = parFullName;
                    pn.ParentID = Guid.Empty; //not a real provider name
                }
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

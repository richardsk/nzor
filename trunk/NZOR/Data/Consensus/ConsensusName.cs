﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace NZOR.Data
{
    public class ConsensusName
    {


        public static bool HasProviderValue(SqlConnection cnn, Guid nameID, String field, object value)
        {
            bool hasVal = false;
            
            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "select count(NameID) from provider.Name pn " +
                    " inner join provider.NameProperty np on np.NameID = pn.NameID " +
                    " inner join dbo.NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID and ncp.Name = '" + field + "' " +
                    " where pn.ConsensusNameID = '" + nameID.ToString() + "' and (np.Value is null or np.Value = '" + value.ToString() + "')";

                int cnt = (int)cmd.ExecuteScalar();
                if (cnt > 0) hasVal = true;
            }

            return hasVal;
        }

        public static DsNameMatch GetNamesWithProperty(SqlConnection cnn, String field, object value)
        {
            DsNameMatch ds = new DsNameMatch();

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "declare @ids table(id uniqueidentifier); " +
                    "insert @ids select distinct n.NameID from cons.Name n inner join cons.NameProperty np on np.NameID = n.NameID " +
                    " inner join dbo.NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID and ncp.Name = '" + field + "' " +
                    " where np.Value = '" + value.ToString() + "'; " +
                    "select n.* from cons.Name n inner join @ids i on i.id = n.NameID; " +
                    "select np.*, ncp.Name from cons.NameProperty np inner join @ids i on i.id = np.NameID inner join dbo.NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID; " +
                    "select c.* from vwConsensusConcepts c inner join @ids i on i.id = c.NameID; ";

                DataSet res = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(res);

                foreach (DataRow row in res.Tables[0].Rows)
                {
                    Guid id = (Guid)row["NameID"];

                    DataRow ntRow = ConsensusName.GetNameConcept(id, res.Tables[2], ConceptProperties.ParentRelationshipType);
                    object nameTo = DBNull.Value;
                    if (ntRow != null && ntRow["NameToID"] != DBNull.Value) nameTo = (Guid)ntRow["NameToID"];

                    ds.Name.Rows.Add(id,
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Canonical),
                        row["FullName"],
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Rank),
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Authors),
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.CombinationAuthors),
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Year),
                        nameTo,
                        100);
                }
            }

            return ds;
        }

        public static void UpdateFlatNameData(SqlConnection cnn, Guid nameID)
        {
            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "delete cons.FlatName where SeedNameID = '" + nameID.ToString() + "'";
                cmd.ExecuteNonQuery();
            }

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "INSERT cons.FlatName EXEC sprSelect_FlatNameToRoot '" + nameID.ToString() + "'";
                cmd.ExecuteNonQuery();
            }
        }

        public static DsNameMatch GetNamesWithConcept(SqlConnection cnn, String conceptType, Guid nameToID)
        {
            DsNameMatch ds = new DsNameMatch();

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"
	                    declare @ids table(id uniqueidentifier)
                    		
                        insert @ids 
                        select distinct n.NameID 
                        from cons.Name n 
                        inner join vwConsensusConcepts cc on cc.NameID = n.NameID 
                        where Relationship = '" + conceptType + "' and NameToID = '" + nameToID.ToString() + @"'
                        
                        select n.* 
                        from cons.Name n 
                        inner join @ids i on i.id = n.NameID
                        
                        select np.*, ncp.Name 
                        from cons.NameProperty np 
                        inner join @ids i on i.id = np.NameID 
                        inner join dbo.NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID
                                        
	                    select c.* 
	                    from vwConsensusConcepts c 
	                    inner join @ids i on i.id = c.NameID";


                DataSet res = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(res);

                foreach (DataRow row in res.Tables[0].Rows)
                {
                    Guid id = (Guid)row["NameID"];

                    DataRow ntRow = ConsensusName.GetNameConcept(id, res.Tables[2], ConceptProperties.ParentRelationshipType);
                    object nameTo = DBNull.Value;
                    if (ntRow != null && ntRow["NameToID"] != DBNull.Value) nameTo = (Guid)ntRow["NameToID"];

                    ds.Name.Rows.Add(id,
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Canonical),
                        row["FullName"],
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Rank),
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Authors),
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.CombinationAuthors),
                        GetNamePropertyValue(id, res.Tables[1], NameProperties.Year),
                        nameTo,
                        100);
                }
            }

            return ds;
        }

        public static Object GetNamePropertyValue(Guid nameID, System.Data.DataTable namePropDt, String field)
        {
            Object val = DBNull.Value;

            foreach (System.Data.DataRow dr in namePropDt.Rows)
            {
                if (dr["NameID"].ToString() == nameID.ToString() && dr["Name"].ToString() == field)
                {
                    val = dr["Value"];
                    break;
                }
            }

            return val;
        }

        public static System.Data.DataRow GetNameConcept(Guid nameID, System.Data.DataTable conceptsDt, String conceptType)
        {
            System.Data.DataRow r = null;

            foreach (System.Data.DataRow dr in conceptsDt.Rows)
            {
                if (dr["NameID"].ToString() == nameID.ToString() && dr["Relationship"].ToString() == conceptType)
                {
                    r = dr;
                    break;
                }
            }

            return r;
        }

        /// <summary>
        /// Add new consensus name from provider name details
        /// </summary>
        /// <param name="provName"></param>
        /// <returns>new consensus name</returns>
        public static DataSet AddConsensusName(SqlConnection cnn, DsIntegrationName.ProviderNameRow provName)
        {
            Guid nameId = Guid.NewGuid();

            string sql = "insert cons.Name(NameID, AddedDate, FullName, GoverningCode, NameClassID, TaxonRankID) select '" +
                nameId.ToString() + "', '" +
                DateTime.Now.ToString("s") + "', '" +
                provName.FullName.Replace("'","''") + "', '" +
                provName.GoverningCode + "', '" +
                provName.NameClassID.ToString() + "', '" +
                provName.TaxonRankID.ToString() + "'";

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }

            DataSet provDs = ProviderName.GetName(cnn, provName.NameID);

            //properties
            foreach (DataRow tpRow in provDs.Tables["NameProperty"].Rows)
            {
                int seq = -1;
                if (!tpRow.IsNull("Sequence")) seq = (int)tpRow["Sequence"];
                Guid relId = Guid.Empty;
                if (tpRow["RelatedID"] != DBNull.Value) relId = (Guid)tpRow["RelatedID"];

                string val = tpRow["Value"].ToString().Replace("'", "''");

                sql = "insert cons.NameProperty(NamePropertyID, NameID, AddedDate, NameClassPropertyID, Value, Sequence, RelatedID) select '" +
                    Guid.NewGuid().ToString() + "', '" +
                    nameId.ToString() + "', '" +
                    DateTime.Now.ToString("s") + "', '" +
                    tpRow["NameClassPropertyID"].ToString() + "', '" +
                    val + "', " +
                    (seq == -1 ? "null, " : seq.ToString() + ", ") +
                    (relId == Guid.Empty ? "null" : "'" + relId.ToString() + "'");

                using (SqlCommand npCmd = cnn.CreateCommand())
                {
                    npCmd.CommandText = sql;
                    npCmd.ExecuteNonQuery();
                }
            }

            //Update Flat Name data
            UpdateFlatNameData(cnn, nameId);

            return GetName(cnn, nameId);
        }

        public static void UpdateConsensusName(SqlConnection cnn, DsIntegrationName.ConsensusNameRow consName)
        {
            //update parts of provider name that can be updated
            string sql = "update cons.Name set " +
                "FullName = '" + consName.FullName + "', TaxonRankID = '" + consName.TaxonRankID.ToString() + "', NameClassID = '" + consName.NameClassID.ToString() + "', " +
                "GoverningCode = '" + consName.GoverningCode + "', UpdatedDate = '" + DateTime.Now.ToString("s") + "'" +
                "where NameID = '" + consName.NameID.ToString() + "'";

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }

            //properties
            //canoncial
            sql = "update cons.NameProperty set Value = '" + consName.Canonical + "', UpdatedDate = '" + DateTime.Now.ToString("s") + "' where " +
                    "NameID = '" + consName.NameID.ToString() + "' and " +
                    "NameClassPropertyID = '" + NameClass.GetPropertyOfClassType(cnn, consName.NameClassID, NameClassProperty.Canonical).ID.ToString() + "'";

            using (SqlCommand npCmd = cnn.CreateCommand())
            {
                npCmd.CommandText = sql;
                npCmd.ExecuteNonQuery();
            }

            //rank
            sql = "update cons.NameProperty set Value = '" + consName.TaxonRank + "', UpdatedDate = '" + DateTime.Now.ToString("s") + "' where " +
                    "NameID = '" + consName.NameID.ToString() + "' and " +
                    "NameClassPropertyID = '" + NameClass.GetPropertyOfClassType(cnn, consName.NameClassID, NameClassProperty.Rank).ID.ToString() + "'";

            using (SqlCommand npCmd = cnn.CreateCommand())
            {
                npCmd.CommandText = sql;
                npCmd.ExecuteNonQuery();
            }

            //authors
            if (!consName.IsAuthorsNull())
            {
                sql = "update cons.NameProperty set Value = '" + consName.Authors + "', UpdatedDate = '" + DateTime.Now.ToString("s") + "' where " +
                        "NameID = '" + consName.NameID.ToString() + "' and " +
                        "NameClassPropertyID = '" + NameClass.GetPropertyOfClassType(cnn, consName.NameClassID, NameClassProperty.Authors).ID.ToString() + "'";
            }
            else
            {
                sql = "delete cons.NameProperty where NameID = '" + consName.NameID.ToString() + "' and " +
                    "NameClassPropertyID = '" + NameClass.GetPropertyOfClassType(cnn, consName.NameClassID, NameClassProperty.Authors).ID.ToString() + "'";
            }

            using (SqlCommand npCmd = cnn.CreateCommand())
            {
                npCmd.CommandText = sql;
                npCmd.ExecuteNonQuery();
            }

            //year on publication
            if (!consName.IsYearOnPublicationNull())
            {
                sql = "update cons.NameProperty set Value = '" + consName.YearOnPublication + "', UpdatedDate = '" + DateTime.Now.ToString("s") + "' where " +
                        "NameID = '" + consName.NameID.ToString() + "' and " +
                        "NameClassPropertyID = '" + NameClass.GetPropertyOfClassType(cnn, consName.NameClassID, NameClassProperty.YearOnPublication).ID.ToString() + "'";
            }
            else
            {
                sql = "delete cons.NameProperty where NameID = '" + consName.NameID.ToString() + "' and " +
                    "NameClassPropertyID = '" + NameClass.GetPropertyOfClassType(cnn, consName.NameClassID, NameClassProperty.YearOnPublication).ID.ToString() + "'";
            }

            using (SqlCommand npCmd = cnn.CreateCommand())
            {
                npCmd.CommandText = sql;
                npCmd.ExecuteNonQuery();
            }

            //basionym
            if (!consName.IsBasionymNull())
            {
                sql = "update cons.NameProperty set Value = '" + consName.Basionym + "', UpdatedDate = '" + DateTime.Now.ToString("s") + "', " +
                    "RelatedID = " + (consName.IsBasionymIDNull() ? "null, " : "'" + consName.BasionymID.ToString() + "', ") + " where " +
                        "NameID = '" + consName.NameID.ToString() + "' and " +
                        "NameClassPropertyID = '" + NameClass.GetPropertyOfClassType(cnn, consName.NameClassID, NameClassProperty.Basionym).ID.ToString() + "'";
            }
            else
            {
                sql = "delete cons.NameProperty where NameID = '" + consName.NameID.ToString() + "' and " +
                    "NameClassPropertyID = '" + NameClass.GetPropertyOfClassType(cnn, consName.NameClassID, NameClassProperty.Basionym).ID.ToString() + "'";
            }
            using (SqlCommand npCmd = cnn.CreateCommand())
            {
                npCmd.CommandText = sql;
                npCmd.ExecuteNonQuery();
            }

            //published in
            if (!consName.IsPublishedInNull())
            {
                sql = "update cons.NameProperty set Value = '" + consName.PublishedIn + "', UpdatedDate = '" + DateTime.Now.ToString("s") + "' where " +
                        "NameID = '" + consName.NameID.ToString() + "' and " +
                        "NameClassPropertyID = '" + NameClass.GetPropertyOfClassType(cnn, consName.NameClassID, NameClassProperty.PublishedIn).ID.ToString() + "'";
            }
            else
            {
                sql = "delete cons.NameProperty where NameID = '" + consName.NameID.ToString() + "' and " +
                    "NameClassPropertyID = '" + NameClass.GetPropertyOfClassType(cnn, consName.NameClassID, NameClassProperty.PublishedIn).ID.ToString() + "'";
            }

            using (SqlCommand npCmd = cnn.CreateCommand())
            {
                npCmd.CommandText = sql;
                npCmd.ExecuteNonQuery();
            }

            //microreference
            if (!consName.IsMicroReferenceNull())
            {
                sql = "update cons.NameProperty set Value = '" + consName.MicroReference + "', UpdatedDate = '" + DateTime.Now.ToString("s") + "' where " +
                        "NameID = '" + consName.NameID.ToString() + "' and " +
                        "NameClassPropertyID = '" + NameClass.GetPropertyOfClassType(cnn, consName.NameClassID, NameClassProperty.MicroReference).ID.ToString() + "'";
            }
            else
            {
                sql = "delete cons.NameProperty where NameID = '" + consName.NameID.ToString() + "' and " +
                    "NameClassPropertyID = '" + NameClass.GetPropertyOfClassType(cnn, consName.NameClassID, NameClassProperty.MicroReference).ID.ToString() + "'";
            }

            using (SqlCommand npCmd = cnn.CreateCommand())
            {
                npCmd.CommandText = sql;
                npCmd.ExecuteNonQuery();
            }

            //TODO - orthography, others ???


            //Update Flat Name data
            UpdateFlatNameData(cnn, consName.NameID);
        }

        public static void AddConsensusName(SqlConnection cnn, DsIntegrationName.ConsensusNameRow consName)
        {
            string sql = "insert cons.Name(NameID, AddedDate, FullName, GoverningCode, NameClassID, TaxonRankID) select '" +
                consName.NameID.ToString() + "', '" +
                DateTime.Now.ToString("s") + "', '" +
                consName.FullName.Replace("'", "''") + "', '" +
                consName.GoverningCode + "', '" +
                consName.NameClassID.ToString() + "', '" +
                consName.TaxonRankID.ToString() + "'";

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
            
            //properties            
            //canoncial
            sql = "insert cons.NameProperty(NamePropertyID, NameID, AddedDate, NameClassPropertyID, Value, Sequence, RelatedID) select '" +
                    Guid.NewGuid().ToString() + "', '" +
                    consName.NameID.ToString() + "', '" +
                    DateTime.Now.ToString("s") + "', '" +
                    NameClass.GetPropertyOfClassType(cnn, consName.NameClassID, NameClassProperty.Canonical).ID.ToString() + "', '" +
                    consName.Canonical + "', " +
                    "null, " + //TODO sequence ??
                    "null"; 

            using (SqlCommand npCmd = cnn.CreateCommand())
            {
                npCmd.CommandText = sql;
                npCmd.ExecuteNonQuery();
            }

            //rank
            sql = "insert cons.NameProperty(NamePropertyID, NameID, AddedDate, NameClassPropertyID, Value, Sequence, RelatedID) select '" +
                    Guid.NewGuid().ToString() + "', '" +
                    consName.NameID.ToString() + "', '" +
                    DateTime.Now.ToString("s") + "', '" +
                    NameClass.GetPropertyOfClassType(cnn, consName.NameClassID, NameClassProperty.Rank).ID.ToString() + "', '" +
                    consName.TaxonRank + "', " +
                    "null, " + //TODO sequence ??
                    "null";

            using (SqlCommand npCmd = cnn.CreateCommand())
            {
                npCmd.CommandText = sql;
                npCmd.ExecuteNonQuery();
            }

            //authors
            if (!consName.IsAuthorsNull())
            {
                sql = "insert cons.NameProperty(NamePropertyID, NameID, AddedDate, NameClassPropertyID, Value, Sequence, RelatedID) select '" +
                        Guid.NewGuid().ToString() + "', '" +
                        consName.NameID.ToString() + "', '" +
                        DateTime.Now.ToString("s") + "', '" +
                        NameClass.GetPropertyOfClassType(cnn, consName.NameClassID, NameClassProperty.Authors).ID.ToString() + "', '" +
                        consName.Authors + "', " +
                        "null, " + //TODO sequence ??
                        "null";

                using (SqlCommand npCmd = cnn.CreateCommand())
                {
                    npCmd.CommandText = sql;
                    npCmd.ExecuteNonQuery();
                }
            }

            //year on publication
            if (!consName.IsYearOnPublicationNull())
            {
                sql = "insert cons.NameProperty(NamePropertyID, NameID, AddedDate, NameClassPropertyID, Value, Sequence, RelatedID) select '" +
                        Guid.NewGuid().ToString() + "', '" +
                        consName.NameID.ToString() + "', '" +
                        DateTime.Now.ToString("s") + "', '" +
                        NameClass.GetPropertyOfClassType(cnn, consName.NameClassID, NameClassProperty.YearOnPublication).ID.ToString() + "', '" +
                        consName.YearOnPublication + "', " +
                        "null, " + //TODO sequence ??
                        "null";

                using (SqlCommand npCmd = cnn.CreateCommand())
                {
                    npCmd.CommandText = sql;
                    npCmd.ExecuteNonQuery();
                }
            }

            //basionym
            if (!consName.IsBasionymNull())
            {
                sql = "insert cons.NameProperty(NamePropertyID, NameID, AddedDate, NameClassPropertyID, Value, Sequence, RelatedID) select '" +
                        Guid.NewGuid().ToString() + "', '" +
                        consName.NameID.ToString() + "', '" +
                        DateTime.Now.ToString("s") + "', '" +
                        NameClass.GetPropertyOfClassType(cnn, consName.NameClassID, NameClassProperty.Basionym).ID.ToString() + "', '" +
                        consName.Basionym + "', " +
                        "null, " + //TODO sequence ??
                        (consName.IsBasionymIDNull() ? "null" : "'" + consName.BasionymID.ToString() + "'");

                using (SqlCommand npCmd = cnn.CreateCommand())
                {
                    npCmd.CommandText = sql;
                    npCmd.ExecuteNonQuery();
                }
            }
            
            //published in
            if (!consName.IsPublishedInNull())
            {
                sql = "insert cons.NameProperty(NamePropertyID, NameID, AddedDate, NameClassPropertyID, Value, Sequence, RelatedID) select '" +
                        Guid.NewGuid().ToString() + "', '" +
                        consName.NameID.ToString() + "', '" +
                        DateTime.Now.ToString("s") + "', '" +
                        NameClass.GetPropertyOfClassType(cnn, consName.NameClassID, NameClassProperty.PublishedIn).ID.ToString() + "', '" +
                        consName.PublishedIn + "', " +
                        "null, " + //TODO sequence ??
                        "null";

                using (SqlCommand npCmd = cnn.CreateCommand())
                {
                    npCmd.CommandText = sql;
                    npCmd.ExecuteNonQuery();
                }
            }
            
            //microreference
            if (!consName.IsMicroReferenceNull())
            {
                sql = "insert cons.NameProperty(NamePropertyID, NameID, AddedDate, NameClassPropertyID, Value, Sequence, RelatedID) select '" +
                        Guid.NewGuid().ToString() + "', '" +
                        consName.NameID.ToString() + "', '" +
                        DateTime.Now.ToString("s") + "', '" +
                        NameClass.GetPropertyOfClassType(cnn, consName.NameClassID, NameClassProperty.MicroReference).ID.ToString() + "', '" +
                        consName.MicroReference + "', " +
                        "null, " + //TODO sequence ??
                        "null";

                using (SqlCommand npCmd = cnn.CreateCommand())
                {
                    npCmd.CommandText = sql;
                    npCmd.ExecuteNonQuery();
                }
            }

            //TODO - orthography, others ???

            //Update Flat Name data
            UpdateFlatNameData(cnn, consName.NameID);
        }

        public static DataSet GetName(SqlConnection cnn, Guid nameId)
        {
            DataSet ds = null;

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "select * from cons.Name where NameID = '" + nameId.ToString() + "'; select * from cons.NameProperty where NameID = '" + nameId.ToString() + "'";

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
            }
            return ds;
        }

        public static void RefreshConsensusData(Guid consNameID, DsIntegrationName data)
        {
            DsIntegrationName.ConsensusNameRow name = (DsIntegrationName.ConsensusNameRow)(data.ConsensusName.Select("NameID = '" + consNameID.ToString() + "'")[0]);
            DsIntegrationName.ProviderNameRow[] provRecords = (DsIntegrationName.ProviderNameRow[])data.ProviderName.Select("ConsensusNameID = '" + consNameID.ToString() + "'");

            foreach (DataColumn dc in data.ConsensusName.Columns)
            {
                if ("NameID,NameClassID,NameClass,TaxonRankID,TaxonRank,TaxonRankSort,ParentIDsToRoot,".IndexOf(dc.ColumnName + ",") == -1)
                {
                    object val = GetConsensusValue(provRecords, dc.ColumnName);
                    name[dc.ColumnName] = val;
                }
            }
        }

        /// <summary>
        /// Refresh all names that could be linked to this name (eg update Basionym for other names that have this name as their BasionymID)
        /// and all values that this name could be linked to (eg update the Basionym of this name, depending on the BasionymID that this name has)
        /// </summary>
        /// <param name="consNameID"></param>
        /// <param name="data"></param>
        public static void RefreshNameLinks(Guid consNameID, DsIntegrationName data)
        {
            //TODO
        }

        private static object GetConsensusValue(DsIntegrationName.ProviderNameRow[] provRecords, string sourceCol)
        {
            Dictionary<object, int> vals = new Dictionary<object, int>();
            
            object editorVal = DBNull.Value;
            foreach (DataRow row in provRecords)
            {
                //TODO - add editor type records ???
                //if (!row.IsNull("ProviderIsEditor") && (bool)row["ProviderIsEditor"] && !row.IsNull(sourceCol))
                //{
                //    editorVal = row[sourceCol];
                //    break;
                //}

                if (row[sourceCol].ToString().Length > 0)
                {
                    if (vals.ContainsKey(row[sourceCol]))
                    {
                        vals[row[sourceCol]] += 1;
                    }
                    else
                    {
                        if (!object.ReferenceEquals(row[sourceCol], DBNull.Value))
                            vals.Add(row[sourceCol], 1);
                    }
                }
            }

            if (editorVal != DBNull.Value)
                return editorVal;

            //get majority value (must be > majority than next common value, ie if equal number of 2 diff values, then there is no consensus)
            object val = DBNull.Value;
            int maxNum = 0;
            bool hasEqual = false;
            foreach (object key in vals.Keys)
            {
                if (vals[key] > maxNum)
                {
                    maxNum = vals[key];
                    val = key;
                    hasEqual = false;
                }
                else if (vals[key] == maxNum)
                {
                    hasEqual = true;
                }
            }

            //TODO how to handle majority issues??
            //always return something for canonical / full name / rank
            if (sourceCol == "Canonical" || sourceCol == "FullName")
                return val;

            if (hasEqual)
                return DBNull.Value;
            
            return val;
        }
    }
}

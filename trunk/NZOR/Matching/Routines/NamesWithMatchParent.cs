using System.Data.SqlClient;
using System.Data;
using System;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithMatchParent : INameMatcher
    {

        private int m_Id = -1;
        private int m_FailId = -1;
        private int m_SuccessId = -1;
        private int m_Threshold = -1;

        public NamesWithMatchParent()
        {
        }

        public NamesWithMatchParent(int id, int failId, int successId, int threshold)
        {
            m_Id = id;
            m_FailId = failId;
            m_SuccessId = successId;
            m_Threshold = threshold;
        }

        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        public int FailId
        {
            get { return m_FailId; }
            set { m_FailId = value; }
        }

        public int SuccessId
        {
            get { return m_SuccessId; }
            set { m_SuccessId = value; }
        }

        public int Threshold
        {
            get { return m_Threshold; }
            set { m_Threshold = value; }
        }


        public DsNameMatch GetMatchingNames(System.Data.DataSet pn)
        {
            //this routine only adds names
            DsNameMatch names = new DsNameMatch();
            AddMatchingNames(pn, ref names);
            return names;
        }

        public void RemoveNonMatches(System.Data.DataSet pn, ref DsNameMatch names)
        {
            //this routine only adds names, so add names under the "match" parent
            AddMatchingNames(pn, ref names);
        }

        private void AddMatchingNames(System.Data.DataSet pn, ref DsNameMatch names)
        {
            if (names == null) names = new DsNameMatch();

            string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            string fullName = pn.Tables["Name"].Rows[0]["FullName"].ToString();
            string govCode = pn.Tables["Name"].Rows[0]["GoverningCode"].ToString();
            Guid rankId = (Guid)pn.Tables["Name"].Rows[0]["TaxonRankID"];
            NZOR.Data.SystemData.TaxonRank tr = Data.SystemData.TaxonRankData.GetTaxonRank(rankId);

            string parentId = "";

            //Below GENUS - use the Genus (first word of the full name)
            if (tr.SortOrder > 3000)
            {
                if (fullName.IndexOf(" ") != -1)
                {
                    String parent = fullName.Substring(0, fullName.IndexOf(" "));
                                        
                    using (SqlConnection cnn = new SqlConnection(ConnectionString))
                    {
                        cnn.Open();
                        using (SqlCommand cmd = cnn.CreateCommand())
                        {
                            cmd.CommandText = "select n.NameID from cons.Name n inner join cons.nameproperty np on np.nameid = n.nameid "
                                + " inner join dbo.nameclassproperty ncp on ncp.nameclasspropertyid = np.nameclasspropertyid where TaxonRankID = '"
                                + NZOR.Data.SystemData.TaxonRankData.GenusRank().TaxonRankID.ToString() + "' and np.Value = '" + parent + "' and ncp.propertyname = '"
                                + NZOR.Data.NameProperties.Canonical + "' and n.GoverningCode = '" + govCode + "'";

                            DataSet ds = new DataSet();
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(ds);

                            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count == 1)
                            {
                                parentId = ds.Tables[0].Rows[0]["NameID"].ToString();
                            }
                        }

                        if (cnn.State != System.Data.ConnectionState.Closed) cnn.Close();
                    }
                }
            }


            //TODO - more smarts at more ranks


            if (parentId != "")
            {
                using (SqlConnection cnn = new SqlConnection(ConnectionString))
                {
                    cnn.Open();

                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = @"                       
                            select n.* 
                            from cons.Name n 
                            inner join cons.FlatName fn on fn.NameID = n.NameID
                            where fn.ParentNameID = '" + parentId + "'; " + @"                            
                            select np.*, ncp.PropertyName 
                            from cons.NameProperty np  
                            inner join cons.FlatName fn on fn.NameID = np.NameID
                            inner join dbo.NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID
                            where fn.ParentNameID = '" + parentId + "'; " + @"                                            
                            select c.* 
                            from vwConsensusConcepts c  
                            inner join cons.FlatName fn on fn.NameID = c.NameID
                            where fn.ParentNameID = '" + parentId + "';";

                        DataSet res = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(res);

                        foreach (DataRow row in res.Tables[0].Rows)
                        {
                            Guid id = (Guid)row["NameID"];

                            if (names.Name.Select("NameID = '" + id + "'").Length == 0)
                            {
                                DataRow ntRow = ConsensusName.GetNameConcept(id, res.Tables[2], ConceptProperties.ParentRelationshipType);
                                object nameTo = DBNull.Value;
                                if (ntRow != null && ntRow["NameToID"] != DBNull.Value) nameTo = (Guid)ntRow["NameToID"];

                                names.Name.Rows.Add(new object[]{id,
                                    ConsensusName.GetNamePropertyValue(id, res.Tables[1], NameProperties.Canonical),
                                    row["FullName"],
                                    ConsensusName.GetNamePropertyValue(id, res.Tables[1], NameProperties.Rank),
                                    ConsensusName.GetNamePropertyValue(id, res.Tables[1], NameProperties.Authors),
                                    ConsensusName.GetNamePropertyValue(id, res.Tables[1], NameProperties.CombinationAuthors),
                                    ConsensusName.GetNamePropertyValue(id, res.Tables[1], NameProperties.Year),
                                    nameTo,
                                    100});
                            }
                        }
                    }

                    if (cnn.State != System.Data.ConnectionState.Closed) cnn.Close();
                }
            }

        }

    }
}

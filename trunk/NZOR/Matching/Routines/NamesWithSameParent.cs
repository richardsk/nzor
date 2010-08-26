using System;
using System.Data;
using System.Data.SqlClient;

using NZOR.Data;

namespace NZOR.Matching
{
    public class NamesWithSameParent : INameMatcher
    {

        private int m_Id = -1;
        private int m_FailId = -1;
        private int m_SuccessId = -1;
        private int m_Threshold = -1;

        public NamesWithSameParent()
        {
        }

        public NamesWithSameParent(int id, int failId, int successId, int threshold)
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
            //This routine makes sure that all names that have been selected for possible match are at the correct rank.
            //  Names that may be selected could be children of the same parent as the matching name, but this does not mean the names will be the 
            //  same rank - eg matching name "Aus bus var. dus" - may select a name in the DB "Aus bus subsp. cus" as "cus" is a child of the naem "Aus bus"            

            System.Data.DataRow parRow = NZOR.Data.ProviderName.GetNameConcept(pn.Tables["Concepts"], NZOR.Data.ConceptProperties.ParentRelationshipType);

            DsNameMatch ds = new DsNameMatch();

            if (parRow != null && !parRow.IsNull("ConsensusNameToID"))
            {
                System.Guid parentId = (System.Guid)parRow["ConsensusNameToID"];
                String rankId = pn.Tables["Name"].Rows[0]["TaxonRankID"].ToString();

                string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
                using (SqlConnection cnn = new SqlConnection(ConnectionString))
                {
                    cnn.Open();

                    //ds = NZOR.Data.ConsensusName.GetNamesWithConcept(cnn, NZOR.Data.ConceptProperties.ParentRelationshipType, parentId);


                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            declare @ids table(id uniqueidentifier)
		
                            insert @ids 
                            select distinct SeedNameID 
                            from cons.FlatName fn 
                            inner join cons.name sn on sn.nameid = fn.seednameid
                            where sn.TaxonRankID = '" + rankId + "' and fn.NameId = '" + parentId.ToString() + "';" +
                           @"                            
                            select n.* 
                            from cons.Name n 
                            inner join @ids i on i.id = n.NameID;
                            
                            select np.*, ncp.PropertyName 
                            from cons.NameProperty np 
                            inner join @ids i on i.id = np.NameID 
                            inner join dbo.NameClassProperty ncp on ncp.NameClassPropertyID = np.NameClassPropertyID;
                                            
                            select c.* 
                            from vwConsensusConcepts c 
                            inner join @ids i on i.id = c.NameID;";

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
                                ConsensusName.GetNamePropertyValue(id, res.Tables[1], NameProperties.Canonical),
                                row["FullName"],
                                ConsensusName.GetNamePropertyValue(id, res.Tables[1], NameProperties.Rank),
                                ConsensusName.GetNamePropertyValue(id, res.Tables[1], NameProperties.Authors),
                                ConsensusName.GetNamePropertyValue(id, res.Tables[1], NameProperties.CombinationAuthors),
                                ConsensusName.GetNamePropertyValue(id, res.Tables[1], NameProperties.Year),
                                nameTo,
                                100);
                        }
                    }


                    if (cnn.State != System.Data.ConnectionState.Closed) cnn.Close();
                }
            }

            return ds;
        }

        public void RemoveNonMatches(DataSet pn, ref DsNameMatch names)
        {
            System.Data.DataRow parRow = NZOR.Data.ProviderName.GetNameConcept(pn.Tables["Concepts"], NZOR.Data.ConceptProperties.ParentRelationshipType);

            DsNameMatch ds = new DsNameMatch();

            if (!parRow.IsNull("ConsensusNameToID"))
            {
                System.Guid parentId = (System.Guid)parRow["ConsensusNameToID"];

                for (int i = names.Name.Count - 1; i >= 0; i--)
                {
                    DsNameMatch.NameRow nmRow = names.Name[i];
                    if (nmRow.ParentID != parentId) nmRow.Delete();
                }
            }

            ds.AcceptChanges();
        }
    }
}

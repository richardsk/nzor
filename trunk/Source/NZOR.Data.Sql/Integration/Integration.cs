using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NZOR.Data.DataSets;
using NZOR.Data.Entities.Common;
using NZOR.Data.Entities.Integration;
using NZOR.Data.LookUps.Common;
using NZOR.Data.Sql.Repositories.Common;
using NZOR.Admin.Data.Datasets;
using System.Threading.Tasks;

namespace NZOR.Data.Sql
{
    public class Integration
    {
        public static int Progress = 0;

        private static Dictionary<Guid, List<Entities.Common.NamePropertyType>> _namePropTypes = new Dictionary<Guid, List<Entities.Common.NamePropertyType>>();
        private static List<Entities.Common.ReferencePropertyType> _refPropTypes = null;
        private static List<Entities.Common.ConceptApplicationType> _conceptAppTypes = null;
        private static List<Entities.Common.ConceptRelationshipType> _conceptRelTypes = null;
        private static TaxonRankLookUp _rankLookup = null;

        #region "Integration By SQL"

        public static void ClearConsensusData(String cnnStr)
        {
            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = NZOR.Data.Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Integration.ClearConsensusData.sql");
                    cmd.CommandTimeout = 10000;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void ClearProviderData(String cnnStr)
        {
            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = NZOR.Data.Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Integration.ClearProviderData.sql");
                    cmd.CommandTimeout = 10000;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void ClearProviderData(String cnnStr, Guid dataSourceId)
        {
            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = NZOR.Data.Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Integration.ClearProviderDataForDS.sql");
                    cmd.Parameters.AddWithValue("@dataSourceId", dataSourceId);
                    cmd.CommandTimeout = 10000;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void GenerateConsensusBackbone(String cnnStr)
        {
            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();
                //run backbone integration
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = NZOR.Data.Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Integration.InsertConsensusBackbone.sql");
                    cmd.CommandTimeout = 200000;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void ProcessProviderConceptConflicts(string cnnStr)
        {
            string sql = NZOR.Data.Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Integration.ProcessConceptConflicts.sql");

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandTimeout = 100000;
                    cmd.ExecuteNonQuery();
                }
            }

            //Admin.Data.Repositories.IProviderRepository pr = new Admin.Data.Sql.Repositories.ProviderRepository(adminCnnStr);
            //List<Admin.Data.Entities.AttachmentPointDataSource> attPointDS = pr.GetAttachmentPointDataSources();

            //Data.Repositories.Consensus.IConceptRepository ccr = new Data.Sql.Repositories.Consensus.ConceptRepository(cnnStr);
            //Data.Repositories.Provider.IConceptRepository pcr = new Data.Sql.Repositories.Provider.ConceptRepository(cnnStr);

            ////find conflicts

            //DataTable tbl = NZOR.Data.Sql.Utility.GetSourceData(cnnStr, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Integration.SelectProviderConceptConflicts.sql"), null, 10000);
            //foreach (DataRow row in tbl.Rows)
            //{
            //    Guid nameId = (Guid)row["NameID"];
            //    Guid dsId1 = (Guid)row["DataSource1"];
            //    Guid dsId2 = (Guid)row["DataSource2"];
            //    Guid relTypeId = (Guid)row["ConceptRelationshipTypeID"];
            //    Guid? accToId1 = (Guid?)(row.IsNull("AccordingToID1") ? null : row["AccordingToID1"]);
            //    Guid? accToId2 = (Guid?)(row.IsNull("AccordingToID2") ? null : row["AccordingToID2"]);

            //    List<Data.Entities.Provider.Concept> concepts = pcr.GetProviderConceptsByName(nameId);

            //    ConsensusValueResult cvr = GetConsensusValue(concepts, relTypeId, ccr);

            //    if (cvr.HasMajority)
            //    {
            //        //set to consval
            //        Guid? accToId = null;
            //        if (cvr.Value == row["NameTo1"]) accToId = accToId1;
            //        if (cvr.Value == row["NameTo2"]) accToId = accToId2;

            //        ccr.SetInUseConcept(nameId, relTypeId, ((ConceptVal)cvr.Value).FromAccordingToId, ((ConceptVal)cvr.Value).ToNameId);
            //    }
            //    else
            //    {
            //        //conflict
            //        Guid prefDsId = GetRankedDataSource(cnnStr, nameId, dsId1, dsId2);

            //        if (prefDsId != Guid.Empty)
            //        {

            //            //get value to use
            //            object val = null;
            //            object seq = null;
            //            object accToId = null;
            //            if (dsId1 == prefDsId)
            //            {
            //                val = row["NameTo1"];
            //                seq = row["Sequence1"];
            //                accToId = accToId1;
            //            }
            //            else if (dsId2 == prefDsId)
            //            {
            //                val = row["NameTo2"];
            //                seq = row["Sequence2"];
            //                accToId = accToId2;
            //            }
            //            else //no preferred - take first - TODO check
            //            {
            //                val = row["NameTo1"];
            //                seq = row["Sequence1"];
            //                accToId = accToId1;
            //            }

            //            ccr.SetInUseConcept(nameId, relTypeId, (Guid?)accToId, (Guid?)val);
            //        }
            //    }
            //}

        }

        public static string LoadXslt(string cnnStr, string xsltName)
        {
            string xslt = "";

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "select convert(nvarchar(max), xslt) from dbo.Transformation where name ='" + xsltName + "'";
                    xslt = cmd.ExecuteScalar().ToString();
                }
            }

            return xslt;
        }

        public static void UpdateConsensusFullNameValues(string cnnStr)
        {

            Data.Repositories.Consensus.INameRepository nr = new Data.Sql.Repositories.Consensus.NameRepository(cnnStr);
            List<Guid> nameIds = nr.GetAllNames();

            string fullNameXslt = LoadXslt(cnnStr, "NameText_FullName");
            string fullNameFmtXslt = LoadXslt(cnnStr, "NameText_FullNameFormatted");
            string partialNameXslt = LoadXslt(cnnStr, "NameText_PartialName");
            string partialNameFmtXslt = LoadXslt(cnnStr, "NameText_PartialNameFormatted");

            foreach (Guid id in nameIds)
            {
                Data.Entities.Consensus.Name n = nr.GetName(id);
                UpdateConsensusFullName(n, fullNameXslt, fullNameFmtXslt, partialNameXslt, partialNameFmtXslt);
                nr.Names.Add(n);
            }

            nr.Save();
        }

        public static void UpdateConsensusFullName(string cnnStr, Guid nameId)
        {
            Data.Repositories.Consensus.INameRepository nr = new Data.Sql.Repositories.Consensus.NameRepository(cnnStr);
            Data.Entities.Consensus.Name n = nr.GetName(nameId);

            string fullNameXslt = LoadXslt(cnnStr, "NameText_FullName");
            string fullNameFmtXslt = LoadXslt(cnnStr, "NameText_FullNameFormatted");
            string partialNameXslt = LoadXslt(cnnStr, "NameText_PartialName");
            string partialNameFmtXslt = LoadXslt(cnnStr, "NameText_PartialNameFormatted");

            UpdateConsensusFullName(n, fullNameXslt, fullNameFmtXslt, partialNameXslt, partialNameFmtXslt);

            nr.Names.Add(n);
            nr.Save();
        }

        private static void UpdateConsensusFullName(Data.Entities.Consensus.Name n, string fullNameXslt, string fullNameFmtXslt, string partialNameXslt, string partialNameFmtXslt)
        {
            if (n.NameClassId.ToString().ToUpper() == "A5233111-61A0-4AE6-9C2B-5E8E71F1473A")
            {
                string val = Utility.ApplyXSLT(n.FullName, fullNameXslt, false);                
                AddNameProperty(n, NamePropertyTypeLookUp.NameTextFullName, new Guid("86E7590B-EF34-4E19-970B-608703B858A5"), val);

                val = Utility.ApplyXSLT(n.FullName, fullNameFmtXslt, false);
                val = System.Web.HttpUtility.HtmlDecode(val);
                AddNameProperty(n, NamePropertyTypeLookUp.NameTextFullNameFormatted, new Guid("86B84828-E1C0-45BD-A5C0-7B272EDC97EF"), val);

                val = Utility.ApplyXSLT(n.FullName, partialNameXslt, false);                
                AddNameProperty(n, NamePropertyTypeLookUp.NameTextPartialName, new Guid("00806321-C8BD-4518-9539-1286DA02CA7D"), val);

                val = Utility.ApplyXSLT(n.FullName, partialNameFmtXslt, false);
                val = System.Web.HttpUtility.HtmlDecode(val);
                AddNameProperty(n, NamePropertyTypeLookUp.NameTextPartialNameFormatted, new Guid("F721F463-5F16-4333-9C7D-DDF848F2D1A9"), val);
            }
            else if (n.NameClassId.ToString().ToUpper() == "3D3A13B8-C673-459C-B98D-8A5B08E3CA44")
            {
                string val = Utility.ApplyXSLT(n.FullName, fullNameXslt, false);
                AddNameProperty(n, NamePropertyTypeLookUp.NameTextFullName, new Guid("C4954CF2-6A07-469B-B470-2D56E60C6666"), val);
            }
            else if (n.NameClassId.ToString().ToUpper() == "05BCC19C-27E8-492C-8ADD-EC5F73325BC5")
            {
                string val = Utility.ApplyXSLT(n.FullName, fullNameXslt, false);
                AddNameProperty(n, NamePropertyTypeLookUp.NameTextFullName, new Guid("88020F95-1282-4D9A-819A-0973F7F50284"), val);
            }

            n.State = Entities.Entity.EntityState.Modified;
        }

        private static void AddNameProperty(Data.Entities.Consensus.Name n, string namePropertyType, Guid namePropertyTypeId, string value)
        {

            Data.Entities.Consensus.NameProperty nameProperty = n.GetNameProperty(namePropertyType);
            if (nameProperty == null)
            {
                nameProperty = new Entities.Consensus.NameProperty();
                nameProperty.State = Entities.Entity.EntityState.Added;
                nameProperty.NamePropertyId = Guid.NewGuid();
                n.NameProperties.Add(nameProperty);
            }
            else
            {
                nameProperty.State = Entities.Entity.EntityState.Modified;
            }

            nameProperty.NamePropertyTypeId = namePropertyTypeId;
            nameProperty.NamePropertyType = namePropertyType;

            nameProperty.Value = value;
        }

        public static void RemoveNamesWithNoProvider(string cnnStr)
        {
            Data.Repositories.Consensus.INameRepository nr = new Data.Sql.Repositories.Consensus.NameRepository(cnnStr);

            using (SqlConnection acnn = new SqlConnection(cnnStr))
            {
                acnn.Open();

                using (SqlCommand cmd = acnn.CreateCommand())
                {
                    cmd.CommandText = "select cn.NameID from consensus.Name cn left join provider.Name pn on pn.ConsensusNameID = cn.NameID where pn.NameID is null and cn.NameID <> '7C087DE1-FD0C-4997-8874-06D61D7CB244'";
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        nr.DeleteName((Guid)row["NameId"], null);
                    }
                }
            } 
        }

        public static void ProcessProviderDataConflicts(string cnnStr)
        {
            //Admin.Data.Repositories.IProviderRepository pr = new Admin.Data.Sql.Repositories.ProviderRepository(adminCnnStr);
            //List<Admin.Data.Entities.AttachmentPointDataSource> attPointDS = pr.GetAttachmentPointDataSources();

            Data.Repositories.Consensus.INameRepository nr = new Data.Sql.Repositories.Consensus.NameRepository(cnnStr);
            Data.Repositories.Provider.INameRepository pnr = new Data.Sql.Repositories.Provider.NameRepository(cnnStr);

            //find conflicts

            DataTable tbl = NZOR.Data.Sql.Utility.GetSourceData(cnnStr, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Integration.SelectProviderDataConflicts.sql"), null, 10000);
            foreach (DataRow row in tbl.Rows)
            {
                Guid nameId = (Guid)row["NameID"];
                Guid dsId1 = (Guid)row["DataSource1"];
                Guid dsId2 = (Guid)row["DataSource2"];
                Guid propTypeId = (Guid)row["NamePropertyTypeID"];

                List<Data.Entities.Provider.NameProperty> props = pnr.GetNamePropertiesForConsensusName(nameId, propTypeId);

                ConsensusValueResult cvr = GetConsensusValue(props, propTypeId);

                if (cvr.HasMajority)
                {
                    //set to consval
                    nr.SetNamePropertyValue(nameId, propTypeId, cvr.Value, cvr.Sequence, cvr.RelatedId);
                }
                else
                {
                    //conflict
                    Guid prefDsId = GetRankedDataSource(cnnStr, nameId, dsId1, dsId2);

                    if (prefDsId != Guid.Empty)
                    {

                        //get value to use
                        object val = null;
                        object seq = null;
                        object relId = null;
                        if (dsId1 == prefDsId)
                        {
                            val = row["Value1"];
                            seq = row["Sequence1"];
                            relId = row["RelatedID1"];
                        }
                        else if (dsId2 == prefDsId)
                        {
                            val = row["Value2"];
                            seq = row["Sequence2"];
                            relId = row["RelatedID2"];
                        }
                        else //no preferred - take first - TODO check
                        {
                            val = row["Value1"];
                            seq = row["Sequence1"];
                            relId = row["RelatedID1"];
                        }

                        //update property value
                        nr.SetNamePropertyValue(nameId, propTypeId, val, seq, relId);
                    }
                }
            }
        }

        /// <summary>
        /// Return the datasource that is the "preferred" data source by ranking, for this name.  Look up the hierarchy to find the first preferred datasource.
        /// </summary>
        /// <param name="nameId"></param>
        /// <param name="dataSourceID1"></param>
        /// <param name="dataSourceID2"></param>
        /// <returns></returns>
        public static Guid GetRankedDataSource(String cnnStr, Guid nameId, Guid dataSourceID1, Guid dataSourceID2)
        {
            Guid dsID = Guid.Empty;

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@nameId", nameId));

            DataTable tbl = NZOR.Data.Sql.Utility.GetSourceData(cnnStr, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Integration.GetPreferredDataSource.sql"), parameters);
            if (tbl != null)
            {
                foreach (DataRow row in tbl.Rows)
                {
                    if ((Guid)row["DataSourceID"] == dataSourceID1)
                    {
                        dsID = dataSourceID1;
                        break;
                    }
                    if ((Guid)row["DataSourceID"] == dataSourceID2)
                    {
                        dsID = dataSourceID2;
                        break;
                    }
                }
            }

            return dsID;
        }


        #endregion

        #region "Integration Data"
        /// <summary>
        /// Dataset file in binary format
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DataForIntegration LoadDataFile(string filePath)
        {
            DataForIntegration data = new DataForIntegration();

            //data.RemotingFormat = SerializationFormat.Binary;

            //IFormatter fmt = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            //System.IO.Stream str = new System.IO.FileStream(filePath, System.IO.FileMode.Open);
            //data = (DsIntegrationName)fmt.Deserialize(str);
            //str.Close();

            System.IO.Stream str = new System.IO.FileStream(filePath, System.IO.FileMode.Open);
            System.Xml.Serialization.XmlSerializer ds = new System.Xml.Serialization.XmlSerializer(typeof(DataForIntegration));
            data = (DataForIntegration)ds.Deserialize(str);
            str.Close();

            //data.ReadXml(filePath);

            if (data.ConsensusData != null) data.ConsensusData.AcceptChanges();
            if (data.References != null) data.References.AcceptChanges();
            if (data.AttachmentPoints != null) data.AttachmentPoints.AcceptChanges();
            if (data.SingleNamesSet != null) data.SingleNamesSet.AcceptChanges();
            if (data.NamesByRank != null)
            {
                foreach (List<DsIntegrationName> dsList in data.NamesByRank.Values)
                {
                    foreach (DsIntegrationName dsn in dsList)
                    {
                        dsn.AcceptChanges();
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Gets grouped data for an integration run.  Both provider and consensus records that are need for matching and integration.  
        /// Names must be integrated by Rank so the return double List is a grid of Rank by Group (Null rank == any rank)
        /// First dataset returned is for the references.
        /// </summary>
        /// <returns></returns>
        public static DataForIntegration GetGroupedDataForIntegration(IntegrationDataGroup group, String cnnStr, Guid? dataSourceId)
        {
            DataForIntegration data = new DataForIntegration(IntegrationDatasetType.NamesByRank);

            //attachment points
            data.AttachmentPoints = new DsAttachmentPoint();
            using (SqlConnection acnn = new SqlConnection(cnnStr))
            {
                acnn.Open();

                using (SqlCommand cmd = acnn.CreateCommand())
                {
                    cmd.CommandText = "select * from [admin].AttachmentPoint";

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.TableMappings.Add("Table", "AttachmentPoint");
                    da.Fill(data.AttachmentPoints);
                }
            }

            NZOR.Data.Sql.Repositories.Common.LookUpRepository lr = new LookUpRepository(cnnStr);
            List<TaxonRank> ranks = lr.GetTaxonRanks();

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                //references
                data.References = new DsIntegrationReference();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Reference-INTEGRATION.sql");
                    cmd.Parameters.Add("@dataSourceId", SqlDbType.UniqueIdentifier).Value = (object)dataSourceId ?? DBNull.Value;

                    cmd.CommandTimeout = 300000;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.TableMappings.Add("Table", "ProviderReference");

                    da.Fill(data.References);
                }

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Reference-INTEGRATION.sql");

                    cmd.CommandTimeout = 300000;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.TableMappings.Add("Table", "ConsensusReference");

                    da.Fill(data.References);
                }

                //get names above family
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Name-INTEGRATION-AboveFamily.sql");

                    cmd.Parameters.Add("@dataSourceId", SqlDbType.UniqueIdentifier).Value = (object)dataSourceId ?? DBNull.Value;

                    cmd.CommandTimeout = 300000;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.TableMappings.Add("Table", "ProviderName");

                    List<DsIntegrationName> dsList = new List<DsIntegrationName>();
                    DsIntegrationName dsn = new DsIntegrationName();

                    da.Fill(dsn);

                    if (dsn.ProviderName.Count > 0)
                    {
                        dsList.Add(dsn);
                        data.NamesByRank.Add(Guid.Empty, dsList);
                    }
                }

                foreach (TaxonRank tr in ranks)
                {
                    if (tr.SortOrder >= 2000) //only family and below
                    {
                        List<DsIntegrationName> dsList = new List<DsIntegrationName>();

                        if (group == IntegrationDataGroup.FirstCharacterOfTaxonName)
                        {
                            DsIntegrationName namesAtX = new DsIntegrationName();

                            //names and concepts by letter
                            for (int charIndex = 65; charIndex < 91; charIndex++)
                            {
                                DsIntegrationName ds = new DsIntegrationName();

                                //for letter X - merge these with the hybrids and 'other' names
                                if ((char)charIndex == 'X')
                                {
                                    using (SqlCommand cmd = cnn.CreateCommand())
                                    {

                                        cmd.CommandText = Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Name-INTEGRATION-NonCharactersAtRank.sql");

                                        cmd.Parameters.Add("@dataSourceId", SqlDbType.UniqueIdentifier).Value = (object)dataSourceId ?? DBNull.Value;
                                        cmd.Parameters.Add("@rankId", SqlDbType.UniqueIdentifier).Value = tr.TaxonRankId;

                                        cmd.CommandTimeout = 300000;
                                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                                        da.TableMappings.Add("Table", "ProviderName");

                                        da.Fill(namesAtX);
                                    }
                                }
                                else
                                {
                                    using (SqlCommand cmd = cnn.CreateCommand())
                                    {

                                        cmd.CommandText = Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Name-INTEGRATION-FirstCharacterAtRank.sql");

                                        cmd.Parameters.Add("@dataSourceId", SqlDbType.UniqueIdentifier).Value = (object)dataSourceId ?? DBNull.Value;
                                        cmd.Parameters.Add("@character", SqlDbType.NVarChar).Value = (char)charIndex;
                                        cmd.Parameters.Add("@rankId", SqlDbType.UniqueIdentifier).Value = tr.TaxonRankId;

                                        cmd.CommandTimeout = 300000;
                                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                                        da.TableMappings.Add("Table", "ProviderName");

                                        da.Fill(ds);
                                    }
                                }

                                if (ds.ProviderName.Count > 0) dsList.Add(ds);
                            }

                            //add X/hybrids last as they may match any other lettered name
                            if (namesAtX != null && namesAtX.ProviderName.Count > 0) dsList.Add(namesAtX);
                        }

                        data.NamesByRank.Add(tr.TaxonRankId, dsList);
                    }
                }

                GetConsensusNameDataForIntegration(cnnStr, ref data);
            }

            return data;
        }

        /// <summary>
        /// Gets all data for an integration run.  Both provider and consensus records that are need for matching and integration.  
        /// /// Names must be integrated by Rank so the return double List is a grid of Rank by Group (Null rank == any rank)
        /// </summary>
        /// <param name="cnnStr"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public static DataForIntegration GetAllDataForIntegration(String cnnStr, Guid? dataSourceId)
        {
            DataForIntegration dfi = new DataForIntegration(IntegrationDatasetType.SingleNamesList);

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                //names and concepts
                dfi.SingleNamesSet = new DsIntegrationName();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Name-INTEGRATION.sql");

                    cmd.Parameters.Add("@dataSourceId", SqlDbType.UniqueIdentifier).Value = (object)dataSourceId ?? DBNull.Value;

                    cmd.CommandTimeout = 300000;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.TableMappings.Add("Table", "ProviderName");

                    da.Fill(dfi.SingleNamesSet);
                }

                //references
                dfi.References = new DsIntegrationReference();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Provider.Reference-INTEGRATION.sql");

                    cmd.Parameters.Add("@dataSourceId", SqlDbType.UniqueIdentifier).Value = (object)dataSourceId ?? DBNull.Value;

                    cmd.CommandTimeout = 300000;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.TableMappings.Add("Table", "ProviderReference");

                    da.Fill(dfi.References);
                }

                //ref consensus data
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Reference-INTEGRATION.sql");

                    cmd.CommandTimeout = 300000;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.TableMappings.Add("Table", "ConsensusReference");

                    da.Fill(dfi.References);
                }
            }

            //cons names
            GetConsensusNameDataForIntegration(cnnStr, ref dfi);

            //attachment points
            dfi.AttachmentPoints = new DsAttachmentPoint();
            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "select * from [admin].AttachmentPoint";

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.TableMappings.Add("Table", "AttachmentPoint");

                    da.Fill(dfi.AttachmentPoints);
                }
            }

            return dfi;
        }

        /// <summary>
        /// Get consensus name data for integrating provider data.  Includes attachment points.
        /// </summary>
        /// <param name="cnnStr"></param>
        /// <param name="dfi"></param>
        public static void GetConsensusNameDataForIntegration(String cnnStr, ref DataForIntegration dfi)
        {
            if (dfi == null) dfi = new DataForIntegration(IntegrationDatasetType.SingleNamesList);

            dfi.ConsensusData = new DsConsensusData();

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = Sql.Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Name-INTEGRATION.sql");
                    cmd.CommandTimeout = 500;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.TableMappings.Add("Table", "ConsensusName");

                    da.Fill(dfi.ConsensusData);

                    //if no parent concept, get "fuzzy" match parents
                    // -- do during integration
                    //GetParentDataAll(cnn, ds);
                }
            }

            if (dfi.AttachmentPoints == null)
            {
                dfi.AttachmentPoints = GetAttachmentPoints(cnnStr);
            }
        }

        public static DsAttachmentPoint GetAttachmentPoints(string cnnStr)
        {
            DsAttachmentPoint ap = new DsAttachmentPoint();
            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "select * from [admin].AttachmentPoint";

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.TableMappings.Add("Table", "AttachmentPoint");

                    da.Fill(ap);
                }
            }
            return ap;
        }

        public static void PostIntegrationCleanup(String cnnStr)
        {
            //reset all provider records with status "Integrating", to "Unmatched"
            //TODO other stuff?

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {                
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandTimeout = 500000;
                    cmd.CommandText = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Integration.PostIntegrationCleanup.sql");

                    //cmd.CommandText = "update provider.Name set LinkStatus = 'Unmatched', MatchScore = null, MatchPath = null, ConsensusNameID = null " +
                    //    "where LinkStatus = 'Integrating'";

                    //cmd.ExecuteNonQuery();

                    //cmd.CommandText = "update provider.Concept set LinkStatus = 'Unmatched', MatchScore = null, ConsensusConceptID = null " +
                    //    "where LinkStatus = 'Integrating'";

                    //cmd.ExecuteNonQuery();

                    //cmd.CommandText = "update provider.Reference set LinkStatus = 'Unmatched', MatchScore = null, ConsensusReferenceID = null " +
                    //    "where LinkStatus = 'Integrating'";

                    //cmd.ExecuteNonQuery();

                    //cmd.CommandText = "update provider.TaxonProperty set LinkStatus = 'Unmatched', MatchScore = null, ConsensusTaxonPropertyID = null " +
                    //    "where LinkStatus = 'Integrating'";

                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region "Save"
        public static void SaveDataFile(DataForIntegration data, string filePath)
        {
            //data.RemotingFormat = SerializationFormat.Binary;
            //IFormatter fmt = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            //System.IO.Stream str = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
            //fmt.Serialize(str, data);
            //str.Flush();
            //str.Close();

            System.IO.Stream str = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
            System.Xml.Serialization.XmlSerializer s = new System.Xml.Serialization.XmlSerializer(typeof(DataForIntegration));
            s.Serialize(str, data);
            str.Flush();
            str.Close();

            //data.WriteXml(filePath);
        }

        private static List<NamePropertyType> NamePropertyTypes(String cnnStr, Guid nameClassId)
        {
            if (!_namePropTypes.ContainsKey(nameClassId))
            {
                LookUpRepository lookUpRepository = new LookUpRepository(cnnStr);
                _namePropTypes.Add(nameClassId, lookUpRepository.GetNamePropertyTypes(nameClassId));
            }
            return _namePropTypes[nameClassId];
        }

        private static List<Entities.Common.ReferencePropertyType> ReferencePropertyTypes(String cnnStr)
        {
            if (_refPropTypes == null)
            {
                LookUpRepository lookUpRepository = new LookUpRepository(cnnStr);
                _refPropTypes = lookUpRepository.GetReferencePropertyTypes();
            }
            return _refPropTypes;
        }

        private static List<Entities.Common.ConceptRelationshipType> ConceptRelationshipTypes(String cnnStr)
        {
            if (_conceptRelTypes == null)
            {
                LookUpRepository lookUpRepository = new LookUpRepository(cnnStr);
                _conceptRelTypes = lookUpRepository.GetConceptRelationshipTypes();
            }
            return _conceptRelTypes;
        }

        private static Entities.Common.ConceptRelationshipType ConceptRelationshipType(String cnnStr, string relationship)
        {
            if (_conceptRelTypes == null)
            {
                LookUpRepository lookUpRepository = new LookUpRepository(cnnStr);
                _conceptRelTypes = lookUpRepository.GetConceptRelationshipTypes();
            }

            Entities.Common.ConceptRelationshipType crType = null;
            foreach (Entities.Common.ConceptRelationshipType crt in ConceptRelationshipTypes(cnnStr))
            {
                if (crt.Relationship.ToLower() == relationship.ToLower())
                {
                    crType = crt;
                    break;
                }
            }

            return crType;
        }

        private static List<Entities.Common.ConceptApplicationType> ConceptApplicationTypes(String cnnStr)
        {
            if (_conceptAppTypes == null)
            {
                LookUpRepository lookUpRepository = new LookUpRepository(cnnStr);
                _conceptAppTypes = lookUpRepository.GetConceptApplicationTypes();
            }
            return _conceptAppTypes;
        }

        private static TaxonRankLookUp TaxonRankLookup(String cnnStr)
        {
            if (_rankLookup == null)
            {
                LookUpRepository lookUpRepository = new LookUpRepository(cnnStr);
                _rankLookup = new TaxonRankLookUp(lookUpRepository.GetTaxonRanks());
            }
            return _rankLookup;
        }

        /// <summary>
        /// Saves all consensus names and provider names that were modified during an integration run
        /// </summary>
        /// <param name="cnnStr"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IntegrationSaveResult SaveIntegrationData(String cnnStr, DataForIntegration data, bool updateStackedNameData)
        {
            Progress = 0;

            //roughly refs + names + concepts + relationships 
            int total = 0;
            if (data.References != null) total += data.References.ProviderReference.Count;
            if (data.ConsensusData != null) total += (data.ConsensusData.ConsensusName.Count * 2);

            if (data.DatasetType == IntegrationDatasetType.NamesByRank)
            {
                foreach (List<DsIntegrationName> dsList in data.NamesByRank.Values)
                {
                    foreach (DsIntegrationName dsn in dsList)
                    {
                        int failCount = dsn.ProviderName.Select("ConsensusNameID is null").Length;
                        total += failCount + (dsn.ProviderName.Count * 2);
                    }
                }
            }
            else
            {
                int failCount = data.SingleNamesSet.ProviderName.Select("ConsensusNameID is null").Length;
                total += failCount + (data.SingleNamesSet.ProviderName.Count * 2);
            }

            int done = 0;

            IntegrationSaveResult isr = new IntegrationSaveResult();

            DsIntegrationReference refs = data.References;

            if (data.DatasetType == IntegrationDatasetType.NamesByRank)
            {
                foreach (List<DsIntegrationName> dsList in data.NamesByRank.Values)
                {
                    foreach (DsIntegrationName dsn in dsList)
                    {

                        using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, new TimeSpan(1, 0, 0, 0, 0)))
                        {
                            SaveIntegrationData(cnnStr, dsn, refs, data.ConsensusData, ref isr, total, -1, ref done);
                            scope.Complete();
                        }

                        refs = null; // done references
                    }
                }
            }
            else
            {
                //save a few at a time - seems to cause issues if we do too many
                bool more = true;
                while (more)
                {
                    using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, new TimeSpan(1, 0, 0, 0, 0)))
                    {
                        more = SaveIntegrationData(cnnStr, data.SingleNamesSet, refs, data.ConsensusData, ref isr, total, 500, ref done);
                        scope.Complete();
                    }

                    refs = null;
                }
            }

            Progress = 97; //nearly done

            try
            {
                if (updateStackedNameData) UpdateConsensusStackedNameData(cnnStr);
            }
            catch (Exception)
            {
                //TODO log error
                throw;
            }


            Progress = 100;

            return isr;
        }

        private static bool SaveIntegrationData(String cnnStr, DsIntegrationName names, DsIntegrationReference refs, DsConsensusData consData, ref IntegrationSaveResult isr, int totalRecords, int maxRecords, ref int totalDone)
        {
            int recordsProcessed = 0;

            List<Guid> changedReferences = new List<Guid>();
            Dictionary<Guid, List<Entities.Provider.Reference>> providerRefs = new Dictionary<Guid, List<Entities.Provider.Reference>>();

            Dictionary<Guid, Entities.Consensus.Name> changedNames = new Dictionary<Guid, Entities.Consensus.Name>();
            Dictionary<Guid, List<Entities.Provider.Name>> providerNames = new Dictionary<Guid, List<Entities.Provider.Name>>();

            List<Entities.Consensus.Concept> changedConcepts = new List<Entities.Consensus.Concept>(); //modified concepts
            Dictionary<Guid, List<Entities.Provider.Concept>> providerConcepts = new Dictionary<Guid, List<Entities.Provider.Concept>>(); //prov concepts for consensus nameId
            Dictionary<Guid, List<Entities.Provider.Concept>> providerConceptsForConcept = new Dictionary<Guid, List<Entities.Provider.Concept>>(); //prov concepts for consensus conceptid

            Sql.Repositories.Consensus.NameRepository nr = new Sql.Repositories.Consensus.NameRepository(cnnStr);
            Sql.Repositories.Consensus.ConceptRepository cr = new Sql.Repositories.Consensus.ConceptRepository(cnnStr);
            Sql.Repositories.Provider.ReferenceRepository prr = new Sql.Repositories.Provider.ReferenceRepository(cnnStr);

            //update references
            if (refs != null)
            {
                foreach (DsIntegrationReference.ProviderReferenceRow prRow in refs.ProviderReference)
                {
                    if (!isr.HaveProcessedReference(prRow.ReferenceID))
                    {
                        if (!prRow.IsConsensusReferenceIDNull())
                        {
                            if (!changedReferences.Contains(prRow.ConsensusReferenceID))
                            {
                                Sql.Repositories.Consensus.ReferenceRepository crr = new Sql.Repositories.Consensus.ReferenceRepository(cnnStr);
                                Entities.Consensus.Reference r = crr.GetReference(prRow.ConsensusReferenceID);
                                if (r == null)
                                {
                                    DsConsensusData.ConsensusReferenceRow cref = consData.ConsensusReference.FindByReferenceID(prRow.ConsensusReferenceID);
                                    r = ConsensusReferenceFromIntegrationData(cref, cnnStr);
                                    crr.References.Add(r);
                                    isr.ReferencesInserted++;
                                }
                                else
                                {
                                    r.ModifiedDate = DateTime.Now;
                                    r.State = Entities.Entity.EntityState.Modified;

                                    isr.ReferencesUpdated++;
                                }
                                crr.Save(r);

                                r.State = Entities.Entity.EntityState.Unchanged;
                            }
                        }

                        Entities.Provider.Reference pr = prr.GetReference(prRow.ReferenceID);

                        if (pr != null)
                        {
                            pr.ConsensusReferenceId = prRow.Field<Guid?>("ConsensusReferenceID");
                            pr.LinkStatus = prRow.Field<String>("LinkStatus");
                            pr.MatchScore = prRow.Field<int?>("MatchScore");
                            pr.ModifiedDate = DateTime.Now;
                            pr.State = Entities.Entity.EntityState.Modified;

                            prr.Save(pr);

                            pr.State = Entities.Entity.EntityState.Unchanged;
                        }

                        if (pr.ConsensusReferenceId != null)
                        {
                            List<Entities.Provider.Reference> prList = null;
                            if (providerRefs.ContainsKey(pr.ConsensusReferenceId.Value))
                            {
                                prList = providerRefs[pr.ConsensusReferenceId.Value];
                            }
                            else
                            {
                                prList = new List<Entities.Provider.Reference>();
                                providerRefs.Add(pr.ConsensusReferenceId.Value, prList);
                            }
                            prList.Add(pr);
                        }

                        if (prRow.IsLinkStatusNull() || prRow.LinkStatus == LinkStatus.DataFail.ToString() || prRow.LinkStatus == LinkStatus.Multiple.ToString() || prRow.LinkStatus == LinkStatus.MultipleParent.ToString() ||
                            prRow.LinkStatus == LinkStatus.ParentMissing.ToString() || prRow.LinkStatus == LinkStatus.ParentNotIntegrated.ToString())
                        {
                            isr.ProviderReferencesWithErrors.Add(prRow.ReferenceID);
                        }
                        else
                        {
                            isr.ProviderReferencesIntegrated.Add(prRow.ReferenceID);
                        }

                        totalDone++;
                        if (totalRecords == 0) Progress = 100;
                        else Progress = (totalDone * 100 / totalRecords);

                        recordsProcessed++;
                        if (recordsProcessed >= maxRecords && maxRecords != -1) break;
                    }
                }
            }

            //build cons name/prov name lists
            Sql.Repositories.Provider.NameRepository pnr = new Sql.Repositories.Provider.NameRepository(cnnStr);
            if (recordsProcessed < maxRecords || maxRecords == -1)
            {
                foreach (DsIntegrationName.ProviderNameRow pnRow in names.ProviderName)
                {
                    if (!isr.HaveProcessedName(pnRow.NameID))
                    {
                        Entities.Provider.Name pn = pnr.GetName(pnRow.NameID);

                        pn.ConsensusNameId = pnRow.Field<Guid?>("ConsensusNameID");
                        pn.LinkStatus = pnRow.Field<String>("LinkStatus");
                        pn.MatchScore = pnRow.Field<int?>("MatchScore");
                        pn.MatchPath = pnRow.Field<String>("MatchPath");
                        pn.ModifiedDate = DateTime.Now;
                        pn.State = Entities.Entity.EntityState.Modified;

                        if (pn.ConsensusNameId != null)
                        {
                            List<Entities.Provider.Name> pnList = null;
                            if (providerNames.ContainsKey(pn.ConsensusNameId.Value))
                            {
                                pnList = providerNames[pn.ConsensusNameId.Value];
                            }
                            else
                            {
                                pnList = new List<Entities.Provider.Name>();
                                providerNames.Add(pn.ConsensusNameId.Value, pnList);
                            }
                            pnList.Add(pn);
                        }
                        else
                        {
                            //just update the provider name - failed integration
                            pnr.Save(pn);

                            pn.State = Entities.Entity.EntityState.Unchanged;


                            totalDone++;
                            if (totalRecords == 0) Progress = 100;
                            else Progress = (totalDone * 100 / totalRecords);
                        }

                        recordsProcessed++;
                        if (recordsProcessed >= maxRecords && maxRecords != -1) break;
                    }
                }
            }

            foreach (Guid cnId in providerNames.Keys)
            {
                DsConsensusData.ConsensusNameRow cnRow = consData.ConsensusName.FindByNameID(cnId);

                //consensus name
                Entities.Consensus.Name cn = nr.GetName(cnRow.NameID);

                if (cn == null)
                {
                    //need to create a new consensus name
                    cn = ConsensusNameFromIntegrationData(cnRow, cnnStr);

                    if (providerNames.ContainsKey(cn.NameId))
                    {
                        //get all data from provider name, not just thst used for integration
                        RefreshConsensusNameValues(cn, providerNames[cn.NameId], cnnStr);
                    }

                    nr.Names.Add(cn);

                    isr.NamesInserted++;
                }
                else
                {
                    List<Entities.Provider.Name> pnList = null;
                    if (providerNames.ContainsKey(cn.NameId)) pnList = providerNames[cn.NameId];
                    else pnList = pnr.GetNamesForConsensusName(cn.NameId);

                    RefreshConsensusNameValues(cn, pnList, cnnStr);

                    cn.ModifiedDate = DateTime.Now;
                    cn.State = Entities.Entity.EntityState.Modified;

                    List<Entities.Consensus.Concept> ccList = cr.GetConceptsByName(cn.NameId);
                    foreach (Entities.Consensus.Concept cc in ccList)
                    {
                        cc.State = Entities.Entity.EntityState.Modified;
                        cc.ModifiedDate = DateTime.Now;
                        changedConcepts.Add(cc);
                    }

                    isr.NamesUpdated++;
                }

                changedNames.Add(cn.NameId, cn);

                nr.Save(cn);


                foreach (Entities.Provider.Name pn in providerNames[cn.NameId])
                {
                    pnr.Save(pn);
                    pn.State = Entities.Entity.EntityState.Unchanged;

                    if (pn.LinkStatus == LinkStatus.DataFail.ToString() || pn.LinkStatus == LinkStatus.Multiple.ToString() || pn.LinkStatus == LinkStatus.MultipleParent.ToString() ||
                        pn.LinkStatus == LinkStatus.ParentMissing.ToString() || pn.LinkStatus == LinkStatus.ParentNotIntegrated.ToString())
                    {
                        isr.ProviderNamesWithErrors.Add(pn.NameId);
                    }
                    else
                    {
                        isr.ProviderNamesIntegrated.Add(pn.NameId);
                    }
                }

                totalDone++;
                if (totalRecords == 0) Progress = 100;
                else Progress = (totalDone * 100 / totalRecords);
            }

            //save concepts
            Sql.Repositories.Provider.ConceptRepository pcr = new Sql.Repositories.Provider.ConceptRepository(cnnStr);
            Sql.Repositories.Consensus.ConceptRepository ccr = new Sql.Repositories.Consensus.ConceptRepository(cnnStr);
            foreach (Entities.Consensus.Name cn in changedNames.Values)
            {
                //build list of provider concepts for the name
                pcr.Concepts.Clear();

                List<Entities.Provider.Name> pnList = providerNames[cn.NameId];
                foreach (Entities.Provider.Name pn in pnList)
                {
                    List<Entities.Provider.Concept> pcList = pcr.GetConceptsByName(pn.NameId);
                    foreach (Entities.Provider.Concept pc in pcList)
                    {
                        pc.State = Entities.Entity.EntityState.Modified; //flag to update
                        pc.ModifiedDate = DateTime.Now;
                        pcr.Concepts.Add(pc);
                    }

                    if (providerConcepts.ContainsKey(cn.NameId))
                    {
                        providerConcepts[cn.NameId].AddRange(pcList);
                    }
                    else
                    {
                        providerConcepts.Add(cn.NameId, pcList);
                    }
                }

                //new concepts
                if (cn.State == Entities.Entity.EntityState.Added)
                {
                    //generate from consensus integration data
                    DsConsensusData.ConsensusNameRow cnRow = consData.ConsensusName.FindByNameID(cn.NameId);
                    List<Entities.Consensus.Concept> ccList = ConsensusConceptsFromIntegrationData(cnRow, providerConcepts[cn.NameId], cnnStr);

                    foreach (Entities.Consensus.Concept cc in ccList)
                    {
                        ccr.Save(cc, false);
                        cc.State = Entities.Entity.EntityState.Unchanged;
                        changedConcepts.Add(cc);
                    }

                }
                else
                {
                    //updated concepts
                    List<Entities.Consensus.Concept> ccList = ccr.GetConceptsByName(cn.NameId);
                    foreach (Entities.Consensus.Concept cc in ccList)
                    {
                        cc.State = Entities.Entity.EntityState.Modified; //need refreshing
                        cc.ModifiedDate = DateTime.Now;

                        ccr.Save(cc, false);
                        cc.State = Entities.Entity.EntityState.Unchanged;
                        changedConcepts.Add(cc);
                    }
                }

                //save prov concepts
                foreach (Entities.Provider.Concept pc in pcr.Concepts)
                {
                    pcr.Save(pc, false);
                    pc.State = Entities.Entity.EntityState.Unchanged;

                    //record provider concepts for the saved concept
                    if (pc.ConsensusConceptId.HasValue)
                    {
                        if (providerConceptsForConcept.ContainsKey(pc.ConsensusConceptId.Value))
                        {
                            providerConceptsForConcept[pc.ConsensusConceptId.Value].Add(pc);
                        }
                        else
                        {
                            providerConceptsForConcept.Add(pc.ConsensusConceptId.Value, new List<Entities.Provider.Concept>() { pc });
                        }
                    }
                }


                //save prov concept rels                    
                foreach (Entities.Provider.Concept pc in pcr.Concepts)
                {
                    pcr.SaveRelationships(pc);
                }

                totalDone++;
                if (totalRecords == 0) Progress = 100;
                else Progress = (totalDone * 100 / totalRecords);
            }

            Admin.Data.Sql.Repositories.ProviderRepository pRep = new Admin.Data.Sql.Repositories.ProviderRepository(cnnStr);
            List<Admin.Data.Entities.AttachmentPoint> attPoints = pRep.GetAllAttachmentPoints();
            //update relationships
            foreach (Entities.Consensus.Concept cc in changedConcepts)
            {
                RefreshConsensusConcept(cnnStr, cc, providerConceptsForConcept, providerConcepts[cc.NameId], attPoints);

                totalDone++;
                if (totalRecords == 0) Progress = 100;
                else Progress = (totalDone * 100 / totalRecords);
            }

            totalDone++;
            if (totalRecords == 0) Progress = 100;
            else Progress = (totalDone * 100 / totalRecords);

            //update taxon properties
            //refresh full names and name link type info
            NZOR.Data.Repositories.Provider.ITaxonPropertyRepository ptpr = new NZOR.Data.Sql.Repositories.Provider.TaxonPropertyRepository(cnnStr);
            NZOR.Data.Repositories.Consensus.ITaxonPropertyRepository ctpr = new NZOR.Data.Sql.Repositories.Consensus.TaxonPropertyRepository(cnnStr);
            foreach (Entities.Consensus.Name cn in changedNames.Values)
            {
                try
                {
                    List<NZOR.Data.Entities.Provider.TaxonProperty> props = ptpr.GetTaxonPropertiesByConsensusName(cn.NameId);
                    List<NZOR.Data.Entities.Consensus.TaxonProperty> consProps = GetConsensusTaxonProperties(props, cnnStr);
                    ctpr.TaxonProperties.AddRange(consProps);

                    nr.UpdateFullName(cn);
                }
                catch (Exception ex)
                {
                    isr.Errors.Add(ex.Message);
                    throw;
                }
            }
            ctpr.Save();

            foreach (Entities.Consensus.Name cn in changedNames.Values)
            {
                try
                {
                    RefreshNameLinks(cnnStr, cn, providerNames[cn.NameId]);
                    UpdateConsensusStackedNameData(cnnStr, cn.NameId);
                }
                catch (Exception ex)
                {
                    isr.Errors.Add(ex.Message);
                    throw;
                }

                totalDone++;
                if (totalRecords == 0) Progress = 100;
                else Progress = (totalDone * 100 / totalRecords);
            }

            return (maxRecords == -1 || recordsProcessed <= maxRecords);
        }

        public static void RefreshConsensusConcept(string cnnStr, NZOR.Data.Entities.Consensus.Concept concept, Dictionary<Guid, List<NZOR.Data.Entities.Provider.Concept>> pcLookupList, List<NZOR.Data.Entities.Provider.Concept> provConceptsForName, List<Admin.Data.Entities.AttachmentPoint> attPoints)
        {
            NZOR.Data.Sql.Repositories.Provider.ConceptRepository pcr = new Repositories.Provider.ConceptRepository(cnnStr);
            NZOR.Data.Sql.Repositories.Consensus.ConceptRepository ccr = new Repositories.Consensus.ConceptRepository(cnnStr);

            //update concepts that point to this concept, now that this concept exists
            List<Entities.Provider.Concept> pcList = null;
            if (pcLookupList != null && pcLookupList.ContainsKey(concept.ConceptId)) pcList = pcLookupList[concept.ConceptId];
            else pcList = pcr.GetProviderConcepts(concept.ConceptId);

            if (provConceptsForName == null)
            {
                provConceptsForName = pcr.GetProviderConceptsByName(concept.NameId);
            }

            Entities.Common.ConceptRelationshipType parCrType = ConceptRelationshipType(cnnStr, LookUps.Common.ConceptRelationshipTypeLookUp.IsChildOf);
            bool hasParentRel = false;
            foreach (Entities.Provider.Concept pc in provConceptsForName)
            {
                foreach (Entities.Provider.ConceptRelationship pccr in pc.ConceptRelationships)
                {
                    if (pccr.ConceptRelationshipTypeId == parCrType.ConceptRelationshipTypeId && (!pccr.InUse.HasValue || pccr.InUse.Value))
                    {
                        hasParentRel = true;
                        break;
                    }
                }
            }

            RefreshConsensusConcept(concept, pcList, hasParentRel, cnnStr, attPoints);

            ccr.Save(concept, true);

            UpdateRelatedConcepts(concept, pcList, cnnStr, attPoints);
        }

        public static List<NZOR.Data.Entities.Consensus.TaxonProperty> GetConsensusTaxonProperties(List<NZOR.Data.Entities.Provider.TaxonProperty> provProps, String cnnStr)
        {
            List<NZOR.Data.Entities.Consensus.TaxonProperty> props = new List<Entities.Consensus.TaxonProperty>();

            foreach (NZOR.Data.Entities.Provider.TaxonProperty prop in provProps)
            {
                NZOR.Data.Entities.Consensus.TaxonProperty ctp = GetConsensusTaxonProperty(prop, cnnStr);

                if (ctp != null)
                {
                    bool tpExists = false;
                    foreach (Data.Entities.Consensus.TaxonProperty p in props)
                    {
                        if (ctp.NameId.Equals(p.NameId) && ctp.ConceptId.Equals(p.ConceptId) && ctp.TaxonPropertyClassId.Equals(p.TaxonPropertyClassId))
                        {
                            //same taxon property
                            tpExists = true;

                            //check taxon prop values
                            foreach (Data.Entities.Consensus.TaxonPropertyValue tpv in ctp.TaxonPropertyValues)
                            {
                                bool found = false;
                                foreach (Data.Entities.Consensus.TaxonPropertyValue existingTpv in p.TaxonPropertyValues)
                                {
                                    if (existingTpv.TaxonPropertyTypeId.Equals(tpv.TaxonPropertyTypeId) && String.Compare(existingTpv.Value, tpv.Value, true) == 0)
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    p.TaxonPropertyValues.Add(tpv);
                                }
                            }

                            break;
                        }
                    }

                    if (!tpExists)
                    {
                        props.Add(ctp);
                    }
                }
            }

            return props;
        }

        private static NZOR.Data.Entities.Consensus.TaxonProperty GetConsensusTaxonProperty(NZOR.Data.Entities.Provider.TaxonProperty provProp, String cnnStr)
        {
            NZOR.Data.Entities.Consensus.TaxonProperty consProp = new Entities.Consensus.TaxonProperty();

            consProp.State = Entities.Entity.EntityState.Added;
            consProp.AddedDate = DateTime.Now;

            if (provProp.ConceptId.HasValue)
            {
                NZOR.Data.Sql.Repositories.Provider.ConceptRepository pcr = new Repositories.Provider.ConceptRepository(cnnStr);
                Data.Entities.Provider.Concept pc = pcr.GetConcept(provProp.ConceptId.Value);
                consProp.ConceptId = pc.ConsensusConceptId;
            }

            consProp.InUse = provProp.InUse;

            if (provProp.NameId.HasValue)
            {
                NZOR.Data.Sql.Repositories.Provider.NameRepository pnr = new Repositories.Provider.NameRepository(cnnStr);
                Data.Entities.Provider.Name pn = pnr.GetName(provProp.NameId.Value);
                consProp.NameId = pn.ConsensusNameId;
            }

            if (!consProp.ConceptId.HasValue && !consProp.NameId.HasValue)
            {
                //Cant connect to consensus name or concept!
                consProp = null;
                return consProp;
            }

            if (provProp.ReferenceId.HasValue)
            {
                NZOR.Data.Sql.Repositories.Provider.ReferenceRepository prr = new Repositories.Provider.ReferenceRepository(cnnStr);
                Data.Entities.Provider.Reference pr = prr.GetReference(provProp.ReferenceId.Value);
                consProp.ReferenceId = pr.ConsensusReferenceId;
            }

            consProp.TaxonPropertyClassId = provProp.TaxonPropertyClassId;

            NZOR.Data.Sql.Repositories.Common.LookUpRepository lr = new LookUpRepository(cnnStr);

            foreach (Data.Entities.Provider.TaxonPropertyValue tpv in provProp.TaxonPropertyValues)
            {
                Data.Entities.Consensus.TaxonPropertyValue ctp = new Entities.Consensus.TaxonPropertyValue();
                ctp.TaxonPropertyId = consProp.TaxonPropertyId;
                ctp.TaxonPropertyTypeId = tpv.TaxonPropertyTypeId;
                ctp.Value = tpv.Value;

                if (ctp.TaxonPropertyValueId == TaxonPropertyTypeLookup.PropertyTypeGeoRegionId)
                {
                    Entities.Common.GeoRegion region = lr.GetGeoRegionByName(ctp.Value);
                    if (region != null)
                    {
                        consProp.GeoRegionId = region.GeoRegionId;
                        consProp.GeoRegion = region.Name;
                    }
                }

                consProp.TaxonPropertyValues.Add(ctp);
            }

            return consProp;
        }

        public static void UpdateRelatedConcepts(Entities.Consensus.Concept concept, List<Entities.Provider.Concept> providerConcepts, String cnnStr, List<Admin.Data.Entities.AttachmentPoint> attPoints)
        {
            Sql.Repositories.Provider.ConceptRepository pcr = new Sql.Repositories.Provider.ConceptRepository(cnnStr);
            Sql.Repositories.Consensus.ConceptRepository ccr = new Sql.Repositories.Consensus.ConceptRepository(cnnStr);

            //update all other concepts that can now point to this concept as part of their relationships
            foreach (Entities.Provider.Concept pc in providerConcepts)
            {
                List<Entities.Consensus.Concept> concepts = ccr.GetRelatedConcepts(pc.NameId, pc.AccordingToReferenceId);

                foreach (Entities.Consensus.Concept cc in concepts)
                {
                    if (cc.ConceptId != concept.ConceptId)
                    {
                        List<Entities.Provider.Concept> pcList = pcr.GetProviderConcepts(cc.ConceptId);
                        RefreshConsensusConcept(cc, pcList, true, cnnStr, attPoints);

                        ccr.Save(cc, true);
                    }
                }
            }
        }


        #endregion

        #region "Consensus Name"

        public static DataSet AddConsensusName(String cnnStr, Entities.Provider.Name provName)
        {
            Guid nameId = Guid.NewGuid();

            string sql = "insert consensus.Name(NameID, AddedDate, FullName, GoverningCode, NameClassID, TaxonRankID, IsRecombination) select '" +
                nameId.ToString() + "', '" +
                DateTime.Now.ToString("s") + "', " +
                "'', " +
                (provName.GoverningCode == null ? "null, '" : "'" + provName.GoverningCode.ToString() + "', '") +
                provName.NameClassId.ToString() + "', '" +
                provName.TaxonRankId.ToString() + "', " +
                (!provName.IsRecombination.HasValue ? "null" : (provName.IsRecombination.Value ? "1" : "0"));

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }

            DataSet provDs = GetProviderName(cnnStr, provName.NameId);

            //properties
            foreach (DataRow tpRow in provDs.Tables["NameProperty"].Rows)
            {
                int seq = -1;
                if (!tpRow.IsNull("Sequence")) seq = (int)tpRow["Sequence"];
                Guid relId = Guid.Empty;
                Guid consRelId = Guid.Empty;
                if (tpRow["RelatedID"] != DBNull.Value)
                {
                    relId = (Guid)tpRow["RelatedID"];

                    //try names
                    using (SqlConnection cnn = new SqlConnection(cnnStr))
                    {
                        cnn.Open();
                        using (SqlCommand relCmd = cnn.CreateCommand())
                        {
                            relCmd.CommandText = "select ConsensusNameID from provider.Name where NameID = '" + relId + "'";
                            object relVal = relCmd.ExecuteScalar();
                            if (relVal != null && relVal != DBNull.Value) consRelId = (Guid)relVal;
                        }

                        if (consRelId == Guid.Empty)
                        {
                            //try refs
                            using (SqlCommand relCmd = cnn.CreateCommand())
                            {
                                relCmd.CommandText = "select ConsensusReferenceID from provider.Reference where ReferenceID = '" + relId + "'";
                                object relVal = relCmd.ExecuteScalar();
                                if (relVal != null && relVal != DBNull.Value) consRelId = (Guid)relVal;
                            }
                        }
                    }
                }

                string val = tpRow["Value"].ToString().Replace("'", "''");

                sql = "insert consensus.NameProperty(NamePropertyID, NameID, NamePropertyTypeID, Value, Sequence, RelatedID) select '" +
                    Guid.NewGuid().ToString() + "', '" +
                    nameId.ToString() + "', '" +
                    tpRow["NamePropertyTypeID"].ToString() + "', '" +
                    val + "', " +
                    (seq == -1 ? "null, " : seq.ToString() + ", ") +
                    (consRelId == Guid.Empty ? "null" : "'" + consRelId.ToString() + "'");

                using (SqlConnection cnn = new SqlConnection(cnnStr))
                {
                    cnn.Open();
                    using (SqlCommand npCmd = cnn.CreateCommand())
                    {
                        npCmd.CommandText = sql;
                        npCmd.ExecuteNonQuery();
                    }
                }
            }

            //full name
            sql = "update consensus.Name set FullName = consensus.GetFullName('" + nameId.ToString() + "') where NameId = '" + nameId.ToString() + "'";
            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();
                using (SqlCommand npCmd = cnn.CreateCommand())
                {
                    npCmd.CommandText = sql;
                    npCmd.ExecuteNonQuery();
                }
            }
            UpdateConsensusFullName(cnnStr, nameId);

            //Update Flat Name data
            UpdateConsensusStackedNameData(cnnStr, nameId);

            return GetConsensusName(cnnStr, nameId, true);
        }

        /// <summary>
        /// Add new consensus name from provider name details
        /// </summary>
        /// <param name="provName"></param>
        /// <returns>new consensus name</returns>
        public static DataSet AddConsensusName(String cnnStr, DsIntegrationName.ProviderNameRow provName)
        {
            Guid nameId = Guid.NewGuid();

            string sql = "insert consensus.Name(NameID, AddedDate, FullName, GoverningCode, NameClassID, TaxonRankID, IsRecombination) select '" +
                nameId.ToString() + "', '" +
                DateTime.Now.ToString("s") + "', " +
                "'', " + //full name later
                (provName.IsGoverningCodeNull() ? "null, '" : "'" + provName.GoverningCode.ToString() + "', '") +
                provName.NameClassID.ToString() + "', '" +
                provName.TaxonRankID.ToString() + "', " +
                (provName.IsIsRecombinationNull() ? "null" : (provName.IsRecombination ? "1" : "0"));

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }

            DataSet provDs = GetProviderName(cnnStr, provName.NameID);

            //properties
            foreach (DataRow tpRow in provDs.Tables["NameProperty"].Rows)
            {
                int seq = -1;
                if (!tpRow.IsNull("Sequence")) seq = (int)tpRow["Sequence"];
                Guid relId = Guid.Empty;
                Guid consRelId = Guid.Empty;
                if (tpRow["RelatedID"] != DBNull.Value)
                {
                    relId = (Guid)tpRow["RelatedID"];

                    //try names
                    using (SqlConnection cnn = new SqlConnection(cnnStr))
                    {
                        cnn.Open();
                        using (SqlCommand relCmd = cnn.CreateCommand())
                        {
                            relCmd.CommandText = "select ConsensusNameID from provider.Name where NameID = '" + relId + "'";
                            object relVal = relCmd.ExecuteScalar();
                            if (relVal != null && relVal != DBNull.Value) consRelId = (Guid)relVal;
                        }

                        if (consRelId == Guid.Empty)
                        {
                            //try refs
                            using (SqlCommand relCmd = cnn.CreateCommand())
                            {
                                relCmd.CommandText = "select ConsensusReferenceID from provider.Reference where ReferenceID = '" + relId + "'";
                                object relVal = relCmd.ExecuteScalar();
                                if (relVal != null && relVal != DBNull.Value) consRelId = (Guid)relVal;
                            }
                        }
                    }
                }

                string val = tpRow["Value"].ToString().Replace("'", "''");

                sql = "insert consensus.NameProperty(NamePropertyID, NameID, NamePropertyTypeID, Value, Sequence, RelatedID) select '" +
                    Guid.NewGuid().ToString() + "', '" +
                    nameId.ToString() + "', '" +
                    tpRow["NamePropertyTypeID"].ToString() + "', '" +
                    val + "', " +
                    (seq == -1 ? "null, " : seq.ToString() + ", ") +
                    (consRelId == Guid.Empty ? "null" : "'" + consRelId.ToString() + "'");

                using (SqlConnection cnn = new SqlConnection(cnnStr))
                {
                    cnn.Open();
                    using (SqlCommand npCmd = cnn.CreateCommand())
                    {
                        npCmd.CommandText = sql;
                        npCmd.ExecuteNonQuery();
                    }
                }
            }

            //Update Flat Name data
            UpdateConsensusStackedNameData(cnnStr, nameId);

            //full name
            sql = "update consensus.Name set FullName = consensus.GetFullName('" + nameId.ToString() + "') where NameId = '" + nameId.ToString() + "'";
            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();
                using (SqlCommand npCmd = cnn.CreateCommand())
                {
                    npCmd.CommandText = sql;
                    npCmd.ExecuteNonQuery();
                }
            }

            UpdateConsensusFullName(cnnStr, nameId);


            return GetConsensusName(cnnStr, nameId, true);
        }

        public static DataSet GetConsensusName(String cnnStr, Guid nameId, bool consensusOnly)
        {
            DataSet ds = null;

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "select * from consensus.Name where NameID = '" + nameId.ToString() + "'; select * from consensus.NameProperty where NameID = '" + nameId.ToString() + "';";

                    if (!consensusOnly) cmd.CommandText += " select * from provider.Name where ConsensusNameID = '" + nameId.ToString() + "'; select np.* from provider.NameProperty np inner join provider.Name pn on pn.NameID = np.NameID where pn.ConsensusNameID = '" + nameId.ToString() + "'";

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count == 0) ds = null;
                }
            }
            return ds;
        }

        /// <summary>
        /// Refresh ALL names.  Returnes a list of error messages.
        /// </summary>
        /// <param name="cnnStr"></param>
        /// <returns></returns>
        public static List<String> RefreshAllNames(string cnnStr)
        {
            List<String> errMsgs = new List<string>();
            Progress = 0;
            int done = 0;


            Admin.Data.Sql.Repositories.ProviderRepository pRep = new Admin.Data.Sql.Repositories.ProviderRepository(cnnStr);
            List<Admin.Data.Entities.AttachmentPoint> attPoints = pRep.GetAllAttachmentPoints();

            NZOR.Data.Sql.Repositories.Consensus.NameRepository nr = new Data.Sql.Repositories.Consensus.NameRepository(cnnStr);
            List<Guid> names = nr.GetAllNames();

            foreach (Guid nameId in names)
            {
                try
                {
                    RefreshConsensusName(nameId, cnnStr, attPoints);
                }
                catch (Exception ex)
                {
                    errMsgs.Add("ERROR : NameId = '" + nameId.ToString() + "' : " + ex.Message + " : " + ex.StackTrace);
                }

                done += 1;
                Progress = done * 100 / names.Count;
            }

            return errMsgs;
        }

        public static void RefreshConsensusName(Guid consensusNameId, string cnnStr, List<Admin.Data.Entities.AttachmentPoint> attPoints)
        {
            NZOR.Data.Sql.Repositories.Consensus.NameRepository nr = new Data.Sql.Repositories.Consensus.NameRepository(cnnStr);
            NZOR.Data.Sql.Repositories.Provider.NameRepository pnr = new Data.Sql.Repositories.Provider.NameRepository(cnnStr);
            NZOR.Data.Sql.Repositories.Consensus.ConceptRepository cr = new Repositories.Consensus.ConceptRepository(cnnStr);
            NZOR.Data.Sql.Repositories.Provider.ConceptRepository pcr = new Repositories.Provider.ConceptRepository(cnnStr);

            NZOR.Data.Entities.Consensus.Name cn = nr.GetName(consensusNameId);

            if (cn == null) return;

            List<NZOR.Data.Entities.Provider.Name> pnList = pnr.GetNamesForConsensusName(consensusNameId);

            NZOR.Data.Sql.Integration.RefreshConsensusNameValues(cn, pnList, cnnStr);
               
            //update name fields (must match in prov records, so just pick first one)
            cn.GoverningCode = pnList[0].GoverningCode;
            NZOR.Data.Entities.Provider.Name irName = pnList.Where(pn => pn.IsRecombination != null).FirstOrDefault();
            cn.IsRecombination = (irName != null ? irName.IsRecombination : null);
            cn.TaxonRankId = pnList[0].TaxonRankId;

            cn.State = Entities.Entity.EntityState.Modified;
            cn.ModifiedDate = DateTime.Now;

            nr.Names.Add(cn);
            nr.Save();

            cn.State = Entities.Entity.EntityState.Unchanged;

            //List<Entities.Provider.Concept> pcList = pcr.GetProviderConceptsByName(cn.NameId);


            //is there a parent relationship at all - if not we need to know this further down the line
            bool hasParentRel = false;
            Sql.Repositories.Common.LookUpRepository lr = new LookUpRepository(cnnStr);
            LookUps.Common.ConceptRelationshipTypeLookUp crtl = new ConceptRelationshipTypeLookUp(lr.GetConceptRelationshipTypes());
            ConceptRelationshipType parRelType = crtl.GetConceptRelationshipType(LookUps.Common.ConceptRelationshipTypeLookUp.IsChildOf);
            List<Entities.Provider.Concept> provConcepts = pcr.GetProviderConceptsByName(consensusNameId);
            foreach (Entities.Provider.Concept pc in provConcepts)
            {
                foreach (Entities.Provider.ConceptRelationship pccr in pc.ConceptRelationships)
                {
                    if (pccr.ConceptRelationshipTypeId == parRelType.ConceptRelationshipTypeId && (!pccr.InUse.HasValue || pccr.InUse.Value))
                    {
                        hasParentRel = true;
                        break;
                    }
                }
            }

            List<Data.Entities.Consensus.Concept> ccList = cr.GetConceptsByName(consensusNameId);
            foreach (Data.Entities.Consensus.Concept cc in ccList)
            {
                List<Data.Entities.Provider.Concept> pcList = pcr.GetProviderConcepts(cc.ConceptId);
                RefreshConsensusConcept(cc, pcList, hasParentRel, cnnStr, attPoints);

                if (cc.State != Entities.Entity.EntityState.Unchanged)
                {
                    cr.Save(cc, true);
                }
                UpdateRelatedConcepts(cc, pcList, cnnStr, attPoints);
            }

            NZOR.Data.Repositories.Provider.ITaxonPropertyRepository ptpr = new NZOR.Data.Sql.Repositories.Provider.TaxonPropertyRepository(cnnStr);
            NZOR.Data.Repositories.Consensus.ITaxonPropertyRepository ctpr = new NZOR.Data.Sql.Repositories.Consensus.TaxonPropertyRepository(cnnStr);
            List<NZOR.Data.Entities.Provider.TaxonProperty> props = ptpr.GetTaxonPropertiesByConsensusName(cn.NameId);
            List<NZOR.Data.Entities.Consensus.TaxonProperty> consProps = GetConsensusTaxonProperties(props, cnnStr);
            ctpr.TaxonProperties.AddRange(consProps);
            ctpr.Save();

            //refresh related data (works on database directly)
            UpdateConsensusStackedNameData(cnnStr, cn.NameId);

            //refresh annotations
            RefreshNameAnnotations(cnnStr, cn.NameId);
            
            nr.UpdateFullName(cn);

            RefreshNameLinks(cnnStr, cn, pnList);
        }

        public static void RefreshNameAnnotations(string cnnStr, Guid nameId)
        {
            string sql = @"delete consensus.Annotation where NameID = '" + nameId.ToString() + @"';

                    declare @ann table(nameId uniqueidentifier, conceptId uniqueidentifier, refId uniqueidentifier, annText nvarchar(max), annType nvarchar(250));

                    insert @ann
                    select distinct pn.[ConsensusNameID]
                          ,pc.[ConsensusConceptID]
                          ,pr.[ConsensusReferenceID]
                          ,[AnnotationText]
                          ,[AnnotationType]
                    from provider.Annotation a
                    inner join provider.Name pn on pn.NameID = a.NameID
                    left join provider.Concept pc on pc.ConceptID = a.ConceptID
                    left join provider.Reference pr on pr.ReferenceID = a.ReferenceID
                    where pn.ConsensusNameID = '" + nameId.ToString() + @"';

                    insert consensus.Annotation
                    select newid(),
	                      nameId,
	                      conceptId,
	                      refId,
	                      annType,
	                      annText,
                          getdate(),
	                      null
                    from @ann;

                    update pa
                    set pa.ConsensusAnnotationID = ca.annotationid
                    from provider.annotation pa
                    inner join provider.Name pn on pn.NameID = pa.NameID
                    left join provider.Concept pc on pc.ConceptID = pa.ConceptID
                    left join provider.Reference pr on pr.ReferenceID = pa.ReferenceID
                    inner join consensus.annotation ca on isnull(ca.nameid, '00000000-0000-0000-0000-000000000000') = isnull(pn.consensusnameid, '00000000-0000-0000-0000-000000000000')
	                    and isnull(ca.conceptid, '00000000-0000-0000-0000-000000000000') = isnull(pc.consensusconceptid, '00000000-0000-0000-0000-000000000000')
	                    and isnull(ca.referenceid, '00000000-0000-0000-0000-000000000000') = isnull(pr.consensusreferenceid, '00000000-0000-0000-0000-000000000000')
	                    and ca.annotationtype = pa.annotationtype
	                    and ca.annotationtext = pa.annotationtext
                    where pn.ConsensusNameID = '" + nameId.ToString() + "';";


            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }

        }

        public static void RefreshConceptAnnotations(string cnnStr, Guid conceptId)
        {
            string sql = @"delete consensus.Annotation where ConceptID = '" + conceptId.ToString() + @"';

                    declare @ann table(nameId uniqueidentifier, conceptId uniqueidentifier, refId uniqueidentifier, annText nvarchar(max), annType nvarchar(250));

                    insert @ann
                    select distinct pn.[ConsensusNameID]
                          ,pc.[ConsensusConceptID]
                          ,pr.[ConsensusReferenceID]
                          ,[AnnotationText]
                          ,[AnnotationType]
                    from provider.Annotation a
                    left join provider.Name pn on pn.NameID = a.NameID
                    inner join provider.Concept pc on pc.ConceptID = a.ConceptID
                    left join provider.Reference pr on pr.ReferenceID = a.ReferenceID
                    where pc.ConsensusConceptID = '" + conceptId.ToString() + @"';

                    insert consensus.Annotation
                    select newid(),
	                      nameId,
	                      conceptId,
	                      refId,
	                      annType,
	                      annText,
                          getdate(),
	                      null
                    from @ann;

                    update pa
                    set pa.ConsensusAnnotationID = ca.annotationid
                    from provider.annotation pa
                    left join provider.Name pn on pn.NameID = pa.NameID
                    inner join provider.Concept pc on pc.ConceptID = pa.ConceptID
                    left join provider.Reference pr on pr.ReferenceID = pa.ReferenceID
                    inner join consensus.annotation ca on isnull(ca.nameid, '00000000-0000-0000-0000-000000000000') = isnull(pn.consensusnameid, '00000000-0000-0000-0000-000000000000')
	                    and isnull(ca.conceptid, '00000000-0000-0000-0000-000000000000') = isnull(pc.consensusconceptid, '00000000-0000-0000-0000-000000000000')
	                    and isnull(ca.referenceid, '00000000-0000-0000-0000-000000000000') = isnull(pr.consensusreferenceid, '00000000-0000-0000-0000-000000000000')
	                    and ca.annotationtype = pa.annotationtype
	                    and ca.annotationtext = pa.annotationtext
                    where pc.ConsensusConceptID = '" + conceptId.ToString() + "';";


            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }

        }

        private static void RefreshConsensusNameValues(Entities.Consensus.Name consName, List<Entities.Provider.Name> provNames, string cnnStr)
        {
            foreach (Entities.Common.NamePropertyType npt in NamePropertyTypes(cnnStr, consName.NameClassId))
            {
                bool allowMultiple = (!npt.MaxOccurrences.HasValue || npt.MaxOccurrences > 1);

                List<object> vals = GetConsensusValues(provNames, npt.Name, allowMultiple);

                if (vals != null && vals.Count > 0)
                {
                    foreach (object val in vals) consName.SetNameProperty(npt, val.ToString(), allowMultiple);
                }
            }

            foreach (Entities.Provider.Name pn in provNames)
            {
                if (pn.IsRecombination.HasValue) consName.IsRecombination = pn.IsRecombination;
            }
        }

        /// <summary>
        /// Refresh all concept relationships for this concept
        /// Based on the provider concepts passed - the provider concepts passed in must all be for the associated consensus name
        /// Any new consensus concepts and relationships are not saved to the database, but ToConcepts ARE created in the database to provide a hook to attach the new concepts to
        /// </summary>
        /// <param name="consConcept"></param>
        /// <param name="provConcepts"></param>
        /// <param name="cnnStr"></param>
        public static void RefreshConsensusConcept(Entities.Consensus.Concept consConcept, List<Entities.Provider.Concept> provConcepts, bool parentConceptDefined, string cnnStr, List<Admin.Data.Entities.AttachmentPoint> attPoints)
        {
            //get providr concepts for this consConcept
            List<Entities.Provider.Concept> pcList = new List<Entities.Provider.Concept>();
            foreach (Entities.Provider.Concept pc in provConcepts)
            {
                if (pc.ConsensusConceptId == consConcept.ConceptId) pcList.Add(pc);
            }

            if (pcList.Count == 0) //provider concepts have been removed
            {
                //if no parent concept defined, need to calculate it
                if (!parentConceptDefined)
                {
                    Sql.Repositories.Consensus.NameRepository cnr = new Repositories.Consensus.NameRepository(cnnStr);
                    Sql.Repositories.Common.LookUpRepository lr = new LookUpRepository(cnnStr);
                    LookUps.Common.NameClassLookUp ncl = new NameClassLookUp(lr.GetNameClasses());
                    LookUps.Common.ConceptRelationshipTypeLookUp crtl = new ConceptRelationshipTypeLookUp(lr.GetConceptRelationshipTypes());

                    Entities.Consensus.Name consName = cnr.GetName(consConcept.NameId);
                    NameClass cnc = ncl.GetNameClassById(consName.NameClassId);

                    if (cnc.Name == NZOR.Data.LookUps.Common.NameClassLookUp.ScientificName)
                    {
                        CalculateNameParentData calcData = new CalculateNameParentData();
                        calcData.ProviderNameRankSort = TaxonRankLookup(cnnStr).GetTaxonRank(consName.TaxonRankId).SortOrder.Value;
                        calcData.ProviderNameID = Guid.Empty;
                        calcData.AttachmentPoints = attPoints;
                        calcData.DataSourceID = Guid.Empty;
                        calcData.ProviderNameParents = new List<Guid>(); //no parents have been defined by prov records
                        calcData.ProviderRecordID = "";
                        calcData.GoverningCode = consName.GoverningCode;
                        calcData.ProviderNameCanonical = consName.GetNameProperty(NamePropertyTypeLookUp.Canonical).Value;
                        calcData.ProviderNameFullName = consName.FullName;
                        calcData.ProviderNameRankID = consName.TaxonRankId;

                        CalculateNameParentResult calcRes = CalculateNameParent(cnnStr, calcData);

                        if (calcRes.ParentID != Guid.Empty)
                        {
                            //get TO concept
                            Sql.Repositories.Consensus.ConceptRepository ccrep = new Sql.Repositories.Consensus.ConceptRepository(cnnStr);
                            Entities.Consensus.Concept toConcept = ccrep.GetConcept(calcRes.ParentID, null);

                            if (toConcept == null)
                            {
                                toConcept = new Entities.Consensus.Concept();
                                toConcept.NameId = calcRes.ParentID;
                                toConcept.ConceptId = Guid.NewGuid();
                                toConcept.AddedDate = DateTime.Now;
                                toConcept.State = Entities.Entity.EntityState.Added;

                                ccrep.Save(toConcept, true);
                            }

                            Entities.Consensus.ConceptRelationship ccr = new Entities.Consensus.ConceptRelationship();
                            ccr.ConceptRelationshipId = Guid.NewGuid();
                            ccr.AddedDate = DateTime.Now;
                            ccr.ConceptRelationshipTypeId = crtl.GetConceptRelationshipType(ConceptRelationshipTypeLookUp.IsChildOf).ConceptRelationshipTypeId;
                            ccr.FromConceptId = consConcept.ConceptId;
                            ccr.ToConceptId = toConcept.ConceptId;
                            ccr.IsActive = true;

                            consConcept.ConceptRelationships.Add(ccr);
                            consConcept.State = Entities.Entity.EntityState.Modified;
                        }
                    }
                    else
                    {
                        NZOR.Data.Repositories.Consensus.IConceptRepository ccr = new Repositories.Consensus.ConceptRepository(cnnStr);
                        ccr.DeleteConcept(consConcept, null);
                    }
                }
                else
                {
                    NZOR.Data.Repositories.Consensus.IConceptRepository ccr = new Repositories.Consensus.ConceptRepository(cnnStr);
                    ccr.DeleteConcept(consConcept, null);
                }
            }
            else
            {
                //update fields
                object val = GetConsensusConceptValue(pcList, "Orthography");
                consConcept.Orthography = (val == DBNull.Value ? null : val.ToString());
                val = GetConsensusConceptValue(pcList, "TaxonRank");
                consConcept.TaxonRank = (val == DBNull.Value ? null : val.ToString());
                val = GetConsensusConceptValue(pcList, "HigherClassification");
                consConcept.HigherClassification = (val == DBNull.Value ? null : val.ToString());

                consConcept.ConceptRelationships.Clear();
                foreach (Entities.Common.ConceptRelationshipType crt in ConceptRelationshipTypes(cnnStr))
                {
                    List<Entities.Consensus.ConceptRelationship> rels = GetConsensusConceptRelationships(pcList, crt, parentConceptDefined, cnnStr, attPoints);
                    consConcept.ConceptRelationships.AddRange(rels);
                }

                consConcept.ConceptApplications.Clear();
                foreach (Entities.Common.ConceptApplicationType cat in ConceptApplicationTypes(cnnStr))
                {
                    List<Entities.Consensus.ConceptApplication> apps = GetConsensusConceptApplications(pcList, cat, cnnStr);
                    consConcept.ConceptApplications.AddRange(apps);
                }

                if (consConcept.State != Entities.Entity.EntityState.Added)
                {
                    consConcept.State = Entities.Entity.EntityState.Modified;
                    consConcept.ModifiedDate = DateTime.Now;
                }
            }

            //refresh annotations
            RefreshConceptAnnotations(cnnStr, consConcept.ConceptId);
        }

        public static void RefreshConsensusNameData(Guid consNameID, List<DsIntegrationName.ProviderNameRow> provRecords, DsConsensusData consData)
        {
            DsConsensusData.ConsensusNameRow name = consData.ConsensusName.FindByNameID(consNameID);

            foreach (DataColumn dc in consData.ConsensusName.Columns)
            {
                if ("NameID,NameClassID,NameClass,TaxonRankID,TaxonRank,TaxonRankSort,ParentIDsToRoot,ParentID,Parent,PreferredName,PreferredNameID,BasionymID,".IndexOf(dc.ColumnName + ",") == -1)
                {
                    object val = GetConsensusValue(provRecords, dc.ColumnName);
                    name[dc.ColumnName] = val;
                }
            }
        }

        public static void RefreshConsensusReferenceData(Guid consRefID, DsIntegrationReference refData, DsConsensusData consData)
        {
            DsConsensusData.ConsensusReferenceRow cref = consData.ConsensusReference.FindByReferenceID(consRefID);
            DsIntegrationReference.ProviderReferenceRow[] provRecords = (DsIntegrationReference.ProviderReferenceRow[])refData.ProviderReference.Select("ConsensusReferenceID = '" + consRefID.ToString() + "'");

            foreach (DsIntegrationReference.ProviderReferenceRow pr in provRecords)
            {
                //at this stage just simple exact copies (ie consesus == provider) - so all prov records are the same, so just copy
                cref.Citation = pr.Citation;
                cref.Properties = pr.Properties;
                break;
            }
        }

        /// <summary>
        /// Refresh all linking Ids (RelatedId in the NameProperty table)
        /// Also Refresh all names that could be linked to this name (eg update Basionym for other names that have this name as their BasionymID)
        /// and all values that this name could be linked to (eg update the Basionym of this name, depending on the BasionymID that this name has)
        /// </summary>
        /// <param name="consNameID"></param>
        /// <param name="data"></param>
        public static void RefreshNameLinks(string cnnStr, Entities.Consensus.Name cn, List<Entities.Provider.Name> providerNames)
        {
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Name-UpdateLinks.sql");

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@nameID", cn.NameId);

                    cmd.ExecuteNonQuery();
                }
            }

            Data.Sql.Repositories.Provider.NameRepository pnr = new Repositories.Provider.NameRepository(cnnStr);
            Data.Sql.Repositories.Provider.ReferenceRepository pcr = new Repositories.Provider.ReferenceRepository(cnnStr);
            Data.Sql.Repositories.Consensus.NameRepository nr = new Repositories.Consensus.NameRepository(cnnStr);
            Data.Sql.Repositories.Common.LookUpRepository lr = new LookUpRepository(cnnStr);
            Data.LookUps.Common.NamePropertyTypeLookUp nptl = new NamePropertyTypeLookUp(lr.GetNamePropertyTypes());

            foreach (Entities.Consensus.NameProperty np in cn.NameProperties)
            {
                //get consensus related id
                List<Entities.Provider.NameProperty> props = new List<Entities.Provider.NameProperty>();
                Guid? provRelId = null;
                foreach (Entities.Provider.Name pn in providerNames)
                {
                    Entities.Provider.NameProperty pnp = pn.GetNameProperty(np.NamePropertyType);
                    if (pnp != null)
                    {
                        NamePropertyType npt = nptl.GetNamePropertyType(pnp.NamePropertyTypeId);
                        bool allowMultiple = (!npt.MaxOccurrences.HasValue || npt.MaxOccurrences > 1);
                        if (pnp.Value.Equals(np.Value))
                        {
                            if (provRelId != null && provRelId != pnp.RelatedId && !allowMultiple) provRelId = null; //clash - two properties of the same type with the same value but point to different related data
                            //TODO - mark somehow, or alert, report?
                            else provRelId = pnp.RelatedId;
                        }
                    }
                }

                if (provRelId != null)
                {
                    //get consensus related id
                    if (np.NamePropertyType == LookUps.Common.NamePropertyTypeLookUp.PublishedIn)
                    {
                        np.RelatedId = pcr.GetReference(provRelId.Value).ConsensusReferenceId;
                    }
                    else
                    {
                        np.RelatedId = pnr.GetName(provRelId.Value).ConsensusNameId;
                    }

                    if (np.State != Entities.Entity.EntityState.Added)
                    {
                        np.State = Entities.Entity.EntityState.Modified;
                        np.ModifiedDate = DateTime.Now;
                    }

                    nr.SaveNameProperty(cn.NameId, np);

                    np.State = Entities.Entity.EntityState.Unchanged;
                }
            }
        }

        private static List<object> GetConsensusValues(List<Entities.Provider.Name> provRecords, string field, bool allowMultiple)
        {
            Dictionary<object, int> vals = new Dictionary<object, int>();

            object editorVal = DBNull.Value;
            foreach (Entities.Provider.Name pn in provRecords)
            {
                //TODO - add editor type records ???
                //if (!row.IsNull("ProviderIsEditor") && (bool)row["ProviderIsEditor"] && !row.IsNull(sourceCol))
                //{
                //    editorVal = row[sourceCol];
                //    break;
                //}

                Entities.Provider.NameProperty pnp = pn.GetNameProperty(field);
                if (pnp != null)
                {
                    object objVal = pnp.Value;
                    if (objVal != null && objVal.ToString().Length > 0)
                    {
                        if (vals.ContainsKey(objVal))
                        {
                            vals[objVal] += 1;
                        }
                        else
                        {
                            if (!object.ReferenceEquals(objVal, DBNull.Value))
                                vals.Add(objVal, 1);
                        }
                    }
                }
            }

            if (editorVal != DBNull.Value)
                return new List<object> { editorVal };

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
            //always return something for canonical / full name 
            if (!allowMultiple && hasEqual && field == NamePropertyTypeLookUp.Canonical || field == "FullName")
                return new List<object> { val };

            if ((hasEqual || vals.Keys.Count > 1) && !allowMultiple)
                return null;

            return new List<object>(vals.Keys);
        }

        private static object GetConsensusValue(List<DsIntegrationName.ProviderNameRow> provRecords, string sourceCol)
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
            //always return something for canonical / full name 
            if (sourceCol == "Canonical" || sourceCol == "FullName")
                return val;

            if (hasEqual)
                return DBNull.Value;

            return val;
        }

        private static ConsensusValueResult GetConsensusValue(List<Entities.Provider.NameProperty> properties, Guid propertyTypeId)
        {
            ConsensusValueResult cvr = new ConsensusValueResult();

            Dictionary<object, int> vals = new Dictionary<object, int>();
            Dictionary<object, object> seqs = new Dictionary<object, object>();
            Dictionary<object, object> relIds = new Dictionary<object, object>();

            object editorVal = DBNull.Value;
            foreach (Entities.Provider.NameProperty np in properties)
            {
                if (np.NamePropertyTypeId == propertyTypeId)
                {
                    //TODO - add editor type records ???
                    //if (!row.IsNull("ProviderIsEditor") && (bool)row["ProviderIsEditor"] && !row.IsNull(sourceCol))
                    //{
                    //    editorVal = row[sourceCol];
                    //    break;
                    //}

                    if (np.Value != null && np.Value.ToString().Length > 0)
                    {
                        if (vals.ContainsKey(np.Value))
                        {
                            vals[np.Value] += 1;
                        }
                        else
                        {
                            if (!object.ReferenceEquals(np.Value, DBNull.Value) && np.Value != null)
                            {
                                vals.Add(np.Value, 1);
                                seqs.Add(np.Value, np.Sequence);
                                relIds.Add(np.Value, np.RelatedId);
                            }
                        }
                    }
                }
            }

            if (editorVal != DBNull.Value)
            {
                cvr.HasMajority = true;
                cvr.Value = editorVal;
                //todo rel and seq vals
            }
            else
            {
                //get majority value (must be > majority than next common value, ie if equal number of 2 diff values, then there is no consensus)
                int maxNum = 0;
                bool hasEqual = false;
                foreach (object key in vals.Keys)
                {
                    if (vals[key] > maxNum)
                    {
                        maxNum = vals[key];
                        cvr.Value = key;
                        cvr.RelatedId = (Guid?)relIds[key];
                        cvr.Sequence = (int?)seqs[key];
                        hasEqual = false;
                    }
                    else if (vals[key] == maxNum)
                    {
                        hasEqual = true;
                    }
                }

                if (hasEqual)
                {
                    cvr.Value = DBNull.Value;
                    cvr.Sequence = null;
                    cvr.RelatedId = null;
                    cvr.HasMajority = false;
                }
                else
                {
                    cvr.HasMajority = true;
                }

            }


            return cvr;
        }

        private class ConceptVal : IEquatable<ConceptVal>
        {
            public Guid? FromAccordingToId = null;
            public Guid ToNameId = Guid.Empty;
            public ConceptVal(Guid? fromAccTodId, Guid toNameId)
            {
                FromAccordingToId = fromAccTodId;
                ToNameId = toNameId;
            }
            public override int GetHashCode()
            {
                int accTo = 0;
                if (FromAccordingToId.HasValue) accTo = FromAccordingToId.Value.GetHashCode();
                return accTo ^ ToNameId.GetHashCode();
            }

            public bool Equals(ConceptVal other)
            {
                if (((!other.FromAccordingToId.HasValue && !this.FromAccordingToId.HasValue) || other.FromAccordingToId.Value == this.FromAccordingToId.Value) &&
                    this.ToNameId == other.ToNameId)
                {
                    return true;
                }
                return false;
            }
        }

        public static ConsensusValueResult GetConsensusValue(List<Entities.Provider.Concept> concepts, Guid conceptRelationshipTypeId, Data.Repositories.Consensus.IConceptRepository consConceptRepository)
        {
            ConsensusValueResult cvr = new ConsensusValueResult();

            Dictionary<ConceptVal, int> vals = new Dictionary<ConceptVal, int>();
            Dictionary<ConceptVal, object> seqs = new Dictionary<ConceptVal, object>();

            object editorVal = DBNull.Value;
            foreach (Entities.Provider.Concept pc in concepts)
            {
                //TODO - add editor type records ???
                //if (!row.IsNull("ProviderIsEditor") && (bool)row["ProviderIsEditor"] && !row.IsNull(sourceCol))
                //{
                //    editorVal = row[sourceCol];
                //    break;
                //}

                foreach (Entities.Provider.ConceptRelationship pcr in pc.ConceptRelationships)
                {
                    if (pcr.ConceptRelationshipTypeId == conceptRelationshipTypeId)
                    {
                        if ((!pcr.InUse.HasValue || pcr.InUse.Value) && pcr.ToConceptId.HasValue)
                        {
                            Entities.Consensus.Concept cc = consConceptRepository.GetConcensusConcept(pcr.ToConceptId.Value);
                            Entities.Consensus.Concept ccFrom = consConceptRepository.GetConcensusConcept(pcr.FromConceptId.Value);

                            if (cc != null)
                            {
                                Guid accToId = Guid.Empty;
                                if (ccFrom.AccordingToReferenceId.HasValue) accToId = ccFrom.AccordingToReferenceId.Value;

                                ConceptVal cv = new ConceptVal(accToId, cc.NameId);

                                if (vals.ContainsKey(cv))
                                {
                                    vals[cv] += 1;
                                }
                                else
                                {
                                    vals.Add(cv, 1);
                                    seqs.Add(cv, pcr.Sequence);
                                }
                            }
                        }
                    }
                }
            }

            if (editorVal != DBNull.Value)
            {
                cvr.HasMajority = true;
                cvr.Value = editorVal;
                //todo rel and seq vals
            }
            else
            {
                //get majority value (must be > majority than next common value, ie if equal number of 2 diff values, then there is no consensus)
                int maxNum = 0;
                bool hasEqual = false;
                foreach (ConceptVal key in vals.Keys)
                {
                    if (vals[key] > maxNum)
                    {
                        maxNum = vals[key];
                        cvr.Value = key;
                        cvr.Sequence = (int?)seqs[key];
                        hasEqual = false;
                    }
                    else if (vals[key] == maxNum)
                    {
                        hasEqual = true;
                    }
                }

                if (hasEqual)
                {
                    cvr.Value = DBNull.Value;
                    cvr.Sequence = null;
                    cvr.HasMajority = false;
                }
                else
                {
                    cvr.HasMajority = true;
                }

            }


            return cvr;
        }

        public static bool HasProviderValue(SqlConnection cnn, Guid nameID, String field, object value)
        {
            bool hasVal = false;

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "select count(NameID) from provider.Name pn " +
                    " inner join provider.NameProperty np on np.NameID = pn.NameID " +
                    " inner join dbo.NamePropertyType ncp on ncp.NamePropertyTypeID = np.NamePropertyTypeID and ncp.Name = '" + field + "' " +
                    " where pn.ConsensusNameID = '" + nameID.ToString() + "' and (np.Value is null or np.Value = '" + value.ToString() + "')";

                int cnt = (int)cmd.ExecuteScalar();
                if (cnt > 0) hasVal = true;
            }

            return hasVal;
        }

        /// <summary>
        /// Get consensus values for concept relationships of a particular relationship type (and for the same concept - ie NameId + AccToId)
        /// The provider concepts passed in must be for the same concept (nameid + acctoid)
        /// </summary>
        /// <param name="provRecords"></param>
        /// <param name="crType"></param>
        /// <returns></returns>
        private static List<Entities.Consensus.ConceptRelationship> GetConsensusConceptRelationships(List<Entities.Provider.Concept> provRecords, Entities.Common.ConceptRelationshipType crType, bool parentConceptDefined, String cnnStr, List<Admin.Data.Entities.AttachmentPoint> attPoints)
        {
            List<Entities.Consensus.ConceptRelationship> crList = new List<Entities.Consensus.ConceptRelationship>();

            List<Guid> relToIds = new List<Guid>();
            List<Guid> inUseIds = new List<Guid>();

            Sql.Repositories.Provider.ConceptRepository pcr = new Sql.Repositories.Provider.ConceptRepository(cnnStr);
            Sql.Repositories.Provider.NameRepository pnr = new Repositories.Provider.NameRepository(cnnStr);
            Sql.Repositories.Common.LookUpRepository lr = new LookUpRepository(cnnStr);
            LookUps.Common.ConceptRelationshipTypeLookUp crtl = new ConceptRelationshipTypeLookUp(lr.GetConceptRelationshipTypes());
            LookUps.Common.NameClassLookUp ncl = new NameClassLookUp(lr.GetNameClasses());

            foreach (Entities.Provider.Concept pc in provRecords)
            {
                if (pc.ConsensusConceptId.HasValue)
                {
                    //if this is the parent relationship type and there are no provider relationships for it then we need to calculate it
                    if (crType.Relationship == LookUps.Common.ConceptRelationshipTypeLookUp.IsChildOf && !parentConceptDefined)
                    {
                        Entities.Provider.Name provName = pnr.GetName(pc.NameId);
                        NameClass pnnc = ncl.GetNameClassById(provName.NameClassId);

                        if (pnnc.Name == NZOR.Data.LookUps.Common.NameClassLookUp.ScientificName)
                        {
                            CalculateNameParentData calcData = new CalculateNameParentData();
                            calcData.ProviderNameRankSort = TaxonRankLookup(cnnStr).GetTaxonRank(provName.TaxonRankId).SortOrder.Value;
                            calcData.ProviderNameID = provName.NameId;
                            calcData.AttachmentPoints = attPoints;
                            calcData.DataSourceID = provName.DataSourceId;
                            calcData.ProviderNameParents = new List<Guid>(); //no parents have been defined by prov records
                            calcData.ProviderRecordID = provName.ProviderRecordId;
                            calcData.GoverningCode = provName.GoverningCode;
                            calcData.ProviderNameCanonical = provName.GetNameProperty(NamePropertyTypeLookUp.Canonical).Value;
                            calcData.ProviderNameFullName = provName.FullName;
                            calcData.ProviderNameRankID = provName.TaxonRankId;

                            CalculateNameParentResult calcRes = CalculateNameParent(cnnStr, calcData);

                            if (calcRes.ParentID != Guid.Empty)
                            {
                                //get TO concept
                                Sql.Repositories.Consensus.ConceptRepository ccrep = new Sql.Repositories.Consensus.ConceptRepository(cnnStr);
                                Entities.Consensus.Concept toConcept = ccrep.GetConcept(calcRes.ParentID, null);

                                if (toConcept == null)
                                {
                                    toConcept = new Entities.Consensus.Concept();
                                    toConcept.NameId = calcRes.ParentID;
                                    toConcept.ConceptId = Guid.NewGuid();
                                    toConcept.AddedDate = DateTime.Now;
                                    toConcept.State = Entities.Entity.EntityState.Added;

                                    ccrep.Save(toConcept, true);
                                }

                                Entities.Consensus.ConceptRelationship ccr = new Entities.Consensus.ConceptRelationship();
                                ccr.ConceptRelationshipId = Guid.NewGuid();
                                ccr.AddedDate = DateTime.Now;
                                ccr.ConceptRelationshipTypeId = crtl.GetConceptRelationshipType(ConceptRelationshipTypeLookUp.IsChildOf).ConceptRelationshipTypeId;
                                ccr.FromConceptId = pc.ConsensusConceptId;
                                ccr.ToConceptId = toConcept.ConceptId;
                                ccr.IsActive = true;

                                crList.Add(ccr);
                            }
                        }
                    }


                    //add distinct relationships
                    List<Entities.Provider.ConceptRelationship> rels = pc.GetRelationships(crType.ConceptRelationshipTypeId);

                    foreach (Entities.Provider.ConceptRelationship cr in rels)
                    {
                        if (!relToIds.Contains(cr.ToConceptId.Value))
                        {
                            if ((cr.InUse ?? false) && crType.MaxOccurrences.HasValue && inUseIds.Count == crType.MaxOccurrences)
                            {
                                //too many!
                                //TODO alert?
                                crList.Clear();
                                return crList;
                            }

                            relToIds.Add(cr.ToConceptId.Value);
                            if (cr.InUse.HasValue && cr.InUse.Value) inUseIds.Add(cr.ToConceptId.Value);

                            Entities.Provider.Concept toPC = pcr.GetConcept(cr.ToConceptId.Value);

                            if (toPC.ConsensusConceptId.HasValue)
                            {
                                Entities.Consensus.ConceptRelationship ccr = new Entities.Consensus.ConceptRelationship();
                                ccr.ConceptRelationshipId = Guid.NewGuid();
                                ccr.AddedDate = DateTime.Now;
                                ccr.ConceptRelationshipTypeId = cr.ConceptRelationshipTypeId;
                                ccr.FromConceptId = pc.ConsensusConceptId;
                                ccr.ToConceptId = toPC.ConsensusConceptId;
                                ccr.IsActive = cr.InUse ?? false;

                                crList.Add(ccr);
                            }
                        }
                    }
                }
            }

            return crList;
        }

        private static List<Entities.Consensus.ConceptApplication> GetConsensusConceptApplications(List<Entities.Provider.Concept> provRecords, Entities.Common.ConceptApplicationType appType, string cnnStr)
        {
            List<Entities.Consensus.ConceptApplication> caList = new List<Entities.Consensus.ConceptApplication>();

            Sql.Repositories.Provider.ConceptRepository pcr = new Sql.Repositories.Provider.ConceptRepository(cnnStr);
            Sql.Repositories.Common.LookUpRepository lr = new LookUpRepository(cnnStr);

            foreach (Entities.Provider.Concept pc in provRecords)
            {
                if (pc.ConsensusConceptId.HasValue)
                {
                    List<Entities.Provider.ConceptApplication> apps = pc.GetApplications(appType.ConceptApplicationTypeId);
                    foreach (Entities.Provider.ConceptApplication ca in apps)
                    {
                        //distinct appl?
                        Entities.Consensus.ConceptApplication exCa = caList.FirstOrDefault<Entities.Consensus.ConceptApplication>(o => o.ConceptApplicationTypeId == ca.ConceptApplicationTypeId
                            && o.FromConceptId == ca.FromConceptId
                            && o.Gender == ca.Gender
                            && o.GeoRegion == ca.GeoRegion
                            && o.GeographicSchema == ca.GeographicSchema
                            && o.LifeStage == ca.LifeStage
                            && o.PartOfTaxon == ca.PartOfTaxon
                            && o.ToConceptId == ca.ToConceptId);

                        if (exCa == null)
                        {
                            Entities.Provider.Concept toPC = pcr.GetConcept(ca.ToConceptId);

                            if (toPC.ConsensusConceptId.HasValue)
                            {
                                Entities.Consensus.ConceptApplication cca = new Entities.Consensus.ConceptApplication();
                                cca.ConceptApplicationId = Guid.NewGuid();
                                cca.AddedDate = DateTime.Now;
                                cca.ConceptApplicationTypeId = ca.ConceptApplicationTypeId;
                                cca.FromConceptId = pc.ConsensusConceptId.Value;
                                cca.ToConceptId = toPC.ConsensusConceptId.Value;

                                cca.Gender = ca.Gender;
                                cca.PartOfTaxon = ca.PartOfTaxon;
                                cca.LifeStage = ca.LifeStage;

                                Entities.Common.GeoRegion region = lr.GetGeoRegionByName(ca.GeoRegion);
                                if (region != null)
                                {
                                    cca.GeoRegionId = region.GeoRegionId;
                                    if (region.CorrectGeoRegionId != null) cca.GeoRegionId = region.CorrectGeoRegionId;
                                    cca.GeographicSchemaId = region.GeographicSchemaId;
                                }

                                caList.Add(cca);
                            }
                        }
                    }
                }
            }

            return caList;
        }

        public static void UpdateChildrenParentLinks(DsIntegrationName.ProviderNameRow pnRow, DataForIntegration nameData)
        {
            //provider name has just been linked to a consensus name so the children need to be updated - ie the child.ParentNames value to include the consensus name id 
            if (!pnRow.IsConsensusNameIDNull())
            {
                List<DsIntegrationName.ProviderNameRow> children = nameData.GetChildProviderNames(pnRow.NameID);
                foreach (DataRow ch in children)
                {
                    //for some weird reason we get null sometimes
                    if (ch != null)
                    {
                        //format is [parent id:parent consensus id:parent full name],[ ...]
                        int pos = ch["ParentNames"].ToString().IndexOf(pnRow.NameID.ToString() + ":", StringComparison.InvariantCultureIgnoreCase) + 1 + pnRow.NameID.ToString().Length;
                        int endPos = ch["ParentNames"].ToString().IndexOf(":", pos);

                        if (pos > 0 && pos == endPos) //consensus name id not currently specified
                        {
                            ch["ParentNames"] = ch["ParentNames"].ToString().Insert(pos, pnRow.ConsensusNameID.ToString());
                        }
                    }
                }
            }
        }

        public static DsNameMatch GetNamesWithProperty(SqlConnection cnn, String field, object value)
        {
            DsNameMatch ds = new DsNameMatch();

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "declare @ids table(id uniqueidentifier); " +
                    "insert @ids select distinct n.NameID from consensus.Name n inner join consensus.NameProperty np on np.NameID = n.NameID " +
                    " inner join dbo.NamePropertyType ncp on ncp.NamePropertyTypeID = np.NamePropertyTypeID and ncp.Name = '" + field + "' " +
                    " where np.Value = '" + value.ToString() + "'; " +
                    "select n.* from consensus.Name n inner join @ids i on i.id = n.NameID; " +
                    "select np.*, ncp.Name from consensus.NameProperty np inner join @ids i on i.id = np.NameID inner join dbo.NamePropertyType ncp on ncp.NamePropertyTypeID = np.NamePropertyTypeID; " +
                    "select c.* from consensus.vwConcepts c inner join @ids i on i.id = c.NameID and c.IsActive = 1; ";

                DataSet res = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(res);

                if (res != null && res.Tables.Count > 0)
                {
                    foreach (DataRow row in res.Tables[0].Rows)
                    {
                        Guid id = (Guid)row["NameID"];

                        DataRow ntRow = GetNameConcept(id, res.Tables[2], LookUps.Common.ConceptRelationshipTypeLookUp.IsChildOf);
                        object nameTo = DBNull.Value;
                        if (ntRow != null && ntRow["NameToID"] != DBNull.Value) nameTo = (Guid)ntRow["NameToID"];

                        ds.Name.Rows.Add(id,
                            GetNamePropertyValue(id, res.Tables[1], NamePropertyTypeLookUp.Canonical),
                            row["FullName"],
                            GetNamePropertyValue(id, res.Tables[1], NamePropertyTypeLookUp.Rank),
                            GetNamePropertyValue(id, res.Tables[1], NamePropertyTypeLookUp.Authors),
                            GetNamePropertyValue(id, res.Tables[1], NamePropertyTypeLookUp.CombinationAuthors),
                            GetNamePropertyValue(id, res.Tables[1], NamePropertyTypeLookUp.Year),
                            nameTo,
                            row["GoverningCode"],
                            100);
                    }
                }
            }

            return ds;
        }


        //        public static DsNameMatch GetNamesWithConcept(String cnnStr, String conceptType, Guid nameToID)
        //        {
        //            DsNameMatch ds = new DsNameMatch();

        //            using (SqlConnection cnn = new SqlConnection(cnnStr))
        //            {
        //                cnn.Open();

        //                using (SqlCommand cmd = cnn.CreateCommand())
        //                {
        //                    cmd.CommandText = @"
        //	                    declare @ids table(id uniqueidentifier)
        //                    		
        //                        insert @ids 
        //                        select distinct n.NameID 
        //                        from consensus.Name n 
        //                        inner join consensus.vwConcepts cc on cc.NameID = n.NameID 
        //                        where Relationship = '" + conceptType + "' and NameToID = '" + nameToID.ToString() + @"'
        //                        
        //                        select n.* 
        //                        from consensus.Name n 
        //                        inner join @ids i on i.id = n.NameID
        //                        
        //                        select np.*, ncp.Name 
        //                        from consensus.NameProperty np 
        //                        inner join @ids i on i.id = np.NameID 
        //                        inner join dbo.NamePropertyType ncp on ncp.NamePropertyTypeID = np.NamePropertyTypeID
        //                                        
        //	                    select c.* 
        //	                    from consensus.vwConcepts c 
        //	                    inner join @ids i on i.id = c.NameID";


        //                    DataSet res = new DataSet();
        //                    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //                    da.Fill(res);

        //                    foreach (DataRow row in res.Tables[0].Rows)
        //                    {
        //                        Guid id = (Guid)row["NameID"];

        //                        DataRow ntRow = GetNameConcept(id, res.Tables[2], ConceptProperties.ParentRelationshipType);
        //                        object nameTo = DBNull.Value;
        //                        if (ntRow != null && ntRow["NameToID"] != DBNull.Value) nameTo = (Guid)ntRow["NameToID"];

        //                        ds.Name.Rows.Add(id,
        //                            GetNamePropertyValue(id, res.Tables[1], NameProperties.Canonical),
        //                            row["FullName"],
        //                            GetNamePropertyValue(id, res.Tables[1], NameProperties.Rank),
        //                            GetNamePropertyValue(id, res.Tables[1], NameProperties.Authors),
        //                            GetNamePropertyValue(id, res.Tables[1], NameProperties.CombinationAuthors),
        //                            GetNamePropertyValue(id, res.Tables[1], NameProperties.Year),
        //                            nameTo,
        //                            100);
        //                    }
        //                }
        //            }

        //            return ds;
        //        }

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

        public static Entities.Consensus.Name ConsensusNameFromIntegrationData(DsIntegrationName.ProviderNameRow provName)
        {
            Entities.Consensus.Name n = new Entities.Consensus.Name();
            throw new NotImplementedException();
            //return n;
        }

        public static Entities.Consensus.Reference ConsensusReferenceFromIntegrationData(DsConsensusData.ConsensusReferenceRow consRef, String cnnStr)
        {
            Entities.Consensus.Reference r = new Entities.Consensus.Reference();

            r.AddedDate = DateTime.Now;
            r.ReferenceId = consRef.ReferenceID;
            r.ReferenceTypeId = consRef.ReferenceTypeID;
            r.State = Entities.Entity.EntityState.Added;

            Entities.Common.ReferencePropertyType rpType = (Entities.Common.ReferencePropertyType)(from rpt in ReferencePropertyTypes(cnnStr) where rpt.Name.Equals(LookUps.Common.ReferencePropertyTypeLookUp.Citation) select rpt).SingleOrDefault();
            Entities.Consensus.ReferenceProperty rp = new Entities.Consensus.ReferenceProperty();
            rp.ReferencePropertyId = Guid.NewGuid();
            rp.ReferencePropertyTypeId = rpType.ReferencePropertyTypeId;
            rp.Value = consRef.Citation;
            rp.State = Entities.Entity.EntityState.Added;
            r.ReferenceProperties.Add(rp);

            return r;
        }

        public static Entities.Consensus.Name ConsensusNameFromIntegrationData(DsConsensusData.ConsensusNameRow consName, String cnnStr)
        {
            Entities.Consensus.Name n = new Entities.Consensus.Name();

            n.AddedDate = DateTime.Now;
            n.FullName = consName.FullName;
            n.GoverningCode = (consName.IsGoverningCodeNull() ? null : consName.GoverningCode);
            n.NameClassId = consName.NameClassID;
            n.NameId = consName.NameID;
            n.TaxonRankId = consName.TaxonRankID;
            n.State = Entities.Entity.EntityState.Added;

            if (consName.IsCanonicalNull())
            {
                //string msg = "Canonical is null!";
            }

            //get properties for class type
            foreach (Entities.Common.NamePropertyType npt in NamePropertyTypes(cnnStr, consName.NameClassID))
            {
                //get value
                object val = DBNull.Value;
                switch (npt.Name)
                {
                    case NamePropertyTypeLookUp.Canonical:
                        val = consName["Canonical"];
                        break;
                    case NamePropertyTypeLookUp.Authors:
                        val = consName["Authors"];
                        break;
                    case NamePropertyTypeLookUp.BasionymAuthors:
                        val = consName["BasionymAuthors"];
                        break;
                    case NamePropertyTypeLookUp.CombinationAuthors:
                        val = consName["CombinationAuthors"];
                        break;
                    case NamePropertyTypeLookUp.Country:
                        val = consName["Country"];
                        break;
                    case NamePropertyTypeLookUp.Language:
                        val = consName["Language"];
                        break;
                    case NamePropertyTypeLookUp.MicroReference:
                        val = consName["MicroReference"];
                        break;
                    case NamePropertyTypeLookUp.PublishedIn:
                        val = consName["PublishedIn"];
                        break;
                    case NamePropertyTypeLookUp.Rank:
                        val = consName["TaxonRank"];
                        break;
                    case NamePropertyTypeLookUp.YearOfPublication:
                        val = consName["YearOfPublication"];
                        break;
                }

                if (val != DBNull.Value)
                {
                    Entities.Consensus.NameProperty np = new Entities.Consensus.NameProperty();
                    np.AddedDate = DateTime.Now;
                    np.NamePropertyId = Guid.NewGuid();
                    np.NamePropertyTypeId = npt.NamePropertyTypeId;
                    np.NamePropertyType = npt.Name;
                    np.Value = val.ToString();
                    np.State = Entities.Entity.EntityState.Added;

                    n.NameProperties.Add(np);
                }
            }

            return n;
        }

        /// <summary>
        /// Get the Consensus Concept data for a particular consensus name data row.  This does not include the relationships as this cant really be done until after ALL concepts have been saved.
        /// </summary>
        /// <param name="consName"></param>
        /// <param name="provConcepts"></param>
        /// <param name="cnnStr"></param>
        /// <returns></returns>
        public static List<Entities.Consensus.Concept> ConsensusConceptsFromIntegrationData(DsConsensusData.ConsensusNameRow consName, List<Entities.Provider.Concept> provConcepts, String cnnStr)
        {
            List<Entities.Consensus.Concept> ccList = new List<Entities.Consensus.Concept>();

            //need to create a concept for each provider concept accordingtoId
            Dictionary<Guid, Entities.Consensus.Concept> accToIds = new Dictionary<Guid, Entities.Consensus.Concept>();
            //Dictionary<Guid, List<Entities.Provider.Concept>> provConceptsFoConcept = new Dictionary<Guid, List<Entities.Provider.Concept>>();

            Sql.Repositories.Provider.ReferenceRepository rr = new Sql.Repositories.Provider.ReferenceRepository(cnnStr);

            foreach (Entities.Provider.Concept pc in provConcepts)
            {
                Entities.Consensus.Concept cc = null;
                Guid id = Guid.Empty;
                if (pc.AccordingToReferenceId.HasValue) id = pc.AccordingToReferenceId.Value;
                if (!accToIds.ContainsKey(id))
                {
                    Entities.Provider.Reference pr = null;
                    if (pc.AccordingToReferenceId.HasValue) pr = rr.GetReference(pc.AccordingToReferenceId.Value);

                    cc = new Entities.Consensus.Concept();
                    cc.ConceptId = Guid.NewGuid();
                    cc.NameId = consName.NameID;

                    if (pr != null) cc.AccordingToReferenceId = pr.ConsensusReferenceId;
                    cc.AddedDate = DateTime.Now;
                    cc.State = Entities.Entity.EntityState.Added;

                    accToIds.Add(id, cc);
                    //provConceptsFoConcept.Add(cc.ConceptId, new List<Entities.Provider.Concept>(){pc});

                    ccList.Add(cc);
                }
                else
                {
                    cc = accToIds[id];
                }

                pc.ConsensusConceptId = cc.ConceptId;
                pc.LinkStatus = Data.LinkStatus.Inserted.ToString();
            }

            //List<Entities.Common.ConceptRelationshipType> crTypes = lookUpRepository.GetConceptRelationshipTypes();
            //List<Entities.Common.ConceptApplicationType> caTypes = lookUpRepository.GetConceptApplicationTypes();
            //foreach (Entities.Consensus.Concept cc in ccList)
            //{
            //    cc.ConceptRelationships.Clear();
            //    foreach (Entities.Common.ConceptRelationshipType crt in crTypes)
            //    {
            //        List<Entities.Consensus.ConceptRelationship> rels = GetConsensusConceptRelationships(provConceptsFoConcept[cc.ConceptId], crt, cnnStr);
            //        cc.ConceptRelationships.AddRange(rels);
            //    }

            //    cc.ConceptApplications.Clear();
            //    foreach (Entities.Common.ConceptApplicationType cat in caTypes)
            //    {
            //        List<Entities.Consensus.ConceptApplication> apps = GetConsensusConceptApplications(provConceptsFoConcept[cc.ConceptId], cat, cnnStr);
            //        cc.ConceptApplications.AddRange(apps);
            //    } 
            //}

            return ccList;
        }

        public static void UpdateConsensusStackedNameData(string cnnStr, Guid nameID)
        {
            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "delete consensus.StackedName where SeedNameID = '" + nameID.ToString() + "'";
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT consensus.StackedName(SeedNameID, AccordingToID, NameID, TaxonRankID, CanonicalName, RankName, SortOrder, Depth)
	                                EXEC consensus.sprSelect_StackedNameToRoot '" + nameID.ToString() + "'";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateConsensusStackedNameData(String cnnStr)
        {
            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandTimeout = 10000;
                    cmd.CommandText = @"EXEC consensus.sprUpdate_StackedNameData";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region "Provider Name"

        public static DataSet GetProviderName(String cnnStr, Guid provNameId)
        {
            DataSet ds = new DataSet();

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = @"
	                        select * 
	                        from provider.Name pn
	                        inner join TaxonRank tr on tr.TaxonRankID = pn.TaxonRankID
	                        where NameID = '" + provNameId.ToString() + @"'
                        	
	                        select * 
	                        from provider.NameProperty np
	                        inner join NamePropertyType ncp on ncp.NamePropertyTypeID = np.NamePropertyTypeID
	                        where NameID = '" + provNameId.ToString() + @"'
                        	
	                        select * 
	                        from provider.vwConcepts
	                        where NameID = '" + provNameId.ToString() + @"'";

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);

                    ds.Tables[0].TableName = "Name";
                    ds.Tables[1].TableName = "NameProperty";
                    ds.Tables[2].TableName = "Concepts";
                }
            }

            return ds;
        }

        public static void UpdateProviderStackedNameData(String cnnStr)
        {
            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandTimeout = 10000;
                    cmd.CommandText = @"EXEC provider.sprUpdate_StackedNameData";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateProviderStackedNameData(Guid nameID, SqlConnection cnn)
        {
            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "delete provider.StackedName where SeedNameID = '" + nameID.ToString() + "'";
                cmd.ExecuteNonQuery();
            }

            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"INSERT provider.StackedName(StackedNameID, SeedNameID, AccordingToID, NameID, TaxonRankID, CanonicalName, RankName, SortOrder, Depth)
                                    EXEC provider.sprSelect_StackedNameToRoot '" + nameID.ToString() + "'";
                cmd.ExecuteNonQuery();
            }
        }

        public static List<Guid> GetParentNameIDs(DsIntegrationName.ProviderNameRow pnRow)
        {
            List<Guid> parentIds = new List<Guid>();

            if (pnRow.IsParentNamesNull()) return parentIds;

            //parse ids
            //format is [parent id:parent consensus id:parent full name],[ ...]
            String[] ids = pnRow.ParentNames.Split('[');

            foreach (String id in ids)
            {
                if (id.Length > 0)
                {
                    int pos = 0;
                    int endPos = id.IndexOf(":", pos);

                    Guid pid = Guid.Empty;
                    string idStr = id.Substring(pos, endPos - pos);
                    if (idStr.Length > 0)
                    {
                        if (Guid.TryParse(idStr, out pid))
                        {
                            parentIds.Add(pid);
                        }
                    }
                }
            }

            return parentIds;
        }

        public static List<Guid> GetParentConsensusNameIDs(DsIntegrationName.ProviderNameRow pnRow)
        {
            List<Guid> parentIds = new List<Guid>();

            if (pnRow.IsParentNamesNull()) return parentIds;

            //parse ids
            //format is [parent id:parent consensus id:parent full name],[ ...]
            String[] ids = pnRow.ParentNames.Split('[');

            foreach (String id in ids)
            {
                if (id.Length > 0)
                {
                    int pos = id.IndexOf(":") + 1;
                    int endPos = id.IndexOf(":", pos);

                    Guid pid = Guid.Empty;

                    string idStr = id.Substring(pos, endPos - pos);
                    if (idStr.Length > 0)
                    {
                        if (Guid.TryParse(idStr, out pid)) parentIds.Add(pid);
                    }
                }
            }

            return parentIds;
        }

        public static List<String> GetParentConsensusNames(DsIntegrationName.ProviderNameRow pnRow)
        {
            List<String> parentNames = new List<String>();

            if (pnRow.IsParentNamesNull()) return parentNames;

            String[] ids = pnRow.ParentNames.Split('[');

            //parse names
            //format is [parent id:parent consensus id:parent full name],[ ...]
            foreach (String id in ids)
            {
                if (id.Length > 0)
                {
                    int pos = id.IndexOf(":") + 1;
                    pos = id.IndexOf(":", pos) + 1;
                    int endPos = id.IndexOf("]", pos);

                    String pn = id.Substring(pos, endPos - pos);
                    parentNames.Add(pn);
                }
            }

            return parentNames;
        }

        public static Guid GetPreferredNameID(DsIntegrationName.ProviderNameRow pnRow)
        {
            Guid id = Guid.Empty;

            if (pnRow.IsPreferredNamesNull()) return id;
            if (pnRow.PreferredNames.Split(']').Length > 1)  //more than 1 ???
            {
                return id;
            }

            //parse id
            //format is [pref id:pref consensus id:pref full name],[ ...]
            int pos = 1;
            int endPos = pnRow.PreferredNames.IndexOf(":", pos);

            string idStr = pnRow.PreferredNames.Substring(pos, endPos - pos);
            if (idStr.Length > 0) Guid.TryParse(idStr, out id);

            return id;
        }

        public static Guid GetPreferredConsensusNameID(DsIntegrationName.ProviderNameRow pnRow)
        {
            Guid id = Guid.Empty;

            if (pnRow.IsPreferredNamesNull()) return id;
            if (pnRow.PreferredNames.Split(']').Length > 1) //more than 1 ??
            {
                return id;
            }

            //parse id
            //format is [pref id:pref consensus id:pref full name],[ ...]
            int pos = pnRow.PreferredNames.IndexOf(":") + 1;
            int endPos = pnRow.PreferredNames.IndexOf(":", pos);

            string idStr = pnRow.PreferredNames.Substring(pos, endPos - pos);
            if (idStr.Length > 0) Guid.TryParse(idStr, out id);

            return id;
        }

        public static String GetPreferredConsensusName(DsIntegrationName.ProviderNameRow pnRow)
        {
            String pn = "";

            if (pnRow.IsPreferredNamesNull()) return pn;
            if (pnRow.PreferredNames.Split(']').Length > 1) //more than 1 ??
            {
                return pn;
            }

            //parse id
            //format is [pref id:pref consensus id:pref full name],[ ...]
            int pos = pnRow.PreferredNames.IndexOf(":") + 1;
            pos = pnRow.PreferredNames.IndexOf(":", pos) + 1;
            int endPos = pnRow.PreferredNames.IndexOf("]", pos) - 1;

            pn = pnRow.PreferredNames.Substring(pos, endPos - pos);

            return pn;
        }

        public static DsIntegrationName.ProviderNameRow GetNameMatchData(String cnnStr, Guid provNameId, List<NZOR.Admin.Data.Entities.AttachmentPoint> attachmentPoints)
        {

            DsIntegrationName ds = new DsIntegrationName();

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

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
                    //	                        inner join NamePropertyType ncp on ncp.NamePropertyTypeID = np.NamePropertyTypeID
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
                        set concat_null_yields_null off

                        declare @concepts table(nameId uniqueidentifier, parentNames nvarchar(max), prefNames nvarchar(max))

                        insert @concepts
                        select pn.NameId, C.par, C.pref
                        from provider.Name pn
                        CROSS APPLY 
                        ( 
                            select (SELECT distinct '[' + CONVERT(VARCHAR(38), pc.NameToID) + ':' + convert(varchar(38), pc.ConsensusNameToID) + ':' + replace(REPLACE(pc.NameToFull, '[', ''), ']', '') + '],' AS [text()] 
                            FROM provider.vwConcepts pc 
                            where pc.NameID = pn.NameID 
		                        and pc.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' --and pc.InUse = 1
                            FOR XML PATH('')),
                            (SELECT distinct '[' + CONVERT(VARCHAR(38), pc.NameToID) + ':' + convert(varchar(38), pc.ConsensusNameToID) + ':' + replace(REPLACE(pc.NameToFull, '[', ''), ']', '') + '],' AS [text()] 
                            FROM provider.vwConcepts pc 
                            where pc.NameID = pn.NameID 
		                        and pc.ConceptRelationshipTypeID = '0CA79AB3-E213-4F51-88B9-4CE01F735A1D' --and pc.InUse = 1
                            FOR XML PATH(''))
                        ) C (par, pref)
                        where pn.NameID = '" + provNameId.ToString() + @"'

                        select distinct pn.NameID,
                            pn.ConsensusNameID,
                            pn.LinkStatus,
                            pn.MatchScore,
                            pn.MatchPath,
                            pn.FullName,
                            pn.NameClassID,
                            pn.IsRecombination,
                            nc.Name as NameClass,
	                        nc.HasClassification,
                            tr.TaxonRankID,
                            tr.Name as TaxonRank,
                            tr.SortOrder as TaxonRankSort,
                            tr.MatchRuleSetID,
                            ap.Value as Authors,
                            pn.GoverningCode,	
                            pn.DataSourceID,
                            cp.Value as Canonical, --canonical
                            yp.Value as YearOfPublication, --year on pub
                            bp.RelatedID as BasionymID, --basionym
                            bp.Value as Basionym, 
                            bap.Value as BasionymAuthors, --basionym authors
                            cap.Value as CombinationAuthors, --comb authors
                            mrp.Value as MicroReference, --micro ref
                            pip.Value as PublishedIn, --published in
                            pc.parentNames as ParentNames,
                            pc.prefNames as PreferredNames,
                            pn.ProviderRecordId
                        from provider.Name pn
                        inner join dbo.TaxonRank tr on tr.TaxonRankID = pn.TaxonRankID
                        inner join dbo.NameClass nc on nc.NameClassID = pn.NameClassID
                        left join provider.NameProperty cp on cp.NameID = pn.NameID and cp.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
                        left join provider.NameProperty yp on yp.NameID = pn.NameID and yp.NamePropertyTypeID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'
                        left join provider.NameProperty bp on bp.NameID = pn.NameID and bp.NamePropertyTypeID = 'F496FBCC-8DA6-4CA1-9884-11BD9B5DF63B'
                        left join provider.NameProperty ap on ap.NameID = pn.NameID and ap.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
                        left join provider.NameProperty bap on bap.NameID = pn.NameID and bap.NamePropertyTypeID = '6272B3D0-C91B-4FD4-A714-662B10FA6E68'
                        left join provider.NameProperty cap on cap.NameID = pn.NameID and cap.NamePropertyTypeID = '6196CDC4-BACB-4172-8186-14BA494621A7'
                        left join provider.NameProperty mrp on mrp.NameID = pn.NameID and mrp.NamePropertyTypeID = '4A344D40-7448-49D6-956B-4392B33A749F'
                        left join provider.NameProperty pip on pip.NameID = pn.NameID and pip.NamePropertyTypeID = 'DEDC63F0-FB2A-420B-9932-786B4347DA45'
                        inner join @concepts pc on pc.nameId = pn.NameID";

                    cmd.CommandTimeout = 100000;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.TableMappings.Add("Table", "ProviderName");

                    da.Fill(ds);
                }
            }

            if (ds.ProviderName.Count == 0) return null;

            //if no parent concept, get "fuzzy" match parents
            CalculateParentData(cnnStr, ds.ProviderName[0], attachmentPoints);

            return ds.ProviderName[0];
        }

        public static CalculateNameParentResult CalculateNameParent(String cnnStr, CalculateNameParentData calcData)
        {
            CalculateNameParentResult calcRes = new CalculateNameParentResult();

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                //NO Parent CONCEPT - check for higher ranks
                int trSort = calcData.ProviderNameRankSort;

                //TODO - CHECK THIS !  - do we need to allow for Provider/Dataset preferences - ie provider specifies the location in the taxon hierarchy where names should fit

                //check for parent concept
                if (calcData.ProviderNameParents.Count == 1)
                {
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = @"select nameid, fullname, consensusnameid, taxonrankid 
                            from provider.Name n 
                            where n.NameID = '" + calcData.ProviderNameParents[0].ToString() + "' and n.consensusnameid is not null";

                        DataSet pds = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(pds);

                        if (pds.Tables.Count > 0)
                        {
                            if (pds.Tables[0].Rows.Count == 1)
                            {
                                calcRes.ParentID = (Guid)pds.Tables[0].Rows[0]["consensusnameid"];
                                calcRes.ParentFullName = pds.Tables[0].Rows[0]["FullName"].ToString();
                                calcRes.ParentRankID = (Guid)pds.Tables[0].Rows[0]["TaxonRankID"];
                            }
                            else if (pds.Tables[0].Rows.Count > 1)
                            {
                                calcRes.ErrorMessage = "Multiple parent matches";
                            }
                        }
                    }
                }

                if (calcRes.ParentID == Guid.Empty && calcData.DataSourceID != Guid.Empty)
                {
                    //check attachment points
                    List<NZOR.Admin.Data.Entities.AttachmentPoint> attPts = null;
                    lock (calcData.AttachmentPoints)
                    {
                        attPts = calcData.AttachmentPoints.FindAll(o => o.ProviderRecordId == calcData.ProviderRecordID && o.DataSourceId == calcData.DataSourceID);
                    }
                    if (attPts.Count == 1)
                    {
                        String consId = attPts[0].ConsensusNameId.ToString();

                        using (SqlCommand cmd = cnn.CreateCommand())
                        {
                            cmd.CommandText = @"select n.nameid, n.fullname, n.taxonrankid 
                            from consensus.Name n 
                            inner join consensus.vwConcepts c on c.nametoid = n.nameid and c.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155'
                            where c.NameID = '" + consId + "'";

                            DataSet pds = new DataSet();
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(pds);

                            if (pds.Tables.Count > 0)
                            {
                                if (pds.Tables[0].Rows.Count == 1)
                                {
                                    calcRes.ParentID = (Guid)pds.Tables[0].Rows[0]["NameId"];
                                    calcRes.ParentFullName = pds.Tables[0].Rows[0]["FullName"].ToString();
                                    calcRes.ParentRankID = (Guid)pds.Tables[0].Rows[0]["TaxonRankID"];
                                }
                                else if (pds.Tables[0].Rows.Count > 1)
                                {
                                    calcRes.ErrorMessage = "Multiple parent matches";
                                }
                            }
                        }
                    }
                }

                if (calcRes.ParentID == Guid.Empty)
                {
                    //for Kingdoms, insert under root ??
                    if (trSort == 400)
                    {
                        using (SqlCommand cmd = cnn.CreateCommand())
                        {
                            cmd.CommandText = "select distinct NameID, FullName, TaxonRankID from consensus.Name where FullName = '<FullName><Name>ROOT</Name></FullName>'";

                            DataSet pds = new DataSet();
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(pds);

                            if (pds.Tables.Count > 0)
                            {
                                if (pds.Tables[0].Rows.Count == 1)
                                {
                                    calcRes.ParentID = (Guid)pds.Tables[0].Rows[0]["NameId"];
                                    calcRes.ParentFullName = pds.Tables[0].Rows[0]["FullName"].ToString();
                                    calcRes.ParentRankID = (Guid)pds.Tables[0].Rows[0]["TaxonRankID"];
                                }
                                else if (pds.Tables[0].Rows.Count > 1)
                                {
                                    calcRes.ErrorMessage = "Multiple parent matches";
                                }
                            }
                        }
                    }

                    //GENUS and above - just match canonical and rank 
                    if (calcRes.ParentID == Guid.Empty && trSort <= LookUps.Common.TaxonRankLookUp.SortOrderGenus)
                    {
                        using (SqlCommand cmd = cnn.CreateCommand())
                        {
                            cmd.CommandText = @"select distinct nto.ConsensusNameID, nto.FullName, nto.TaxonRankID 
	                                from provider.Name n 
	                                inner join provider.nameproperty np on np.nameid = n.nameid  
	                                inner join dbo.namepropertytype ncp on ncp.NamePropertyTypeID = np.NamePropertyTypeID  
	                                inner join provider.StackedName sn on sn.SeedNameID = n.NameID and sn.TaxonRankID = '" + calcData.ProviderNameRankID.ToString() + @"'
	                                inner join provider.StackedName sn2 on sn2.SeedNameID = sn.SeedNameID and sn2.Depth = sn.Depth + 1
	                                inner join provider.Name nto on nto.NameID = sn2.NameID
	                                where n.TaxonRankID = '" + calcData.ProviderNameRankID.ToString() + "' and np.Value = '" + calcData.ProviderNameCanonical.Replace("'", "''") +
                                        "' and ncp.name = 'Canonical' and n.GoverningCode = '" + calcData.GoverningCode + "'";

                            DataSet pds = new DataSet();
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            da.Fill(pds);

                            if (pds.Tables.Count > 0)
                            {
                                if (pds.Tables[0].Rows.Count == 1)
                                {
                                    calcRes.ParentID = (Guid)pds.Tables[0].Rows[0]["ConsensusNameID"];
                                    calcRes.ParentFullName = pds.Tables[0].Rows[0]["FullName"].ToString();
                                    calcRes.ParentRankID = (Guid)pds.Tables[0].Rows[0]["TaxonRankID"];
                                }
                                else if (pds.Tables[0].Rows.Count > 1)
                                {
                                    calcRes.ErrorMessage = "Multiple parent matches";
                                }
                            }
                        }
                    }

                    //below species - use the species
                    if (trSort > LookUps.Common.TaxonRankLookUp.SortOrderSpecies)
                    {
                        int spIndex = calcData.ProviderNameFullName.IndexOf(" ");
                        if (spIndex != -1 && calcData.ProviderNameFullName.IndexOf(" ", spIndex + 1) != -1)
                        {
                            calcRes.ParentFullName = calcData.ProviderNameFullName.Substring(0, calcData.ProviderNameFullName.IndexOf(" ", spIndex + 1));

                            using (SqlCommand cmd = cnn.CreateCommand())
                            {
                                cmd.CommandText = "select n.NameID from consensus.Name n where TaxonRankID = '"
                                    + TaxonRankLookup(cnnStr).GetTaxonRank("Species", calcData.GoverningCode).TaxonRankId.ToString() + "' and n.FullName like '" + calcRes.ParentFullName.Replace("'", "''") + " %' "
                                    + " or n.FullName = '" + calcRes.ParentFullName.Replace("'", "''") + "'";
                                if (calcData.GoverningCode != null) cmd.CommandText += " and n.GoverningCode = '" + calcData.GoverningCode + "'";

                                DataSet pds = new DataSet();
                                SqlDataAdapter da = new SqlDataAdapter(cmd);
                                da.Fill(pds);

                                if (pds.Tables.Count > 0)
                                {
                                    if (pds.Tables[0].Rows.Count == 1)
                                    {
                                        calcRes.ParentID = (Guid)pds.Tables[0].Rows[0]["NameId"];
                                    }
                                    else if (pds.Tables[0].Rows.Count > 1)
                                    {
                                        calcRes.ErrorMessage = "Multiple parent matches";
                                    }
                                }
                            }
                        }
                    }
                    else if (trSort > LookUps.Common.TaxonRankLookUp.SortOrderGenus) //Below GENUS - use the Genus (first word of the full name)
                    {
                        if (calcData.ProviderNameFullName.IndexOf(" ") != -1)
                        {
                            calcRes.ParentFullName = calcData.ProviderNameFullName.Substring(0, calcData.ProviderNameFullName.IndexOf(" "));

                            using (SqlCommand cmd = cnn.CreateCommand())
                            {
                                cmd.CommandText = "select n.NameID from consensus.Name n inner join consensus.nameproperty np on np.nameid = n.nameid "
                                    + " inner join dbo.NamePropertyType ncp on ncp.NamePropertyTypeID = np.NamePropertyTypeID where TaxonRankID = '"
                                    + TaxonRankLookup(cnnStr).GetTaxonRank("Genus", calcData.GoverningCode).TaxonRankId.ToString() + "' and np.Value = '" + calcRes.ParentFullName.Replace("'", "''") + "' and ncp.name = '"
                                    + NamePropertyTypeLookUp.Canonical + "'";
                                if (calcData.GoverningCode != null) cmd.CommandText += " and n.GoverningCode = '" + calcData.GoverningCode + "'";

                                DataSet pds = new DataSet();
                                SqlDataAdapter da = new SqlDataAdapter(cmd);
                                da.Fill(pds);

                                if (pds.Tables.Count > 0)
                                {
                                    if (pds.Tables[0].Rows.Count == 1)
                                    {
                                        calcRes.ParentID = (Guid)pds.Tables[0].Rows[0]["NameID"];
                                    }
                                    else if (pds.Tables[0].Rows.Count > 1)
                                    {
                                        calcRes.ErrorMessage = "Multiple parent matches";
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return calcRes;
        }

        public static void CalculateParentData(String cnnStr, DsIntegrationName.ProviderNameRow pn, List<NZOR.Admin.Data.Entities.AttachmentPoint> attachmentPoints)
        {
            //Check we have a parent concept.  If not get fuzzy matches for use in matching process
            //This routine makes sure that all names that have been selected for possible match are at the correct rank.
            //  Names that may be selected could be children of the same parent as the matching name, but this does not mean the names will be the 
            //  same rank - eg matching name "Aus bus var. dus" - may select a name in the DB "Aus bus subsp. cus" as "cus" is a child of the naem "Aus bus"            
            //Check for attachment points - ie if this name should connect directly to the name defined by an attachment point

            List<Guid> parentIds = Integration.GetParentNameIDs(pn);
            List<Guid> parentConsNameIDs = Integration.GetParentConsensusNameIDs(pn);
            List<String> parents = Integration.GetParentConsensusNames(pn);


            if (pn.NameClass.ToLower() == NZOR.Data.LookUps.Common.NameClassLookUp.ScientificName.ToLower())
            {
                if (parentIds.Count == 0 || parentConsNameIDs.Count == 0)
                {
                    CalculateNameParentData calcData = new CalculateNameParentData();
                    calcData.ProviderNameRankSort = pn.TaxonRankSort;
                    calcData.ProviderNameID = pn.NameID;
                    calcData.AttachmentPoints = attachmentPoints;
                    if (!pn.IsDataSourceIDNull()) calcData.DataSourceID = pn.DataSourceID;
                    calcData.ProviderNameParents = parentIds;
                    calcData.ProviderRecordID = pn.ProviderRecordID;
                    if (!pn.IsGoverningCodeNull()) calcData.GoverningCode = pn.GoverningCode;
                    calcData.ProviderNameCanonical = pn.Canonical;
                    calcData.ProviderNameFullName = pn.FullName;
                    calcData.ProviderNameRankID = pn.TaxonRankID;

                    CalculateNameParentResult calcRes = CalculateNameParent(cnnStr, calcData);

                    if (calcRes.ParentID != Guid.Empty)
                    {
                        //add parent concept
                        pn.ParentNames = "[:" + calcRes.ParentID.ToString() + ":" + calcRes.ParentFullName + "]";
                        //not a real provider name so no ParentId, just consensus id
                    }
                }
            }
        }

        public static void GetParentData(DsIntegrationName.ProviderNameRow pn, DataForIntegration allData)
        {
            //Same as abovwe, but no DB connection
            //Check we have a parent concept.  If not get fuzzy matches for use in matching process
            //This routine makes sure that all names that have been selected for possible match are at the correct rank.
            //  Names that may be selected could be children of the same parent as the matching name, but this does not mean the names will be the 
            //  same rank - eg matching name "Aus bus var. dus" - may select a name in the DB "Aus bus subsp. cus" as "cus" is a child of the naem "Aus bus"            
            //Check for attachment points - ie if this name should connect directly to the name defined by an attachment point

            try
            {
                List<Guid> parentIDs = Integration.GetParentNameIDs(pn);
                List<Guid> parentConsNameIDs = Integration.GetParentConsensusNameIDs(pn);
                List<String> parents = Integration.GetParentConsensusNames(pn);

                if (parentIDs.Count == 0 || parentConsNameIDs.Count == 0)
                {
                    String parFullName = "";
                    Guid parentConsNameID = Guid.Empty;

                    //check for parent concept
                    if (parentIDs.Count == 1)
                    {
                        DsIntegrationName.ProviderNameRow pr = allData.GetProviderName(parentIDs[0]);
                        if (pr != null)
                        {
                            if (!pr.IsConsensusNameIDNull())
                            {
                                parentConsNameID = pr.ConsensusNameID;
                                parFullName = pr.FullName;
                            }
                        }
                    }

                    if (parentConsNameID == Guid.Empty && !pn.IsDataSourceIDNull())
                    {
                        //check attachment points
                        DataRow[] attPts = allData.AttachmentPoints.AttachmentPoint.Select("ProviderRecordId = '" + pn.ProviderRecordID + "' and DataSourceId = '" + pn.DataSourceID.ToString() + "'");
                        if (attPts.Length == 1)
                        {
                            String consId = attPts[0]["ConsensusNameID"].ToString();
                            DataRow[] consPar = allData.ConsensusData.ConsensusName.Select("NameID = '" + consId + "'");
                            if (consPar.Length == 1)
                            {
                                parentConsNameID = (Guid)consPar[0]["ParentID"];
                                parFullName = consPar[0]["Parent"].ToString();
                            }
                        }
                    }

                    if (parentConsNameID == Guid.Empty && !pn.IsGoverningCodeNull())
                    {
                        //NO Parent CONCEPT 
                        //check higher ranks
                        int trSort = pn.TaxonRankSort;

                        //for Kingdoms, insert under root ??
                        if (trSort == 400)
                        {
                            DataRow[] root = allData.ConsensusData.ConsensusName.Select("FullName='<FullName><Name>ROOT</Name></FullName>'");
                            parentConsNameID = (Guid)root[0]["NameID"];
                            parFullName = root[0]["FullName"].ToString();
                        }

                        //ORDER and above - just match canonical and rank 
                        if (trSort <= LookUps.Common.TaxonRankLookUp.SortOrderOrder)
                        {
                            DataRow[] parentRows = allData.ConsensusData.ConsensusName.Select("TaxonRankID = '" + pn.TaxonRankID.ToString() + "' and Canonical = '" + pn.Canonical.Replace("'", "''") + "' and GoverningCode = '" + pn.GoverningCode + "'");
                            if (parentRows.Length > 0 && !parentRows[0].IsNull("ParentID"))
                            {
                                parentConsNameID = (Guid)parentRows[0]["ParentID"];
                                parFullName = parentRows[0]["Parent"].ToString();
                            }
                        }

                        //below species - use the species
                        if (trSort > LookUps.Common.TaxonRankLookUp.SortOrderSpecies)
                        {
                            int spIndex = pn.FullName.IndexOf(" ");
                            if (spIndex != -1 && pn.FullName.IndexOf(" ", spIndex + 1) != -1)
                            {
                                parFullName = pn.FullName.Substring(0, pn.FullName.IndexOf(" ", spIndex + 1));

                                String sel = "TaxonRank = 'species' and FullName like '" + parFullName.Replace("'", "''") + " %' or FullName = '" + parFullName.Replace("'", "''") + "'";
                                if (!pn.IsGoverningCodeNull()) sel += " and GoverningCode = '" + pn.GoverningCode + "'";

                                DataRow[] parentRows = allData.ConsensusData.ConsensusName.Select(sel);

                                if (parentRows.Length > 0)
                                {
                                    parentConsNameID = (Guid)parentRows[0]["NameID"];
                                }
                            }
                        }
                        else if (trSort > LookUps.Common.TaxonRankLookUp.SortOrderGenus) //Below GENUS - use the Genus (first word of the full name)
                        {
                            if (pn.FullName.IndexOf(" ") != -1)
                            {
                                parFullName = pn.FullName.Substring(0, pn.FullName.IndexOf(" "));

                                String sel = "TaxonRank = 'genus' and Canonical = '" + parFullName.Replace("'", "''") + "'";
                                if (!pn.IsGoverningCodeNull()) sel += " and GoverningCode = '" + pn.GoverningCode + "'";

                                DataRow[] parentRows = allData.ConsensusData.ConsensusName.Select(sel);

                                if (parentRows.Length > 0)
                                {
                                    parentConsNameID = (Guid)parentRows[0]["NameID"];
                                }
                            }
                        }
                    }

                    if (parentConsNameID != Guid.Empty)
                    {
                        //add parent concept
                        pn.ParentNames = "[:" + parentConsNameID.ToString() + ":" + parFullName + "]";
                        //not a real provider name so no ParentId, just consensus id
                    }
                }
            }
            catch (Exception)
            {
                //failed to work out parent
            }
        }
        public static void UpdateProviderNameLink(string cnnStr, Guid providerNameID, LinkStatus status, Guid? nameId, int matchScore, string matchPath, Guid? integrationBatchId)
        {
            using (System.Data.SqlClient.SqlConnection cnn = new System.Data.SqlClient.SqlConnection(cnnStr))
            {
                cnn.Open();
                NZOR.Data.Sql.Integration.UpdateProviderNameLink(cnn, providerNameID, status, nameId, matchScore, matchPath, integrationBatchId);
            }
        }

        public static void UpdateProviderNameLink(SqlConnection cnn, Guid providerNameID, LinkStatus status, Guid? nameId, int matchScore, string matchPath, Guid? integrationBatchId)
        {
            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "update provider.Name set LinkStatus = '" + status.ToString() + "', MatchScore = " + matchScore.ToString() + ", MatchPath = '" + matchPath.Replace("'", "''") +
                    "', ConsensusNameID = " + (nameId.HasValue ? "'" + nameId.Value.ToString() + "', " : "null, ") +
                    "IntegrationBatchID = " + (integrationBatchId.HasValue ? "'" + integrationBatchId.ToString() + "', " : "null, ") + "ModifiedDate = getdate() " +
                    "where NameID = '" + providerNameID.ToString() + "'";

                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region "Consensus Reference"
        public static DataSet AddConsensusReference(String cnnStr, DsIntegrationReference.ProviderReferenceRow provRef)
        {
            Guid refId = Guid.NewGuid();

            string sql = "insert consensus.Reference(ReferenceID, ReferenceTypeID, AddedDate) select '" +
                refId.ToString() + "', '" +
                provRef.ReferenceTypeID.ToString() + "', '" +
                DateTime.Now.ToString("s") + "'";

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }

                DataSet provDs = GetProviderReference(cnnStr, provRef.ReferenceID);

                //properties
                foreach (DataRow tpRow in provDs.Tables["ReferenceProperty"].Rows)
                {
                    string val = tpRow["Value"].ToString().Replace("'", "''");

                    if (val != string.Empty)
                    {
                        sql = "insert consensus.ReferenceProperty(ReferencePropertyID, ReferenceID, ReferencePropertyTypeID, Value) select '" +
                            Guid.NewGuid().ToString() + "', '" +
                            refId.ToString() + "', '" +
                            tpRow["ReferencePropertyTypeID"].ToString() + "', '" +
                            val + "'";

                        using (SqlCommand npCmd = cnn.CreateCommand())
                        {
                            npCmd.CommandText = sql;
                            npCmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            return GetConsensusReference(cnnStr, refId, true);
        }

        public static DataSet GetConsensusReference(String cnnStr, Guid refId, bool consensusOnly)
        {
            DataSet ds = null;

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "select * from consensus.Reference where ReferenceID = '" + refId.ToString() + "'; select * from consensus.ReferenceProperty where ReferenceID = '" + refId.ToString() + "';";

                    if (!consensusOnly) cmd.CommandText += " select * from provider.Reference where ConsensusReferenceID = '" + refId.ToString() + "'; select rp.* from provider.ReferenceProperty rp inner join provider.Reference pr on rp.ReferenceID = pr.ReferenceID where pr.ConsensusReferenceID = '" + refId.ToString() + "'";

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count == 0) ds = null;
                }
            }
            return ds;
        }

        public static void RefreshConsensusReference(Guid consensusReferenceId, string cnnStr)
        {
            NZOR.Data.Sql.Repositories.Consensus.ReferenceRepository rr = new Repositories.Consensus.ReferenceRepository(cnnStr);
            NZOR.Data.Sql.Repositories.Provider.ReferenceRepository prr = new Repositories.Provider.ReferenceRepository(cnnStr);

            NZOR.Data.Entities.Consensus.Reference cr = rr.GetReference(consensusReferenceId);
            List<NZOR.Data.Entities.Provider.Reference> prList = prr.GetReferencesForConcensusReference(consensusReferenceId);

            NZOR.Data.Sql.Integration.RefreshConsensusReferenceValues(cr, prList, cnnStr);

            cr.State = Entities.Entity.EntityState.Modified;
            cr.ModifiedDate = DateTime.Now;

            rr.References.Add(cr);
            rr.Save();

            cr.State = Entities.Entity.EntityState.Unchanged;
        }

        private static void RefreshConsensusReferenceValues(Entities.Consensus.Reference consRef, List<Entities.Provider.Reference> provRefs, string cnnStr)
        {
            foreach (Entities.Common.ReferencePropertyType rpt in ReferencePropertyTypes(cnnStr))
            {
                object val = GetConsensusRefValue(provRefs, rpt.Name);

                if (val != DBNull.Value && val.ToString() != string.Empty) consRef.SetReferenceProperty(rpt, val.ToString());
            }
        }

        #endregion

        #region "Provider Reference"
        public static DataSet GetProviderReference(String cnnStr, Guid provRefId)
        {
            DataSet ds = new DataSet();

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = @"
	                        select * 
	                        from provider.Reference r
	                        where ReferenceID = '" + provRefId.ToString() + @"'
                        	
	                        select * 
	                        from provider.ReferenceProperty rp
	                        inner join ReferencePropertyType rpt on rpt.ReferencePropertyTypeID = rp.ReferencePropertyTypeID
	                        where ReferenceID = '" + provRefId.ToString() + "'";

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);

                    ds.Tables[0].TableName = "Reference";
                    ds.Tables[1].TableName = "ReferenceProperty";
                }
            }

            return ds;
        }

        public static void UpdateProviderReferenceLink(String cnnStr, DsIntegrationReference.ProviderReferenceRow provRef, LinkStatus status, Guid? refId, int matchScore, String matchPath, Guid integrationBatchId)
        {
            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "update provider.Reference set LinkStatus = '" + status.ToString() + "', MatchScore = " + matchScore.ToString() + ", MatchPath = '" + matchPath.Replace("'", "''") + "'" +
                        ", ConsensusReferenceID = " + (refId.HasValue ? "'" + refId.Value.ToString() + "', " : "null, ") +
                        "IntegrationBatchID = '" + integrationBatchId.ToString() + "', ModifiedDate = getdate()" +
                        " where ReferenceID = '" + provRef.ReferenceID.ToString() + "'";

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static object GetConsensusRefValue(List<Entities.Provider.Reference> provRecords, string field)
        {
            Dictionary<object, int> vals = new Dictionary<object, int>();

            object editorVal = DBNull.Value;
            foreach (Entities.Provider.Reference pr in provRecords)
            {
                Entities.Provider.ReferenceProperty prp = pr.GetReferenceProperty(field);
                if (prp != null)
                {
                    object objVal = prp.Value;
                    if (objVal != null && objVal.ToString().Length > 0)
                    {
                        if (vals.ContainsKey(objVal))
                        {
                            vals[objVal] += 1;
                        }
                        else
                        {
                            if (!object.ReferenceEquals(objVal, DBNull.Value))
                                vals.Add(objVal, 1);
                        }
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
            //always return something for citation
            if (field == ReferencePropertyTypeLookUp.Citation)
                return val;

            if (hasEqual)
                return DBNull.Value;

            return val;
        }

        #endregion

        #region "Provider Concept"

        public static void UpdateProviderConceptLink(string cnnStr, Guid providerConceptId, LinkStatus status, Guid? conceptId, int matchScore, string matchPath, Guid? integrationBatchId)
        {
            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();
                UpdateProviderConceptLink(cnn, providerConceptId, status, conceptId, matchScore, matchPath, integrationBatchId);
            }
        }

        public static void UpdateProviderConceptLink(SqlConnection cnn, Guid provConcId, LinkStatus status, Guid? conceptId, int matchScore, string matchPath, Guid? integrationBatchId)
        {
            using (SqlCommand cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "update provider.Concept set LinkStatus = '" + status.ToString() + "', MatchScore = " + matchScore.ToString() + ", MatchPath = " +
                    (matchPath == null ? "null, " : "'" + matchPath.Replace("'", "''") + "', ") +
                    "ConsensusConceptID = " + (conceptId.HasValue ? "'" + conceptId.Value.ToString() + "', " : "null, ") +
                    "IntegrationBatchID = " + (integrationBatchId.HasValue ? "'" + integrationBatchId.Value.ToString() + "', " : "null, ") + "ModifiedDate = getdate() " +
                    "where ConceptID = '" + provConcId.ToString() + "'";

                cmd.ExecuteNonQuery();
            }
        }

        private static object GetConsensusConceptValue(List<Entities.Provider.Concept> provRecords, string field)
        {
            Dictionary<object, int> vals = new Dictionary<object, int>();

            object editorVal = DBNull.Value;
            foreach (Entities.Provider.Concept pc in provRecords)
            {
                object objVal = null;
                if (field == "Orthography") objVal = pc.Orthography;
                if (field == "TaxonRank") objVal = pc.TaxonRank;
                if (field == "HigherClassification") objVal = pc.HigherClassification;

                if (objVal != null && objVal.ToString().Length > 0)
                {
                    if (vals.ContainsKey(objVal))
                    {
                        vals[objVal] += 1;
                    }
                    else
                    {
                        if (!object.ReferenceEquals(objVal, DBNull.Value))
                            vals.Add(objVal, 1);
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

            if (hasEqual)
                return DBNull.Value;

            return val;
        }

        #endregion

        #region "Rollback"


        #endregion


    }
}

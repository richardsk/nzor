using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NZOR.Data.Entities.Consensus;
using NZOR.Data.Repositories.Consensus;

namespace NZOR.Data.Sql.Repositories.Consensus
{
    public class NameRepository : Repository<Name>, INameRepository
    {
        private List<Name> _names;

        public NameRepository(String connectionString)
            : base(connectionString)
        {
            _names = new List<Name>();
        }

        public List<Name> Names
        {
            get { return _names; }
        }

        public Name GetName(Guid nameId)
        {
            Name name = null;

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@nameId", nameId));

            using (var tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Name-GET.sql"), parameters))
            {
                if (tbl.Rows.Count > 0)
                {
                    var row = tbl.Rows[0];

                    name = new Name();
                    name.NameId = row.Field<Guid>("NameId");
                    name.AddedDate = row.Field<DateTime?>("AddedDate");
                    name.FullName = row.Field<String>("FullName");
                    name.GoverningCode = row.Field<String>("GoverningCode");
                    name.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    name.NameClassId = row.Field<Guid>("NameClassId");
                    name.TaxonRankId = row.Field<Guid>("TaxonRankId");
                    name.IsRecombination = row.Field<bool?>("Isrecombination");

                    name.NameProperties.AddRange(GetNameProperties(name.NameId));
                }
            }

            return name;
        }

        private List<NameProperty> GetNameProperties(Guid nameId)
        {
            List<NameProperty> props = new List<NameProperty>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@nameId", nameId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.NameProperties-LIST.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    NameProperty np = new NameProperty();

                    np.NamePropertyId = row.Field<Guid>("NamePropertyID");
                    np.NamePropertyTypeId = row.Field<Guid>("NamePropertyTypeID");
                    np.NamePropertyType = row.Field<String>("NamePropertyType");
                    np.RelatedId = row.Field<Guid?>("RelatedID");
                    np.Sequence = row.Field<int?>("Sequence");
                    np.Value = row.Field<String>("Value");

                    props.Add(np);
                }
            }

            return props;
        }

        public List<Name> GetRelatedNames(Guid nameId)
        {
            List<Name> names = new List<Name>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@nameId", nameId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Name-GETRelated.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Name n = new Name();
                    n.NameId = row.Field<Guid>("NameID");
                    n.AddedDate = row.Field<DateTime?>("AddedDate");
                    n.FullName = row.Field<String>("FullName");
                    n.GoverningCode = row.Field<String>("GoverningCode");
                    n.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                    n.NameClassId = row.Field<Guid>("NameClassId");
                    n.TaxonRankId = row.Field<Guid>("TaxonRankId");
                    n.IsRecombination = row.Field<bool?>("Isrecombination");

                    n.NameProperties.AddRange(GetNameProperties(n.NameId));

                    names.Add(n);
                }
            }

            return names;
        }

        public void Save(Name name)
        {
            string sql = String.Empty;

            if (name.State == Entities.Entity.EntityState.Added)
            {
                sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Name-INSERT.sql");
            }
            else if (name.State == Entities.Entity.EntityState.Modified)
            {
                sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Name-UPDATE.sql");
            }
            using (var cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@NameID", name.NameId);

                    cmd.Parameters.AddWithValue("@TaxonRankID", name.TaxonRankId);
                    cmd.Parameters.AddWithValue("@NameClassID", name.NameClassId);

                    cmd.Parameters.AddWithValue("@FullName", name.FullName);
                    cmd.Parameters.AddWithValue("@GoverningCode", (object)name.GoverningCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsRecombination", (object)name.IsRecombination ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@AddedDate", (object)name.AddedDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ModifiedDate", (object)name.ModifiedDate ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }

            if (name.State != Entities.Entity.EntityState.Added)
            {
                DeleteNameProperties(name);
            }

            InsertNameProperties(name);
        }

        public void Save()
        {
            foreach (Name name in _names.Where(o => o.State == Entities.Entity.EntityState.Added || o.State == Entities.Entity.EntityState.Modified))
            {
                Save(name);
            }
        }

        private void DeleteNameProperty(NameProperty nameProperty)
        {
            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.NameProperty-DELETE.sql");

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@NamePropertyID", nameProperty.NamePropertyId);

                    cmd.ExecuteNonQuery();
                }
            }

            nameProperty.State = Entities.Entity.EntityState.Deleted;
        }

        public void SetNamePropertyValue(Guid nameId, Guid namePropertyTypeId, object value, object sequence, object relatedId)
        {
            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.NameProperty-UPDATE.sql");

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@NameID", nameId);
                    cmd.Parameters.AddWithValue("@NamePropertyTypeID", namePropertyTypeId);
                    cmd.Parameters.AddWithValue("@Value", (object)value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Sequence", (object)sequence ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RelatedID", (object)relatedId ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveNameProperty(Guid nameId, NameProperty nameProperty)
        {
            if (nameProperty.State == Entities.Entity.EntityState.Modified) DeleteNameProperty(nameProperty);

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.NameProperty-INSERT.sql");

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@NamePropertyID", nameProperty.NamePropertyId);

                    cmd.Parameters.AddWithValue("@NameID", nameId);
                    cmd.Parameters.AddWithValue("@NamePropertyTypeID", nameProperty.NamePropertyTypeId);
                    cmd.Parameters.AddWithValue("@RelatedID", (object)nameProperty.RelatedId ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@Sequence", (object)nameProperty.Sequence ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Value", (object)nameProperty.Value ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void InsertNameProperties(Name name)
        {
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.NameProperty-INSERT.sql");

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();
                foreach (NameProperty nameProperty in name.NameProperties)
                {
                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = sql;

                        cmd.Parameters.AddWithValue("@NamePropertyID", nameProperty.NamePropertyId);

                        cmd.Parameters.AddWithValue("@NameID", name.NameId);
                        cmd.Parameters.AddWithValue("@NamePropertyTypeID", nameProperty.NamePropertyTypeId);
                        cmd.Parameters.AddWithValue("@RelatedID", (object)nameProperty.RelatedId ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@Sequence", (object)nameProperty.Sequence ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Value", (object)nameProperty.Value ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }

                    nameProperty.State = Entities.Entity.EntityState.Unchanged;
                }
            }
        }

        public void UpdateFullName(Name name)
        {
            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                string sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Name-UpdateFullName.sql");
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@nameClassID", name.NameClassId);
                    cmd.Parameters.AddWithValue("@nameID", name.NameId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void DeleteNameProperties(Name name)
        {
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.NameProperties-DELETE.sql");

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@NameID", name.NameId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<StackedName> GetStackedNamesForName(Guid nameId)
        {
            List<StackedName> names = new List<StackedName>();

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@nameId", nameId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.StackedName-GETByName.sql"), parameters))
            {
                ReferenceRepository rr = new ReferenceRepository(_connectionString);

                foreach (DataRow dr in tbl.Rows)
                {
                    StackedName sn = new StackedName();
                    sn.StatckedNameId = dr.Field<Guid>("StackedNameId");
                    sn.SeedNameId = dr.Field<Guid>("SeedNameId");
                    sn.NameId = dr.Field<Guid>("NameId");
                    sn.TaxonRankId = dr.Field<Guid?>("TaxonRankId");
                    sn.AccordingToId = dr.Field<Guid?>("AccordingToId");
                    sn.CanonicalName = dr.Field<String>("CanonicalName");
                    sn.RankName = dr.Field<String>("RankName");
                    sn.SortOrder = dr.Field<int?>("SortOrder");
                    sn.Depth = dr.Field<int?>("Depth");

                    if (sn.AccordingToId.HasValue)
                    {
                        Reference r = rr.GetReference(sn.AccordingToId.Value);
                        ReferenceProperty rp = r.GetProperty(LookUps.Common.ReferencePropertyTypeLookUp.Citation);
                        if (rp != null) sn.AccordingTo = rp.Value;
                    }

                    names.Add(sn);
                }
            }

            return names;
        }

        public List<Name> GetNameChildren(Guid nameId)
        {
            List<Name> names = new List<Name>();
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.NameChildren-LIST.sql");

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@nameId", nameId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            Name n = new Name();
                            n.NameId = row.Field<Guid>("NameID");
                            n.AddedDate = row.Field<DateTime?>("AddedDate");
                            n.FullName = row.Field<String>("FullName");
                            n.GoverningCode = row.Field<String>("GoverningCode");
                            n.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                            n.NameClassId = row.Field<Guid>("NameClassId");
                            n.TaxonRankId = row.Field<Guid>("TaxonRankId");
                            n.IsRecombination = row.Field<bool?>("Isrecombination");

                            n.NameProperties.AddRange(GetNameProperties(n.NameId));

                            names.Add(n);
                        }
                    }
                }
            }

            return names;
        }

        public List<Name> GetNameSynonyms(Guid nameId)
        {
            List<Name> names = new List<Name>();
            String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.NameSynonyms-LIST.sql");

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@nameId", nameId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            Name n = new Name();
                            n.NameId = row.Field<Guid>("NameID");
                            n.AddedDate = row.Field<DateTime?>("AddedDate");
                            n.FullName = row.Field<String>("FullName");
                            n.GoverningCode = row.Field<String>("GoverningCode");
                            n.ModifiedDate = row.Field<DateTime?>("ModifiedDate");
                            n.NameClassId = row.Field<Guid>("NameClassId");
                            n.TaxonRankId = row.Field<Guid>("TaxonRankId");
                            n.IsRecombination = row.Field<bool?>("Isrecombination");

                            n.NameProperties.AddRange(GetNameProperties(n.NameId));

                            names.Add(n);
                        }
                    }
                }
            }

            return names;
        }

        public List<Guid> GetAllNames()
        {
            List<Guid> names = new List<Guid>();

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                string sql = @"SELECT n.NameID FROM consensus.Name n inner join TaxonRank tr on tr.TaxonRankID = n.TaxonRankId order by SortOrder";
		
                //SELECT n.NameID FROM consensus.Name n inner join TaxonRank tr on tr.TaxonRankID = n.TaxonRankId order by SortOrder";

                SqlCommand cmd = new SqlCommand(sql, cnn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        names.Add((Guid)row["NameID"]);
                    }
                }
            }

            return names;
        }

        public void DeleteName(Guid nameId, Guid? replacementId)
        {
            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                String sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Name-DELETE.sql");

                using (SqlCommand cmd = new SqlCommand(sql, cnn))
                {
                    cmd.Parameters.AddWithValue("@nameId", nameId);
                    cmd.Parameters.AddWithValue("@replacementId", (object)replacementId ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }

        }

        public Entities.Consensus.NameProfile GetNameProfile(Guid nameId)
        {
            NameProfile n = null;

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                string sql = @"SELECT n.[NameID],
                              n.[TaxonRankID],
                              n.[NameClassID],
                              n.FullName,
                              pnp.Value as PartialName,
                              n.[GoverningCode],
                              n.[AddedDate],
                              n.[ModifiedDate],
                              anp.Value as Authors,
                              ynp.Value as Year,
	                          tr.Name as TaxonRank,
	                          nc.Name as NameClass
                        FROM consensus.Name n
                        inner join dbo.TaxonRank tr on tr.TaxonRankId = n.TaxonRankId
                        inner join dbo.NameClass nc on nc.NameClassId = n.NameClassId
                        left join consensus.NameProperty anp on anp.NameId = n.NameId and anp.NamePropertyTypeId = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
                        left join consensus.NameProperty ynp on ynp.NameId = n.NameId and ynp.NamePropertyTypeId = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7' 
                        left join consensus.NameProperty pnp on pnp.NameId = n.NameId and pnp.NamePropertyTypeId = '00806321-C8BD-4518-9539-1286DA02CA7D' 
                        WHERE n.NameId = '" + nameId.ToString() + "'";

                // TODO Comment: Are NamePropertyTypes differentiated by class so can we use constants here?

                SqlCommand cmd = new SqlCommand(sql, cnn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    n = new NameProfile();

                    n.NameId = row.Field<Guid>("NameID");
                    n.UpdatedDate = row.Field<DateTime>("AddedDate");
                    if (!row.IsNull("ModifiedDate")) n.UpdatedDate = (DateTime)row["ModifiedDate"];
                    n.FullName = row.Field<String>("FullName");
                    n.PartialName = row.Field<String>("PartialName");
                    n.NameClass = row.Field<String>("NameClass");
                    n.TaxonRank = row.Field<String>("TaxonRank");
                    n.GoverningCode = row.Field<String>("GoverningCode");
                    
                    //n.ProviderNamesXml = row["ProviderNames"].ToString();

                    n.NameProperties = GetNameProperties(n.NameId);

                    n.TaxonHierarchy = GetStackedNamesForName(nameId);

                    ConceptRepository cr = new ConceptRepository(_connectionString);
                    n.NameConcepts = cr.GetConceptsByName(nameId);

                    NZOR.Data.Sql.Repositories.Provider.ConceptRepository pcr = new Provider.ConceptRepository(_connectionString);
                    n.ProviderConcepts = pcr.GetProviderConceptsByName(n.NameId);

                    n.VernacularApplications = new List<NameApplication>();

                    ReferenceRepository rr = new ReferenceRepository(_connectionString);

                    foreach (Concept nc in n.NameConcepts)
                    {
                        if (nc.AccordingToReferenceId.HasValue)
                        {
                            ReferenceProperty citProp = rr.GetReference(nc.AccordingToReferenceId.Value).GetProperty(LookUps.Common.ReferencePropertyTypeLookUp.Citation);
                            if (citProp != null)
                            {
                                nc.AccordingToReference = citProp.Value;
                            }
                            else
                            {
                                ReferenceProperty titleProp = rr.GetReference(nc.AccordingToReferenceId.Value).GetProperty(LookUps.Common.ReferencePropertyTypeLookUp.Title);
                                if (titleProp != null) nc.AccordingToReference = titleProp.Value;
                            }
                        }

                        foreach (ConceptRelationship ncr in nc.ConceptRelationships)
                        {
                            if (ncr.ConceptRelationshipType == LookUps.Common.ConceptRelationshipTypeLookUp.IsChildOf && ncr.IsActive)
                            {
                                n.ParentName = new Name();
                                n.ParentName.NameId = ncr.NameToId.Value;
                                n.ParentName.FullName = ncr.NameTo;
                                n.ParentNameAccordingTo = nc.AccordingToReference;
                                n.ParentNameFull = n.ParentName.FullName;
                            }

                            if (ncr.ConceptRelationshipType == LookUps.Common.ConceptRelationshipTypeLookUp.IsSynonymOf && ncr.IsActive)
                            {
                                n.AcceptedName = new Name();
                                n.AcceptedName.NameId = ncr.NameToId.Value;
                                n.AcceptedName.FullName = ncr.NameTo;
                                n.AcceptedNameAccordingTo = nc.AccordingToReference;
                                n.AcceptedNameFull = n.AcceptedName.FullName;
                            }
                        }

                        foreach (NZOR.Data.Entities.Consensus.ConceptApplication ca in nc.ConceptApplications)
                        {
                            //assuming this is a vernacular
                            NZOR.Data.Entities.Consensus.Concept sc = cr.GetConcept(ca.ToConceptId);
                            if (sc.AccordingToReferenceId.HasValue) sc.AccordingToReference = rr.GetReference(sc.AccordingToReferenceId.Value).GetProperty(LookUps.Common.ReferencePropertyTypeLookUp.Citation).Value;

                            NZOR.Data.Entities.Consensus.Name sn = GetName(sc.NameId);

                            NZOR.Data.Entities.Consensus.NameApplication na = new NZOR.Data.Entities.Consensus.NameApplication();
                            na.Application = ca;
                            na.Name = sn;

                            n.VernacularApplications.Add(na);
                        }
                    }

                    TaxonPropertyRepository tpr = new TaxonPropertyRepository(_connectionString);
                    n.TaxonProperties = tpr.GetTaxonPropertiesByName(nameId);

                    n.Synonyms = GetNameSynonyms(nameId);

                    n.Children = GetNameChildren(nameId);

                    NZOR.Data.Sql.Repositories.Provider.NameRepository pnr = new Provider.NameRepository(_connectionString);
                    n.ProviderNames = pnr.GetNamesForConsensusName(nameId);
                }
            }


            return n;
        }

        public List<Entities.Consensus.NameProfile> SearchNames(List<NZOR.Data.Entities.Common.SearchField> searchFields, int startIndex, int recordCount, NZOR.Data.Entities.Common.SearchField.OrderByField orderByField, out int totalRecordCount)
        {
            totalRecordCount = 0;

            List<NameProfile> names = new List<NameProfile>();

            if (searchFields == null || searchFields.Count == 0) return names;

            string whereSql = "";

            bool first = true;
            foreach (NZOR.Data.Entities.Common.SearchField sf in searchFields)
            {
                if (!first)
                {
                    whereSql += " AND ";
                }

                switch (sf.SearchColumn)
                {
                    case Entities.Common.SearchField.SearchFieldColumn.FullName:
                        whereSql += " n.FullName ";
                        break;
                    case Entities.Common.SearchField.SearchFieldColumn.AcceptedName:
                        whereSql += " AcceptedName ";
                        break;
                    case Entities.Common.SearchField.SearchFieldColumn.ParentName:
                        whereSql += " ParentName ";
                        break;
                    case Entities.Common.SearchField.SearchFieldColumn.Authors:
                        whereSql += " Authors ";
                        break;
                    case Entities.Common.SearchField.SearchFieldColumn.Year:
                        whereSql += " Year ";
                        break;
                    case Entities.Common.SearchField.SearchFieldColumn.Biostatus:
                        whereSql += @" exists(select t.TaxonPropertyID 
                                                from consensus.Name n
                                                inner join consensus.TaxonProperty t on t.NameID = n.NameID and t.InUse = 1
                                                inner join dbo.GeoRegion gr on gr.GeoRegionID = t.GeoRegionID 
                                                inner join consensus.TaxonPropertyValue tv on tv.TaxonPropertyID = t.TaxonPropertyID and tv.TaxonPropertyTypeID = '9BB63B14-0208-4070-A575-94F90DFD47B0'
                                                where tv.Value = 'Present' or tv.Value = 'Sometimes present') ";
                        break;
                }

                if (sf.SearchColumn != Entities.Common.SearchField.SearchFieldColumn.Biostatus)
                {
                    whereSql += " LIKE '";
                    if (sf.AnywhereInText) whereSql += "%";
                    whereSql += sf.SearchText.Replace("'", "''") + "%'";
                }

                first = false;
            }

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                string sql = @"SELECT n.[NameID],
                              n.[TaxonRankID],
                              n.[NameClassID],
                              n.[FullName],
                              pnp.Value as PartialName,
                              n.[GoverningCode],
                              n.[AddedDate],
                              n.[ModifiedDate],
                              anp.Value as Authors,
                              ynp.Value as Year,
	                          tr.Name as TaxonRank,
	                          nc.Name as NameClass,
	                          an.NameToId as AcceptedNameId,
	                          an.NameToFull as AcceptedName,
	                          an.AccordingToReference as AcceptedNameAccordingTo,
	                          pn.NameToId as ParentNameId,
	                          pn.NameToFull as ParentName,
	                          pn.AccordingToReference as ParentNameAccordingTo                              
                        FROM consensus.Name n
                        inner join dbo.TaxonRank tr on tr.TaxonRankId = n.TaxonRankId
                        inner join dbo.NameClass nc on nc.NameClassId = n.NameClassId
                        left join consensus.vwConcepts an on an.NameId = n.NameId and an.IsActive = 1 and an.ConceptRelationshipTypeId = '0CA79AB3-E213-4F51-88B9-4CE01F735A1D'
                        left join consensus.vwConcepts pn on pn.NameId = n.NameId and pn.IsActive = 1 and pn.ConceptRelationshipTypeId = '6A11B466-1907-446F-9229-D604579AA155'
                        left join consensus.NameProperty anp on anp.NameId = n.NameId and anp.NamePropertyTypeId = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
                        left join consensus.NameProperty ynp on ynp.NameId = n.NameId and ynp.NamePropertyTypeId = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7' 
                        left join consensus.NameProperty pnp on pnp.NameId = n.NameId and pnp.NamePropertyTypeId = '00806321-C8BD-4518-9539-1286DA02CA7D' ";

                /*    cross apply
                          (SELECT pn.NameID, pn.FullName, ds.Code as DataSource, p.Name as Provider
                            FROM provider.Name pn
                            inner join [admin].DataSource ds on ds.DataSourceID = pn.DataSourceID
                            inner join [admin].Provider p on p.ProviderID = ds.ProviderID
                            WHERE pn.ConsensusNameID = n.NameID 
                            FOR XML PATH('ProviderName'), type) as ProviderNames(val)";*/


                sql += " WHERE " + whereSql;

                if (orderByField == Entities.Common.SearchField.OrderByField.FullName) sql += " order by n.FullName";
                if (orderByField == Entities.Common.SearchField.OrderByField.AcceptedName) sql += " order by AcceptedName";
                if (orderByField == Entities.Common.SearchField.OrderByField.Authors) sql += " order by Authors";
                if (orderByField == Entities.Common.SearchField.OrderByField.ParentName) sql += " order by ParentName";
                if (orderByField == Entities.Common.SearchField.OrderByField.TaxonRank) sql += " order by TaxonRank";
                if (orderByField == Entities.Common.SearchField.OrderByField.Year) sql += " order by Year";

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        totalRecordCount = ds.Tables[0].Rows.Count;

                        int count = 1;
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            if (count >= startIndex || startIndex == -1)
                            {
                                NameProfile n = new NameProfile();
                                n.NameId = row.Field<Guid>("NameID");
                                n.UpdatedDate = row.Field<DateTime>("AddedDate");
                                if (!row.IsNull("ModifiedDate")) n.UpdatedDate = (DateTime)row["ModifiedDate"];
                                n.FullName = row.Field<String>("FullName");
                                n.PartialName = row.Field<String>("PartialName");
                                n.NameClass = row.Field<String>("NameClass");
                                n.TaxonRank = row.Field<String>("TaxonRank");
                                n.GoverningCode = row.Field<String>("GoverningCode");

                                if (!row.IsNull("AcceptedNameId") && row["AcceptedNameId"].ToString() != row["NameId"].ToString())
                                {
                                    n.AcceptedName = new Name();
                                    n.AcceptedName.NameId = row.Field<Guid>("AcceptedNameId");
                                    n.AcceptedName.FullName = row.Field<String>("AcceptedName");
                                    n.AcceptedNameAccordingTo = row.Field<String>("AcceptedNameAccordingTo");
                                    n.AcceptedNameFull = n.AcceptedName.FullName;
                                }

                                if (!row.IsNull("ParentNameId"))
                                {
                                    n.ParentName = new Name();
                                    n.ParentName.NameId = row.Field<Guid>("ParentNameId");
                                    n.ParentName.FullName = row.Field<String>("ParentName");
                                    n.ParentNameAccordingTo = row.Field<String>("ParentNameAccordingTo");
                                    n.ParentNameFull = n.ParentName.FullName;
                                }

                                //n.ProviderNamesXml = row["ProviderNames"].ToString();

                                names.Add(n);
                            }

                            count++;
                            if (count > startIndex + recordCount && recordCount > 0) break;
                        }
                    }
                }
            }

            return names;
        }

    }
}

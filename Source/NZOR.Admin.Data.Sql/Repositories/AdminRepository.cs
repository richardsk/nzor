using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using NZOR.Admin.Data.Entities;
using NZOR.Admin.Data.Sql.Helpers;

namespace NZOR.Admin.Data.Sql.Repositories
{
    public class AdminRepository : NZOR.Admin.Data.Repositories.IAdminRepository
    {
        private List<User> _users = null;
        private List<NameRequest> _nameRequests = null;
        private List<BrokeredName> _brokeredNames = null;

        public List<User> Users
        {
            get { return _users; }
        }

        public List<NameRequest> NameRequests
        {
            get { return _nameRequests; }
        }

        public List<BrokeredName> BrokeredNames
        {
            get { return _brokeredNames; }
        }

        private string _connectionString = null;

        public AdminRepository(String connectionString)
        {
            _users = new List<User>();
            _nameRequests = new List<NameRequest>();
            _brokeredNames = new List<BrokeredName>();
            _connectionString = connectionString;
        }

        public NZORStatistics GetStatistics()
        {
            NZORStatistics stats = new NZORStatistics();
            
            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.NZORStatistics-GET.sql"), 50000))
            {
                if (tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];

                    stats.NZORNameCount = row.Field<int>("NZORNameCount");
                    stats.NZORConceptCount = row.Field<int>("NZORConceptCount");
                    stats.NZORReferenceCount = row.Field<int>("NZORReferenceCount");
                }
            }

            return stats;
        }

        public void UpdateStatistics()
        {
            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.NZORStatistics-UPDATE.sql");
                    cmd.CommandTimeout = 50000;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.User-LIST.sql")))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    User u = new User();
                    u.UserId = row.Field<Guid>("UserId");
                    u.Name = row.Field<string>("Name");
                    u.Email = row.Field<string>("Email");
                    if (!row.IsNull("Password")) u.Password = (byte[])row["Password"];
                    u.Organisation = row.Field<string>("Organisation");
                    u.APIKey = row.Field<string>("APIKey");
                    u.Status = row.Field<string>("Status");
                    u.AddedDate = row.Field<DateTime>("AddedDate");
                    u.ModifiedDate = row.Field<DateTime>("ModifiedDate");
                    users.Add(u);
                }
            }

            return users;
        }

        public User GetUser(Guid userId)
        {
            User u = null;
            
            List<SqlParameter> pars = new List<SqlParameter>();
            pars.Add(new SqlParameter("@userId", userId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.User-GET.sql"), pars))
            {
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];
                    u = new User();
                    u.UserId = row.Field<Guid>("UserId");
                    u.Name = row.Field<string>("Name");
                    u.Email = row.Field<string>("Email");
                    if (!row.IsNull("Password")) u.Password = (byte[])row["Password"];
                    u.Organisation = row.Field<string>("Organisation");
                    u.APIKey = row.Field<string>("APIKey");
                    u.Status = row.Field<string>("Status");
                    u.AddedDate = row.Field<DateTime>("AddedDate");
                    u.ModifiedDate = row.Field<DateTime>("ModifiedDate");
                }
            }

            return u;
        }

        public User GetUserByApiKey(string apiKey)
        {
            User u = null;

            List<SqlParameter> pars = new List<SqlParameter>();
            pars.Add(new SqlParameter("@apiKey", apiKey));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.User-GETByApiKey.sql"), pars))
            {
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];
                    u = new User();
                    u.UserId = row.Field<Guid>("UserId");
                    u.Name = row.Field<string>("Name");
                    u.Email = row.Field<string>("Email");
                    if (!row.IsNull("Password")) u.Password = (byte[])row["Password"];
                    u.Organisation = row.Field<string>("Organisation");
                    u.APIKey = row.Field<string>("APIKey");
                    u.Status = row.Field<string>("Status");
                    u.AddedDate = row.Field<DateTime>("AddedDate");
                    u.ModifiedDate = row.Field<DateTime>("ModifiedDate");
                }
            }

            return u;
        }

        public User GetUserByEmail(string email)
        {
            User u = null;

            List<SqlParameter> pars = new List<SqlParameter>();
            pars.Add(new SqlParameter("@email", email));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.User-GETByEmail.sql"), pars))
            {
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];
                    u = new User();
                    u.UserId = row.Field<Guid>("UserId");
                    u.Name = row.Field<string>("Name");
                    u.Email = row.Field<string>("Email");
                    if (!row.IsNull("Password")) u.Password = (byte[])row["Password"];
                    u.Organisation = row.Field<string>("Organisation");
                    u.APIKey = row.Field<string>("APIKey");
                    u.Status = row.Field<string>("Status");
                    u.AddedDate = row.Field<DateTime>("AddedDate");
                    u.ModifiedDate = row.Field<DateTime>("ModifiedDate");
                }
            }

            return u;
        }

        public void Save()
        {
            using (var cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                foreach (User u in _users)
                {
                    string sql = String.Empty;

                    if (u.State == Entities.Entity.EntityState.Added)
                    {
                        sql = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.User-INSERT.sql");
                    }
                    else if (u.State == Entities.Entity.EntityState.Modified)
                    {
                        sql = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.User-UPDATE.sql");
                    }
                    using (var cmd = cnn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = sql;

                        cmd.Parameters.AddWithValue("@UserID", u.UserId);
                        cmd.Parameters.AddWithValue("@Name", u.Name);
                        cmd.Parameters.AddWithValue("@Email", u.Email);
                        cmd.Parameters.AddWithValue("@Password", (object)u.Password ?? DBNull.Value); 
                        cmd.Parameters.AddWithValue("@Organisation", (object)u.Organisation ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@APIKey", (object)u.APIKey ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Status", (object)u.Status ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AddedDate", u.AddedDate);
                        cmd.Parameters.AddWithValue("@ModifiedDate", u.ModifiedDate);

                        cmd.ExecuteNonQuery();
                    }
                }

                foreach (NameRequest nr in _nameRequests)
                {
                    string sql = String.Empty;

                    if (nr.State == Entities.Entity.EntityState.Added)
                    {
                        sql = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.NameRequest-INSERT.sql");
                    }
                    else if (nr.State == Entities.Entity.EntityState.Modified)
                    {
                        sql = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.NameRequest-UPDATE.sql");
                    }
                    using (var cmd = cnn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = sql;

                        cmd.Parameters.AddWithValue("@NameRequestId", nr.NameRequestId);
                        cmd.Parameters.AddWithValue("@FullName", nr.FullName);
                        cmd.Parameters.AddWithValue("@RequestDate", nr.RequestDate);
                        cmd.Parameters.AddWithValue("@ApiKey", nr.ApiKey);
                        cmd.Parameters.AddWithValue("@BatchMatchId", (object)nr.BatchMatchId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@RequesterEmail", nr.RequesterEmail);
                        cmd.Parameters.AddWithValue("@ExternalLookupServiceId", (object)nr.ExternalLooksupServiceId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ExternalLookupDataUrl", (object)nr.ExternalLookupDataUrl ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Status", nr.Status);
                        cmd.Parameters.AddWithValue("@AddedDate", (object)nr.AddedDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AddedBy", (object)nr.AddedBy ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModifiedDate", (object)nr.ModifiedDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)nr.ModifiedBy ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }

                foreach (BrokeredName bn in _brokeredNames)
                {
                    string sql = String.Empty;

                    if (bn.State == Entities.Entity.EntityState.Added)
                    {
                        sql = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.BrokeredName-INSERT.sql");
                    }
                    else if (bn.State == Entities.Entity.EntityState.Modified)
                    {
                        sql = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.BrokeredName-UPDATE.sql");
                    }
                    using (var cmd = cnn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = sql;

                        cmd.Parameters.AddWithValue("@BrokeredNameId", bn.BrokeredNameId);
                        cmd.Parameters.AddWithValue("@NameRequestId", bn.NameRequestId);
                        cmd.Parameters.AddWithValue("@ExternalLookupServiceId", bn.ExternalLookupServiceId);
                        cmd.Parameters.AddWithValue("@ProviderRecordId", bn.ProviderRecordId);
                        cmd.Parameters.AddWithValue("@NZORProviderNameId", bn.NZORProviderNameId);
                        cmd.Parameters.AddWithValue("@DataUrl", (object)bn.DataUrl ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@WebUrl", (object)bn.WebUrl ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AddedDate", (object)bn.AddedDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@AddedBy", (object)bn.AddedBy ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModifiedDate", (object)bn.ModifiedDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModifiedBy", (object)bn.ModifiedBy ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }

            }
        }

        public Setting GetSetting(string name)
        {
            Setting s = null;

            List<SqlParameter> pars = new List<SqlParameter>();
            pars.Add(new SqlParameter("@name", name));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.Setting-GET.sql"), pars))
            {
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];
                    s = new Setting();
                    s.SettingId = row.Field<Guid>("SettingId");
                    s.Name = row["Name"].ToString();
                    s.Value = row["Value"].ToString();
                }
            }

            return s;
        }

        public void SetSetting(string name, string value)
        {
            using (var cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                string sql = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.Setting-UPDATE.sql");
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@value", value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<NameRequest> GetPendingNameRequests()
        {
            var requests = new List<NameRequest>();

            using (var tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.NameRequest-ListPending.sql")))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    requests.Add(LoadNameRequest(row));
                }
            }

            return requests;
        }


        private NameRequest LoadNameRequest(DataRow row)
        {
            var req = new NameRequest();

            req.NameRequestId = row.Field<Guid>("NameRequestId");

            req.FullName = row.Field<string>("FullName");
            req.RequestDate = row.Field<DateTime>("RequestDate");
            req.ApiKey = row.Field<string>("ApiKey");
            req.BatchMatchId = row.Field<Guid?>("BatchMatchId");
            req.RequesterEmail = row.Field<string>("RequesterEmail");
            req.ExternalLooksupServiceId = row.Field<Guid?>("ExternalLookupServiceId");
            req.ExternalLookupDataUrl = row.Field<string>("ExternalLookupDataUrl");
            req.Status = row.Field<string>("Status");
            req.AddedBy = row.Field<string>("AddedBy");
            req.AddedDate = row.Field<DateTime?>("AddedDate");
            req.ModifiedBy = row.Field<string>("ModifiedBy");
            req.ModifiedDate = row.Field<DateTime?>("ModifiedDate");

            return req;
        }

        public List<NameRequest> GetNameRequestsByApiKey(string apiKey)
        {
            var requests = new List<NameRequest>();

            List<SqlParameter> pars = new List<SqlParameter>();
            pars.Add(new SqlParameter("@ApiKey", apiKey));

            using (var tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.NameRequest-ListByApiKey.sql"), pars))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    requests.Add(LoadNameRequest(row));
                }
            }

            return requests;
        }

        public NameRequest GetNameRequestByFullName(string fullName)
        {
            NameRequest request = null;

            List<SqlParameter> pars = new List<SqlParameter>();
            pars.Add(new SqlParameter("@FullName", fullName));

            using (var tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.NameRequest-GETByFullName.sql"), pars))
            {
                if (tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];                
                    request = LoadNameRequest(row);
                }
            }

            return request;
        }

        public List<BrokeredName> GetBrokeredNamesForNameRequest(Guid nameRequestId)
        {
            List<BrokeredName> bnList = new List<BrokeredName>();

            List<SqlParameter> pars = new List<SqlParameter>();
            pars.Add(new SqlParameter("@NameRequestId", nameRequestId));

            using (var tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.BrokeredName-GETByNameRequest.sql"), pars))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    BrokeredName bn = new BrokeredName();

                    bn.BrokeredNameId = row.Field<Guid>("BrokeredNameId");
                    bn.DataUrl = row.Field<string>("DataUrl");
                    bn.ExternalLookupServiceId = row.Field<Guid>("ExternalLookupServiceId");
                    bn.NameRequestId = row.Field<Guid>("NameRequestId");
                    bn.NZORProviderNameId = row.Field<Guid?>("NZORProviderNameId");
                    bn.ProviderRecordId = row.Field<string>("ProviderRecordId");
                    bn.WebUrl = row.Field<string>("WebUrl");
                    bn.AddedBy = row.Field<string>("AddedBy");
                    bn.AddedDate = row.Field<DateTime?>("AddedDate");
                    bn.ModifiedBy = row.Field<string>("ModifiedBy");
                    bn.ModifiedDate = row.Field<DateTime?>("ModifiedDate");

                    bnList.Add(bn);
                }
            }

            return bnList;
        }

        /// <summary>
        /// List any brokered names that are new - ie have a provider name but not integrated yet.
        /// </summary>
        /// <returns></returns>
        public List<BrokeredName> GetNewBrokeredNames()
        {
            List<BrokeredName> bnList = new List<BrokeredName>();

            using (var tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.BrokeredName-ListNew.sql")))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    BrokeredName bn = new BrokeredName();

                    bn.BrokeredNameId = row.Field<Guid>("BrokeredNameId");
                    bn.DataUrl = row.Field<string>("DataUrl");
                    bn.ExternalLookupServiceId = row.Field<Guid>("ExternalLookupServiceId");
                    bn.NameRequestId = row.Field<Guid>("NameRequestId");
                    bn.NZORProviderNameId = row.Field<Guid?>("NZORProviderNameId");
                    bn.ProviderRecordId = row.Field<string>("ProviderRecordId");
                    bn.WebUrl = row.Field<string>("WebUrl");
                    bn.AddedBy = row.Field<string>("AddedBy");
                    bn.AddedDate = row.Field<DateTime?>("AddedDate");
                    bn.ModifiedBy = row.Field<string>("ModifiedBy");
                    bn.ModifiedDate = row.Field<DateTime?>("ModifiedDate");

                    bnList.Add(bn);
                }
            }

            return bnList;
        }

        public void DeleteBrokeredName(Guid brokeredNameId)
        {
            using (var cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                string sql = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.BrokeredName-DELETE.sql");
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@brokeredNameId", brokeredNameId);
                    
                    cmd.ExecuteNonQuery();
                }
            }

        }

    
    }
}

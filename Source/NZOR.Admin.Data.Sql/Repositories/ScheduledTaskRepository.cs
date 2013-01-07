using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

using NZOR.Admin.Data.Repositories;
using NZOR.Admin.Data.Entities;
using NZOR.Admin.Data.Sql.Helpers;

namespace NZOR.Admin.Data.Sql.Repositories
{
    public class ScheduledTaskRepository : Repository<ScheduledTask>, IScheduledTaskRepository
    {
        public ScheduledTaskRepository(String connectionString)
            : base(connectionString)
        {
        }

        private List<ScheduledTask> _tasks = null;

        public List<ScheduledTask> ScheduledTasks
        {
            get
            {
                if (_tasks == null) _tasks = new List<ScheduledTask>();
                return _tasks;
            }           
        }

        public List<ScheduledTask> ListScheduledTasks()
        {
            List<ScheduledTask> tasks = new List<ScheduledTask>();
            
            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.ScheduledTask-LIST.sql")))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    ScheduledTask sch = new ScheduledTask();

                    sch.ScheduledTaskId = row.Field<Guid>("ScheduledTaskId");
                    sch.RelatedId = row.Field<Guid>("RelatedId");
                    sch.Name = row.Field<String>("Name");
                    sch.FrequencyDays = row.Field<int>("FrequencyDays");
                    sch.PreferredStartTimeGMT = row.Field<String>("PreferredStartTimeGMT");
                    sch.LastRun = row.Field<DateTime?>("LastRun");
                    sch.LastRunOutcome = row.Field<String>("LastRunOutcome");
                    sch.Status = row.Field<String>("Status");

                    tasks.Add(sch);
                }
            }

            return tasks; 
        }

        public Entities.ScheduledTask GetScheduledTask(Guid relatedId)
        {
            ScheduledTask sch = null;

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter("@relatedId", relatedId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.ScheduledTask-GET.sql"), sqlParameters))
            {
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];

                    sch = new ScheduledTask();

                    sch.ScheduledTaskId = row.Field<Guid>("ScheduledTaskId");
                    sch.RelatedId = row.Field<Guid>("RelatedId");
                    sch.Name = row.Field<String>("Name");
                    sch.FrequencyDays = row.Field<int>("FrequencyDays");
                    sch.PreferredStartTimeGMT = row.Field<String>("PreferredStartTimeGMT");
                    sch.LastRun = row.Field<DateTime?>("LastRun");
                    sch.LastRunOutcome = row.Field<String>("LastRunOutcome");
                    sch.Status = row.Field<String>("Status");
                }
            }

            return sch;        
        }

        public Entities.ScheduledTask GetScheduledTask(string name)
        {
            ScheduledTask sch = null;

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter("@name", name));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.ScheduledTask-GETByName.sql"), sqlParameters))
            {
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];

                    sch = new ScheduledTask();

                    sch.ScheduledTaskId = row.Field<Guid>("ScheduledTaskId");
                    sch.RelatedId = row.Field<Guid>("RelatedId");
                    sch.Name = row.Field<String>("Name");
                    sch.FrequencyDays = row.Field<int>("FrequencyDays");
                    sch.PreferredStartTimeGMT = row.Field<String>("PreferredStartTimeGMT");
                    sch.LastRun = row.Field<DateTime?>("LastRun");
                    sch.LastRunOutcome = row.Field<String>("LastRunOutcome");
                    sch.Status = row.Field<String>("Status");
                }
            }

            return sch; 
        }

        public void Save()
        {
            String sql = String.Empty;

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                foreach (ScheduledTask st in ScheduledTasks)
                {
                    if (st.State == Entities.Entity.EntityState.Added)
                    {
                        sql = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.ScheduledTask-INSERT.sql");
                    }
                    else if (st.State == Entities.Entity.EntityState.Modified)
                    {
                        sql = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.ScheduledTask-UPDATE.sql");
                    }

                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = sql;

                        cmd.Parameters.AddWithValue("@scheduledTaskId", st.ScheduledTaskId);
                        cmd.Parameters.AddWithValue("@name", st.Name);
                        cmd.Parameters.AddWithValue("@relatedId", st.RelatedId);
                        cmd.Parameters.AddWithValue("@frequencyDays", st.FrequencyDays);
                        cmd.Parameters.AddWithValue("@preferredStartTimeGMT", (object)st.PreferredStartTimeGMT ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@lastRun", (object)st.LastRun ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@lastRunOutcome", (object)st.LastRunOutcome ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@status", (object)st.Status ?? DBNull.Value);
                        
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}

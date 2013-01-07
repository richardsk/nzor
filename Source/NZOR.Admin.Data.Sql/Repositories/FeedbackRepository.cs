using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Admin.Data.Entities;
using NZOR.Admin.Data.Repositories;
using System.Data.SqlClient;
using NZOR.Admin.Data.Sql.Helpers;
using System.Data;

namespace NZOR.Admin.Data.Sql.Repositories
{
    public class FeedbackRepository : Repository<Feedback>, IFeedbackRepository
    {
        public FeedbackRepository(String connectionString)
            : base(connectionString)
        {
        }

        private List<Feedback> _feedbacks = null;

        public List<Feedback> Feedbacks
        {
            get
            {
                if (_feedbacks == null) _feedbacks = new List<Feedback>();
                return _feedbacks;
            }           
        }

        public List<Feedback> GetFeedbackWithStatus(FeedbackStatus status)
        {
            List<Feedback> feedbacks = new List<Feedback>();

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(new SqlParameter("@status", status.ToString()));

                using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.Feedback-GETByStatus.sql"), sqlParameters))
                {
                    foreach (DataRow row in tbl.Rows)
                    {
                        Feedback fb = new Feedback();
                        fb.FeedbackId = row.Field<Guid>("FeedbackId");
                        fb.NameId = row.Field<Guid?>("NameId");
                        fb.Message = row.Field<string>("Message");
                        fb.Status = row.Field<string>("Status");
                        fb.Resolution = row.Field<string>("Resolution");
                        fb.Sender = row.Field<string>("Sender");
                        fb.SenderEmail = row.Field<string>("SenderEmail");
                        fb.SentTo = row.Field<string>("SentTo");
                        fb.AddedDate = row.Field<DateTime>("AddedDate");
                        fb.ModifiedDate = row.Field<DateTime?>("ModifiedDate");

                        feedbacks.Add(fb);
                    }
                }
            }
            
            return feedbacks;
        }

        public void Save()
        {
            String sql = String.Empty;

            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                foreach (Feedback fb in Feedbacks)
                {
                    if (fb.State == Entities.Entity.EntityState.Added)
                    {
                        sql = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.Feedback-INSERT.sql");
                    }
                    else if (fb.State == Entities.Entity.EntityState.Modified)
                    {
                        sql = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.Feedback-UPDATE.sql");
                    }

                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = sql;

                        cmd.Parameters.AddWithValue("@feedbackId", fb.FeedbackId);
                        cmd.Parameters.AddWithValue("@nameId", (object)fb.NameId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@message", fb.Message);
                        cmd.Parameters.AddWithValue("@status", fb.Status);
                        cmd.Parameters.AddWithValue("@resolution", (object)fb.Resolution ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@sender", fb.Sender);
                        cmd.Parameters.AddWithValue("@senderEmail", fb.SenderEmail);
                        cmd.Parameters.AddWithValue("@sentTo", fb.SentTo);
                        cmd.Parameters.AddWithValue("@addedDate", fb.AddedDate);
                        cmd.Parameters.AddWithValue("@modifiedDate", (object)fb.ModifiedDate ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}

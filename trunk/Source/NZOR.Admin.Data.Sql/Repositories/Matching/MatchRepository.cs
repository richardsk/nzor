using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using NZOR.Admin.Data.Entities.Matching;
using NZOR.Admin.Data.Repositories.Matching;
using NZOR.Admin.Data.Sql.Helpers;

namespace NZOR.Admin.Data.Sql.Repositories.Matching
{
    public class MatchRepository : Repository<Match>, IMatchRepository
    {
        public MatchRepository(string connectionString)
            : base(connectionString)
        {
        }

        public Match GetMatch(Guid id, bool includeData)
        {
            Match match = null;

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@matchId", id));
            parameters.Add(new SqlParameter("@includeData", includeData));

            using (var tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.Matching.Match-GET.sql"), parameters))
            {
                if (tbl.Rows.Count > 0)
                {
                    var row = tbl.Rows[0];

                    match = LoadMatch(row);
                }
            }

            return match;
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Save(Match match)
        {
            string sql = String.Empty;

            if (match.State == Entities.Entity.EntityState.Added)
            {
                sql = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.Matching.Match-INSERT.sql");
            }
            else if (match.State == Entities.Entity.EntityState.Modified)
            {
                sql = Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.Matching.Match-UPDATE.sql");
            }
            using (var cnn = new SqlConnection(_connectionString))
            {
                cnn.Open();

                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText = sql;

                    cmd.Parameters.AddWithValue("@MatchId", match.MatchId);

                    cmd.Parameters.AddWithValue("@SubmitterEmail", match.SubmitterEmail);
                    cmd.Parameters.AddWithValue("@ReceivedDate", match.ReceivedDate);
                    cmd.Parameters.AddWithValue("@Status", match.Status);
                    cmd.Parameters.AddWithValue("@InputData", match.InputData);
                    cmd.Parameters.AddWithValue("@ResultData", (object)match.ResultData ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ExternalLookupResults", (object)match.ExternalLookupResults ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsServiceMediated", (object)match.IsServiceMediated ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ApiKey", (object)match.ApiKey ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Error", (object)match.Error ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DoExternalLookup", (object)match.DoExternalLookup ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Match> GetPendingMatches()
        {
            var matches = new List<Match>();

            using (var tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.Matching.Match-ListPending.sql")))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    matches.Add(LoadMatch(row));
                }
            }

            return matches;
        }

        private Match LoadMatch(DataRow row)
        {
            var match = new Match();

            match.MatchId = row.Field<Guid>("MatchId");

            match.SubmitterEmail = row.Field<string>("SubmitterEmail");
            match.ReceivedDate = row.Field<DateTime>("ReceivedDate");
            match.Status = row.Field<string>("Status");
            if (row.Table.Columns.Contains("InputData")) match.InputData = row.Field<string>("InputData");
            if (row.Table.Columns.Contains("ResultData")) match.ResultData = row.Field<string>("ResultData");
            if (row.Table.Columns.Contains("ExternalLookupResults")) match.ExternalLookupResults = row.Field<string>("ExternalLookupResults");
            match.IsServiceMediated = row.Field<bool?>("IsServiceMediated");
            match.ApiKey = row.Field<string>("ApiKey");
            match.Error = row.Field<string>("Error");
            match.DoExternalLookup = row.Field<bool?>("DoExternalLookup");

            return match;
        }
    }
}

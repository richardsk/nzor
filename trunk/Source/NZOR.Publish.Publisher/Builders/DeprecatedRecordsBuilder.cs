using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Publish.Model.Common;
using NZOR.Publish.Publisher.Helpers;

namespace NZOR.Publish.Publisher.Builders
{
    class DeprecatedRecordsBuilder
    {
        private readonly string _connectionString;

        private List<DeprecatedRecord> _deprecatedRecords;

        public DeprecatedRecordsBuilder(string connectionString)
        {
            _connectionString = connectionString;

            _deprecatedRecords = new List<DeprecatedRecord>();
        }

        public void Build()
        {
            LoadDeprecatedRecords();
        }

        public List<DeprecatedRecord> DeprecatedRecords
        {
            get { return _deprecatedRecords; }
        }

        private void LoadDeprecatedRecords()
        {
            string sql = @"

SELECT
	OldId,
	NewId,
	[Table] AS Type,
	DeprecationDate AS DeprecatedDate
FROM
	dbo.Deprecated

";

            using (var drd = DataAccess.ExecuteReader(_connectionString, sql))
            {
                while (drd.Read())
                {
                    var deprecatedRecord = new DeprecatedRecord();

                    deprecatedRecord.OldId = drd.GetGuid("OldId");
                    deprecatedRecord.NewId = drd.GetGuid("NewId");

                    deprecatedRecord.Type = drd.GetString("Type");
                    deprecatedRecord.DeprecatedDate = drd.GetDateTime("DeprecatedDate");

                    _deprecatedRecords.Add(deprecatedRecord);
                }
            }
        }
    }
}

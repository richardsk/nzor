using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Publish.Model.Common;
using System.Data.SqlClient;
using NZOR.Publish.Publisher.Helpers;

namespace NZOR.Publish.Publisher.Builders
{
    class TaxonRanksBuilder
    {
        string _connectionString;

        List<TaxonRank> _taxonRanks;

        public TaxonRanksBuilder(string connectionString)
        {
            _connectionString = connectionString;

            _taxonRanks = new List<TaxonRank>();
        }

        public void Build()
        {
            LoadTaxonRanks();
        }

        public List<TaxonRank> TaxonRanks
        {
            get { return _taxonRanks; }
        }

        private void LoadTaxonRanks()
        {
            string sql = @"

SELECT
    TaxonRankId,

	DisplayName AS Name,
	Name AS Abbreviation,
	SortOrder
FROM
	dbo.TaxonRank
ORDER BY
	SortOrder

";

            using (SqlDataReader drd = DataAccess.ExecuteReader(_connectionString, sql))
            {
                while (drd.Read())
                {
                    TaxonRank taxonRank = new TaxonRank();

                    taxonRank.TaxonRankId = drd.GetGuid("TaxonRankId");

                    taxonRank.Name = drd.GetString("Name");
                    taxonRank.Abbreviation = drd.GetString("Abbreviation");
                    taxonRank.SortOrder = drd.GetInt32("SortOrder");

                    _taxonRanks.Add(taxonRank);
                }
            }
        }
    }
}

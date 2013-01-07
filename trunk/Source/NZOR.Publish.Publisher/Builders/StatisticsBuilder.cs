using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Publish.Model.Administration;
using NZOR.Publish.Publisher.Helpers;

namespace NZOR.Publish.Publisher.Builders
{
    class StatisticsBuilder
    {
        private readonly string _connectionString;

        private List<Statistic> _statistics;

        public StatisticsBuilder(string connectionString)
        {
            _connectionString = connectionString;

            _statistics = new List<Statistic>();
        }

        public void Build()
        {
            LoadStatistics();
        }

        public List<Statistic> Statistics
        {
            get { return _statistics; }
        }

        private void LoadStatistics()
        {
            string sql = @"

SELECT 
    Name,
    DisplayName,
    DateGenerated,
    CAST(Value AS INT) AS Value
FROM
    [admin].Statistic
    INNER JOIN [admin].StatisticType
        ON Statistic.StatisticTypeId = StatisticType.StatisticTypeId
ORDER BY
    DateGenerated,
    Name

";

            using (var drd = DataAccess.ExecuteReader(_connectionString, sql))
            {
                while (drd.Read())
                {
                    var statistic = new Statistic();

                    statistic.Name = drd.GetString("Name");

                    statistic.DisplayName = drd.GetString("DisplayName");
                    statistic.Date = drd.GetDateTime("DateGenerated");
                    statistic.Value = drd.GetInt32("Value");

                    _statistics.Add(statistic);
                }
            }
        }
    }
}
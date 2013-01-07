using System.Collections.Generic;
using NZOR.Publish.Model.Common;
using NZOR.Publish.Publisher.Helpers;

namespace NZOR.Publish.Publisher.Builders
{
    class ProvidersBuilder
    {
        private readonly string _connectionString;

        private List<Provider> _providers;

        public ProvidersBuilder(string connectionString)
        {
            _connectionString = connectionString;

            _providers = new List<Provider>();
        }

        public void Build()
        {
            LoadProviders();
        }

        public List<Provider> Providers
        {
            get { return _providers; }
        }

        private void LoadProviders()
        {
            string sql = @"

SELECT
	ProviderId,

	Name,
	Code
FROM
	[admin].Provider

";

            using (var drd = DataAccess.ExecuteReader(_connectionString, sql))
            {
                while (drd.Read())
                {
                    var provider = new Provider();

                    provider.ProviderId = drd.GetGuid("ProviderId");

                    provider.Code = drd.GetString("Code");
                    provider.Name = drd.GetString("Name");

                    _providers.Add(provider);
                }
            }
        }
    }
}

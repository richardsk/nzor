using Microsoft.Practices.Unity;
using NZOR.Admin.Data.Sql.Repositories.Matching;
using NZOR.Matching.Batch.Matchers;
using NZOR.Publish.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;

namespace NZOR.Web.Service
{
    public class DependencyConfig
    {
        public static void RegisterDependencies()
        {
            var container = new UnityContainer();

            string baseSourceFolderPathFullName = HttpContext.Current.Server.MapPath(@"~/App_Data/");
            string connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            var nameRepository = new NameRepository(System.IO.Path.Combine(baseSourceFolderPathFullName, "Indexes/Names"));
            var providerRepository = new ProviderRepository(System.IO.Path.Combine(baseSourceFolderPathFullName, "Providers.xml"));
            var geographicSchemaRepository = new GeographicSchemaRepository(System.IO.Path.Combine(baseSourceFolderPathFullName, "GeographicSchemas.xml"));
            var taxonRanksRepository = new TaxonRankRepository(System.IO.Path.Combine(baseSourceFolderPathFullName, "TaxonRanks.xml"));
            var vocabulariesRepository = new VocabularyRepository(System.IO.Path.Combine(baseSourceFolderPathFullName, "Vocabularies.xml"));
            var taxonPropertyRepository = new TaxonPropertyRepository(System.IO.Path.Combine(baseSourceFolderPathFullName, "TaxonProperties.xml"));
            var deprecatedRecordRepository = new DeprecatedRecordRepository(System.IO.Path.Combine(baseSourceFolderPathFullName, "DeprecatedRecords.xml"));
            var statisticRepository = new StatisticRepository(System.IO.Path.Combine(baseSourceFolderPathFullName, "Statistics.xml"));
            var matchRepository = new MatchRepository(connectionString);
            var nameMatcher = new NameMatcher(System.IO.Path.Combine(baseSourceFolderPathFullName, "Indexes/Names"), connectionString);
            var adminRepository = new Admin.Data.Sql.Repositories.AdminRepository(connectionString);
            var feedbackRepository = new Admin.Data.Sql.Repositories.FeedbackRepository(connectionString);
            var provRepository = new Admin.Data.Sql.Repositories.ProviderRepository(connectionString);
            var provNameRepository = new NZOR.Data.Sql.Repositories.Provider.NameRepository(connectionString);

            container.RegisterInstance<NameRepository>(nameRepository);
            container.RegisterInstance<ProviderRepository>(providerRepository);
            container.RegisterInstance<GeographicSchemaRepository>(geographicSchemaRepository);
            container.RegisterInstance<TaxonRankRepository>(taxonRanksRepository);
            container.RegisterInstance<VocabularyRepository>(vocabulariesRepository);
            container.RegisterInstance<TaxonPropertyRepository>(taxonPropertyRepository);
            container.RegisterInstance<DeprecatedRecordRepository>(deprecatedRecordRepository);
            container.RegisterInstance<StatisticRepository>(statisticRepository);
            container.RegisterInstance<MatchRepository>(matchRepository);
            container.RegisterInstance<NameMatcher>(nameMatcher);
            container.RegisterInstance<NZOR.Admin.Data.Sql.Repositories.AdminRepository>(adminRepository);
            container.RegisterInstance<NZOR.Admin.Data.Sql.Repositories.FeedbackRepository>(feedbackRepository);
            container.RegisterInstance<NZOR.Admin.Data.Sql.Repositories.ProviderRepository>(provRepository);
            container.RegisterInstance<NZOR.Data.Sql.Repositories.Provider.NameRepository>(provNameRepository);

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }

    public class UnityDependencyResolver : UnityDependencyScope, IDependencyResolver
    {
        public UnityDependencyResolver(IUnityContainer container)
            : base(container)
        {
        }

        public IDependencyScope BeginScope()
        {
            var childContainer = Container.CreateChildContainer();

            return new UnityDependencyScope(childContainer);
        }
    }

    public class UnityDependencyScope : IDependencyScope
    {
        protected IUnityContainer Container { get; private set; }

        public UnityDependencyScope(IUnityContainer container)
        {
            Container = container;
        }

        public object GetService(Type serviceType)
        {
            if (typeof(IHttpController).IsAssignableFrom(serviceType))
            {
                return Container.Resolve(serviceType);
            }

            try
            {
                return Container.Resolve(serviceType);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Container.ResolveAll(serviceType);
        }

        public void Dispose()
        {
            Container.Dispose();
        }
    }
}
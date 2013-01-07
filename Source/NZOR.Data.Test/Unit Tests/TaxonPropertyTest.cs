using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using NUnit.Framework;
using NZOR.Data.Entities.Consensus;
using NZOR.Data.Repositories.Consensus;
using NZOR.Data.Sql.Repositories.Consensus;

namespace NZOR.Data.Test.Unit_Tests
{
    [TestFixture]
    public class TaxonPropertyTest
    {
        [Test()]
        public void TestTaxonPropertyByName()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;

            TaxonPropertyRepository tpr = new TaxonPropertyRepository(connectionString);

            List<TaxonProperty> tpList = tpr.GetTaxonPropertiesByName(new Guid("C1BF7EA9-5BC3-4F2D-9F5C-06A996E43BCC"));
                        
        }

        [Test()]
        public void SaveAndGetTaxonProperty()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            NZOR.Data.Sql.Repositories.Common.LookUpRepository lr = new Sql.Repositories.Common.LookUpRepository(connectionString);

            NZOR.Data.Repositories.Provider.ITaxonPropertyRepository tpr = new Sql.Repositories.Provider.TaxonPropertyRepository(connectionString);
            NZOR.Data.Repositories.Provider.INameRepository nr = new NZOR.Data.Sql.Repositories.Provider.NameRepository(connectionString);
                            
            List<NZOR.Data.Entities.Common.TaxonPropertyType> types = lr.GetTaxonPropertyTypes();
            NZOR.Data.LookUps.Common.TaxonPropertyTypeLookup tpl = new LookUps.Common.TaxonPropertyTypeLookup(types);

            NZOR.Data.Entities.Provider.TaxonProperty tp = new Entities.Provider.TaxonProperty();
            tp.AddedDate = DateTime.Now;
            tp.ConceptId = Guid.NewGuid();
            tp.InUse = true;
            tp.ProviderRecordId = "test_1";
            tp.ProviderNameId = "8D03B9C1-DE3E-4DD8-99C4-01035CE70FC2";

            NZOR.Data.Entities.Provider.Name pn = nr.GetNameByProviderId("NZOR_Test", tp.ProviderNameId);
            tp.NameId = pn.NameId;

            tp.TaxonPropertyClassId = new Guid("CFA152D5-831C-4A4E-BA4F-50F9F18E7B70");
            tp.TaxonPropertyId = Guid.NewGuid();
            tp.DataSourceId = new Guid("2DD748D7-0CF4-4A74-8E01-3464F688603B");

            tp.State = Entities.Entity.EntityState.Added;

            NZOR.Data.Entities.Provider.TaxonPropertyValue tpv = new Entities.Provider.TaxonPropertyValue();
            tpv.TaxonPropertyValueId = Guid.NewGuid();
            tpv.TaxonPropertyId = tp.TaxonPropertyId;
            tpv.TaxonPropertyType = NZOR.Data.LookUps.Common.TaxonPropertyTypeLookup.PropertyTypeGeoRegion;
            tpv.TaxonPropertyTypeId = tpl.GetTaxonPropertyType(new Guid("CFA152D5-831C-4A4E-BA4F-50F9F18E7B70"), NZOR.Data.LookUps.Common.TaxonPropertyTypeLookup.PropertyTypeGeoRegion).TaxonPropertyTypeId;
            tpv.Value = "New Zealand";

            tp.TaxonPropertyValues.Add(tpv);

            tpr.TaxonProperties.Add(tp);
            tpr.Save();

            List<NZOR.Data.Entities.Provider.TaxonProperty> tpList = tpr.GetTaxonPropertiesByName(tp.NameId.Value);
            Assert.That(tpList.Count > 0);

            List<NZOR.Data.Entities.Provider.TaxonProperty> provProps = new List<Entities.Provider.TaxonProperty>();
            provProps.Add(tp);
            List<TaxonProperty> consProps = Data.Sql.Integration.GetConsensusTaxonProperties(provProps, connectionString);

            //Assert.AreEqual(1, consProps[0].TaxonPropertyValues.Count);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Data;
using System.Data.SqlClient;

using NZOR.Data.Entities;

namespace NZOR.Integration.Test.Unit_Tests
{
    [TestFixture]
    public class DataValidTests
    {
        [Test]
        public void TestConsensusNameProperties()
        {
            string cnnStr = System.Configuration.ConfigurationManager.ConnectionStrings["NZOR"].ConnectionString;
            string sql = @"select COUNT(*)
                from provider.Name pn
                inner join provider.NameProperty pnp on pnp.NameID = pn.NameID
                inner join consensus.Name n on n.NameID = pn.ConsensusNameID
                left join consensus.NameProperty np on np.NameID = n.NameID and np.NamePropertyTypeID = pnp.NamePropertyTypeID
                where np.NamePropertyID is null and pnp.Value <> ''";

            using (SqlConnection cnn = new SqlConnection(cnnStr))
            {
                cnn.Open();

                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = sql;

                    int count = (int)cmd.ExecuteScalar();

                    Assert.AreEqual(0, count);
                }
            }
        }

    }
}

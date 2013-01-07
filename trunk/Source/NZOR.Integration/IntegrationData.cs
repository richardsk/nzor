using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NZOR.Matching;
using NZOR.Data.DataSets;

namespace NZOR.Integration
{
    public class IntegrationData
    {
        public bool UseDB = true;
        public string DBCnnStr = null;
        public Guid NameID = Guid.Empty;
        public String FullName = "";
        public List<Guid> ParentConsNameIDs = new List<Guid>();
        public ConfigSet Config = null;
        public Guid IntegrationBatchID = Guid.Empty;
        public DsIntegrationName.ProviderNameRow ProviderName = null;
        public List<Admin.Data.Entities.AttachmentPoint> AttachmentPoints = null;
        
        /// If useDB then the dbCnnStr DB is used to get the data and check for matches, otherwise all data for matching is contained in the nameData dataset, in memory.
        public IntegrationData(Guid nameID, String fullName, List<Guid> parentConsNameIDs, ConfigSet config, bool useDB, string dbCnnStr, Guid batchID, List<Admin.Data.Entities.AttachmentPoint> attPoints)
        {
            this.UseDB = useDB;
            this.DBCnnStr = dbCnnStr;
            this.NameID = nameID;
            this.FullName = fullName;
            this.ParentConsNameIDs = parentConsNameIDs;
            this.Config = config;
            this.IntegrationBatchID = batchID;
            this.AttachmentPoints = attPoints;
        }
    }
}

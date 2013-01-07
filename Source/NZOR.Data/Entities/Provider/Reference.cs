using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Provider
{
    public class Reference : Entity
    {
        public Guid ReferenceId { get; set; }

        public Guid ReferenceTypeId { get; set; }
        public Guid DataSourceId { get; set; }

        public Guid? ConsensusReferenceId { get; set; }
        public Guid? IntegrationBatchId { get; set; }
        public String LinkStatus { get; set; }
        public Int32? MatchScore { get; set; }
        public String MatchPath { get; set; } 

        public String ProviderRecordId { get; set; }
        public DateTime? ProviderCreatedDate { get; set; }
        public DateTime? ProviderModifiedDate { get; set; }

        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public List<ReferenceProperty> ReferenceProperties { get; private set; }

        public Reference()
        {
            ReferenceId = Guid.Empty;

            ReferenceTypeId = Guid.Empty;
            DataSourceId = Guid.Empty;

            ConsensusReferenceId = null;
            IntegrationBatchId = null;
            LinkStatus = null;
            MatchScore = null;
            MatchPath = null;

            ProviderRecordId = null;
            ProviderCreatedDate = null;
            ProviderModifiedDate = null;

            AddedDate = null;
            ModifiedDate = null;

            ReferenceProperties = new List<ReferenceProperty>();
        }

        public ReferenceProperty GetReferenceProperty(string name)
        {
            foreach (ReferenceProperty rp in ReferenceProperties)
            {
                if (rp.ReferencePropertyType.ToLower() == name.ToLower()) return rp;
            }
            return null;
        }
    }
}

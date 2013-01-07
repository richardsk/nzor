using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Admin.Data.Entities
{
    public class IntegrationIssue
    {
        public NZORRecordType RecordType { get; set; }
        public Guid RecordId { get; set; }
        public string RecordText { get; set; }
        public string LinkStatus { get; set; }
        public string Message { get; set; }
        
    }

    public class ProviderStatistics
    {
        public Provider Provider { get; set; }
        public DataSource DataSource { get; set; }
        public int ProviderNameCount { get; set; }
        public int ProviderConceptCount { get; set; }
        public int ProviderReferenceCount { get; set; }
        public int IntegratedNameCount { get; set; }
        public int IntegratedConceptCount { get; set; }
        public int IntegratedReferenceCount { get; set; }
        public DateTime? LastNameUpdatedDate { get; set; }
        public DateTime? LastConceptUpdatedDate { get; set; }
        public DateTime? LastReferenceUpdatedDate { get; set; }
        public DateTime? LastHarvestDate { get; set; }

        public List<IntegrationIssue> Issues { get; set; }

        public ProviderStatistics()
        {
            Provider = null;
            DataSource = null;

            ProviderNameCount = 0;
            ProviderConceptCount = 0;
            ProviderReferenceCount = 0;

            IntegratedNameCount = 0;
            IntegratedConceptCount = 0;
            IntegratedReferenceCount = 0;

            LastNameUpdatedDate = null;
            LastConceptUpdatedDate = null;
            LastReferenceUpdatedDate = null;
            LastHarvestDate = null;
            
            Issues = new List<IntegrationIssue>();
        }
    }
}

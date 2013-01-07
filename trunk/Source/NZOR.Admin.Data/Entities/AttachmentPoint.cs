using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Admin.Data.Entities
{
    public class AttachmentPointDataSource
    {
        public Guid AttachmentPointId { get; set; }
        public Guid DataSourceId { get; set; }
        public int Ranking { get; set; }

        public AttachmentPointDataSource()
        {
            AttachmentPointId = Guid.Empty;
            DataSourceId = Guid.Empty;
            Ranking = -1;
        }
    }

    public class AttachmentPoint
    {
        public Guid AttachmentPointId { get; set; }
        public Guid ProviderId { get; set; }
        public Guid DataSourceId { get; set; }
        public String ProviderRecordId { get; set; }
        public Guid ConsensusNameId { get; set; }
        public String FullName { get; set; }
        public DateTime AddedDate { get; set; }
        public String AddedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public String ModifiedBy { get; set; }

        
        public AttachmentPoint()
        {
            DataSourceId = Guid.Empty;
            ProviderId = Guid.Empty;
            DataSourceId = Guid.Empty;
            ProviderRecordId = String.Empty;
            ConsensusNameId = Guid.Empty;
            FullName = String.Empty;
            AddedDate = DateTime.MinValue;
            AddedBy = String.Empty;
            ModifiedDate = DateTime.MinValue;
            ModifiedBy = String.Empty;

        }

        public void LoadFromRow(Datasets.DsAttachmentPoint.AttachmentPointRow row)
        {
            AttachmentPointId = row.AttachmentPointId;
            DataSourceId = row.DataSourceId;
            ProviderId = row.ProviderId;
            ProviderRecordId = row.ProviderRecordId;
            ConsensusNameId = row.ConsensusNameId;
            FullName = row.FullName;           
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NZOR.Admin.Data.Entities;

namespace NZOR.Web.Admin.Models
{
    public class AttachmentPointDSDetail
    {
        public string DataSource { get; set; }
        public int Ranking { get; set; }
    }

    public class AttachmentPointDetail
    {
        public string Provider { get; set; }
        public string DataSource { get; set; }
        public string FullName { get; set; }
        public string ProviderRecordId { get; set; }
        public string Rank { get; set; }
        public int SortOrder { get; set; }
        public List<AttachmentPointDSDetail> AttPointDataSources { get; set; }
    }

    public class AttachmentPointModel
    {
        public List<AttachmentPointDetail> AttachmentPointDetails { get; set; }
    }

}
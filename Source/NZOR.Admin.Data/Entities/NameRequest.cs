using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Admin.Data.Entities
{
    public class NameRequest : Entity
    {
        public struct Statuses
        {
            public const string Pending = "Pending";
            public const string Processed = "Processed";
        }

        public Guid NameRequestId { get; set; }
        public string FullName { get; set; }
        public DateTime RequestDate { get; set; }
        public string ApiKey { get; set; }
        public Guid? BatchMatchId { get; set; } 
        public string RequesterEmail { get; set; }
        public Guid? ExternalLooksupServiceId { get; set; }
        public string ExternalLookupDataUrl { get; set; }
        public string Status { get; set; }
        public DateTime? AddedDate { get; set;}
        public string AddedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Admin.Data.Entities
{
    public class BrokeredName : Entity
    {
        public Guid BrokeredNameId { get; set; }
        public Guid NameRequestId { get; set; }
        public Guid ExternalLookupServiceId { get; set; }
        public string ProviderRecordId { get; set; }
        public Guid? NZORProviderNameId { get; set; }
        public string DataUrl { get; set; }
        public string WebUrl { get; set; }
        public DateTime? AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

    }
}

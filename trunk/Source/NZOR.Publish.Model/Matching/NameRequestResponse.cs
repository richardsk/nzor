using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Matching
{
    public class BrokeredName
    {
        public string FullName { get; set; }
        public string DataSource { get; set; }
        public string ProviderNameStatus { get; set; }
        public string ProviderNameSource { get; set; }
    }

    public class NameRequestResponse
    {
        public string NameRequestId { get; set; }
        public string FullName { get; set; }
        public string RequestDate { get; set; }
        public string BatchMatchId { get; set; }
        public string Status { get; set; }

        public List<BrokeredName> BrokeredNames { get; set; }
    }
}

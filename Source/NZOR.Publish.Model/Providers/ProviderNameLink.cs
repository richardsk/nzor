using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Providers
{
    public class ProviderNameLink
    {
        public Guid NameId { get; set; }

        public string FullName { get; set; }
        public string ProviderCode { get; set; }
        public string ProviderRecordId { get; set; }

        public ProviderNameLink()
        {
            FullName = String.Empty;
            ProviderCode = String.Empty;
            ProviderRecordId = String.Empty;
        }
    }
}

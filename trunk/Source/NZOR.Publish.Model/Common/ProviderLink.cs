using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Common
{
    public class ProviderLink
    {
        public Guid ProviderId { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }

        public ProviderLink()
        {
            Code = String.Empty;
            Name = String.Empty;
        }
    }
}

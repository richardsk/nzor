using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Common
{
    /// <summary>
    /// Represents a provider that submits details to NZOR.
    /// </summary>
    public class Provider
    {
        public Guid ProviderId { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }

        public Provider()
        {
            Code = String.Empty;
            Name = String.Empty;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Common
{
    /// <summary>
    /// Represents a record that has been deprecated in NZOR.
    /// </summary>
    public class DeprecatedRecord
    {
        public Guid OldId { get; set; }
        public Guid NewId { get; set; }
        public string Type { get; set; }
        public DateTime DeprecatedDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Names
{
    /// <summary>
    /// A summary version of a name for linking to further details.
    /// </summary>
    public class NameLink
    {
        public Guid NameId { get; set; }

        public string FullName { get; set; }
        public string PartialName { get; set; }
        public string Rank { get; set; }

        public NameLink()
        {
            FullName = String.Empty;
            PartialName = String.Empty;
            Rank = String.Empty;
        }
    }
}

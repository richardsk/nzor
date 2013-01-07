using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.References
{
    public class ReferenceLink
    {
        public Guid ReferenceId { get; set; }

        public string Type { get; set; }
        public string Citation { get; set; }
    }
}

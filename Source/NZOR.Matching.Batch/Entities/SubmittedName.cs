using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Matching.Batch
{
    class SubmittedName
    {
        public string Id { get; set; }
        public string ScientificName { get; set; }

        public SubmittedName()
        {
            Id = String.Empty;
            ScientificName = String.Empty;
        }
    }
}

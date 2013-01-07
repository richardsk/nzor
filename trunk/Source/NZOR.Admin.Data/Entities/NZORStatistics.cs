using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Admin.Data.Entities
{
    public class NZORStatistics
    {
        public int NZORNameCount { get; set; }
        public int NZORConceptCount { get; set; }
        public int NZORReferenceCount { get; set; }

        public NZORStatistics()
        {
            NZORNameCount = 0;
            NZORConceptCount = 0;
            NZORReferenceCount = 0;
        }
    }
}

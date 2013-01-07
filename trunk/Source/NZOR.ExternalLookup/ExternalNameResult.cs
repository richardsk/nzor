using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Admin.Data.Entities;
using NZOR.Data.Entities.Provider;

namespace NZOR.ExternalLookups
{
    public class ExternalNameResult
    {
        public ExternalLookupService LookupService { get; set; }
        public Name Name { get; set; }
        public List<Concept> Concepts { get; set; }
        public string DataSourceCode { get; set; }
        public string DataUrl { get; set; }
        public string WebUrl { get; set; }

        public ExternalNameResult()
        {
            Concepts = new List<Concept>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Common
{
    public class VocabularyValue
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }

        public int? SortOrder { get; set; }

        public VocabularyValue()
        {
            SortOrder = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace NZOR.Publish.Model.Common
{
    public class Vocabulary
    {
        public string Title { get; set; }
        public string Description { get; set; }

        [XmlArrayItem(ElementName = "Use")]
        public List<string> Uses { get; set; }

        [XmlArrayItem(ElementName = "Value")]
        public List<VocabularyValue> Values { get; set; }

        public Vocabulary()
        {
            Title = String.Empty;
            Description = String.Empty;

            Uses = new List<string>();
            Values = new List<VocabularyValue>();
        }
    }
}

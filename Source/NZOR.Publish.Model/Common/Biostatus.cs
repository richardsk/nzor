using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace NZOR.Publish.Model.Common
{
    public class Biostatus
    {
        [XmlArrayItem(ElementName = "Value")]
        public List<string> Biome { get; set; }
        [XmlArrayItem(ElementName = "Value")]
        public List<string> EnvironmentalContext { get; set; }
        [XmlArrayItem(ElementName = "Value")]
        public List<string> Origin { get; set; }
        [XmlArrayItem(ElementName = "Value")]
        public List<string> Occurrence { get; set; }
        [XmlArrayItem(ElementName = "Value")]
        public List<string> GeoRegion { get; set; }
        [XmlArrayItem(ElementName = "Value")]
        public List<string> GeoSchema { get; set; }

        public Biostatus()
        {
            Biome = new List<string>();
            EnvironmentalContext = new List<string>();
            Origin = new List<string>();
            Occurrence = new List<string>();
            GeoRegion = new List<string>();
            GeoSchema = new List<string>();
        }
    }
}

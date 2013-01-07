using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace NZOR.Publish.Model.Matching
{
    public class AutoBatchMatchResponse
    {
        public string BatchMatchId { get; set; }
        public string Status { get; set; }
        public string DownloadUrl { get; set; }        
    }
}

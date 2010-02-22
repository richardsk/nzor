using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OAIServer
{
    public class OAIRequestSession
    {
        public String CallerIP = "";
        public DateTime CallDate = DateTime.MinValue;
        public String RequestUrl = "";
        public String ResumptionToken = "";
        public DateTime ResumptionExpiry = DateTime.MinValue;
        public String Repository = "";
        public String Set = "";
        public String MetadataPrefix = "";
        public String FromDate = "";
        public String ToDate = "";
        public String Identifier = "";
        
        public int NumRecords = 0;
        public int Cursor = 0;

        public System.Collections.Hashtable NextRecordPositions = new System.Collections.Hashtable();
    }
}

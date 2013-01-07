using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Admin.Data.Entities.Matching
{
    public class Match : Entity
    {
        public struct Statuses
        {
            public const string Pending = "Pending";
            public const string Processing = "Processing";
            public const string Processed = "Processed";
            public const string Completed = "Completed";
            public const string Sent = "Sent";
            public const string Error = "Error";
        }

        public Guid MatchId { get; set; }

        public string SubmitterEmail { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string Status { get; set; }
        public string InputData { get; set; }
        public string ResultData { get; set; }
        public string ExternalLookupResults { get; set; }
        public bool? IsServiceMediated { get; set; }
        public string ApiKey { get; set; }
        public string Error { get; set; }
        public bool? DoExternalLookup { get; set; }

        public Match()
        {
            MatchId = Guid.NewGuid();

            SubmitterEmail = string.Empty;
            ReceivedDate = DateTime.UtcNow;
            Status = Statuses.Pending;
            InputData = string.Empty;
            ResultData = string.Empty;
            ExternalLookupResults = string.Empty;
            IsServiceMediated = false;
            ApiKey = string.Empty;
            Error = string.Empty;
            DoExternalLookup = false;
        }
    }
}

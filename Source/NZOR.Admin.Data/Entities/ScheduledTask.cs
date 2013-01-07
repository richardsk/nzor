using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Admin.Data.Entities
{
    public class ScheduledTask : Entity
    {
        public Guid ScheduledTaskId { get; set; }
        public Guid RelatedId { get; set; }
        public String Name { get; set; }
        public int FrequencyDays { get; set; }
        public String PreferredStartTimeGMT { get; set; }
        public DateTime? LastRun { get; set; }
        public string LastRunOutcome { get; set;}
        public string Status { get; set; }

        public ScheduledTask()
        {
            ScheduledTaskId = Guid.Empty;
            RelatedId = Guid.Empty;
            Name = null;

            FrequencyDays = -1;
            PreferredStartTimeGMT = null;
            LastRun = null;
            LastRunOutcome = null;
            Status = null;
        }
    }
}

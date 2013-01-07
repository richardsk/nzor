using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Admin.Data.Process
{
    public enum ProcessResultOutcome
    {
        Unknown,
        Succeded,
        Failed
    }

    public class ProcessResult
    {
        public ProcessResultOutcome Result { get; set; }
        public List<String> ErrorMessages { get; set; }
        public List<String> Log { get; set; }
        public bool UpdateRequired { get; set; } //if changes from the previous step require the next step to run

        public ProcessResult()
        {
            Result = ProcessResultOutcome.Unknown;
            ErrorMessages = new List<string>();
            Log = new List<string>();
            UpdateRequired = false;
        }
    }
}

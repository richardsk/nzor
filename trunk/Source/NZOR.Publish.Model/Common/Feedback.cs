using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Publish.Model.Common
{
    public class Feedback
    {
        public Guid FeedbackId { get; set; }
        public Guid? NameId { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string Resolution { get; set; }
        public string Sender { get; set; }
        public string SenderEmail { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<string> ProvidersToEmail { get; set; }

        public Feedback()
        {
            ProvidersToEmail = new List<string>();
        }
    }
}

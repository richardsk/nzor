using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Admin.Data.Entities
{
    public enum FeedbackStatus
    {
        Created,
        Answered,
        Resolved,
        NotToBeResolved,
        Discarded
    }

    public class Feedback : Entity
    {
        public Guid FeedbackId { get; set; }
        public Guid? NameId { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string Resolution { get; set; }
        public string Sender { get; set; }
        public string SenderEmail { get; set; }
        public string SentTo { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public Feedback()
        {
            Message = string.Empty;
            Status = string.Empty;
            Sender = string.Empty;
            SenderEmail = string.Empty;
            SentTo = string.Empty;
        }
    }
}

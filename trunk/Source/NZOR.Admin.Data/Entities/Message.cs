using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Admin.Data.Entities
{
    public class MessageType : Entity 
    {
        public Guid MessageTypeId { get; set; }
        public string Name { get; set; }
    }

    public class Message : Entity
    {
        public Guid MessageId { get; set; }
        public Guid MessageTypeId { get; set; }
        public Guid? TaskId { get; set; }
        public DateTime AddedDate { get; set; }
        public string MessageText { get; set; }
        public string Data { get; set; }
        public int? Level { get; set; }
    }
}

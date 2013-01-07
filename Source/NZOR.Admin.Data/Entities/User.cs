using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Admin.Data.Entities
{
    public enum UserStatus
    {
        Requested,
        EmailSent,
        Registered,
        PasswordReset,
        Unknown
    }

    public class User : Entity
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public string Organisation { get; set; }
        public string APIKey { get; set; }
        public string Status { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class UserMessageType : Entity
    {
        public Guid UserMessageTypeId { get; set; }
        public Guid UserId { get; set; }
        public Guid MessageTypeId { get; set; }
    }
}

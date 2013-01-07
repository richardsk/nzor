using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Admin.Data.Entities
{
    public class DataSource : Entity
    {
        public Guid DataSourceId { get; set; }
        public Guid ProviderId { get; set; }
        public String Name { get; set; }
        public String Code { get; set; }
        public String Description { get; set; }
        public DateTime? AddedDate { get; set; }
        public String AddedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public String ModifiedBy { get; set; }

        public DataSource()
        {
            DataSourceId = Guid.Empty;
            ProviderId = Guid.Empty;
            Name = null;
            Code = null;
            Description = null;
            AddedDate = null;
            AddedBy = null;
            ModifiedDate = null;
            ModifiedBy = null;

        }
    }
}

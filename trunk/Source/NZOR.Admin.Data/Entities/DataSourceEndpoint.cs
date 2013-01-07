using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Admin.Data.Entities
{
    public class DataSourceEndpoint : Entity
    {
        public Guid DataSourceEndpointId { get; set; }
        public Guid DataSourceId { get; set; }
        public Guid DataTypeId { get; set; }
        public String DataType { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String Url { get; set; }
        public DateTime? LastHarvestDate { get; set; }

        public ScheduledTask Schedule { get; set; }

        public DataSourceEndpoint()
        {
            DataSourceEndpointId = Guid.Empty;
            DataSourceId = Guid.Empty;
            DataTypeId = Guid.Empty;
            
            DataType = null;

            Name = null;
            Description = null;
            Url = null;
            LastHarvestDate = null;

            Schedule = null;
        }
    }
}

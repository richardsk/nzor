using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Common
{
    public class NamePropertyType
    {
        public Guid NamePropertyTypeId { get; set; }
        public Guid NameClassId { get; set; }
        public String Name { get; set; }
        public int? MinOccurrences { get; set; }
        public int? MaxOccurrences { get; set; }
        public String GoverningCode { get; set; }

        public NamePropertyType()
        {
            NamePropertyTypeId = Guid.Empty;
            NameClassId = Guid.Empty;
            Name = String.Empty;
            MinOccurrences = null;
            MaxOccurrences = null;
            GoverningCode = null;
        }
    }
}

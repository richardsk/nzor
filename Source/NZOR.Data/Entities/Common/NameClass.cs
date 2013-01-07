using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Common
{
    public class NameClass
    {
        public Guid NameClassId { get; set; }

        public String Code { get; set; }
        public String Name { get; set; }
        public bool HasClassification { get; set; }

        public NameClass()
        {
            NameClassId = Guid.Empty;

            Code = String.Empty;
            Name = String.Empty;
            HasClassification = false;
        }
    }
}

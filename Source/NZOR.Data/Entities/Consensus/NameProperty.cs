using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Consensus
{
    public class NameProperty : Entities.Entity
    {
        public Guid NamePropertyId { get; set; }

        public Guid NamePropertyTypeId { get; set; }
        public string NamePropertyType { get; set; }
        public Guid? RelatedId { get; set; }

        public Int32? Sequence { get; set; }
        public String Value { get; set; }

        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public NameProperty()
        {
            NamePropertyId = Guid.NewGuid();

            NamePropertyTypeId = Guid.Empty;
            NamePropertyType = null;
            RelatedId = null;
            
            Sequence = null;
            Value = String.Empty;

            AddedDate = null;
            ModifiedDate = null;
        }
    }
}

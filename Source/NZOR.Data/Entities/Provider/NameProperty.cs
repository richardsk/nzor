using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Provider
{
    public class NameProperty
    {
        public Guid NamePropertyId { get; set; }

        public Guid NamePropertyTypeId { get; set; }
        public String NamePropertyType { get; set; }
        public Guid? RelatedId { get; set; }

        public String ProviderRelatedId { get; set; }

        public Int32? Sequence { get; set; }
        public String Value { get; set; }

        public NameProperty()
        {
            NamePropertyId = Guid.NewGuid();

            NamePropertyTypeId = Guid.Empty;
            NamePropertyType = null;
            RelatedId = null;

            ProviderRelatedId = null;

            Sequence = null;
            Value = String.Empty;
        }
    }
}

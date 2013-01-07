using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Consensus
{
    public class Reference : Entity
    {
        public Guid ReferenceId { get; set; }

        public Guid ReferenceTypeId { get; set; }
        
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public List<ReferenceProperty> ReferenceProperties { get; private set; }

        public Reference()
        {
            ReferenceId = Guid.Empty;

            ReferenceTypeId = Guid.Empty;
         
            AddedDate = null;
            ModifiedDate = null;

            ReferenceProperties = new List<ReferenceProperty>();
        }

        public ReferenceProperty GetProperty(string propertyType)
        {
            ReferenceProperty rp = null;

            foreach (ReferenceProperty prop in ReferenceProperties)
            {
                if (prop.ReferencePropertyType == propertyType)
                {
                    rp = prop;
                    break;
                }
            }

            return rp;
        }

        public void SetReferenceProperty(Entities.Common.ReferencePropertyType prop, string value)
        {
            if (State == EntityState.Deleted) return;

            bool done = false;
            foreach (ReferenceProperty rp in ReferenceProperties)
            {
                if (rp.ReferencePropertyType == prop.Name)
                {
                    rp.Value = value;
                    done = true;
                    break;
                }
            }

            if (!done)
            {
                //add property
                ReferenceProperty rp = new ReferenceProperty();
                rp.ReferencePropertyId = Guid.NewGuid();
                rp.ReferencePropertyType = prop.Name;
                rp.ReferencePropertyTypeId = prop.ReferencePropertyTypeId;
                rp.Value = value;

                ReferenceProperties.Add(rp);
            }

            if (State != EntityState.Added) State = EntityState.Modified;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Consensus
{
    public class Name : Entity
    {
        public Guid NameId { get; set; }

        public Guid TaxonRankId { get; set; }
        public Guid NameClassId { get; set; }
                
        public String FullName { get; set; }
        public String GoverningCode { get; set; }
        public bool? IsRecombination { get; set; }

        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public List<NameProperty> NameProperties { get; private set; }

        public Name()
        {
            NameId = Guid.Empty;

            TaxonRankId = Guid.Empty;
            NameClassId = Guid.Empty;
        
            FullName = String.Empty;            
            GoverningCode = null;
            IsRecombination = null;

            AddedDate = null;
            ModifiedDate = null;

            NameProperties = new List<NameProperty>();
        }

        public NameProperty GetNameProperty(string name)
        {
            foreach (NameProperty np in NameProperties)
            {
                if (np.NamePropertyType == name) return np;
            }
            return null;
        }

        public void SetNameProperty(Entities.Common.NamePropertyType prop, string value, bool addMultiple)
        {
            if (State == EntityState.Deleted) return;

            bool done = false;            
            foreach (NameProperty np in NameProperties)
            {
                if (np.NamePropertyType == prop.Name)
                {
                    if (!addMultiple || string.Compare(np.Value, value, true) == 0)
                    {
                        np.Value = value;
                        done = true;
                        break;
                    }
                }
            }

            if (!done)
            {
                //add property
                NameProperty np = new NameProperty();
                np.NamePropertyId = Guid.NewGuid();
                np.AddedDate = DateTime.Now;
                np.NamePropertyType = prop.Name;
                np.NamePropertyTypeId = prop.NamePropertyTypeId;
                np.Value = value;
                
                NameProperties.Add(np);
            }

            if (State != EntityState.Added) State = EntityState.Modified;
        }
    }
}

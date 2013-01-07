using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Provider
{
    public class Name : Entity
    {
        public Guid NameId { get; set; }

        public Guid TaxonRankId { get; set; }
        public Guid NameClassId { get; set; }
        public Guid DataSourceId { get; set; }
        public String DataSourceName { get; set; }

        public Guid? ConsensusNameId { get; set; }
        public String LinkStatus { get; set; }
        public Int32? MatchScore { get; set; }
        public String MatchPath { get; set; }

        public String ProviderRecordId { get; set; }
        public DateTime? ProviderCreatedDate { get; set; }
        public DateTime? ProviderModifiedDate { get; set; }
        public String ProviderName { get; set; }
        public Guid ProviderId { get; set; }
        
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
            DataSourceId = Guid.Empty;

            ConsensusNameId = null;
            LinkStatus = null;
            MatchScore = null;
            MatchPath = null;

            ProviderRecordId = String.Empty;
            ProviderCreatedDate = null;
            ProviderModifiedDate = null;

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
                if (np.NamePropertyType.ToLower() == name.ToLower()) return np;
            }
            return null;
        }
    }
}

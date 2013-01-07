using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NZOR.Data.Entities.Consensus
{
    public class NameProfile
    {
        public Guid NameId { get; set; }
        public string NameClass { get; set; }
        public string FullName { get; set; }
        public string PartialName { get; set; }
        public string GoverningCode { get; set; }
        public string TaxonRank { get; set; }
        public string Language { get; set; }
        public DateTime UpdatedDate { get; set; }

        public NZOR.Data.Entities.Consensus.Name ParentName { get; set; }
        public String ParentNameAccordingTo { get; set; }
        public String ParentNameFull { get; set; }
        public NZOR.Data.Entities.Consensus.Name AcceptedName { get; set; }
        public String AcceptedNameAccordingTo { get; set; }
        public String AcceptedNameFull { get; set; }

        public List<NameProperty> NameProperties { get; set; }

        public List<Concept> NameConcepts { get; set; }

        public List<NZOR.Data.Entities.Consensus.StackedName> TaxonHierarchy { get; set; }

        public List<NZOR.Data.Entities.Consensus.Name> Children { get; set; }
        public List<NZOR.Data.Entities.Consensus.Name> Synonyms { get; set; }

        public List<NameApplication> VernacularApplications { get; set; }
        public List<TaxonProperty> TaxonProperties { get; set; }

        public String ProviderNamesXml { get; set; }

        public List<Provider.Name> ProviderNames { get; set; }
        public List<Provider.Concept> ProviderConcepts { get; set; }

        public List<NameProperty> GetProperties(string propertyTypeName)
        {
            List<NameProperty> props = new List<NameProperty>();
            foreach (NameProperty np in NameProperties)
            {
                if (string.Compare(np.NamePropertyType, propertyTypeName, true) == 0) props.Add(np);
            }
            return props;
        }
    }

    public class NameApplication
    {
        public NZOR.Data.Entities.Consensus.Name Name { get; set; }
        public NZOR.Data.Entities.Consensus.ConceptApplication Application { get; set; }

        public String DisplayText()
        {
            String text = "";

            if ((Application.Gender != null && Application.Gender.Length > 0) ||
                (Application.GeoRegion != null && Application.GeoRegion.Length > 0) ||
                (Application.LifeStage != null && Application.LifeStage.Length > 0) ||
                (Application.PartOfTaxon != null && Application.PartOfTaxon.Length > 0))
            {
                text = "(";

                if (Application.GeoRegion != null && Application.GeoRegion.Length > 0)
                {
                    text += "Geo Region : " + Application.GeoRegion + "; ";
                }
                if (Application.Gender != null && Application.Gender.Length > 0)
                {
                    text += "Gender : " + Application.Gender + "; ";
                }
                if (Application.LifeStage != null && Application.LifeStage.Length > 0)
                {
                    text += "Life Stage : " + Application.LifeStage + "; ";
                }
                if (Application.PartOfTaxon != null && Application.PartOfTaxon.Length > 0)
                {
                    text += "Part of Taxon : " + Application.PartOfTaxon + "; ";
                }

                text = text.TrimEnd();
                text += ")";
            }

            return text;
        }
    }
}
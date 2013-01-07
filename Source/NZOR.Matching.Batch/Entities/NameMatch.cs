using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Admin.Data.Entities.Matching;

namespace NZOR.Matching.Batch
{
    /// <summary>
    /// A potential match for a submitted name.
    /// </summary>
    public class NameMatch
    {
        public string NzorId { get; set; }

        public string NzorFullName { get; set; }
        public string PartialName { get; set; }
        public string Authors { get; set; }
        public string OriginalAuthors { get; set; }
        public string Year { get; set; }
        public string OriginalYear { get; set; }
        public string PreferredName { get; set; }
        public string PreferredNameId { get; set; }
        public string TaxonomicStatus { get; set; }
        public string NomenclaturalStatus { get; set; }
        public string ParentAccordingTo { get; set; }
        public string PreferredAccordingTo { get; set; }
        public string ScientificNamesForVernacular { get; set; }
        public string ScientificNamesForVernacularIds { get; set; }
        public string VernacularNamesForScientific { get; set; }
        public string VernacularNamesForScientificIds { get; set; }        
        public string Classification { get; set; }
        public string ClassificationRanks { get; set; }
        public string ClassificationIds { get; set; }
        public string Biostatus { get; set; }
        public decimal Score { get; set; }

        public List<ExternalLookup> ExternalLookups { get; set; }

        public NameMatch()
        {
            NzorId = String.Empty;

            NzorFullName = String.Empty;
            PartialName = String.Empty;
            Authors = String.Empty;
            OriginalAuthors = String.Empty;
            Year = String.Empty;
            OriginalYear = String.Empty;
            PreferredName = String.Empty;
            PreferredNameId = String.Empty;
            TaxonomicStatus = String.Empty;
            NomenclaturalStatus = String.Empty;
            ScientificNamesForVernacular = String.Empty;
            ScientificNamesForVernacularIds = String.Empty;
            VernacularNamesForScientific = String.Empty;
            VernacularNamesForScientificIds = String.Empty;
            Classification = String.Empty;
            ClassificationRanks = String.Empty;
            ClassificationIds = String.Empty;
            Biostatus = String.Empty;
            ParentAccordingTo = String.Empty;
            PreferredAccordingTo = String.Empty;

            Score = 0M;

            ExternalLookups = new List<ExternalLookup>();
        }
    }
}

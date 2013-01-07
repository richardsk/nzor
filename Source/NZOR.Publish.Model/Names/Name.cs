using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Publish.Model.Common;
using NZOR.Publish.Model.Concepts;
using NZOR.Publish.Model.References;
using NZOR.Publish.Model.Providers;

namespace NZOR.Publish.Model.Names
{
    /// <summary>
    /// Represents the full details of a name.
    /// </summary>
    public class Name
    {
        public Guid NameId { get; set; }

        public string Class { get; set; }
        public string FullName { get; set; }
        public string FormattedFullName { get; set; }
        public string PartialName { get; set; }
        public string FormattedPartialName { get; set; }
        public string Rank { get; set; }
        public int RankSortOrder { get; set; }
        public string Status { get; set; }
        public string GoverningCode { get; set; }
        public string Language { get; set; }
        public string MicroReference { get; set; }
        public string Authors { get; set; }
        public string BasionymAuthors { get; set; }
        public string CombinationAuthors { get; set; }
        public string Country { get; set; }
        public string CultivarNameGroup { get; set; }
        public string Orthography { get; set; }
        public string ProtologueOrthography { get; set; }
        public string InfragenericEpithet { get; set; }
        public string InfraspecificEpithet { get; set; }
        public string NomenclaturalStatus { get; set; }
        public string QualityCode { get; set; }
        public string SpecificEpithet { get; set; }
        public string Uninomial { get; set; }
        public string Year { get; set; }
        public string YearOnPublication { get; set; }
        public bool IsRecombination { get; set; }

        public ReferenceLink PublishedIn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Assigned using the active concept relationship 'is synonym of'.
        /// </remarks>
        public NameLink AcceptedName { get; set; }
        public NameLink BasionymName { get; set; }
        public NameLink BlockedName { get; set; }
        public NameLink LaterHomonymOfName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Assigned using the active concept relationship 'is child of'.
        /// </remarks>
        public NameLink ParentName { get; set; }
        public NameLink RecombinedName { get; set; }
        public NameLink TypeName { get; set; }

        public List<NameLink> ClassificationHierarchy { get; set; }

        public List<Concept> Concepts { get; set; }

        public List<ProviderNameLink> ProviderNames { get; set; }

        public List<Biostatus> Biostatuses { get; set; }

        public List<Annotation> Annotations { get; set; }

        public DateTime AddedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Name()
        {
            Class = String.Empty;
            FullName = String.Empty;
            Rank = String.Empty;
            RankSortOrder = 0;
            Status = String.Empty;
            GoverningCode = String.Empty;
            Language = String.Empty;
            IsRecombination = false;

            ClassificationHierarchy = new List<NameLink>();

            Concepts = new List<Concept>();

            ProviderNames = new List<ProviderNameLink>();

            Biostatuses = new List<Biostatus>();

            Annotations = new List<Annotation>();
        }
    }
}

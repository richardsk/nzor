using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.Entities.Common
{
    public class SearchField
    {
        public enum SearchFieldColumn
        {
            FullName,
            AcceptedName,
            ParentName,
            Authors,
            Year,
            Biostatus,
            ProviderName
        }

        public enum OrderByField
        {
            FullName,
            AcceptedName,
            TaxonRank,
            Authors,
            Year,
            ParentName
        }


        public SearchFieldColumn SearchColumn { get; set; }
        public String SearchText { get; set; }
        public bool AnywhereInText { get; set; }

        public SearchField()
        {
            SearchText = null;
            AnywhereInText = true;
        }

        public SearchField(SearchFieldColumn column, string searchText)
        {
            AnywhereInText = true;
            SearchColumn = column;
            SearchText = searchText;
        }
    }
}

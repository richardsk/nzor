using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Common;

namespace NZOR.Data.LookUps.Common
{
    public class TaxonRankLookUp
    {
        private List<TaxonRank> _taxonRanks;

        public const string RankNone = "none";

        public const int SortOrderKingdom = 400;
        public const int SortOrderOrder = 1600;
        public const int SortOrderFamily = 2000;
        public const int SortOrderGenus = 3000;
        public const int SortOrderSpecies = 4200;

        public TaxonRankLookUp(List<TaxonRank> taxonRanks)
        {
            _taxonRanks = taxonRanks;
        }

        /// <summary>
        /// Get the first taxon (according to SortOrder) that matches on TaxonRankName or KnownAbbreviations.
        /// </summary>
        /// <param name="taxonRankName"></param>
        /// <returns></returns>
        public TaxonRank GetTaxonRank(string taxonRankName, string governingCode)
        {
            TaxonRank taxonRank = null;

            if (_taxonRanks != null)
            {
                taxonRank = (from o in _taxonRanks
                             where o.Name.Equals(taxonRankName, StringComparison.OrdinalIgnoreCase) || o.KnownAbbreviations.ToLower().Contains("@" + taxonRankName.ToLower() + "@")
                                && (o.GoverningCode == null || o.GoverningCode == governingCode)
                             orderby o.SortOrder
                             select o).FirstOrDefault();
            }

            return taxonRank;
        }

        public TaxonRank GetTaxonRank(Guid taxonRankId)
        {
            TaxonRank taxonRank = null;

            if (_taxonRanks != null)
            {
                taxonRank = (from o in _taxonRanks
                             where o.TaxonRankId.Equals(taxonRankId)
                             orderby o.SortOrder
                             select o).FirstOrDefault();
            }

            return taxonRank;
        }
    }
}

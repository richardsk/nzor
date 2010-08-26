using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data.SystemData
{
    public class TaxonRankData
    {
        private static TaxonRank _genusRank = null;

        public static TaxonRank GenusRank()
        {
            if (_genusRank == null)
            {
                NZOR.Data.SystemData.NZOR_System se = new NZOR.Data.SystemData.NZOR_System();
                var ranks = from tr in se.TaxonRank where tr.Name.ToLower().Equals("genus") select tr;
                _genusRank = ranks.First();
            }
            return _genusRank;
        }
        
        public static SystemData.TaxonRank GetTaxonRank(Guid taxonRankID)
        {
            NZOR.Data.SystemData.NZOR_System se = new NZOR.Data.SystemData.NZOR_System();
            return (from tr in se.TaxonRank where tr.TaxonRankID.Equals(taxonRankID) select tr).First();
        }
    }
}

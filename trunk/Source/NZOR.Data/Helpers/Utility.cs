using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NZOR.Data.LookUps.Common;

namespace NZOR.Data.Helpers
{
    public class Utility
    {
        //public static string GetFullName(Entities.Consensus.Name cn, TaxonRankLookUp rankLookup, List<Entities.Consensus.Name> parents)
        //{
        //    string fullName = "";

        //    //work through parents first
        //    foreach (Entities.Consensus.Name par in parents)
        //    {
        //        Entities.Common.TaxonRank rank = rankLookup.GetTaxonRank(par.TaxonRankId);

        //        if (rank.SortOrder == TaxonRankLookUp.SortOrderGenus) //genus 
        //        {
        //            fullName += par.GetNameProperty(NamePropertyTypeLookUp.Canonical).Value + " ";
        //        }
        //        else if (rank.SortOrder == TaxonRankLookUp.SortOrderSpecies) //species
        //        {
        //            Entities.Consensus.NameProperty infra = par.GetNameProperty(NamePropertyTypeLookUp.InfragenericEpithet);
        //            if (infra != null) fullName += infra.Value + " ";

        //            fullName += par.GetNameProperty(NamePropertyTypeLookUp.Canonical).Value + " ";
        //        }
        //        else if (rank.SortOrder > TaxonRankLookUp.SortOrderSpecies)
        //        {
        //            Entities.Consensus.NameProperty infra = par.GetNameProperty(NamePropertyTypeLookUp.InfraspecificEpithet);
        //            if (infra != null) fullName += infra.Value + " ";
        //        }
        //    }

        //    //canonical
        //    Entities.Consensus.NameProperty can = cn.GetNameProperty(NamePropertyTypeLookUp.Canonical);
        //    if (can != null) fullName += can.Value + " ";

        //    //authors
        //    //TODO standardised authors??

        //    Entities.Consensus.NameProperty auth = cn.GetNameProperty(NamePropertyTypeLookUp.Authors);
        //    if (auth != null) fullName += auth.Value;
            
        //    return fullName;
        //}
    }
}

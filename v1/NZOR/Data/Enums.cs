using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Data
{
    public enum MatchTypeSelection
    {
        ProviderData,
        ConsensusData,
        Both
    }

    public enum LinkStatus
    {
        Unmatched,        //initial status? 
        Matched,        //matched an existing name/reference 
        Multiple,        //matches multiple names/refs in the db (failed match) - could indicate inconsistencies in the names/ref db 
        DataFail,        //failed to match record due to lack of data in source record 
        Inserted,        //no match and new name/ref was inserted 
        Integrating,    //while the name is being integrated
        Discarded,        //record has been discarded (will not be automatically matched in the future) 
        ParentMissing,        // 
        MultipleParent,        //when a parent match has been done (ie no prov concept was supplied) and multiple possible parents have been found. Allows for reports to locates these names. 
        ParentNotIntegrated        //when the prov names has a parent but for some reason the parent failed to integrate 
    } 


    public class NameProperties
    {
        public static String Year = "Year";
        public static String Canonical = "Canonical";
        public static String Rank = "Rank";
        public static String Authors = "Authors";
        public static String CombinationAuthors = "CombinationAuthors";
    }

    public class ConceptProperties
    {
        public static String ParentRelationshipType = "is child of";
    }

}

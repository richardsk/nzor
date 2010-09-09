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
        ManualMatch,        //manually attached to a name/ref 
        ManualInsert,        //manually inserted a new name/ref record 
        Merge,        //records are linked to current name/ref as a result of a merge of two names/refs 
        Discarded,        //record has been discarded (will not be automatically matched in the future) 
        Editor,        //Editor record link 
        EditorDeferred,        //Editor record that is not high prority (has same standing as a normal soruce record). This occurs when two editor records exist for a name/ref - one is chosen as the prority editor record 
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

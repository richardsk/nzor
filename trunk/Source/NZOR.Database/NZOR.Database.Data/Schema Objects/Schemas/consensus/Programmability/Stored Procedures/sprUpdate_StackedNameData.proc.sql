CREATE PROCEDURE 
	[consensus].[sprUpdate_StackedNameData]

AS

-- clear table
TRUNCATE TABLE consensus.StackedName; 

WITH StackN (SeedNameID, AccordingToID, NameID, TaxonRankID, CanonicalName, RankName, SortOrder, Depth) AS 
( 
    SELECT n.NameID, c.AccordingToReferenceID, n.NameID, tr.TaxonRankID, np.Value, tr.Name, tr.SortOrder, 0
    FROM consensus.Name N 
        inner join TaxonRank tr ON n.TaxonRankID = tr.TaxonRankID
        inner join consensus.NameProperty np on np.NameID = n.NameID and np.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750' 
        inner join consensus.Concept c on c.NameID = n.NameID
        inner join consensus.ConceptRelationship cr on cr.FromConceptID = c.ConceptID and cr.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' and cr.IsActive = 1
        inner join consensus.Concept c2 on c2.ConceptID = cr.ToConceptID        
    --WHERE n.NameID = '03B346BE-9EDF-4910-91B4-0005CB9CBA44' 
     
    UNION ALL 
     
    SELECT s.SeedNameID, c.AccordingToReferenceID, n2.NameID, tr.TaxonRankID, np.Value, tr.Name, tr.SortOrder, s.Depth + 1
    FROM consensus.Name n
		inner join StackN s on s.NameID = n.NameID
        inner join consensus.Concept c on c.NameID = n.NameID --and isnull(c.AccordingToReferenceID,'00000000-0000-0000-0000-000000000000') = isnull(s.AccordingToID,'00000000-0000-0000-0000-000000000000')
        inner join consensus.ConceptRelationship cr on cr.FromConceptID = c.ConceptID and cr.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' and cr.IsActive = 1
        inner join consensus.Concept c2 on c2.ConceptID = cr.ToConceptID
        inner join consensus.Name n2 on n2.NameID = c2.NameID and n2.NameID <> n.NameID
        inner join TaxonRank tr ON n2.TaxonRankID = tr.TaxonRankID
        inner join consensus.NameProperty np on np.NameID = n2.NameID and np.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'         
    
) 


insert consensus.StackedName(SeedNameID, AccordingToID, NameID, TaxonRankID, CanonicalName, RankName, SortOrder, Depth)
SELECT distinct *
FROM StackN 
ORDER BY SeedNameID, Depth DESC 
     
	 
--update stacked name for those names that dont have concepts

insert consensus.StackedName(SeedNameID, AccordingToID, NameID, TaxonRankID, CanonicalName, RankName, SortOrder, Depth)
SELECT n.NameID, 
	(select top 1 AccordingToReferenceID from consensus.Concept where NameID = n.nameid), 
	n.NameID, tr.TaxonRankID, np.Value, tr.Name, tr.SortOrder, 0
FROM consensus.Name N 
	inner join TaxonRank tr ON n.TaxonRankID = tr.TaxonRankID
	inner join consensus.NameProperty np on np.NameID = n.NameID and np.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
WHERE not exists (select * from consensus.Name cn 
	inner join consensus.Concept c on c.NameID = n.NameID		
	inner join consensus.ConceptRelationship cr on cr.FromConceptID = c.ConceptID and cr.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' and cr.IsActive = 1
	where cn.NameID = n.NameID)


RETURN @@ERROR
CREATE PROCEDURE [provider].[sprUpdate_StackedNameData]
	
AS

-- clear table
truncate table provider.StackedName;

WITH StackN (SeedNameID, AccordingToID, NameID, TaxonRankID, CanonicalName, RankName, SortOrder, Depth) AS 
( 
    SELECT n.NameID, c.AccordingToReferenceID, n.NameID, tr.TaxonRankID, np.Value, tr.Name, tr.SortOrder, 0
    FROM provider.Name N 
        inner join TaxonRank tr ON n.TaxonRankID = tr.TaxonRankID
        inner join provider.NameProperty np on np.NameID = n.NameID and np.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750' 
        inner join provider.Concept c on c.NameID = n.NameID
        inner join provider.ConceptRelationship cr on cr.FromConceptID = c.ConceptID and cr.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' and InUse = 1
        inner join provider.Concept c2 on c2.ConceptID = cr.ToConceptID        
    --WHERE n.NameID = '03B346BE-9EDF-4910-91B4-0005CB9CBA44' 
     
    UNION ALL 
     
    SELECT s.SeedNameID, c.AccordingToReferenceID, n2.NameID, tr.TaxonRankID, np.Value, tr.Name, tr.SortOrder, s.Depth + 1
    FROM provider.Name n
        inner join provider.Concept c on c.NameID = n.NameID
        inner join provider.ConceptRelationship cr on cr.FromConceptID = c.ConceptID and cr.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' and InUse = 1
        inner join provider.Concept c2 on c2.ConceptID = cr.ToConceptID
        inner join provider.Name n2 on n2.NameID = c2.NameID and n2.NameID <> n.NameID
        inner join TaxonRank tr ON n2.TaxonRankID = tr.TaxonRankID
        inner join provider.NameProperty np on np.NameID = n2.NameID and np.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750' 
        inner join StackN s on s.NameID = n.NameID
    
) 


insert provider.StackedName
SELECT NEWID(), *
FROM StackN 
    ORDER BY SeedNameID, Depth DESC 
     
	 
--update stacked name for those names that dont have concepts

insert provider.StackedName(StackedNameID, SeedNameID, AccordingToID, NameID, TaxonRankID, CanonicalName, RankName, SortOrder, Depth)
SELECT newid(), n.NameID, c.AccordingToReferenceID, n.NameID, tr.TaxonRankID, np.Value, tr.Name, tr.SortOrder, 0
FROM provider.Name N 
	inner join TaxonRank tr ON n.TaxonRankID = tr.TaxonRankID
	inner join provider.NameProperty np on np.NameID = n.NameID and np.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750' 
	left join provider.Concept c on c.NameID = n.NameID		
WHERE not exists (select * from provider.Name cn 
	inner join provider.Concept c on c.NameID = n.NameID		
	inner join provider.ConceptRelationship cr on cr.FromConceptID = c.ConceptID and cr.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' and cr.InUse = 1
	where cn.NameID = n.NameID)

RETURN @@ERROR
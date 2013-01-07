--delete name data

delete provider.NameProperty
where NameId = @nameId

delete cr
from provider.Name n
inner join provider.Concept c on c.NameId = n.NameId
inner join provider.ConceptRelationship cr on cr.FromConceptId = c.ConceptId
where n.NameId = @nameId

delete cr
from provider.Name n
inner join provider.Concept c on c.NameId = n.NameId
inner join provider.ConceptRelationship cr on cr.ToConceptId = c.ConceptId
where n.NameId = @nameId

delete ca
from provider.ConceptApplication ca
inner join provider.Concept c on c.ConceptID = ca.FromConceptID 
where c.NameID = @nameId

delete ca
from provider.ConceptApplication ca
inner join provider.Concept c on c.ConceptID = ca.ToConceptID 
where c.NameID = @nameId

delete tp
from provider.TaxonProperty tp
inner join provider.Concept c on c.ConceptID = tp.ConceptID
where c.NameID = @nameId

delete tp
from provider.TaxonProperty tp
where tp.NameID = @nameId

delete provider.Concept
where NameId = @nameId

delete provider.Name
where NameId = @nameId


--update stacked names
delete provider.StackedName
where SeedNameID = @nameId

declare @ids table(id uniqueidentifier)
insert @ids
select distinct SeedNameID from provider.StackedName where NameID = @nameId

delete sn
from provider.StackedName sn
inner join @ids i on i.id = sn.seednameid;

WITH StackN (SeedNameID, AccordingToID, NameID, TaxonRankID, CanonicalName, RankName, SortOrder, Depth) AS 
( 
    SELECT n.NameID, c.AccordingToReferenceID, n.NameID, tr.TaxonRankID, np.Value, tr.Name, tr.SortOrder, 0
    FROM provider.Name N 
		inner join @ids i on i.id = n.nameid
        inner join TaxonRank tr ON n.TaxonRankID = tr.TaxonRankID
        inner join provider.NameProperty np on np.NameID = n.NameID and np.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750' 
        inner join provider.Concept c on c.NameID = n.NameID
        inner join provider.ConceptRelationship cr on cr.FromConceptID = c.ConceptID and cr.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' and InUse = 1
        inner join provider.Concept c2 on c2.ConceptID = cr.ToConceptID        
         
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

insert provider.StackedName(SeedNameID, AccordingToID, NameID, TaxonRankID, CanonicalName, RankName, SortOrder, Depth)
SELECT distinct *
FROM StackN 
ORDER BY SeedNameID, Depth DESC 



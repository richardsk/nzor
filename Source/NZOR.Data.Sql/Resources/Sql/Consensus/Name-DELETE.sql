--update provider records
update provider.Name set ConsensusNameID = null, LinkStatus = 'Unmatched', MatchPath = null, MatchScore = null, IntegrationBatchID = null
where ConsensusNameID = @nameId

update c
set ConsensusConceptID = null, c.LinkStatus = 'Unmatched', c.MatchScore = null
from provider.Concept c
inner join provider.Name n on n.nameid = c.nameid
where ConsensusNameID = @nameId

update tp
set ConsensusTaxonPropertyID = null, tp.LinkStatus = 'Unmatched', tp.MatchScore = null
from provider.TaxonProperty tp
inner join provider.Name n on n.NameID = tp.NameID
where ConsensusNameID = @nameId

update tp
set ConsensusTaxonPropertyID = null, tp.LinkStatus = 'Unmatched', tp.MatchScore = null
from provider.TaxonProperty tp
inner join provider.Concept pc on pc.ConceptId = tp.ConceptId
inner join provider.Name n on n.NameID = pc.NameID
where ConsensusNameID = @nameId

update a
set ConsensusAnnotationID = null
from provider.Annotation a
inner join provider.Name n on n.NameID = a.NameID
where ConsensusNameID = @nameId

update a
set ConsensusAnnotationID = null
from provider.Annotation a
inner join provider.Concept pc on pc.ConceptID = a.ConceptID
inner join provider.Name n on n.NameID = pc.NameID
where ConsensusNameID = @nameId


--insert deprecated records
INSERT INTO [dbo].[Deprecated]([DeprecatedId], [Table], [OldId], [NewId], [DeprecationDate])
VALUES (newid(), 'consensus.Name', @nameId, @replacementId, getdate())

INSERT INTO [dbo].[Deprecated]([DeprecatedId], [Table], [OldId], [NewId], [DeprecationDate])
select newid(), 'consensus.Concept', ConceptId, 
	(select ConceptId from consensus.Concept where NameID = @replacementId and
		isnull(AccordingToReferenceID, '00000000-0000-0000-0000-000000000000') = isnull(c.AccordingToReferenceID, '00000000-0000-0000-0000-000000000000')),
	getdate()
from consensus.Concept c
where NameID = @nameId


--delete name data

delete consensus.NameProperty
where NameId = @nameId

delete cr
from consensus.Name n
inner join consensus.Concept c on c.NameId = n.NameId
inner join consensus.ConceptRelationship cr on cr.FromConceptId = c.ConceptId
where n.NameId = @nameId

delete cr
from consensus.Name n
inner join consensus.Concept c on c.NameId = n.NameId
inner join consensus.ConceptRelationship cr on cr.ToConceptId = c.ConceptId
where n.NameId = @nameId

delete ca
from consensus.ConceptApplication ca
inner join consensus.Concept c on c.ConceptID = ca.FromConceptID 
where c.NameID = @nameId

delete ca
from consensus.ConceptApplication ca
inner join consensus.Concept c on c.ConceptID = ca.ToConceptID 
where c.NameID = @nameId

delete tpv
from consensus.TaxonPropertyValue tpv
inner join consensus.TaxonProperty tp on tp.TaxonPropertyID = tpv.TaxonPropertyID
inner join consensus.Concept c on c.ConceptID = tp.ConceptID
where c.NameID = @nameId

delete tp
from consensus.TaxonProperty tp
inner join consensus.Concept c on c.ConceptID = tp.ConceptID
where c.NameID = @nameId

delete tpv
from consensus.TaxonPropertyValue tpv
inner join consensus.TaxonProperty tp on tp.TaxonPropertyID = tpv.TaxonPropertyID
where tp.NameID = @nameId

delete tp
from consensus.TaxonProperty tp
where tp.NameID = @nameId

delete a
from consensus.Annotation a
where a.NameID = @nameId

delete a
from consensus.Annotation a
inner join consensus.Concept c on c.ConceptID = a.ConceptID
where c.NameID = @nameId

delete consensus.Concept
where NameId = @nameId


delete consensus.Name
where NameId = @nameId


--update stacked names
delete consensus.StackedName
where SeedNameID = @nameId

declare @ids table(id uniqueidentifier)
insert @ids
select distinct SeedNameID from consensus.StackedName where NameID = @nameId

delete sn
from consensus.StackedName sn
inner join @ids i on i.id = sn.seednameid;

WITH StackN (SeedNameID, AccordingToID, NameID, TaxonRankID, CanonicalName, RankName, SortOrder, Depth) AS 
( 
    SELECT n.NameID, c.AccordingToReferenceID, n.NameID, tr.TaxonRankID, np.Value, tr.Name, tr.SortOrder, 0
    FROM consensus.Name N 
		inner join @ids i on i.id = n.nameid
        inner join TaxonRank tr ON n.TaxonRankID = tr.TaxonRankID
        inner join consensus.NameProperty np on np.NameID = n.NameID and np.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750' 
        inner join consensus.Concept c on c.NameID = n.NameID
        inner join consensus.ConceptRelationship cr on cr.FromConceptID = c.ConceptID and cr.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' and cr.IsActive = 1
        inner join consensus.Concept c2 on c2.ConceptID = cr.ToConceptID        		
     
    UNION ALL 
     
    SELECT s.SeedNameID, c.AccordingToReferenceID, n2.NameID, tr.TaxonRankID, np.Value, tr.Name, tr.SortOrder, s.Depth + 1
    FROM consensus.Name n
        inner join consensus.Concept c on c.NameID = n.NameID
        inner join consensus.ConceptRelationship cr on cr.FromConceptID = c.ConceptID and cr.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' and cr.IsActive = 1
        inner join consensus.Concept c2 on c2.ConceptID = cr.ToConceptID
        inner join consensus.Name n2 on n2.NameID = c2.NameID and n2.NameID <> n.NameID
        inner join TaxonRank tr ON n2.TaxonRankID = tr.TaxonRankID
        inner join consensus.NameProperty np on np.NameID = n2.NameID and np.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750' 
        inner join StackN s on s.NameID = n.NameID
) 

insert consensus.StackedName(SeedNameID, AccordingToID, NameID, TaxonRankID, CanonicalName, RankName, SortOrder, Depth)
SELECT distinct *
FROM StackN 
ORDER BY SeedNameID, Depth DESC 

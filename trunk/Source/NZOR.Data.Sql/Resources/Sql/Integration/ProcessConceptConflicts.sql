declare @ids table(nameId uniqueidentifier, crId1 uniqueidentifier, cr2Id uniqueidentifier, relType uniqueidentifier, nameToId1 uniqueidentifier, nameToId2 uniqueidentifier, nameToKeep uniqueidentifier)

insert @ids
select cc.NameID, cc.ConceptRelationshipID, cc2.ConceptRelationshipID, cc.ConceptRelationshipTypeID, cc.NameToID, cc2.NameToID, null
from  consensus.vwConcepts cc 
inner join consensus.vwConcepts cc2 on cc.NameID = cc2.NameID and cc.NameToID <> cc2.NameToID
	and cc.ConceptRelationshipTypeID = cc2.ConceptRelationshipTypeID and cc2.IsActive = 1 and cc.ConceptRelationshipID <> cc2.ConceptRelationshipID
where cc.IsActive = 1
	
update i
set nameToKeep = (select top 1 acn.NameID
		from consensus.Name cn
		inner join consensus.StackedName sn on sn.SeedNameID = cn.NameID
		inner join consensus.Name acn on acn.NameID = sn.NameID
		inner join [admin].AttachmentPoint ap on ap.ConsensusNameID = acn.NameID
		inner join [admin].AttachmentPointDataSource apd on apd.AttachmentPointID = ap.AttachmentPointID
		inner join provider.Name pn on pn.DataSourceID = apd.DataSourceID and pn.ConsensusNameID = acn.NameID		
		inner join [admin].DataSource ds on ds.DataSourceID = apd.DataSourceID		
		where cn.NameID = i.nameId
		order by depth, apd.Ranking) 
from @ids i

update cr
set cr.IsActive = 0
from @ids i
inner join consensus.vwConcepts c on c.NameID = i.nameId and c.ConceptRelationshipTypeID = i.relType 
	and c.NameToID <> i.nameToKeep
inner join consensus.ConceptRelationship cr on cr.ConceptRelationshipID = c.ConceptRelationshipID
where i.nameToKeep is not null


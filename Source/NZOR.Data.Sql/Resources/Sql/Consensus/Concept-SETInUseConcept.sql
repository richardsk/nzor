update cr
set IsActive = 0, ModifiedDate = getdate()
from consensus.ConceptRelationship cr
inner join consensus.Concept c on c.ConceptID = cr.FromConceptID
where NameID = @nameId
	and ConceptRelationshipTypeId = @conceptRelationshipTypeId
	and IsActive = 1
	

update cr
set IsActive = 1, ModifiedDate = getdate()
from consensus.ConceptRelationship cr
inner join consensus.Concept c on c.ConceptID = cr.FromConceptID
left join consensus.Concept cto on cto.ConceptID = cr.ToConceptID
where c.NameID = @nameId
	and ConceptRelationshipTypeId = @conceptRelationshipTypeId
	and cto.NameID = @toNameId
	and c.AccordingToReferenceId = @accordingToId

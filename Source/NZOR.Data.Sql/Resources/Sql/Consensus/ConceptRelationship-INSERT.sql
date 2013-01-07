
--only one concept relationship per name + relationship type can be active
if (@isActive = 1)
begin
	declare @nameId uniqueidentifier
	select @nameId = NameId from consensus.Concept where ConceptID = @fromConceptID

	update cr
	set cr.IsActive = 0
	from consensus.ConceptRelationship cr
	inner join consensus.Concept c on c.ConceptID = cr.FromConceptID
	where c.NameID = @nameID and cr.ConceptRelationshipTypeID = @ConceptRelationshipTypeID 
end


INSERT INTO
	consensus.ConceptRelationship
	(
	ConceptRelationshipID, 
	
	FromConceptID, 
	ToConceptID, 
	ConceptRelationshipTypeID, 
	
	IsActive,
	Sequence
	)
VALUES
	(
	@ConceptRelationshipID, 
	
	@FromConceptID, 
	@ToConceptID, 
	@ConceptRelationshipTypeID, 
	
	@isActive,
	@Sequence
	)

CREATE INDEX [idxConceptRelationshipTypeID]
	ON [consensus].[ConceptRelationship] ([ConceptRelationshipTypeID],[IsActive])
	INCLUDE ([FromConceptID],[ToConceptID])


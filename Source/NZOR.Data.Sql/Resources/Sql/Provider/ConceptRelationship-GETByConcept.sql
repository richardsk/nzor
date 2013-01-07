SELECT ConceptRelationshipID
      ,FromConceptID
      ,ToConceptID
      ,ConceptRelationshipTypeID
      ,Sequence
	  ,InUse
From provider.ConceptRelationship
where FromConceptId = @conceptId
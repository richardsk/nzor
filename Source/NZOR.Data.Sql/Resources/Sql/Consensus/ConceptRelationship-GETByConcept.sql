SELECT cr.[ConceptRelationshipID]
      ,cr.[FromConceptID]
      ,cr.[ToConceptID]
      ,cr.[ConceptRelationshipTypeID]
	  ,nto.NameID as NameToID
	  ,nto.FullName as NameTo
	  ,cr.IsActive
      ,cr.[Sequence]
      ,cr.[AddedDate]
      ,cr.[ModifiedDate]
	  ,crt.Relationship as ConceptRelationshipType
From consensus.ConceptRelationship cr
inner join dbo.ConceptRelationshipType crt on crt.ConceptRelationshipTypeID = cr.ConceptRelationshipTypeID
inner join consensus.Concept cto on cto.ConceptID = cr.ToConceptID
inner join consensus.Name nto on nto.NameID = cto.NameID
where FromConceptId = @conceptId
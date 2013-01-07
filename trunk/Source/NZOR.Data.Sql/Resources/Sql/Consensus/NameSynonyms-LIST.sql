SELECT distinct n.[NameID]
      ,n.[TaxonRankID]
      ,n.[NameClassID]
      ,n.[FullName]
      ,n.[GoverningCode]
	  ,n.IsRecombination
      ,n.[AddedDate]
      ,n.[ModifiedDate]
FROM consensus.Name n
inner join consensus.Concept c on c.NameID = n.NameID 
inner join consensus.ConceptRelationship cr on cr.FromConceptID = c.ConceptID and IsActive = 1 and cr.ConceptRelationshipTypeID = '0CA79AB3-E213-4F51-88B9-4CE01F735A1D'
inner join consensus.Concept cto on cto.ConceptID = cr.ToConceptID
WHERE cto.NameID = @nameId or c.NameID = @nameId
order by n.FullName
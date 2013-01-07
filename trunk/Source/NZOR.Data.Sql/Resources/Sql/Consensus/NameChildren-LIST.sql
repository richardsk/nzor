SELECT n.[NameID]
      ,n.[TaxonRankID]
      ,n.[NameClassID]
      ,n.[FullName]
      ,n.[GoverningCode]
	  ,n.IsRecombination
      ,n.[AddedDate]
      ,n.[ModifiedDate]
FROM consensus.Name n
inner join consensus.Concept c on c.NameID = n.NameID 
inner join consensus.ConceptRelationship cr on cr.FromConceptID = c.ConceptID and IsActive = 1 and cr.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155'
inner join consensus.Concept cto on cto.ConceptID = cr.ToConceptID
WHERE cto.NameID = @nameId
order by n.FullName
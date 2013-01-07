SELECT n.[NameID],
      n.[TaxonRankID],
      n.[NameClassID],
      n.[FullName],
      n.[GoverningCode],
      n.[AddedDate],
      n.[ModifiedDate],
	  tr.Name as TaxonRank,
	  nc.Name as NameClass,
	  an.NameToId as AcceptedNameId,
	  an.NameToFull as AcceptedName,
	  an.AccordingToReference as AcceptedNameAccordingTo
FROM consensus.Name n
inner join dbo.TaxonRank tr on tr.TaxonRankId = n.TaxonRankId
inner join dbo.NameClass nc on nc.NameClassId = n.NameClassId
left join consensus.vwConcepts an on an.NameId = n.NameId and an.IsActive = 1 and an.ConceptRelationshipTypeId = '0CA79AB3-E213-4F51-88B9-4CE01F735A1D'
WHERE n.FullName like '%' + @searchText + '%'
order by n.FullName

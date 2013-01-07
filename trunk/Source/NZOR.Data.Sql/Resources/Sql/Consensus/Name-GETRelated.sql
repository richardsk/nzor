SELECT distinct cn.[NameID]
      ,cn.[TaxonRankID]
      ,cn.[NameClassID]
      ,cn.[FullName]
      ,cn.[GoverningCode]
	  ,cn.IsRecombination
      ,cn.[AddedDate]
      ,cn.[ModifiedDate]
FROM consensus.Name cn
inner join consensus.vwConcepts c on c.NameID = cn.NameID
WHERE c.NameToID = @nameId


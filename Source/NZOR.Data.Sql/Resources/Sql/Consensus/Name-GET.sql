SELECT [NameID]
      ,[TaxonRankID]
      ,[NameClassID]
      ,[FullName]
      ,[GoverningCode]
	  ,IsRecombination
      ,[AddedDate]
      ,[ModifiedDate]
FROM consensus.Name
WHERE NameID = @nameId


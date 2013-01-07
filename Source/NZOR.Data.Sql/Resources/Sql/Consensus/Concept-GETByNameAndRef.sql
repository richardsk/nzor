select [ConceptID]
           ,[NameID]
           ,[AccordingToReferenceID]
           ,[Orthography]
           ,[TaxonRank]
           ,[HigherClassification]
           ,[AddedDate]
           ,[ModifiedDate]
from consensus.Concept
where NameId = @nameId and ((AccordingToReferenceId is null and @accordingToId is null) or (AccordingToReferenceId = @accordingToId))
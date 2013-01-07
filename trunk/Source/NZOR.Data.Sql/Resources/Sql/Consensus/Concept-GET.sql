select [ConceptID]
           ,[NameID]
           ,[AccordingToReferenceID]
           ,[Orthography]
           ,[TaxonRank]
           ,[HigherClassification]
           ,[AddedDate]
           ,[ModifiedDate]
from consensus.Concept
where ConceptId = @conceptId
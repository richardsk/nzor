select c.[ConceptID]
        ,c.[NameID]
        ,c.[AccordingToReferenceID]
        ,c.[Orthography]
        ,c.[TaxonRank]
        ,c.[HigherClassification]
        ,c.[AddedDate]
        ,c.[ModifiedDate]
from consensus.Concept c
inner join provider.Concept pc on pc.consensusconceptid = c.conceptid
where pc.conceptId = @providerConceptId

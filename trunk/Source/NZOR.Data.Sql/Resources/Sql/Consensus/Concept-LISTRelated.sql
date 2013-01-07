select cc.[ConceptID]
           ,cc.[NameID]
           ,cc.[AccordingToReferenceID]
           ,cc.[Orthography]
           ,cc.[TaxonRank]
           ,cc.[HigherClassification]
           ,cc.[AddedDate]
           ,cc.[ModifiedDate]
from consensus.Concept cc
inner join provider.Concept pc on pc.consensusconceptid = cc.conceptid
inner join provider.ConceptRelationship pcr on pcr.fromconceptid = pc.conceptid
inner join provider.Concept pc2 on pc2.conceptid = pcr.toconceptid
where pc2.NameId = @providerNameId and ((pc2.AccordingToReferenceId is null and @providerAccordingToId is null) or (pc2.AccordingToReferenceId = @providerAccordingToId))
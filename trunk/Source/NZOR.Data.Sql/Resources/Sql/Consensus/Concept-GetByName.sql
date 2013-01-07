declare @ids table(id uniqueidentifier, updated datetime, created datetime)

insert @ids
select cc.ConceptID, providermodifieddate, providercreateddate
from consensus.Concept cc
left join provider.concept pc on pc.consensusconceptid = cc.conceptid
where cc.NameId = @nameId
order by isnull(providermodifieddate, providercreateddate)

select distinct cc.[ConceptID]
           ,cc.[NameID]
           ,cc.[AccordingToReferenceID]
           ,cc.[Orthography]
           ,cc.[TaxonRank]
           ,cc.[HigherClassification]
           ,cc.[AddedDate]
           ,cc.[ModifiedDate]
from @ids id 
inner join consensus.Concept cc on id.id = cc.ConceptID

select
	(select count(nameid) from consensus.name) as NZORNameCount,
	(select count(conceptid) from consensus.concept) as NZORConceptCount,
	(select count(referenceid) from consensus.reference) as NZORReferenceCount,
	(select COUNT(distinct nameid) from consensus.vwConcepts 
		where ConceptRelationshipTypeID = '0CA79AB3-E213-4F51-88B9-4CE01F735A1D' and NameID = NameToID) as NZORAcceptedNameCount,
	(select COUNT(distinct nameid) from consensus.vwConcepts 
		where ConceptRelationshipTypeID = '0CA79AB3-E213-4F51-88B9-4CE01F735A1D' and NameID <> NameToID) as NZORSynonymCount,
	(select COUNT(nameid) from consensus.Name where NameClassID = '05BCC19C-27E8-492C-8ADD-EC5F73325BC5') as VernacularNameCount,
	(select COUNT(distinct isnull(tp.nameid, c.NameID)) from consensus.TaxonProperty tp
		left join consensus.Name n on n.NameID = tp.NameID
		left join consensus.Concept c on c.ConceptID = tp.ConceptID
		left join consensus.TaxonPropertyValue gtpv on gtpv.TaxonPropertyID = tp.TaxonPropertyID and gtpv.TaxonPropertyTypeID = 'BABCDC8B-E40B-43A8-B6F6-88C97B9197A0'			
		inner join consensus.TaxonPropertyValue tpv on tpv.TaxonPropertyID = tp.TaxonPropertyID and tpv.Value like '%present%'
		where (gtpv.TaxonPropertyValueID is null or gtpv.Value = 'New Zealand')) as PresentInNZCount
		
	declare @presentCount table(SpeciesGroup nvarchar(100), TotalCount int, PresentCount int, IndigenousCount int, EndemicCount int, IntroducedCount int, NoBiostatusCount int)

	insert @presentCount
	exec consensus.sprSelectKingdomStats 'Plantae'
	
	insert @presentCount
	exec consensus.sprSelectKingdomStats 'Fungi'
	
	insert @presentCount
	exec consensus.sprSelectKingdomStats 'Animalia'
	
	insert @presentCount
	exec consensus.sprSelectKingdomStats 'Bacteria'
	
	insert @presentCount
	exec consensus.sprSelectKingdomStats 'Viroid'
	
	insert @presentCount
	exec consensus.sprSelectKingdomStats 'Chromista'
	
	insert @presentCount
	exec consensus.sprSelectKingdomStats 'Virus'
	
	insert @presentCount
	exec consensus.sprSelectKingdomStats 'Mollicute'
	
	insert @presentCount
	exec consensus.sprSelectKingdomStats 'Protozoa'
	
	select * from @presentCount


	
select count(nameid) as ProviderNameCount, ds.Name
from provider.Name n
inner join [admin].DataSource ds on ds.DataSourceID = n.DataSourceID
group by ds.Name

select COUNT(isnull(n.nameid, c.nameid)) as NameCount, replace(tpv.Value,'?','') as Biome
from consensus.TaxonProperty tp
left join consensus.Name n on n.NameID = tp.NameID
left join consensus.Concept c on c.ConceptID = tp.ConceptID
left join consensus.TaxonPropertyValue tpv on tpv.TaxonPropertyID = tp.TaxonPropertyID 
	and (tpv.TaxonPropertyTypeID = '10B3F77E-1A0B-48E5-AD62-BAF86BA0D02D' or tpv.TaxonPropertyTypeID = '9233FE71-4C27-4CE0-86B9-F1B2B392658E')
group by replace(tpv.Value,'?','')

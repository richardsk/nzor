declare @dt datetime
set @dt = GETDATE()

insert [admin].Statistic
values
	(newid(), (select StatisticTypeId from [admin].StatisticType where Name = 'NZORNameCount'), (select count(nameid) from consensus.name), @dt),
	(newid(), (select StatisticTypeId from [admin].StatisticType where Name = 'NZORConceptCount'), (select count(conceptid) from consensus.concept), @dt),
	(newid(), (select StatisticTypeId from [admin].StatisticType where Name = 'NZORReferenceCount'), (select count(referenceid) from consensus.reference), @dt) ,
	(newid(), (select StatisticTypeId from [admin].StatisticType where Name = 'NZORAcceptedNameCount'), (select COUNT(distinct nameid) from consensus.vwConcepts 
		where ConceptRelationshipTypeID = '0CA79AB3-E213-4F51-88B9-4CE01F735A1D' and NameID = NameToID), @dt),
	(newid(), (select StatisticTypeId from [admin].StatisticType where Name = 'NZORSynonymCount'), (select COUNT(distinct nameid) from consensus.vwConcepts 
		where ConceptRelationshipTypeID = '0CA79AB3-E213-4F51-88B9-4CE01F735A1D' and NameID <> NameToID), @dt) ,
	(newid(), (select StatisticTypeId from [admin].StatisticType where Name = 'VernacularNameCount'), (select COUNT(nameid) from consensus.Name where NameClassID = '05BCC19C-27E8-492C-8ADD-EC5F73325BC5'), @dt) ,
	(newid(), (select StatisticTypeId from [admin].StatisticType where Name = 'PresentInNZCount'), (select COUNT(distinct isnull(tp.nameid, c.NameID)) from consensus.TaxonProperty tp
		left join consensus.Name n on n.NameID = tp.NameID
		left join consensus.Concept c on c.ConceptID = tp.ConceptID
		left join consensus.TaxonPropertyValue gtpv on gtpv.TaxonPropertyID = tp.TaxonPropertyID and gtpv.TaxonPropertyTypeID = 'BABCDC8B-E40B-43A8-B6F6-88C97B9197A0'			
		inner join consensus.TaxonPropertyValue tpv on tpv.TaxonPropertyID = tp.TaxonPropertyID and tpv.Value like '%present%'
		where (gtpv.TaxonPropertyValueID is null or gtpv.Value = 'New Zealand')), @dt)


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
	
	
	insert [admin].Statistic
	select NEWID(), (select StatisticTypeId from [admin].StatisticType where Name = SpeciesGroup + 'SpeciesCount'), TotalCount, @dt
	from @presentCount
	
	insert [admin].Statistic
	select NEWID(), (select StatisticTypeId from [admin].StatisticType where Name = SpeciesGroup + 'SpeciesPresentCount'), PresentCount, @dt
	from @presentCount
	
	insert [admin].Statistic
	select NEWID(), (select StatisticTypeId from [admin].StatisticType where Name = SpeciesGroup + 'SpeciesIndigenousCount'), IndigenousCount, @dt
	from @presentCount
	
	insert [admin].Statistic
	select NEWID(), (select StatisticTypeId from [admin].StatisticType where Name = SpeciesGroup + 'SpeciesEndemicCount'), EndemicCount, @dt
	from @presentCount
	
	insert [admin].Statistic
	select NEWID(), (select StatisticTypeId from [admin].StatisticType where Name = SpeciesGroup + 'SpeciesIntroducedCount'), IntroducedCount, @dt
	from @presentCount
	
	insert [admin].Statistic
	select NEWID(), (select StatisticTypeId from [admin].StatisticType where Name = SpeciesGroup + 'SpeciesNoBiostatusCount'), NoBiostatusCount, @dt
	from @presentCount
		

insert [admin].Statistic
select NEWID(), (select StatisticTypeId from [admin].StatisticType where Name = 'ProviderNameCount' + pn.Code), pn.ProviderNameCount, @dt
from (select count(nameid) as ProviderNameCount, p.Code
	from provider.Name n
	inner join [admin].DataSource ds on ds.DataSourceID = n.DataSourceID
	inner join [admin].Provider p on p.ProviderID = ds.ProviderID
	group by p.Code) pn
where charindex('_test', pn.Code) = 0


insert [admin].Statistic
select NEWID(), (select StatisticTypeId from [admin].StatisticType where Name = 'BiomeNameCount' + biome.Biome collate Latin1_General_CI_AI), biome.NameCount, @dt
from (select COUNT(isnull(n.nameid, c.nameid)) as NameCount, replace(tpv.Value,'?','') as Biome
	from consensus.TaxonProperty tp
	left join consensus.Name n on n.NameID = tp.NameID
	left join consensus.Concept c on c.ConceptID = tp.ConceptID
	inner join consensus.TaxonPropertyValue tpv on tpv.TaxonPropertyID = tp.TaxonPropertyID 
		and (tpv.TaxonPropertyTypeID = '10B3F77E-1A0B-48E5-AD62-BAF86BA0D02D' or tpv.TaxonPropertyTypeID = '9233FE71-4C27-4CE0-86B9-F1B2B392658E')
	group by replace(tpv.Value,'?','')) biome
where exists(select StatisticTypeId from [admin].StatisticType where Name = 'BiomeNameCount' + biome.Biome collate Latin1_General_CI_AI)


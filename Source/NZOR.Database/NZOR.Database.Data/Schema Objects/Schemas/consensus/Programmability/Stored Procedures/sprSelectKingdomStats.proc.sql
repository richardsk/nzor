CREATE PROCEDURE consensus.[sprSelectKingdomStats]
	@kingdom nvarchar(250)
AS

	select @kingdom,
	(select COUNT(distinct n.nameid) from consensus.Name n
		inner join consensus.StackedName s on n.NameID = s.SeedNameID
		where s.CanonicalName = @kingdom and n.TaxonRankID ='C21BB221-5291-4540-94D1-55A12D1BD0AD') as TotalCount,
	(select count(distinct x.NameID) from (select n.NameID from consensus.StackedName s
			inner join consensus.StackedName s2 on s2.NameID = s.SeedNameID 
			inner join consensus.Name n on n.NameID = s2.NameID
			inner join consensus.TaxonProperty tp on tp.NameID = n.NameID
			inner join consensus.TaxonPropertyValue tpv on tpv.TaxonPropertyID = tp.TaxonPropertyID
			left join consensus.TaxonPropertyValue gtpv on gtpv.TaxonPropertyID = tp.TaxonPropertyID and gtpv.TaxonPropertyTypeID = 'BABCDC8B-E40B-43A8-B6F6-88C97B9197A0'			
			where s.CanonicalName = @kingdom and s2.SortOrder = 4200 and tpv.Value like '%present%' and (gtpv.TaxonPropertyValueID is null or gtpv.Value = 'New Zealand') 
			union all
			select n.NameID from consensus.TaxonProperty tp 
			inner join consensus.Concept c on c.ConceptID = tp.ConceptID
			inner join consensus.StackedName s on s.SeedNameID = c.NameID
			inner join consensus.Name n on n.NameID = c.NameID
			inner join consensus.TaxonPropertyValue tpv on tpv.TaxonPropertyID = tp.TaxonPropertyID	
			left join consensus.TaxonPropertyValue gtpv on gtpv.TaxonPropertyID = tp.TaxonPropertyID and gtpv.TaxonPropertyTypeID = 'BABCDC8B-E40B-43A8-B6F6-88C97B9197A0'					
			where s.CanonicalName = @kingdom and n.TaxonRankID ='C21BB221-5291-4540-94D1-55A12D1BD0AD' and tpv.Value like '%present%'
				and (gtpv.TaxonPropertyValueID is null or gtpv.Value = 'New Zealand') ) x) as PresentCount,
	(select count(distinct x.NameID) from (select n.NameID from consensus.StackedName s
			inner join consensus.StackedName s2 on s2.NameID = s.SeedNameID 
			inner join consensus.Name n on n.NameID = s2.NameID
			inner join consensus.TaxonProperty tp on tp.NameID = n.NameID
			inner join consensus.TaxonPropertyValue tpv on tpv.TaxonPropertyID = tp.TaxonPropertyID		
			left join consensus.TaxonPropertyValue gtpv on gtpv.TaxonPropertyID = tp.TaxonPropertyID and gtpv.TaxonPropertyTypeID = 'BABCDC8B-E40B-43A8-B6F6-88C97B9197A0'					
			where s.CanonicalName = @kingdom and s2.SortOrder = 4200 and (tpv.Value = 'indigenous' or tpv.Value like 'endemic') and (gtpv.TaxonPropertyValueID is null or gtpv.Value = 'New Zealand') 
			union all
			select n.NameID from consensus.TaxonProperty tp 
			inner join consensus.Concept c on c.ConceptID = tp.ConceptID
			inner join consensus.StackedName s on s.SeedNameID = c.NameID
			inner join consensus.Name n on n.NameID = c.NameID
			inner join consensus.TaxonPropertyValue tpv on tpv.TaxonPropertyID = tp.TaxonPropertyID			
			left join consensus.TaxonPropertyValue gtpv on gtpv.TaxonPropertyID = tp.TaxonPropertyID and gtpv.TaxonPropertyTypeID = 'BABCDC8B-E40B-43A8-B6F6-88C97B9197A0'			
			where s.CanonicalName = @kingdom and n.TaxonRankID ='C21BB221-5291-4540-94D1-55A12D1BD0AD' and (tpv.Value = 'indigenous' or tpv.Value like 'endemic')
				and (gtpv.TaxonPropertyValueID is null or gtpv.Value = 'New Zealand') ) x) as IndigenousCount,
	(select count(distinct x.NameID) from (select n.NameID from consensus.StackedName s
			inner join consensus.StackedName s2 on s2.NameID = s.SeedNameID 
			inner join consensus.Name n on n.NameID = s2.NameID
			inner join consensus.TaxonProperty tp on tp.NameID = n.NameID
			inner join consensus.TaxonPropertyValue tpv on tpv.TaxonPropertyID = tp.TaxonPropertyID				
			left join consensus.TaxonPropertyValue gtpv on gtpv.TaxonPropertyID = tp.TaxonPropertyID and gtpv.TaxonPropertyTypeID = 'BABCDC8B-E40B-43A8-B6F6-88C97B9197A0'			
			where s.CanonicalName = @kingdom and s2.SortOrder = 4200 and tpv.Value = 'endemic' and (gtpv.TaxonPropertyValueID is null or gtpv.Value = 'New Zealand') 
			union all
			select n.NameID from consensus.TaxonProperty tp 
			inner join consensus.Concept c on c.ConceptID = tp.ConceptID
			inner join consensus.StackedName s on s.SeedNameID = c.NameID
			inner join consensus.Name n on n.NameID = c.NameID
			inner join consensus.TaxonPropertyValue tpv on tpv.TaxonPropertyID = tp.TaxonPropertyID			
			left join consensus.TaxonPropertyValue gtpv on gtpv.TaxonPropertyID = tp.TaxonPropertyID and gtpv.TaxonPropertyTypeID = 'BABCDC8B-E40B-43A8-B6F6-88C97B9197A0'			
			where s.CanonicalName = @kingdom and n.TaxonRankID ='C21BB221-5291-4540-94D1-55A12D1BD0AD' 
				and tpv.Value = 'endemic' and (gtpv.TaxonPropertyValueID is null or gtpv.Value = 'New Zealand') ) x) as EndemicCount,
	(select count(distinct x.NameID) from (select n.NameID from consensus.StackedName s
			inner join consensus.StackedName s2 on s2.NameID = s.SeedNameID 
			inner join consensus.Name n on n.NameID = s2.NameID
			inner join consensus.TaxonProperty tp on tp.NameID = n.NameID
			inner join consensus.TaxonPropertyValue tpv on tpv.TaxonPropertyID = tp.TaxonPropertyID				
			left join consensus.TaxonPropertyValue gtpv on gtpv.TaxonPropertyID = tp.TaxonPropertyID and gtpv.TaxonPropertyTypeID = 'BABCDC8B-E40B-43A8-B6F6-88C97B9197A0'			
			where s.CanonicalName = @kingdom and s2.SortOrder = 4200 and tpv.Value <> 'indigenous' and tpv.Value <> 'endemic' and (gtpv.TaxonPropertyValueID is null or gtpv.Value = 'New Zealand') 
				and tpv.TaxonPropertyTypeID = 'D955AD6E-4678-4AC9-B752-6A94F1C07080'
			union all
			select n.NameID from consensus.TaxonProperty tp 
			inner join consensus.Concept c on c.ConceptID = tp.ConceptID
			inner join consensus.StackedName s on s.SeedNameID = c.NameID
			inner join consensus.Name n on n.NameID = c.NameID
			inner join consensus.TaxonPropertyValue tpv on tpv.TaxonPropertyID = tp.TaxonPropertyID			
			left join consensus.TaxonPropertyValue gtpv on gtpv.TaxonPropertyID = tp.TaxonPropertyID and gtpv.TaxonPropertyTypeID = 'BABCDC8B-E40B-43A8-B6F6-88C97B9197A0'			
			where s.CanonicalName = @kingdom and n.TaxonRankID ='C21BB221-5291-4540-94D1-55A12D1BD0AD' 
				and tpv.TaxonPropertyTypeID = 'D955AD6E-4678-4AC9-B752-6A94F1C07080' and tpv.Value <> 'indigenous' and tpv.Value <> 'endemic' and (gtpv.TaxonPropertyValueID is null or gtpv.Value = 'New Zealand') ) x) as IntroducedCount,
	(select COUNT(distinct s.seednameid) from consensus.StackedName s
			inner join consensus.StackedName s2 on s2.NameID = s.SeedNameID 
			where s.CanonicalName = @kingdom and s2.SortOrder = 4200 and 
			not exists(select tp.taxonpropertyid
				from consensus.Name n 				
				inner join consensus.TaxonProperty tp on tp.NameID = n.NameID and tp.TaxonPropertyClassID = 'CFA152D5-831C-4A4E-BA4F-50F9F18E7B70'				
				where n.NameID = s2.NameID and tp.TaxonPropertyID is not null)
			and not exists(select tp.taxonpropertyid
				from consensus.Name n 				
				inner join consensus.Concept c on c.NameID = n.NameID
				inner join consensus.TaxonProperty tp on tp.ConceptID = c.ConceptID and tp.TaxonPropertyClassID = 'CFA152D5-831C-4A4E-BA4F-50F9F18E7B70'				
				where n.NameID = s2.NameID and tp.TaxonPropertyID is not null)) as NoBiostatusCount

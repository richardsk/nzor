select  --DataSet
	(Select --Metadata
		(Select '6609806A-6967-4D83-A4FE-E8609EBE49B0' "@id", --Provider todo ?? ...
				'NZIB' "Name",
				'' "OrganisationURL"
		for xml path('Provider'), type),
		getdate() "DateGenerated"
	for xml path('Metadata'), TYPE),
	(select 'D0A6F62D-1434-4945-B852-01944E12FB3F' "@id", --DataSource TODO
		(Select '' Disclaimer, --Usage
			'' Attribution,
			'' Licensing			
		for xml Path ('Usage'), type),	
		
		(select --Publications
			(select r.ID "@id",  --Publication
				-- TODO ParentPublicationId "@parentPublicationRef",
				'Book' "@type", --todo
				GETDATE() "@createdDate", --todo
				getdate() "@modifiedDate", --todo
				replace(r.Authors, char(13) + CHAR(10),'') + ' : ' + r.Title + ', ' + r.ChapterTitle + isnull(', ' + r.ChecklistTitle,'') + isnull(', ' + r.ChecklistAuthors,'') + ' : ' + r.Editor as Citation, --todo
				replace(r.Authors, char(13) + CHAR(10),'') "Authors/Simple",
				(select --Dates
					(select 'of publication' "@type",
						coalesce(getdate(),'') [text()] --todo
						from tblreference where id = r.id
							--and DateOfPublication is not null
					 for XML path('Date'), type)
					--(select 'on publication' "@type",
					--	coalesce(DateOnPublication,'') [text()]
					--	from Publication where PublicationID = p.PublicationID
					--		and DateOnPublication is not null
					--	for XML path('Date'), type)
				for xml Path('Dates'), type),
				(select
				 (select 1 "@level",
					'full' "@type",
					replace(r.Authors, char(13) + CHAR(10),'') + ' : ' + r.Title + ', ' + r.ChapterTitle + isnull(', ' + r.ChecklistTitle,'') + isnull(', ' + r.ChecklistAuthors,'') + ' : ' + r.Editor [text()]
				  from tblReference where
						ID = r.ID
				  for xml path('Title'), type)
				for XML path('Titles'), type), --TODO ???
				Editor "Editors/Simple",
				'' Volume,
				--Issue, --TODO ??
				--Edition,
				--case isnull(PageStart + PageEnd + PageTotal, '')
				--	when '' then null
				--	else
				--(select 
				--	(Select 'start' "@type",
				--	   coalesce(PageStart,'') [text()]
				--	   from Publication where PublicationID = p.PublicationID
				--			and PageStart is not null
				--			for xml path('Page'), type),
				--	(Select 'end' "@type",
				--	   coalesce(PageEnd,'') [text()]
				--	   from Publication where PublicationID = p.PublicationID
				--			and PageEnd is not null
				--			for xml path('Page'), type),
				--	(Select 'total' "@type",
				--	   coalesce(PageTotal,'') [text()]
				--	   from Publication where PublicationID = p.PublicationID
				--			and PageTotal is not null
				--			for xml path('Page'), type)
				--	for xml path('Pages'), type) end,
				coalesce('xxx','') "Publisher/Name", --todo
				coalesce('Wellington','') "Publisher/City" --todo
					
				from tblReference r
			for xml Path('Publication'), type)
		for xml path('Publications'), type),
		
		(select top 100 (  --TaxonNames
			select distinct --TaxonName
				n.GUID "@id", 
				'' "FullName",
				case when h.TaxonRank = 'epithet' then 'species' else h.TaxonRank end as Rank,
				rtrim(h.Taxon) "CanonicalName",
				n.Authors "Authorship",
				'' "BasionymAuthors",
				'' "CombinationAuthors",
				r.Authors + ' : ' + r.Title + ', ' + r.ChapterTitle + isnull(', ' + r.ChecklistTitle,'') + isnull(', ' + r.ChecklistAuthors,'') + ' : ' + r.Editor as PublishedIn,
				Year,
				'' "MicroReference",
				n.Code as NomenclaturalCode
			from [RankFillNZIBDec09-2010] n
			left join tblHierarchy h on h.TaxonGuid = n.GUID
			left join tblReference r on r.Guid = n.referenceFK
			where h.Taxon is not null and n.ID = n1.ID
			for xml path('TaxonName'), type)
		from [RankFillNZIBDec09-2010] n1
		for xml path('TaxonNames'), type),

		(select (select top 100 nc1.GUID + ':' + nc1.referenceFK "@id",
					GETDATE() "@createdDate", --todo
					GETDATE() "@modifiedDate", --todo
				(select nc1.GUID "@ref"
					from [RankFillNZIBDec09-2010] where GUID = nc1.GUID
					for xml path('Name'), type),
				(select referenceFK "@ref"
					from [RankFillNZIBDec09-2010] where GUID = nc1.GUID
					for xml path('AccordingTo'), type),
				(select null "@ref" --todo accepted name id
					from [RankFillNZIBDec09-2010] where GUID = nc1.GUID
					for xml Path('AcceptedName'), type),
				(select ParentGuid "@ref"
					from tblHierarchy where TaxonGuid = nc1.GUID
					for xml Path('ParentName'), type)
			from [RankFillNZIBDec09-2010] nc1	
			for xml path ('NameBasedConcept'), type)
		for xml path('TaxonConcepts'), type)

	for xml path('DataSource'), type)
for xml path('DataSet'), type


--select * from [RankFillNZIBDec09-2010] where Sensu is not null
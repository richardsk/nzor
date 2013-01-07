CREATE PROCEDURE consensus.[sprTransferNZORToDarwinCore]	
AS

	IF  EXISTS (select * FROM sys.foreign_keys WHERE name = N'frkDistributionTaxon')
	ALTER TABLE [darwincore].[Distribution] DROP CONSTRAINT [frkDistributionTaxon]

	IF  EXISTS (select * FROM sys.foreign_keys WHERE name = N'frkLiteratureTaxon')
	ALTER TABLE [darwincore].[Literature] DROP CONSTRAINT [frkLiteratureTaxon]

	IF  EXISTS (select * FROM sys.foreign_keys WHERE name = N'frkVernacularTaxon')
	ALTER TABLE [darwincore].[Vernacular] DROP CONSTRAINT [frkVernacularTaxon]



	truncate table darwincore.Literature
	truncate table darwincore.Vernacular
	truncate table darwincore.Distribution
	truncate table darwincore.Taxon



	ALTER TABLE [darwincore].[Vernacular]  WITH CHECK ADD  CONSTRAINT [frkVernacularTaxon] FOREIGN KEY([TaxonID])
	REFERENCES [darwincore].[Taxon] ([TaxonID])

	ALTER TABLE [darwincore].[Vernacular] CHECK CONSTRAINT [frkVernacularTaxon]

	ALTER TABLE [darwincore].[Literature]  WITH CHECK ADD  CONSTRAINT [frkLiteratureTaxon] FOREIGN KEY([TaxonID])
	REFERENCES [darwincore].[Taxon] ([TaxonID])

	ALTER TABLE [darwincore].[Literature] CHECK CONSTRAINT [frkLiteratureTaxon]

	ALTER TABLE [darwincore].[Distribution]  WITH CHECK ADD  CONSTRAINT [frkDistributionTaxon] FOREIGN KEY([TaxonID])
	REFERENCES [darwincore].[Taxon] ([TaxonID])

	ALTER TABLE [darwincore].[Distribution] CHECK CONSTRAINT [frkDistributionTaxon]


	--taxon
	create table #taxon (TaxonID uniqueidentifier, 
						NameID nvarchar(1000),
						AcceptedUsageID nvarchar(1000),
						ParentUsageID nvarchar(1000),
						OriginalUsageID nvarchar(1000),
						AccordingToID nvarchar(1000),
						PublishedInID nvarchar(1000),
						ConceptID nvarchar(1000),
						ScientificName nvarchar(2000),
						AcceptedName nvarchar(2000),
						ParentName nvarchar(2000),
						OriginalName nvarchar(2000),
						AccordingTo nvarchar(2000),
						PublishedIn nvarchar(2000),					
						HigherClassification nvarchar(2000),
						Kingdom nvarchar(1000),
						Phylum nvarchar(1000),
						Class nvarchar(1000),
						[Order] nvarchar(1000),
						Family nvarchar(1000),
						Genus nvarchar(1000),
						SubGenus nvarchar(1000),
						SpecificEpithet nvarchar(1000),
						InfraspecificEpithet nvarchar(1000),
						TaxonRank nvarchar(1000),
						VerbatimRank nvarchar(1000),
						Authors nvarchar(1000),
						Vernacular nvarchar(1000),
						NomCode nvarchar(1000),
						TaxonomicStatus nvarchar(1000),
						NomenclaturalStatus nvarchar(1000),
						TaxonRemarks nvarchar(2000),
						Modified datetime,
						Rights nvarchar(2000),
						RightsHolder nvarchar(1000),
						AccessRights nvarchar(2000),
						BibCitation nvarchar(2000),
						InfoWithheld nvarchar(2000),
						DatasetID nvarchar(1000),
						DatasetName nvarchar(1000),
						Source nvarchar(1000))

	insert #taxon(TaxonID, NameID, AccordingToID, AccordingTo, ConceptID, TaxonRank, Vernacular, NomCode, TaxonRemarks, Modified, Rights, RightsHolder, AccessRights, BibCitation, InfoWithheld, Source)
	select distinct c.ConceptID,
		--n.partialname as ScientificName, --substring(n.FullName, 1, len(n.fullname) - len(anp.value) - 1) as ScientificName,
		n.NameID,
		c.AccordingToReferenceID,
		crp.Value as AccordingToReference,
		c.ConceptID as TaxonConceptID,
		tr.Name as TaxonRank,
		'' as VernacularName,
		n.GoverningCode,
		'' as TaxonRemarks,
		n.ModifiedDate,
		'' as Rights,
		'NZOR' as RightsHolder,
		'' as AccessRights,
		'' as BibliographicCitation,
		'' as InformationWitheld,
		'http://demo.nzor.org.nz/names/' + cast(n.NameID as varchar(38)) as Source
	from consensus.Concept c
	inner join consensus.Name n on n.NameID = c.NameID
	inner join dbo.TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
	left join consensus.ReferenceProperty crp on crp.ReferenceID = c.AccordingToReferenceID and crp.ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07'
	inner join consensus.TaxonProperty tp on tp.NameID = n.NameID
	inner join consensus.TaxonPropertyValue gtp on gtp.TaxonPropertyID = tp.TaxonPropertyID and gtp.TaxonPropertyTypeID = 'BABCDC8B-E40B-43A8-B6F6-88C97B9197A0'
	inner join dbo.Country ct on ct.Country = gtp.Value
	where n.NameClassID = 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A'
	union 
	select distinct c.ConceptID,
		--n.partialname as ScientificName, --substring(n.FullName, 1, len(n.fullname) - len(anp.value) - 1) as ScientificName,
		n.NameID,
		c.AccordingToReferenceID,
		crp.Value as AccordingToReference,
		c.ConceptID as TaxonConceptID,
		tr.Name as TaxonRank,
		'' as VernacularName,
		n.GoverningCode,
		'' as TaxonRemarks,
		n.ModifiedDate,
		'' as Rights,
		'NZOR' as RightsHolder,
		'' as AccessRights,
		'' as BibliographicCitation,
		'' as InformationWitheld,
		'http://demo.nzor.org.nz/names/' + cast(n.NameID as varchar(38)) as Source
	from consensus.Concept c
	inner join consensus.Name n on n.NameID = c.NameID
	inner join dbo.TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
	left join consensus.ReferenceProperty crp on crp.ReferenceID = c.AccordingToReferenceID and crp.ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07'
	inner join consensus.TaxonProperty tp on tp.ConceptID = c.ConceptID
	inner join consensus.TaxonPropertyValue gtp on gtp.TaxonPropertyID = tp.TaxonPropertyID and gtp.TaxonPropertyTypeID = 'BABCDC8B-E40B-43A8-B6F6-88C97B9197A0'
	inner join dbo.Country ct on ct.Country = gtp.Value
	where n.NameClassID = 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A'

	update t
	set DatasetName = case when ds.dsName = '' then '' else SUBSTRING(ds.dsName, 1, len(ds.dsName) - 1) end 
	from #taxon t
	cross apply (
		select (SELECT isnull(d.Name + ';','') AS [text()] 
		FROM provider.Name n
		inner join admin.DataSource d on d.DataSourceID = n.DataSourceID
		where n.ConsensusNameID = t.NameID
		FOR XML PATH(''))
	) ds (dsName)

	update t
	set ScientificName = replace(fnp.Value, ' ', ' ')
	from #taxon t
	inner join consensus.NameProperty fnp on fnp.NameID = t.NameID and fnp.NamePropertyTypeID = '00806321-C8BD-4518-9539-1286DA02CA7D'

	update t
	set AcceptedName = replace(fnp.Value, ' ', ' '),
		AcceptedUsageID = anc.ToConceptID
	from #taxon t
	inner join consensus.ConceptRelationship anc on anc.FromConceptID = t.ConceptID and anc.ConceptRelationshipTypeID = '0CA79AB3-E213-4F51-88B9-4CE01F735A1D' and anc.IsActive = 1
	inner join consensus.Concept toc on toc.ConceptID = anc.ToConceptID
	inner join consensus.NameProperty fnp on fnp.NameID = toc.NameID and fnp.NamePropertyTypeID = '00806321-C8BD-4518-9539-1286DA02CA7D'
	inner join #taxon c on c.TaxonID = anc.ToConceptID

	update t
	set PublishedInID = pinp.RelatedID,
		PublishedIn = pinp.Value
	from #taxon t
	inner join consensus.NameProperty pinp on pinp.NameID = t.NameID and pinp.NamePropertyTypeID = 'DEDC63F0-FB2A-420B-9932-786B4347DA45'

	update t
	set ParentUsageID = pnc.ToConceptID,
		ParentName = replace(fnp.Value, ' ', ' ')
	from #taxon t
	inner join consensus.ConceptRelationship pnc on pnc.FromConceptID = t.ConceptID and pnc.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' and pnc.IsActive = 1
	inner join consensus.Concept toc on toc.ConceptID = pnc.ToConceptID
	inner join consensus.NameProperty fnp on fnp.NameID = toc.NameID and fnp.NamePropertyTypeID = '00806321-C8BD-4518-9539-1286DA02CA7D'	
	inner join #taxon c on c.TaxonID = pnc.ToConceptID


	update t
	set OriginalUsageID =  c.ConceptID,
		OriginalName =  fnp.Value
	from #taxon t
	inner join consensus.NameProperty bnp on bnp.NameID = t.NameID and bnp.NamePropertyTypeID = 'F496FBCC-8DA6-4CA1-9884-11BD9B5DF63B'
	inner join consensus.Concept c on c.NameID = bnp.RelatedID 
	inner join consensus.ConceptRelationship cr on cr.FromConceptID = c.ConceptID and cr.IsActive = 1 and cr.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155'	
	inner join consensus.NameProperty fnp on fnp.NameID = c.NameID and fnp.NamePropertyTypeID = '00806321-C8BD-4518-9539-1286DA02CA7D'	
	inner join #taxon ct on ct.TaxonID = c.ConceptID

	update t
	set HigherClassification = 
		case when h.names = '' then '' else SUBSTRING(h.names, 1, len(h.names) - 1) end
	from #taxon t
	cross apply (
		select (SELECT hn.CanonicalName  + ';' AS [text()] from (select distinct top 100 canonicalname, sortorder
								FROM consensus.StackedName sn
								where sn.SeedNameID = t.NameID and CanonicalName <> 'ROOT' and sn.NameID <> t.NameID
								order by SortOrder) hn
		FOR XML PATH(''))
	) h (names)


	update t
	set Kingdom = kn.CanonicalName
	from #taxon t
	inner join consensus.StackedName kn on kn.SeedNameID = t.NameID and kn.RankName = 'kingdom'

	update t
	set Phylum = pn.CanonicalName
	from #taxon t
	inner join consensus.StackedName pn on pn.SeedNameID = t.NameID and pn.RankName = 'phylum'

	update t
	set Class = cn.CanonicalName
	from #taxon t
	inner join consensus.StackedName cn on cn.SeedNameID = t.NameID and cn.RankName = 'class'

	update t
	set [Order] = orn.CanonicalName
	from #taxon t
	inner join consensus.StackedName orn on orn.SeedNameID = t.NameID and orn.RankName = 'order'

	update t
	set Family = fn.CanonicalName
	from #taxon t
	inner join consensus.StackedName fn on fn.SeedNameID = t.NameID and fn.RankName = 'family'

	update t
	set Genus = gn.CanonicalName
	from #taxon t
	inner join consensus.StackedName gn on gn.SeedNameID = t.NameID and gn.RankName = 'genus'

	update t
	set SubGenus = sgn.CanonicalName
	from #taxon t
	inner join consensus.StackedName sgn on sgn.SeedNameID = t.NameID and sgn.RankName = 'subgenus'

	update t
	set SpecificEpithet = spn.CanonicalName
	from #taxon t
	inner join consensus.StackedName spn on spn.SeedNameID = t.NameID and spn.RankName = 'species'

	update t
	set InfraspecificEpithet = isn.CanonicalName
	from #taxon t
	inner join consensus.StackedName isn on isn.SeedNameID = t.NameID and isn.SortOrder > 4200 and isn.Depth = 1

	update t
	set VerbatimRank = rnp.Value
	from #taxon t
	inner join consensus.NameProperty rnp on rnp.NameID = t.NameID and rnp.NamePropertyTypeID = 'A1D57520-3D64-4F7D-97C8-69B449AFA280'

	update t
	set Authors = anp.Value
	from #taxon t
	inner join consensus.NameProperty anp on anp.NameID = t.NameID and anp.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'

	update #taxon
	set TaxonomicStatus = case when AcceptedUsageID <> ConceptID then 'synonym' else 'accepted' end 
	
	update t
	set TaxonomicStatus = ';' +
		case when tnc.ToConceptID is not null then 'teleomorph' else '' end	
	from #taxon t
	inner join consensus.ConceptRelationship tnc on tnc.FromConceptID = t.ConceptID and tnc.ConceptRelationshipTypeID = '1EAA046F-61A6-4F1E-B1A7-0E1D2CE78BBF' and tnc.IsActive = 1
		
	update t
	set TaxonomicStatus = ';' +	
		case when annc.ToConceptID is not null then 'anamorph' else '' end
	from #taxon t
	inner join consensus.ConceptRelationship annc on annc.FromConceptID = t.ConceptID and annc.ConceptRelationshipTypeID = '0018A60A-3C1D-4EFA-B4B3-534B40ACD079' and annc.IsActive = 1

	update t
	set NomenclaturalStatus = nsnp.Value
	from #taxon t
	inner join consensus.NameProperty nsnp on nsnp.NameID = t.NameID and nsnp.NamePropertyTypeID = '65F94532-7687-4958-B5B8-71F54866BEAD'



	insert into darwincore.Taxon
	select *
	from #taxon



	--vernaculars
	insert into darwincore.Vernacular
	select ca.ToConceptID, 
		fnp.Value as FullName, 
		crp.Value as Source,
		isnull(lnp.Value, 'en') as [Language],
		'' as Temporal,
		'' as LocationID,
		gr.Name as Locality,
		cnp.Value as CountryCode,
		ca.Gender,
		ca.LifeStage,
		null as IsPlural,	
		'1' as InUse, --TODO
		ca.PartOfTaxon,
		'' as TaxonRemarks
	from consensus.Name n
	left join consensus.NameProperty lnp on lnp.NameID = n.NameID and lnp.NamePropertyTypeID = '2089E79F-4CD4-4CD7-AA0D-A3AD0CDFD331'
	left join consensus.NameProperty cnp on cnp.NameID = n.NameID and cnp.NamePropertyTypeID = '5F3CD05D-3BCC-4D2B-BE71-BFAC7DF9FD17'
	inner join consensus.NameProperty fnp on fnp.NameID = n.NameID and fnp.NamePropertyTypeID = '88020F95-1282-4D9A-819A-0973F7F50284' 
	inner join consensus.Concept c on n.NameID = c.NameID 
	left join consensus.ReferenceProperty crp on crp.ReferenceID = c.AccordingToReferenceID and crp.ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07'
	inner join consensus.ConceptApplication ca on ca.FromConceptID = c.ConceptID
	left join dbo.GeoRegion gr on gr.GeoRegionID = ca.GeoRegionID
	inner join darwincore.Taxon t on t.TaxonID = ca.ToConceptID
	where n.NameClassID = '05BCC19C-27E8-492C-8ADD-EC5F73325BC5' 


	--literature
	insert into darwincore.Literature
	select distinct c.ConceptID,
		irp.Value as Identifier,
		crp.Value as Citation,
		trp.Value as Title,
		case when auth.authors = '' then '' else SUBSTRING(auth.authors, 1, len(auth.authors) - 1) end as Creator,
		drp.Value as [Date],
		(select top 1 Value from consensus.ReferenceProperty where ReferenceID = parentrp.Value and ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07') as Source,
		'' as Description,
		case when kw.keywords = '' then '' else SUBSTRING(kw.keywords, 1, len(kw.keywords) - 1) end as Subject,
		'' as Rights, -- TODO
		'' as TaxonRemarks,
		'' as Type
	from consensus.Concept c
	inner join darwincore.Taxon t on t.TaxonID = c.ConceptID
	inner join consensus.Name n on n.NameID = c.NameID
	inner join consensus.Reference r on r.ReferenceID = c.AccordingToReferenceID
	inner join consensus.ReferenceProperty crp on crp.ReferenceID = c.AccordingToReferenceID and crp.ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07'
	left join consensus.ReferenceProperty parentrp on parentrp.ReferenceID = c.AccordingToReferenceID and parentrp.ReferencePropertyTypeID = '8600068C-EDE7-4F3A-A8FB-C8A9B59E8FE3'
	left join consensus.ReferenceProperty irp on irp.ReferenceID = c.AccordingToReferenceID and irp.ReferencePropertyTypeID = '79DFAD1F-DDB4-4EEB-AA63-F7A544243A69'
	left join consensus.ReferenceProperty trp on trp.ReferenceID = c.AccordingToReferenceID and trp.ReferencePropertyTypeID = '38B41B0A-E58C-4923-8A9A-031B32AD16F2'
	left join consensus.ReferenceProperty drp on drp.ReferenceID = c.AccordingToReferenceID and drp.ReferencePropertyTypeID = '8D5027BD-39F1-408D-ACEF-A728599A216F'
	cross apply (
		select (SELECT isnull(Value + ';','') AS [text()] 
		FROM consensus.ReferenceProperty rp
		where rp.ReferenceID = r.ReferenceID and rp.ReferencePropertyTypeID = '037D1485-C6C1-47D2-9619-234F743E9FF6'
		FOR XML PATH(''))
	) auth (authors)
	cross apply (
		select (SELECT isnull(Value + ';','') AS [text()] 
		FROM consensus.ReferenceProperty rp
		where rp.ReferenceID = r.ReferenceID and rp.ReferencePropertyTypeID = '36F7FCC8-E11C-4EDA-BBF3-237C5CF9E6A2'
		FOR XML PATH(''))
	) kw (keywords)
	where crp.Value <> '' and n.NameClassID = 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A'
	

	--biostatus / distribution

	INSERT INTO [darwincore].[Distribution]
			   ([TaxonID]
			   ,[LocationID]
			   ,[Locality]
			   ,[CountryCode]
			   ,[LifeStage]
			   ,[OccurrenceStatus]
			   ,[ThreatStatus]
			   ,[EstablishmentMeans]
			   ,[AppendixCITES]
			   ,[EventDate]
			   ,[StartDayOfYear]
			   ,[EndDayOfYear]
			   ,[Source]
			   ,[OccurrenceRemarks])
	select distinct t.TaxonID,
		null,
		gtp.Value as Locality,
		ct.ISOCode as CountryCode,
		null as LifeStage,
		octp.Value as Occurrence,
		case when octp.Value = 'extinct' then 'EX' else '' end as ThreatStatus,
		etp.Value as EstablishmentMeans,
		null as AppendixCITES,
		drp.Value as EventDate,
		null as StartDay,
		null as EndDay,
		'NZOR, http://www.nzor.org.nz' as Source,
		null as Remarks
	from darwincore.Taxon t
	inner join consensus.Concept c on c.ConceptID = t.TaxonID
	inner join consensus.TaxonProperty tp on tp.NameID = c.NameID
	inner join consensus.TaxonPropertyValue gtp on gtp.TaxonPropertyID = tp.TaxonPropertyID and gtp.TaxonPropertyTypeID = 'BABCDC8B-E40B-43A8-B6F6-88C97B9197A0'
	left join consensus.TaxonPropertyValue octp on octp.TaxonPropertyID = tp.TaxonPropertyID and octp.TaxonPropertyTypeID = '9BB63B14-0208-4070-A575-94F90DFD47B0'
	left join consensus.TaxonPropertyValue etp on etp.TaxonPropertyID = tp.TaxonPropertyID and etp.TaxonPropertyTypeID = 'D955AD6E-4678-4AC9-B752-6A94F1C07080'
	left join consensus.Reference r on r.ReferenceID = tp.ReferenceID 
	left join consensus.ReferenceProperty drp on drp.ReferenceID = r.ReferenceID and drp.ReferencePropertyTypeID = '8D5027BD-39F1-408D-ACEF-A728599A216F'
	inner join dbo.Country ct on ct.Country = gtp.Value
	union
	select distinct c.TaxonID,
		null,
		gtp.Value as Locality,
		ct.ISOCode as CountryCode,
		null as LifeStage,
		octp.Value as Occurrence,
		case when octp.Value = 'extinct' then 'EX' else '' end as ThreatStatus,
		etp.Value as EstablishmentMeans,
		null as AppendixCITES,
		drp.Value as EventDate,
		null as StartDay,
		null as EndDay,
		'NZOR, http://www.nzor.org.nz' as Source,
		null as Remarks
	from darwincore.Taxon c
	inner join consensus.TaxonProperty tp on tp.ConceptID = c.taxonid
	inner join consensus.TaxonPropertyValue gtp on gtp.TaxonPropertyID = tp.TaxonPropertyID and gtp.TaxonPropertyTypeID = 'BABCDC8B-E40B-43A8-B6F6-88C97B9197A0'
	left join consensus.TaxonPropertyValue octp on octp.TaxonPropertyID = tp.TaxonPropertyID and octp.TaxonPropertyTypeID = '9BB63B14-0208-4070-A575-94F90DFD47B0'
	left join consensus.TaxonPropertyValue etp on etp.TaxonPropertyID = tp.TaxonPropertyID and etp.TaxonPropertyTypeID = 'D955AD6E-4678-4AC9-B752-6A94F1C07080'
	left join consensus.Reference r on r.ReferenceID = c.NameAccordingToID 
	left join consensus.ReferenceProperty drp on drp.ReferenceID = r.ReferenceID and drp.ReferencePropertyTypeID = '8D5027BD-39F1-408D-ACEF-A728599A216F'
	inner join dbo.Country ct on ct.Country = gtp.Value


	drop table #taxon

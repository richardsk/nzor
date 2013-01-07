CREATE PROCEDURE dwc_nzib.[sprTransferNZORToDwC_nzib]	
AS

	IF  EXISTS (select * FROM sys.foreign_keys WHERE name = N'frkDistributionTaxon')
	ALTER TABLE [dwc_nzib].[Distribution] DROP CONSTRAINT [frkDistributionTaxon]

	IF  EXISTS (select * FROM sys.foreign_keys WHERE name = N'frkLiteratureTaxon')
	ALTER TABLE [dwc_nzib].[Literature] DROP CONSTRAINT [frkLiteratureTaxon]

	IF  EXISTS (select * FROM sys.foreign_keys WHERE name = N'frkVernacularTaxon')
	ALTER TABLE [dwc_nzib].[Vernacular] DROP CONSTRAINT [frkVernacularTaxon]



	truncate table dwc_nzib.Literature
	truncate table dwc_nzib.Vernacular
	truncate table dwc_nzib.Distribution
	truncate table dwc_nzib.Taxon



	ALTER TABLE [dwc_nzib].[Vernacular]  WITH CHECK ADD  CONSTRAINT [frkVernacularTaxon] FOREIGN KEY([TaxonID])
	REFERENCES [dwc_nzib].[Taxon] ([TaxonID])

	ALTER TABLE [dwc_nzib].[Vernacular] CHECK CONSTRAINT [frkVernacularTaxon]

	ALTER TABLE [dwc_nzib].[Literature]  WITH CHECK ADD  CONSTRAINT [frkLiteratureTaxon] FOREIGN KEY([TaxonID])
	REFERENCES [dwc_nzib].[Taxon] ([TaxonID])

	ALTER TABLE [dwc_nzib].[Literature] CHECK CONSTRAINT [frkLiteratureTaxon]

	ALTER TABLE [dwc_nzib].[Distribution]  WITH CHECK ADD  CONSTRAINT [frkDistributionTaxon] FOREIGN KEY([TaxonID])
	REFERENCES [dwc_nzib].[Taxon] ([TaxonID])

	ALTER TABLE [dwc_nzib].[Distribution] CHECK CONSTRAINT [frkDistributionTaxon]


	--taxon
	create table #taxon (TaxonID uniqueidentifier, 
						NameID uniqueidentifier,
						ProviderNameID nvarchar(1000),
						AcceptedUsageID nvarchar(1000),
						ParentUsageID nvarchar(1000),
						OriginalUsageID nvarchar(1000),
						AccordingToID nvarchar(1000),
						PublishedInID nvarchar(1000),
						ConceptID uniqueidentifier,
						ProviderConceptID nvarchar(1000),
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
						Source nvarchar(1000),
						AddedDate datetime,
						ConsensusConceptID uniqueidentifier)

	insert #taxon(TaxonID, NameID, ProviderNameID, ScientificName, AccordingToID, AccordingTo, ConceptID, ProviderConceptID, TaxonRank, Vernacular, NomCode, TaxonRemarks, Modified, Rights, RightsHolder, AccessRights, BibCitation, InfoWithheld, Source, DatasetName, AddedDate, ConsensusConceptID)
	select distinct c.ProviderRecordID,
		n.NameID,
		n.ProviderRecordID,
		replace(n.FullName, ' ', ' '),
		c.ProviderReferenceID,
		crp.Value as AccordingToReference,
		c.ConceptID,
		c.ProviderRecordID as ProviderConceptID,
		tr.Name as TaxonRank,
		'' as VernacularName,
		n.GoverningCode,
		'' as TaxonRemarks,
		n.ModifiedDate,
		'' as Rights,
		'NZIB' as RightsHolder,
		'' as AccessRights,
		'' as BibliographicCitation,
		'' as InformationWitheld,
		'' as Source,
		'New Zealand Inventory of Biodiversity',
		c.AddedDate,
		c.ConsensusConceptID
	from provider.Concept c
	inner join provider.Name n on n.NameID = c.NameID
	inner join dbo.TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
	left join provider.ReferenceProperty crp on crp.ReferenceID = c.AccordingToReferenceID and crp.ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07'
	where n.DataSourceID = 'C93F3E15-92DA-4E93-9DE0-416F937CC8E5' and n.NameClassID = 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A'
		
	update t
	set AcceptedName = replace(fn.FullName, ' ', ' '),
		AcceptedUsageID = toc.ProviderRecordID
	from #taxon t
	inner join provider.ConceptRelationship anc on anc.FromConceptID = t.ConceptID and anc.ConceptRelationshipTypeID = '0CA79AB3-E213-4F51-88B9-4CE01F735A1D' and anc.InUse = 1
	inner join provider.Concept toc on toc.ConceptID = anc.ToConceptID
	inner join provider.Name fn on fn.NameID = toc.NameID 
	inner join #taxon c on c.ConceptID = anc.ToConceptID
	
	update t
	set t.AcceptedUsageID = null, t.AcceptedName = null
	from #taxon t
	inner join #taxon ct on ct.TaxonID = t.AcceptedUsageID
	where isnull(ct.AcceptedUsageID, ct.TaxonID) <> ct.TaxonID

	update t
	set PublishedInID = pinp.RelatedID,
		PublishedIn = pinp.Value
	from #taxon t
	inner join provider.NameProperty pinp on pinp.NameID = t.NameID and pinp.NamePropertyTypeID = 'DEDC63F0-FB2A-420B-9932-786B4347DA45'

	update t
	set ParentUsageID = case when isnull(c.AcceptedUsageID, c.TaxonID) = c.TaxonID then toc.ProviderRecordID else c.AcceptedUsageID end,
		ParentName = case when isnull(c.AcceptedUsageID, c.TaxonID) = c.TaxonID then replace(fn.FullName, ' ', ' ') else replace(c.AcceptedName, ' ', ' ') end
	from #taxon t
	inner join provider.ConceptRelationship pnc on pnc.FromConceptID = t.ConceptID and pnc.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' and pnc.InUse = 1
	inner join provider.Concept toc on toc.ConceptID = pnc.ToConceptID
	inner join provider.Name fn on fn.NameID = toc.NameID 	
	inner join #taxon c on c.ConceptID = pnc.ToConceptID
	

	update t
	set OriginalUsageID =  c.ProviderRecordID,
		OriginalName =  fn.FullName
	from #taxon t
	inner join provider.NameProperty bnp on bnp.NameID = t.NameID and bnp.NamePropertyTypeID = 'F496FBCC-8DA6-4CA1-9884-11BD9B5DF63B'
	inner join provider.Concept c on c.NameID = bnp.RelatedID 
	inner join provider.ConceptRelationship cr on cr.FromConceptID = c.ConceptID and cr.InUse = 1 and cr.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155'	
	inner join provider.Name fn on fn.NameID = c.NameID 
	inner join #taxon ct on ct.ConceptID = c.ConceptID

	update t
	set HigherClassification = 
		case when h.names = '' then '' else SUBSTRING(h.names, 1, len(h.names) - 1) end
	from #taxon t
	cross apply (
		select (SELECT hn.CanonicalName  + ';' AS [text()] from (select distinct top 100 canonicalname, sortorder
								FROM provider.StackedName sn
								where sn.SeedNameID = t.NameID and CanonicalName <> 'ROOT' and sn.NameID <> t.NameID
								order by SortOrder) hn
		FOR XML PATH(''))
	) h (names)


	update t
	set Kingdom = kn.CanonicalName
	from #taxon t
	inner join provider.StackedName kn on kn.SeedNameID = t.NameID and kn.RankName = 'kingdom'

	update t
	set Phylum = pn.CanonicalName
	from #taxon t
	inner join provider.StackedName pn on pn.SeedNameID = t.NameID and pn.RankName = 'phylum'

	update t
	set Class = cn.CanonicalName
	from #taxon t
	inner join provider.StackedName cn on cn.SeedNameID = t.NameID and cn.RankName = 'class'

	update t
	set [Order] = orn.CanonicalName
	from #taxon t
	inner join provider.StackedName orn on orn.SeedNameID = t.NameID and orn.RankName = 'order'

	update t
	set Family = fn.CanonicalName
	from #taxon t
	inner join provider.StackedName fn on fn.SeedNameID = t.NameID and fn.RankName = 'family'

	update t
	set Genus = gn.CanonicalName
	from #taxon t
	inner join provider.StackedName gn on gn.SeedNameID = t.NameID and gn.RankName = 'genus'

	update t
	set SubGenus = sgn.CanonicalName
	from #taxon t
	inner join provider.StackedName sgn on sgn.SeedNameID = t.NameID and sgn.RankName = 'subgenus'

	update t
	set SpecificEpithet = spn.CanonicalName
	from #taxon t
	inner join provider.StackedName spn on spn.SeedNameID = t.NameID and spn.RankName = 'species'

	update t
	set InfraspecificEpithet = isn.CanonicalName
	from #taxon t
	inner join provider.StackedName isn on isn.SeedNameID = t.NameID and isn.SortOrder > 4200 and isn.Depth = 1

	update t
	set VerbatimRank = rnp.Value
	from #taxon t
	inner join provider.NameProperty rnp on rnp.NameID = t.NameID and rnp.NamePropertyTypeID = 'A1D57520-3D64-4F7D-97C8-69B449AFA280'

	update t
	set Authors = anp.Value
	from #taxon t
	inner join provider.NameProperty anp on anp.NameID = t.NameID and anp.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'

	update #taxon
	set TaxonomicStatus = case when AcceptedUsageID <> ProviderConceptID then 'synonym' else 'accepted' end 
	
	update t
	set TaxonomicStatus = ';' +
		case when tnc.ToConceptID is not null then 'teleomorph' else '' end	
	from #taxon t
	inner join provider.ConceptRelationship tnc on tnc.FromConceptID = t.ConceptID and tnc.ConceptRelationshipTypeID = '1EAA046F-61A6-4F1E-B1A7-0E1D2CE78BBF' and tnc.InUse = 1
		
	update t
	set TaxonomicStatus = ';' +	
		case when annc.ToConceptID is not null then 'anamorph' else '' end
	from #taxon t
	inner join provider.ConceptRelationship annc on annc.FromConceptID = t.ConceptID and annc.ConceptRelationshipTypeID = '0018A60A-3C1D-4EFA-B4B3-534B40ACD079' and annc.InUse = 1

	update t
	set NomenclaturalStatus = nsnp.Value
	from #taxon t
	inner join provider.NameProperty nsnp on nsnp.NameID = t.NameID and nsnp.NamePropertyTypeID = '65F94532-7687-4958-B5B8-71F54866BEAD'


	INSERT INTO [dwc_nzib].[Taxon]
           ([TaxonID]
           ,[ScientificNameID]
           ,[AcceptedNameUsageID]
           ,[ParentNameUsageID]
           ,[OriginalNameUsageID]
           ,[NameAccordingToID]
           ,[NamePublishedInID]
           ,[TaxonConceptID]
           ,[ScientificName]
           ,[AcceptedNameUsage]
           ,[ParentNameUsage]
           ,[OriginalNameUsage]
           ,[NameAccordingTo]
           ,[NamePublishedIn]
           ,[HigherClassification]
           ,[Kingdom]
           ,[Phylum]
           ,[Class]
           ,[Order]
           ,[Family]
           ,[Genus]
           ,[SubGenus]
           ,[SpecificEpithet]
           ,[InfraspecificEpithet]
           ,[TaxonRank]
           ,[VerbatimTaxonRank]
           ,[ScientificNameAuthorship]
           ,[VernacularName]
           ,[NomenclaturalCode]
           ,[TaxonomicStatus]
           ,[NomenclaturalStatus]
           ,[TaxonRemarks]
           ,[Modified]
           ,[Rights]
           ,[RightsHolder]
           ,[AccessRights]
           ,[BibliographicCitation]
           ,[InformationWitheld]
           ,[DatasetID]
           ,[DatasetName]
           ,[Source]
           ,[AddedDate]
           ,[ConsensusConceptID])	
	select ProviderConceptID
           ,ProviderNameID
           ,AcceptedUsageID
           ,ParentUsageID
           ,OriginalUsageID
           ,AccordingToID
           ,PublishedInID
           ,ConsensusConceptID
           ,ScientificName
           ,AcceptedName
           ,ParentName
           ,OriginalName
           ,AccordingTo
           ,PublishedIn
           ,HigherClassification
           ,[Kingdom]
           ,[Phylum]
           ,[Class]
           ,[Order]
           ,[Family]
           ,[Genus]
           ,[SubGenus]
           ,[SpecificEpithet]
           ,[InfraspecificEpithet]
           ,[TaxonRank]
           ,[VerbatimRank]
           ,Authors
           ,Vernacular
           ,NomCode
           ,[TaxonomicStatus]
           ,[NomenclaturalStatus]
           ,[TaxonRemarks]
           ,[Modified]
           ,[Rights]
           ,[RightsHolder]
           ,[AccessRights]
           ,BibCitation
           ,InfoWithheld
           ,[DatasetID]
           ,[DatasetName]
           ,[Source]
           ,[AddedDate]
           ,[ConsensusConceptID]
	from #taxon



	--vernaculars
	insert into dwc_nzib.Vernacular
	select distinct cto.ProviderRecordID, 
		n.FullName, 
		crp.Value as Source,
		isnull(lnp.Value, 'en') as [Language],
		'' as Temporal,
		'' as LocationID,
		gr.Name as Locality,
		cnp.Value as CountryCode,
		ca.Gender,
		ca.LifeStage,
		null as IsPlural,	
		ca.InUse,
		ca.PartOfTaxon,
		'' as TaxonRemarks
	from provider.Name n
	left join provider.NameProperty lnp on lnp.NameID = n.NameID and lnp.NamePropertyTypeID = '2089E79F-4CD4-4CD7-AA0D-A3AD0CDFD331'
	left join provider.NameProperty cnp on cnp.NameID = n.NameID and cnp.NamePropertyTypeID = '5F3CD05D-3BCC-4D2B-BE71-BFAC7DF9FD17'
	inner join provider.Concept c on n.NameID = c.NameID 
	left join provider.ReferenceProperty crp on crp.ReferenceID = c.AccordingToReferenceID and crp.ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07'
	inner join provider.ConceptApplication ca on ca.FromConceptID = c.ConceptID
	inner join provider.Concept cto on cto.ConceptID = ca.ToConceptID
	left join dbo.GeographicSchema gs on gs.name = ca.GeographicSchema
	left join dbo.GeoRegion gr on gr.Name = ca.GeoRegion and gr.GeographicSchemaID = gs.GeographicSchemaID	
	inner join dwc_nzib.Taxon t on t.TaxonID = cto.ProviderRecordID
	where n.NameClassID = '05BCC19C-27E8-492C-8ADD-EC5F73325BC5' 


	--literature
	insert into dwc_nzib.Literature
	select distinct c.ProviderRecordID,
		irp.Value as Identifier,
		crp.Value as Citation,
		trp.Value as Title,
		case when auth.authors = '' then '' else SUBSTRING(auth.authors, 1, len(auth.authors) - 1) end as Creator,
		drp.Value as [Date],
		(select top 1 Value from provider.ReferenceProperty where ReferenceID = parentrp.ReferenceID and ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07') as Source,
		'' as Description,
		case when kw.keywords = '' then '' else SUBSTRING(kw.keywords, 1, len(kw.keywords) - 1) end as Subject,
		'NZIB' as Rights, 
		'' as TaxonRemarks,
		'' as Type
	from provider.Concept c
	inner join dwc_nzib.Taxon t on t.TaxonID = c.ProviderRecordID
	inner join provider.Name n on n.NameID = c.NameID
	inner join provider.Reference r on r.ReferenceID = c.AccordingToReferenceID
	inner join provider.ReferenceProperty crp on crp.ReferenceID = c.AccordingToReferenceID and crp.ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07'
	left join provider.ReferenceProperty parentrp on parentrp.ReferenceID = c.AccordingToReferenceID and parentrp.ReferencePropertyTypeID = '8600068C-EDE7-4F3A-A8FB-C8A9B59E8FE3'
	left join provider.ReferenceProperty irp on irp.ReferenceID = c.AccordingToReferenceID and irp.ReferencePropertyTypeID = '79DFAD1F-DDB4-4EEB-AA63-F7A544243A69'
	left join provider.ReferenceProperty trp on trp.ReferenceID = c.AccordingToReferenceID and trp.ReferencePropertyTypeID = '38B41B0A-E58C-4923-8A9A-031B32AD16F2'
	left join provider.ReferenceProperty drp on drp.ReferenceID = c.AccordingToReferenceID and drp.ReferencePropertyTypeID = '8D5027BD-39F1-408D-ACEF-A728599A216F'
	cross apply (
		select (SELECT isnull(Value + ';','') AS [text()] 
		FROM provider.ReferenceProperty rp
		where rp.ReferenceID = r.ReferenceID and rp.ReferencePropertyTypeID = '037D1485-C6C1-47D2-9619-234F743E9FF6'
		FOR XML PATH(''))
	) auth (authors)
	cross apply (
		select (SELECT isnull(Value + ';','') AS [text()] 
		FROM provider.ReferenceProperty rp
		where rp.ReferenceID = r.ReferenceID and rp.ReferencePropertyTypeID = '36F7FCC8-E11C-4EDA-BBF3-237C5CF9E6A2'
		FOR XML PATH(''))
	) kw (keywords)
	where crp.Value <> '' and n.NameClassID = 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A'
	

	--biostatus / distribution

	INSERT INTO [dwc_nzib].[Distribution]
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
		'http://nzbugs.landcareresearch.co.nz/default.aspx?NameId=' + c.ProviderNameID as Source,
		null as Remarks
	from dwc_nzib.Taxon t
	inner join provider.Concept c on c.ProviderRecordID = t.TaxonID
	inner join provider.TaxonProperty tp on tp.NameID = c.NameID
	inner join provider.TaxonPropertyValue gtp on gtp.TaxonPropertyID = tp.TaxonPropertyID and gtp.TaxonPropertyTypeID = 'BABCDC8B-E40B-43A8-B6F6-88C97B9197A0'
	left join provider.TaxonPropertyValue octp on octp.TaxonPropertyID = tp.TaxonPropertyID and octp.TaxonPropertyTypeID = '9BB63B14-0208-4070-A575-94F90DFD47B0'
	left join provider.TaxonPropertyValue etp on etp.TaxonPropertyID = tp.TaxonPropertyID and etp.TaxonPropertyTypeID = 'D955AD6E-4678-4AC9-B752-6A94F1C07080'
	left join provider.Reference r on r.ReferenceID = tp.ReferenceID 
	left join provider.ReferenceProperty drp on drp.ReferenceID = r.ReferenceID and drp.ReferencePropertyTypeID = '8D5027BD-39F1-408D-ACEF-A728599A216F'
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
		'http://nzbugs.landcareresearch.co.nz/default.aspx?NameId=' + pc.ProviderNameID as Source,
		null as Remarks
	from dwc_nzib.Taxon c
	inner join provider.Concept pc on pc.ProviderRecordID = c.TaxonID
	inner join provider.TaxonProperty tp on tp.ConceptID = pc.ConceptID
	inner join provider.TaxonPropertyValue gtp on gtp.TaxonPropertyID = tp.TaxonPropertyID and gtp.TaxonPropertyTypeID = 'BABCDC8B-E40B-43A8-B6F6-88C97B9197A0'
	left join provider.TaxonPropertyValue octp on octp.TaxonPropertyID = tp.TaxonPropertyID and octp.TaxonPropertyTypeID = '9BB63B14-0208-4070-A575-94F90DFD47B0'
	left join provider.TaxonPropertyValue etp on etp.TaxonPropertyID = tp.TaxonPropertyID and etp.TaxonPropertyTypeID = 'D955AD6E-4678-4AC9-B752-6A94F1C07080'
	left join provider.Reference r on r.ReferenceID = c.NameAccordingToID 
	left join provider.ReferenceProperty drp on drp.ReferenceID = r.ReferenceID and drp.ReferencePropertyTypeID = '8D5027BD-39F1-408D-ACEF-A728599A216F'
	inner join dbo.Country ct on ct.Country = gtp.Value


	insert dwc_nzib.ResourceRelationship
	select TaxonID, ConsensusConceptID, 'source record for', 'NZOR', AddedDate
	from #taxon 


	drop table #taxon

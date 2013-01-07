
IF  EXISTS (select * FROM nzor_dev.sys.foreign_keys WHERE name = N'frkDistributionTaxon')
execute nzor_dev.dbo.sp_executesql N'ALTER TABLE [darwincore].[Distribution] DROP CONSTRAINT [frkDistributionTaxon]'

IF  EXISTS (select * FROM nzor_dev.sys.foreign_keys WHERE name = N'frkLiteratureTaxon')
execute nzor_dev.dbo.sp_executesql N'ALTER TABLE darwincore.Literature DROP CONSTRAINT [frkLiteratureTaxon]'

IF  EXISTS (select * FROM nzor_dev.sys.foreign_keys WHERE name = N'frkVernacularTaxon')
execute nzor_dev.dbo.sp_executesql N'ALTER TABLE darwincore.Vernacular DROP CONSTRAINT [frkVernacularTaxon]'



execute nzor_dev.dbo.sp_executesql N'truncate table darwincore.Literature'
execute nzor_dev.dbo.sp_executesql N'truncate table darwincore.Vernacular'
execute nzor_dev.dbo.sp_executesql N'truncate table darwincore.Distribution'
execute nzor_dev.dbo.sp_executesql N'truncate table darwincore.Taxon'



execute nzor_dev.dbo.sp_executesql N'ALTER TABLE darwincore.Vernacular  WITH CHECK ADD  CONSTRAINT [frkVernacularTaxon] FOREIGN KEY([TaxonID])
REFERENCES darwincore.Taxon ([TaxonID])'

execute nzor_dev.dbo.sp_executesql N'ALTER TABLE darwincore.Vernacular CHECK CONSTRAINT [frkVernacularTaxon]'

execute nzor_dev.dbo.sp_executesql N'ALTER TABLE darwincore.Literature  WITH CHECK ADD  CONSTRAINT [frkLiteratureTaxon] FOREIGN KEY([TaxonID])
REFERENCES darwincore.Taxon ([TaxonID])'

execute nzor_dev.dbo.sp_executesql N'ALTER TABLE darwincore.Literature CHECK CONSTRAINT [frkLiteratureTaxon]'

execute nzor_dev.dbo.sp_executesql N'ALTER TABLE darwincore.Distribution  WITH CHECK ADD  CONSTRAINT [frkDistributionTaxon] FOREIGN KEY([TaxonID])
REFERENCES darwincore.Taxon ([TaxonID])'

execute nzor_dev.dbo.sp_executesql N'ALTER TABLE darwincore.Distribution CHECK CONSTRAINT [frkDistributionTaxon]'



--taxon
insert into nzor_dev.darwincore.Taxon
select distinct c.ConceptID,
	n.NameID,
	anc.NameToID as AcceptedNameID,
	pnc.NameToID as ParentNameID,
	bnp.RelatedID as BasionymNameID,
	c.AccordingToReferenceID,
	pinp.RelatedID as NamePublishedInID,
	c.ConceptID as TaxonConceptID,
	substring(n.FullName, 1, len(n.fullname) - len(anp.value) - 1) as ScientificName,
	anc.NameToFull as AcceptedName,
	pnc.NameToFull as ParentName,
	bnp.Value as Basionym,
	crp.Value as AccordingToReference,
	pinp.Value as NamePublishedIn,
	h.names as HigherClassification,
	kn.CanonicalName as Kingdom,
	pn.CanonicalName as Phylum,
	cn.CanonicalName as [Class],
	orn.CanonicalName as [Order],
	fn.CanonicalName as Family,
	gn.CanonicalName as Genus,
	sgn.CanonicalName as SubGenus,
	spn.CanonicalName as Species,
	isn.CanonicalName as InfraspecicEpithet,
	tr.Name as TaxonRank,
	rnp.Value as VerbatimTaxonRank,
	anp.Value as Authors,
	'' as VernacularName,
	n.GoverningCode,
	case when anc.NameToID <> n.NameID then 'synonym;' else 'accepted;' end +
	case when tnc.NameToID is not null then 'teleomorph;' else '' end +
	case when annc.NameToID is not null then 'anamorph;' else '' end
		as TaxonomicStatus,
	nsnp.Value as NomenclaturalStatus,
	'' as TaxonRemarks,
	n.ModifiedDate,
	'' as Rights,
	'NZOR' as RightsHolder,
	'' as AccessRights,
	'' as BibliographicCitation,
	'' as InformationWitheld,
	'NZOR' as DatasetID,
	'NZOR' as DatasetName,
	'NZOR' as Source
from consensus.Concept c
inner join consensus.Name n on n.NameID = c.NameID
inner join dbo.TaxonRank tr on tr.TaxonRankID = n.TaxonRankID
left join consensus.vwConcepts anc on anc.ConceptID = c.ConceptID and anc.ConceptRelationshipTypeID = '0CA79AB3-E213-4F51-88B9-4CE01F735A1D' and anc.IsActive = 1
left join consensus.vwConcepts pnc on pnc.ConceptID = c.ConceptID and pnc.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' and pnc.IsActive = 1
left join consensus.vwConcepts tnc on tnc.ConceptID = c.ConceptID and tnc.ConceptRelationshipTypeID = '1EAA046F-61A6-4F1E-B1A7-0E1D2CE78BBF' and tnc.IsActive = 1
left join consensus.vwConcepts annc on annc.ConceptID = c.ConceptID and annc.ConceptRelationshipTypeID = '0018A60A-3C1D-4EFA-B4B3-534B40ACD079' and annc.IsActive = 1
left join consensus.NameProperty bnp on bnp.NameID = n.NameID and bnp.NamePropertyTypeID = 'F496FBCC-8DA6-4CA1-9884-11BD9B5DF63B'
left join consensus.NameProperty rnp on rnp.NameID = n.NameID and rnp.NamePropertyTypeID = 'A1D57520-3D64-4F7D-97C8-69B449AFA280'
left join consensus.NameProperty anp on anp.NameID = n.NameID and anp.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
left join consensus.NameProperty nsnp on nsnp.NameID = n.NameID and nsnp.NamePropertyTypeID = '65F94532-7687-4958-B5B8-71F54866BEAD'
left join consensus.NameProperty pinp on pinp.NameID = n.NameID and pinp.NamePropertyTypeID = 'DEDC63F0-FB2A-420B-9932-786B4347DA45'
left join consensus.ReferenceProperty crp on crp.ReferenceID = c.AccordingToReferenceID and crp.ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07'
left join consensus.StackedName kn on kn.SeedNameID = n.NameID and kn.RankName = 'kingdom'
left join consensus.StackedName pn on pn.SeedNameID = n.NameID and pn.RankName = 'phylum'
left join consensus.StackedName cn on cn.SeedNameID = n.NameID and cn.RankName = 'class'
left join consensus.StackedName orn on orn.SeedNameID = n.NameID and orn.RankName = 'order'
left join consensus.StackedName fn on fn.SeedNameID = n.NameID and fn.RankName = 'family'
left join consensus.StackedName gn on gn.SeedNameID = n.NameID and gn.RankName = 'genus'
left join consensus.StackedName sgn on sgn.SeedNameID = n.NameID and sgn.RankName = 'subgenus'
left join consensus.StackedName spn on spn.SeedNameID = n.NameID and spn.RankName = 'species'
left join consensus.StackedName isn on isn.SeedNameID = n.NameID and isn.SortOrder > 4200 and isn.Depth = 1
cross apply (
    select (SELECT CanonicalName + ';' AS [text()] 
    FROM consensus.StackedName sn
    where sn.SeedNameID = n.NameID and CanonicalName <> 'ROOT' and NameID <> n.NameID
    order by SortOrder
    FOR XML PATH(''))
) h (names)
where n.NameClassID = 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A'


--vernaculars
insert into nzor_dev.darwincore.Vernacular
select ca.ToConceptID, 
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
	'1' as InUse, --TODO
	ca.PartOfTaxon,
	'' as TaxonRemarks
from consensus.Name n
left join consensus.NameProperty lnp on lnp.NameID = n.NameID and lnp.NamePropertyTypeID = '2089E79F-4CD4-4CD7-AA0D-A3AD0CDFD331'
left join consensus.NameProperty cnp on cnp.NameID = n.NameID and cnp.NamePropertyTypeID = '5F3CD05D-3BCC-4D2B-BE71-BFAC7DF9FD17'
inner join consensus.Concept c on n.NameID = c.NameID 
inner join consensus.ReferenceProperty crp on crp.ReferenceID = c.AccordingToReferenceID and crp.ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07'
inner join consensus.ConceptApplication ca on ca.FromConceptID = c.ConceptID
left join dbo.GeoRegion gr on gr.GeoRegionID = ca.GeoRegionID
inner join consensus.Concept cto on cto.ConceptID = ca.ToConceptID
inner join consensus.Name nto on nto.NameID = cto.NameID
where n.NameClassID = '05BCC19C-27E8-492C-8ADD-EC5F73325BC5'


--literature
insert into nzor_dev.darwincore.Literature
select distinct c.ConceptID,
	irp.Value as Identifier,
	crp.Value as Citation,
	trp.Value as Title,
	auth.authors as Creator,
	drp.Value as [Date],
	(select top 1 Value from consensus.ReferenceProperty where ReferenceID = parentrp.Value and ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07') as Source,
	'' as Description,
	kw.keywords as Subject,
	'' as Rights, -- TODO
	'' as TaxonRemarks,
	'' as Type
from consensus.Concept c
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




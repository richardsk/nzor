declare @pubids table(pubid uniqueidentifier)
declare @ids table(nameid uniqueidentifier)

insert @ids
select top 20 nameid from name

insert @ids
select distinct n.nameid 
from name n
inner join taxonconcept tc on tc.nameid = n.nameid
inner join (select top 10 toconceptid from conceptapplication) c on c.toconceptid = tc.taxonconceptid
left join @ids i on i.nameid = n.nameid
where i.nameid is null

insert @pubids
select distinct p.publicationid 
from publication p
inner join name n on n.publishedinid = p.publicationid
inner join @ids i on i.nameid = n.nameid

insert @pubids
select distinct p.publicationid 
from publication p
inner join taxonconcept tc on tc.accordingtoid = p.publicationid
inner join @ids i on i.nameid = tc.nameid
left join @pubids ep on ep.pubid = p.publicationid
where ep.pubid is null

insert @pubids
select distinct p.publicationid 
from publication p
inner join vernacularconcept vc on vc.accordingtoid = p.publicationid
inner join @ids i on i.nameid = vc.nameid
left join @pubids ep on ep.pubid = p.publicationid
where ep.pubid is null
insert @pubids

select distinct p.publicationid 
from publication p
inner join taxonconcept tc on tc.accordingtoid = p.publicationid
inner join (select top 10 toconceptid from conceptapplication) c on c.toconceptid = tc.taxonconceptid
left join @pubids ep on ep.pubid = p.publicationid
where ep.pubid is null



set concat_null_yields_null off

select 
	(select 
	  (select
		(Select 'NZOR_Test' "@id",
			'NZOR Test' Name,
			'' OrganisationURL,
			(Select Disclaimer,
					Attribution,
					Licensing
				from Metadata
			for xml Path ('Usage'), type)
	    for xml path('Provider'), type)
	  for xml path('Providers'), type),
	getdate() 'DateGenerated',
	'' Usage
	for xml path('Metadata'), TYPE),

	--dummy example deprecated record
	(select (select '1CE4A50E-C343-4201-AE36-073DEE219C37' "@recordId",
			'5759378F-DDA4-42BF-8214-BC3EB6319E27' "@replacementId",
			'2010-10-05' "@replacementDate",
			'Name' "@recordType",
			'withdrawn' "@reason"
			for xml path('DeprecatedRecord'), type)
		for xml path('DeprecatedRecords'), type),

  (select (select top 20 p.PublicationID "@id",
	ParentPublicationId "@parentPublicationRef",
	COALESCE([Type], N'Generic') "@type",
	CreatedDate "@createdDate",
	ModifiedDate "@modifiedDate",
	Citation,
	AuthorsSimple 'Authors/Simple',
	(select
	(select 'of publication' "@type",
		coalesce(DateOfPublication,'') [text()]
		from Publication where PublicationID = p.PublicationID
			and DateOfPublication is not null
		for XML path('Date'), type),
	(select 'on publication' "@type",
		coalesce(DateOnPublication,'') [text()]
		from Publication where PublicationID = p.PublicationID
			and DateOnPublication is not null
		for XML path('Date'), type)
		for xml Path('Dates'), type),
	(select		
	case when not exists(select * from publicationtitle where publicationid = p.publicationid) then	  
		(select
		  1 "@level",
		  'full' "@type",
		  coalesce(Citation,'') [text()]
		  from Publication where
				PublicationId = p.publicationid
		  for xml path('Title'), type)
	else
		(select
		  [level] "@level",
			[type] "@type",
			coalesce(Title,'') [text()]
			from PublicationTitle where
				PublicationId = p.publicationid
			for xml path('Title'), type)
	end
	for XML path('Titles'), type),
	EditorsSimple 'Editors/Simple',
	Volume,
	Issue,
	Edition,
	case isnull(PageStart + PageEnd + PageTotal, '')
		when '' then null
		else
	(select 
		(Select 'start' "@type",
		   coalesce(PageStart,'') [text()]
		   from Publication where PublicationID = p.PublicationID
				and PageStart is not null
				for xml path('Page'), type),
		(Select 'end' "@type",
		   coalesce(PageEnd,'') [text()]
		   from Publication where PublicationID = p.PublicationID
				and PageEnd is not null
				for xml path('Page'), type),
		(Select 'total' "@type",
		   coalesce(PageTotal,'') [text()]
		   from Publication where PublicationID = p.PublicationID
				and PageTotal is not null
				for xml path('Page'), type)
		for xml path('Pages'), type) end,
		coalesce(PublisherName,'') 'Publisher/Name',
		coalesce(PublisherCity,'') 'Publisher/City',

		(select (select 'NZOR_Test' "@ref",
			'NZOR_Test_' + cast(newid() as varchar(38)) "@providersId"
			for xml path('Provider'), type)
		for xml path('Providers'), type)
		
from Publication p
inner join @pubids i on i.pubid = p.publicationid
 for xml Path('Publication'), type
	)
	for xml path('Publications'), type),

(select -- names element
(select -- Name element
	n1.NameId "@id",
	createdDate "@createdDate",
	modifiedDate "@modifiedDate",
	NameFull as FullName,
	NamePart 'Representations/NamePart',
	NameHTML 'Representations/NameHtml',
	[Rank],
	CanonicalName 'CanonicalName/Simple',
	CanonicalName 'CanonicalName/Uninomial',
	coalesce(Authorship,'') 'Authorship',
	coalesce(BasionymAuthors,'') 'BasionymAuthors',
	coalesce(CombiningAuthors,'') 'CombiningAuthors',
	case when PublishedInId is null then 
		(select coalesce(PublishedIn,'') from Name where NameId = n1.nameid for xml path('PublishedIn'), type)
	else 
		(select PublishedInId "@ref", coalesce(PublishedIn,'') [text()] from Name where nameid = n1.nameid
			for xml Path('PublishedIn'),  type)
	end,
	coalesce([Year],'') 'Year',
	coalesce(MicroReference,'') 'MicroReference',
	case when TypeName is null then '' else
	  (select TypeNameId "@ref", TypeName [text()] from Name where nameid = n1.nameid
		for xml path('TypeName'), type)
	end,
	ProtologueOrthography,
	case when BasionymId is null then '' else
	  (select BasionymId "@ref", Basionym [text()] from Name where nameid = n1.nameid
		for xml path('Basionym'), type)
	end,
	case when LaterHomonymOfId is null then '' else
	  (select LaterHomonymOfId "@ref", LaterHomonymOf [text()]
		from Name where nameid = n1.nameid
		for xml path('LaterHomonymOf'), type) 
	end,
	case when n1.BlockedName is null and n1.RecombinedName is null then '' else
	  (select (Select coalesce(cast(BlockedNameId as varchar(38)),'') "@ref", coalesce(BlockedName,'') [text()]
		from Name where nameid = n1.nameid
		for xml path('BlockedName'), type),
	    case when n1.RecombinedNameId is null then '' else
	      (select RecombinedNameId "@ref", RecombinedName [text()] 
	       from Name where nameid = n1.nameid
	       for xml path('RecombinedName'), type)
	    end
	  for xml path('ReplacementFor'), type)
	end,
	NomenclaturalStatus,
	coalesce(NomenclaturalCode, '') 'NomenclaturalCode',
	
	(select (select 'NZOR_Test' "@ref",
		'NZOR_Test_' + cast(newid() as varchar(38)) "@providersId"
		for xml path('Provider'), type)
	for xml path('Providers'), type)

	from Name n1
		inner join @ids i on i.nameid = n1.nameid
		for xml Path('TaxonName'), type),

-- start of vernacular records
(select top 10 vernacularid "@id",
		 'false' "@isTradeName",
		 NameFull as FullName,
		 (select PublishedInId "@ref", PublishedIn [text()]
			from Vernacular where VernacularId = v1.vernacularId
			for xml path('PublishedIn'), type),
		 [Language],
		 Country,
		 
		(select (select 'NZOR_Test' "@ref",
			'NZOR_Test_' + cast(newid() as varchar(38)) "@providersId"
			for xml path('Provider'), type)
		for xml path('Providers'), type)

		 from Vernacular v1
		 for xml path ('VernacularName'), type)
	for xml Path('TaxonNames'), type),

-- concepts
(select 
	(select taxonconceptid "@id",
	createdDate "@createdDate",
	ModifiedDate "@modifiedDate",
	(select NameId "@ref", 
		'true' "@scientific",
		Name [text()]
		from taxonconcept where taxonconceptid = c1.taxonconceptid
		for xml path('Name'), type),
	(select AccordingToId "@ref", 
		AccordingTo [text()]
		from taxonconcept where taxonconceptid = c1.taxonconceptid
		for xml path('AccordingTo'), type),
	(select 
	  case when AcceptedConceptId is null then '' else
	  (select AcceptedConceptID "@ref", 
		(select 'NZOR_Test' "@providerId", 
			case when AcceptedConceptInUse = 1 then 'true'
				else 'false'
			end "@inUse"
			from taxonconcept where taxonconceptid = c1.taxonconceptid
		 for xml path('Status'), type)
		from taxonconcept where taxonconceptid = c1.taxonconceptid
		for xml path('AcceptedConcept'), type) end,
	case when ParentConceptID is null then '' else 
	  (select cast(ParentConceptID as varchar(38)) "@ref", 
		(select 'NZOR_Test' "@providerId", 
			case when ParentConceptInUse = 1 then 'true'
				else 'false'
			end "@inUse"
			from taxonconcept where taxonconceptid = c1.taxonconceptid
		 for xml path('Status'), type)
		from taxonconcept where taxonconceptid = c1.taxonconceptid
		for xml path('ParentConcept'), type) end,
	 (Select ToConceptId "@ref",
		[Type] "@type",
		[rank] "@rank",
		case when InUse = 1 then 'true'
			else 'false'
		end 'InUse'
		from ConceptRelationship
		where FromConceptId = c1.taxonConceptId
		for XML path ('Relationship'), type)
		
		for xml path('Relationships'), type),
		 
		(select (select 'NZOR_Test' "@ref",
			'NZOR_Test_' + cast(newid() as varchar(38)) "@providersId"
			for xml path('Provider'), type)
		for xml path('Providers'), type)
			 
		from taxonConcept c1
		inner join @ids n on n.nameid = c1.nameid
	  for xml path ('TaxonConcept'), type),
		
--vernacular concepts

	(select top 5 vernacularconceptid "@id",
	createdDate "@createdDate",
	ModifiedDate "@modifiedDate",
	(select NameId "@ref", 
		'false' "@scientific",
		Name [text()]
		from vernacularconcept where vernacularconceptid = c1.vernacularconceptid
		for xml path('Name'), type),
	(select AccordingToId "@ref", 
		AccordingTo [text()]
		from vernacularconcept where vernacularconceptid = c1.vernacularconceptid
		for xml path('AccordingTo'), type),	

	  (select (select top 1 isnull(ToConceptId,ToNameId) "@ref",
				Gender,
				PartOfTaxon,
				GeoRegion,
				(select 'NZOR_Test' "@providerId", 
					'true'"@inUse"
				 for xml path('Status'), type)
			 from ConceptApplication where fromconceptid = c1.vernacularconceptid
			 for xml path('Application'), type)
		 for xml path('Applications'), type),
		 
		(select (select 'NZOR_Test' "@ref",
			'NZOR_Test_' + cast(newid() as varchar(38)) "@providersId"
			for xml path('Provider'), type)
		for xml path('Providers'), type)
			 
		from vernacularconcept c1 
		inner join (select top 10 vernacularid from vernacular) n on n.vernacularid = c1.nameid
	  for xml path ('TaxonConcept'), type)
	  
	for xml path ('TaxonConcepts'), type),

	(select --Taxon Properties
	
		(select --BiostatusValues
		(select top 20
			BiostatusID "@id",
			createddate "@createdDate",
			ModifiedDate "@modifiedDate",
			(select NameId "@nameRef",
				ConceptID "@conceptRef"
				from Biostatus where BiostatusID = b1.BiostatusID
				for XML path('Taxon'), type),
			(select AccordingToId "@ref",
				AccordingTo [text()]
				from Biostatus where BiostatusID = b1.BiostatusID
				for XML path('AccordingTo'), type),
			(select GeographicSchema "@geographicSchema",
				Region [text()]
				from Biostatus where BiostatusID = b1.BiostatusID
				for XML path('Region'), type),
			EnvironmentalContext,
			coalesce(Origin, '') Origin,
			coalesce(Occurrence,'') Occurrence,
			
			(select (select 'NZOR_Test' "@ref",
				'NZOR_Test_' + cast(newid() as varchar(38)) "@providersId"
				for xml path('Provider'), type)
			for xml path('Providers'), type)

			 from Biostatus b1
			 inner join @ids n on n.nameid = b1.nameid
		for xml path('Biostatus'), type)
		for XML path ('BiostatusValues'), type)
	for xml path ('TaxonProperties'), type)
  from Metadata 
for xml Path('DataSet')

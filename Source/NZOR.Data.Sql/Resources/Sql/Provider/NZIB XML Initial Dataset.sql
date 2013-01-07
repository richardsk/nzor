
set concat_null_yields_null off

select (select 
	(Select 'NZIB' "@id",
			'NZIB' Name,
			'' OrganisationURL 
		for xml path('Provider'), type),
	getdate() 'DateGenerated'
	for xml path('Metadata'), TYPE),
(select 'NZIB' "@id", --DataSource
    (Select Disclaimer,
		Attribution,
		Licensing
			from nzib_name.Metadata
			for xml Path ('Usage'), type),

  (select (select p.PublicationID "@id",
	ParentPublicationId "@parentPublicationRef",
	COALESCE([Type], N'Generic') "@type",
	CreatedDate "@createdDate",
	p.ModifiedDate "@modifiedDate",
	Citation,
	AuthorsSimple 'Authors/Simple',
	(select
	(select 'of publication' "@type",
		coalesce(DateOfPublication,'') [text()]
		from nzib_name.Publication where PublicationID = p.PublicationID
			and DateOfPublication is not null
		for XML path('Date'), type),
	(select 'on publication' "@type",
		coalesce(DateOnPublication,'') [text()]
		from nzib_name.Publication where PublicationID = p.PublicationID
			and DateOnPublication is not null
		for XML path('Date'), type)
		for xml Path('Dates'), type),
	(select		
	case when not exists(select * from nzib_name.publicationtitle where publicationid = p.publicationid) then	  
		(select
		  1 "@level",
		  'full' "@type",
		  coalesce(Citation,'') [text()]
		  from nzib_name.Publication where
				PublicationId = p.publicationid
		  for xml path('Title'), type)
	else
		(select
		  [level] "@level",
			[type] "@type",
			coalesce(Title,'') [text()]
			from nzib_name.PublicationTitle where
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
		   from nzib_name.Publication where PublicationID = p.PublicationID
				and PageStart is not null
				for xml path('Page'), type),
		(Select 'end' "@type",
		   coalesce(PageEnd,'') [text()]
		   from nzib_name.Publication where PublicationID = p.PublicationID
				and PageEnd is not null
				for xml path('Page'), type),
		(Select 'total' "@type",
		   coalesce(PageTotal,'') [text()]
		   from nzib_name.Publication where PublicationID = p.PublicationID
				and PageTotal is not null
				for xml path('Page'), type)
		for xml path('Pages'), type) end,
		coalesce(PublisherName,'') 'Publisher/Name',
		coalesce(PublisherCity,'') 'Publisher/City'
		
from nzib_name.Publication p
 for xml Path('Publication'), type
	)
	for xml path('Publications'), type),

(select -- names element
(select -- Name element
	NameId "@id",
	createdDate "@createdDate",
	n1.modifiedDate "@modifiedDate",
	NameFull as FullName,
	--NamePart 'Representations/NamePart',
	--NameHTML 'Representations/NameHtml',
	[Rank],
	CanonicalName,
	coalesce(Authorship,'') 'Authorship',
	coalesce(BasionymAuthors,'') 'BasionymAuthors',
	coalesce(CombiningAuthors,'') 'CombiningAuthors',
	case when PublishedInId is null then 
		(select coalesce(PublishedIn,'') from nzib_name.Name where NameId = n1.nameid for xml path('PublishedIn'), type)
	else 
		(select PublishedInId "@ref", coalesce(PublishedIn,'') [text()] from nzib_name.Name where nameid = n1.nameid
			for xml Path('PublishedIn'),  type)
	end,
	coalesce([Year],'') 'Year',
	coalesce(MicroReference,'') 'MicroReference',
	case when TypeName is null then '' else
	  (select TypeNameId "@ref", TypeName [text()] from nzib_name.Name where nameid = n1.nameid
		for xml path('TypeName'), type)
	end,
	ProtologueOrthography as Orthography,
	case when BasionymId is null then '' else
	  (select BasionymId "@ref", Basionym [text()] from nzib_name.Name where nameid = n1.nameid
		for xml path('Basionym'), type)
	end,
	case when LaterHomonymOfId is null then '' else
	  (select LaterHomonymOfId "@ref", LaterHomonymOf [text()]
		from nzib_name.Name where nameid = n1.nameid
		for xml path('LaterHomonymOf'), type) 
	end,
	case when n1.BlockedName is null and n1.RecombinedName is null then '' else
	  (select (Select coalesce(cast(BlockedNameId as varchar(38)),'') "@ref", coalesce(BlockedName,'') [text()]
		from nzib_name.Name where nameid = n1.nameid
		for xml path('BlockedName'), type),
	    case when n1.RecombinedNameId is null then '' else
	      (select RecombinedNameId "@ref", RecombinedName [text()] 
	       from nzib_name.Name where nameid = n1.nameid
	       for xml path('RecombinedName'), type)
	    end
	  for xml path('ReplacementFor'), type)
	end,
	NomenclaturalStatus,
	coalesce(NomenclaturalCode, '') 'NomenclaturalCode',
	case when IsRecombination = 1 then 'true' else null end 'IsRecombination'
	from nzib_name.Name n1
		for xml Path('TaxonName'), type),
-- start of vernacular records
(select vernacularid "@id",
		 createdDate "@createdDate",
		 v1.modifiedDate "@modifiedDate",
		 NameFull as FullName,
		 (select PublishedInId "@ref", PublishedIn [text()]
			from nzib_name.Vernacular where VernacularId = v1.vernacularId
			for xml path('PublishedIn'), type),
		 [Language],
		 Country
		 from nzib_name.Vernacular v1
		 for xml path ('VernacularName'), type)
	for xml Path('TaxonNames'), type),

-- concepts
(select
	(select taxonconceptid "@id",
	createdDate "@createdDate",
	c1.ModifiedDate "@modifiedDate",
	(select NameId "@ref", 
		Name [text()]
		from nzib_name.taxonconcept where taxonconceptid = c1.taxonconceptid
		for xml path('TaxonName'), type),
	(select AccordingToId "@ref", 
		AccordingTo [text()]
		from nzib_name.taxonconcept where taxonconceptid = c1.taxonconceptid
		for xml path('AccordingTo'), type),
	(select 
	  case when AcceptedConceptId is null then '' else
	  (select AcceptedConceptID "@ref", 
		case when AcceptedConceptInUse = 1 then 'true'
			else 'false'
		end "@inUse"
		from nzib_name.taxonconcept where taxonconceptid = c1.taxonconceptid
		for xml path('AcceptedConcept'), type) end,
	case when ParentConceptID is null then '' else 
	  (select cast(ParentConceptID as varchar(38)) "@ref", 
		case when ParentConceptInUse = 1 then 'true'
			else 'false'
		end "@inUse"
		from nzib_name.taxonconcept where taxonconceptid = c1.taxonconceptid
		for xml path('ParentConcept'), type) end,
	 (Select ToConceptId "@ref",
		[Type] "@type",
		[rank] "@rank",
		case when InUse = 1 then 'true'
			else 'false'
		end 'InUse'
		from nzib_name.ConceptRelationship
		where FromConceptId = c1.taxonConceptId
		for XML path ('Relationship'), type)
		
		for xml path('Relationships'), type)
	from nzib_name.taxonConcept c1
	for xml path ('TaxonConcept'), type),
	(select vernacularconceptid "@id",
			createdDate "@createdDate",
			cv1.ModifiedDate "@modifiedDate",
		(select NameId "@ref", 			
			Name [text()]
			from nzib_name.vernacularconcept where vernacularconceptid = cv1.vernacularconceptid
			for xml path('VernacularName'), type),
		(select AccordingToId "@ref", 
			AccordingTo [text()]
			from nzib_name.vernacularconcept where vernacularconceptid = cv1.vernacularconceptid
			for xml path('AccordingTo'), type),
		(select
			(select isnull(ToConceptId,ToNameId) "@ref",
			coalesce(InUse,'false') "@inUse",
			[type] "@type",
			Gender,
			PartOfTaxon,
			(select GeographicSchema "@geographicSchema",
				GeoRegion [text()]
				from nzib_name.ConceptApplication where RowId = ca1.RowId 					
				for XML path('GeoRegion'), type)
			from
			nzib_name.ConceptApplication ca1 where FromConceptId  = cv1.vernacularconceptid
				and isnull(ToConceptId,ToNameId) is not null
			for XML Path('Application'), type)
		for xml Path('Applications'), type)
	from nzib_name.VernacularConcept cv1
	for xml path ('VernacularConcept'), type),
	(select 
		(select createdDate "@createdDate",
				nc1.ModifiedDate "@modifiedDate",
			(select NameId "@ref"
				from nzib_name.taxonnameuse where taxonnameuseid = nc1.taxonnameuseid
				for xml path('Name'), type),
			case when AcceptedNameId is not null then (select AcceptedNameId "@ref", 'true' "@inUse"
				from nzib_name.taxonnameuse where taxonnameuseid = nc1.taxonnameuseid
				for xml Path('AcceptedName'), type) 
			else '' end,
			case when ParentNameId is not null then (select ParentNameId "@ref", 'true' "@inUse"
				from nzib_name.taxonnameuse where taxonnameuseid = nc1.taxonnameuseid
				for xml Path('ParentName'), type)
			else '' end
		from nzib_name.taxonnameuse nc1	
		for xml path ('TaxonNameUse'), type),
		--vernacular uses
		(select createdDate "@createdDate",
				vu1.ModifiedDate "@modifiedDate",
			(select VernacularId "@ref"
				from nzib_name.vernacularuse where vernacularuseid = vu1.vernacularuseid
				for xml path('VernacularName'), type),
			case when vu1.Rank is not null then (select Rank 
				from nzib_name.vernacularuse where vernacularuseid = vu1.vernacularuseid
				for xml path('Rank'), type)
			else '' end,
			(select 
				(select 
					'true' "@inUse",
					(select TaxonNameId "@ref" 
						from nzib_name.vernacularuse where vernacularuseid = vu1.vernacularuseid
						for xml path('TaxonName'), type),
					case when vu1.Gender is not null then (select Gender
						from nzib_name.vernacularuse where vernacularuseid = vu1.vernacularuseid
						for xml path('Gender'), type)
					else '' end,
					case when vu1.PartOfTaxon is not null then (select PartOfTaxon 
						from nzib_name.vernacularuse where vernacularuseid = vu1.vernacularuseid
						for xml path('PartOfTaxon'), type)
					else '' end,
					case when vu1.LifeStage is not null then (select LifeStage
						from nzib_name.vernacularuse where vernacularuseid = vu1.vernacularuseid
						for xml path('LifeStage'), type)
					else '' end,
					case when vu1.GeoRegion is not null then (select GeographicSchema "@geographicSchema", GeoRegion [text()]
						from nzib_name.vernacularuse where vernacularuseid = vu1.vernacularuseid
						for xml path('GeoRegion'), type)
					else '' end
				for xml path('Application'), type)
			for xml path('Applications'), type)
		from nzib_name.vernacularuse vu1	
		for xml path ('VernacularUse'), type)
	for xml path('NameBasedConcept'), type)
	for xml path ('TaxonConcepts'), type),
	
	case when exists(select * from nzib_name.Biostatus b1) then
	(select --Taxon Properties
		(select --BiostatusValues
		(select 
			BiostatusID "@id",
			createddate "@createdDate",
			b1.ModifiedDate "@modifiedDate",
			(select NameId "@nameRef",
				ConceptID "@conceptRef"
				from nzib_name.Biostatus where BiostatusID = b1.BiostatusID
				for XML path('Taxon'), type),
			(select AccordingToId "@ref",
				AccordingTo [text()]
				from nzib_name.Biostatus where BiostatusID = b1.BiostatusID
				for XML path('AccordingTo'), type),
			(select GeographicSchema "@geographicSchema",
				Region [text()]
				from nzib_name.Biostatus where BiostatusID = b1.BiostatusID
				for XML path('Region'), type),
			Biome,
			EnvironmentalContext,
			Origin,
			coalesce(Occurrence,'') Occurrence
			 from nzib_name.Biostatus b1
		for xml path('Biostatus'), type)
		for XML path ('BiostatusValues'), type)
	for xml path ('TaxonProperties'), type)
	else '' end,

	case when exists(select * from nzib_name.annotation) then
		(select -- Annotations
			(select  --Annotation 
				AnnotationId "@id",
				[Type] "@type",
				createdDate "@createdDate",
				modifiedDate "@modifiedDate",
			(select NameId "@nameRef",
				ConceptID "@conceptRef"
				from nzib_name.Annotation where AnnotationID = a1.AnnotationID
				for XML path('Taxon'), type),
			Annotation "AnnotationText",
			(select AccordingToId "@ref"
				from nzib_name.Annotation where AnnotationID = a1.AnnotationID
				for XML path('AccordingTo'), type)
			from nzib_name.annotation a1
			for xml path('Annotation'), type)
		for xml path('Annotations'), type)
	else '' end

  from nzib_name.Metadata for xml Path ('DataSource'), type)
for xml Path('DataSet')

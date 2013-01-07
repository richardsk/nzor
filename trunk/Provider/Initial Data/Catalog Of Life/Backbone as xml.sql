use CatOfLife10
go


/*
Description:  Query results in an xml file in format NZOR_provider version 1
				for the data in tblExtractedHigherTaxa.  It creates all the 
				required elements, even when these are empty.

 


*/

declare @refId uniqueidentifier

set @refId = NEWID()


select 
	-- metadata
	(select '14AFD938-734E-40F5-9021-F252B7357BC8' "Provider/@id",
		'CoL2010' 'Provider/Name',
		'http://www.catalogueoflife.org/' 'Provider/OrganisationURL',
		GETDATE() DateGenerated
	for xml path('Metadata'), type),
	-- publication for CatLife
	(select
		(select @refId "@id",
			'Catalogue of Life' Citation,
			'of publication' 'Dates/Date/@type',			2010 'Dates/Date',
			1 'Titles/Title/@level',
			'full' 'Titles/Title/@type',
			'Catalogue of Life' 'Titles/Title'
			for XML path('Publication'), TYPE)
	for XML path('Publications'), type),
	-- taxon names
	(select (
		select ID "@id",
			GETDATE() "@createdDate",
			Taxon NameFull,
			[Rank] Rank,
			Taxon 'CanonicalName/Simple',
			Taxon 'CanonicalName/Uninomial',
			'' Authorship,
			'' BasionymAuthors,
			'' CombiningAuthors,
			'' PublishedIn,
			'' Year,
			'' Microreference,
			case ISNULL(Kingdom, '')
				when 'Plantae' then 'ICBN'
				when 'Animalia' then 'ICZN'
				when 'Bacteria' then 'ICNB'
				when 'Chromista' then 'ICBN'
				when 'Fungi' then 'ICBN'
				when 'Protozoa' then 'ICZN'
				when 'Viruses' then 'ICTV'
				else '' end NomenclaturalCode
		from tblExtractedHighertaxa 
		for xml path('TaxonName'), type)
	for xml path('TaxonNames'), type),
	-- taxon concepts
	(select (
		select ID "@id",
				GETDATE() "@createdDate",
				--Taxon Name,
				id "Name/@ref",
				'true' "Name/@scientific",
				Taxon Name,
				@refId 'AccordingTo/@ref',
				'Catalogue of Life 2010' AccordingTo,
				ID "Relationships/AcceptedConcept/@ref",
				'true' "Relationships/AcceptedConcept/@inUse",
				ParentTaxon "Relationships/ParentConcept/@ref",
				'true' "Relationships/ParentConcept/@inUse",
				'' HigherClassification
			from tblExtractedHighertaxa
		for xml path('TaxonConcept'), type)
	for xml path('TaxonConcepts'), type)
for xml path ('DataSet')


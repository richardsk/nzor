IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NZIBCache15]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[NZIBCache15]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[NZIBCache15]
as

set concat_null_yields_null off

delete [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.TaxonConcept
delete [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.TaxonNameUse


insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.TaxonConcept(taxonconceptid,
	CreatedDate, ModifiedDate, Name, NameId, AccordingTo, AccordingToId, [Rank])
select bib.BibliographyGuid, BibliographyAddedDate, BibliographyUpdatedDate,
	bib.BibliographyName, bib.BibliographyNameFk, 
	ref.ReferenceGenCitation,
	bib.BibliographyReferenceFk, null
	from tblBibliography bib
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.Name n on n.nameid = bib.BibliographyNameFk
		left join tblReference ref on bib.BibliographyReferenceFk = ref.ReferenceID
where (bib.BibliographyIsDeleted = 0 or bib.BibliographyIsDeleted is null)


update con
	set AcceptedConceptId  = bibr.BibliographyRelationshipBibliographyToFk,
	AcceptedConceptInUse = 
		case when (select count(BibliographyRelationshipPk)  
			from tblBibliographyRelationship pbr
			inner join tblBibliography pb on pb.BibliographyGuid = pbr.BibliographyRelationshipBibliographyFromFk 
			where pb.BibliographyNameFk = b.BibliographyNameFk and pbr.BibliographyRelationshipTypeFk = 8
				and isnull(pb.BibliographyIsDeleted,0) = 0 and pbr.BibliographyRelationshipIsActive = 1) = 1
			then 
				case when (select top 1 BibliographyGuid from tblBibliographyRelationship pbr
							inner join tblBibliography pb on pb.BibliographyGuid = pbr.BibliographyRelationshipBibliographyFromFk
							where pb.BibliographyNameFk = b.BibliographyNameFk and pbr.BibliographyRelationshipTypeFk = 8
							and isnull(pb.BibliographyIsDeleted,0) = 0 and pbr.BibliographyRelationshipIsActive = 1) = con.taxonconceptid
				then 
					1
				else
					null
				end 
		else
			null
		end
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.TaxonConcept con
	inner join tblBibliographyRelationship bibr on con.taxonconceptid = bibr.BibliographyRelationshipBibliographyFromFk
		and bibr.BibliographyRelationshipTypeFk = 8
	inner join tblbibliography b on b.BibliographyGuid = bibr.BibliographyRelationshipBibliographyFromFk
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.TaxonConcept cto on cto.taxonconceptid = bibr.BibliographyRelationshipBibliographyToFk
		
update con
	set ParentconceptId  = bibr.BibliographyRelationshipBibliographyToFk,
	ParentConceptInUse = 
		case when (select count(BibliographyRelationshipPk)  --only one parent concept for this name
			from tblBibliographyRelationship pbr
			inner join tblBibliography pb on pb.BibliographyGuid = pbr.BibliographyRelationshipBibliographyFromFk 
			where pb.BibliographyNameFk = b.BibliographyNameFk and pbr.BibliographyRelationshipTypeFk = 7 
				and isnull(pb.BibliographyIsDeleted,0) = 0 and pbr.BibliographyRelationshipIsActive = 1) = 1			
			then 
				case when (select top 1 BibliographyGuid from tblBibliographyRelationship pbr
							inner join tblBibliography pb on pb.BibliographyGuid = pbr.BibliographyRelationshipBibliographyFromFk
							where pb.BibliographyNameFk = b.BibliographyNameFk and pbr.BibliographyRelationshipTypeFk = 7 
							and isnull(pb.BibliographyIsDeleted,0) = 0 and pbr.BibliographyRelationshipIsActive = 1) = con.taxonconceptid
				then 
					1
				else
					null
				end 
		else
			null
		end		
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.TaxonConcept con
	inner join tblBibliographyRelationship bibr on con.Taxonconceptid = bibr.BibliographyRelationshipBibliographyFromFk
		and bibr.BibliographyRelationshipTypeFk = 7
	inner join tblbibliography b on b.BibliographyGuid = bibr.BibliographyRelationshipBibliographyFromFk
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.TaxonConcept cto on cto.taxonconceptid = bibr.BibliographyRelationshipBibliographyToFk

--add missing concepts as name based concepts

declare @accToId uniqueidentifier
if (not exists(select publicationid from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.publication where citation = 'NZIB'))
begin
	set @accToId = NEWID()
	insert [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.publication(PublicationID, Citation, [Type]) select @accToId, 'NZIB', 'Electronic source'
	insert [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.publicationTitle(PublicationID, Title, Level, Type) select @accToId, 'NZIB', 1, 'full'
end
else
begin
	select @accToId = publicationid from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.publication where citation = 'NZIB'
end

insert [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.TaxonNameUse 
	(TaxonNameUseid, CreatedDate, ModifiedDate, NameId, AccordingToId, AcceptedNameId, ParentNameId)
select newid(), n.nameaddeddate, n.nameupdateddate, n.NameGuid, @accToId, NameCurrentFk, NameParentFk
from tblName n
inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.Name n1 on n1.nameid = n.nameguid
--left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.TaxonConcept con on con.nameid = n.NameGuid 
left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.TaxonNameUse nbc on nbc.nameid = n.NameGuid 
where nbc.TaxonNameUseid is null and namecurrentfk is not null
	and NameParentFk <> '1EB48E72-8C12-4637-9064-0E2D1BBAAD30'
	and not exists(select taxonconceptid from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.taxonconcept 
					where nameid = n1.nameid and parentconceptid is not null and parentconceptinuse = 1)
		
--null out accepted concepts if they have been provided by another 
update nbc
set acceptednameid = null
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.TaxonNameUse nbc
inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.taxonconcept tc on tc.nameid = nbc.nameid 
	and acceptedconceptid is not null and acceptedconceptinuse = 1
	
--accepted names defined by name uses when parent has been defined by a concept
insert [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.TaxonNameUse 
	(TaxonNameUseid, CreatedDate, ModifiedDate, NameId, AccordingToId, AcceptedNameId, ParentNameId)
select newid(), n.nameaddeddate, n.nameupdateddate, n.NameGuid, @accToId, NameCurrentFk, null
from tblName n
inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.Name n1 on n1.nameid = n.nameguid
--left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.TaxonConcept con on con.nameid = n.NameGuid 
left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.TaxonNameUse nbc on nbc.nameid = n.NameGuid 
where nbc.TaxonNameUseid is null
	and not exists(select taxonconceptid from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.taxonconcept 
					where nameid = n1.nameid and acceptedconceptid is not null and acceptedconceptinuse = 1)

	

/*  other relationships..

insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzib_name.ConceptRelationship(
	ConceptRelationshipID, FromConceptId, ToConceptId, [Type], InUse)
select BibliographyRelationshipPk, BibliographyRelationshipBibliographyFromFk,
	BibliographyRelationshipBibliographyToFk, 
	Case BibliographyRelationshipTypeFk
		when 
	end,
	BibliographyRelationshipIsActive
	from tblBibliographyRelationship
	where BibliographyRelationshipTypeFk <> 8
	 and BibliographyRelationshipTypeFk <> 7
	 and BibliographyRelationshipTypeFk <> 0
*/

-- to do HigherClassification
-- to do Hybrid concept relationships


go

grant execute on dbo.[NZIBCache15] to dbi_user

go

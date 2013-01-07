IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PNDCache15]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[PNDCache15]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[PNDCache15]
as

set concat_null_yields_null off

delete [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.TaxonConcept
delete [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.TaxonNameUse


insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.TaxonConcept(taxonconceptid,
	CreatedDate, ModifiedDate, Name, NameId, AccordingTo, AccordingToId, [Rank])
select bib.BibliographyGuid, BibliographyAddedDate, BibliographyUpdatedDate,
	bib.BibliographyName, bib.BibliographyNameFk, 
	ref.ReferenceGenCitation,
	bib.BibliographyReferenceFk, null
	from tblBibliography bib
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.Name n on n.nameid = bib.BibliographyNameFk
		left join tblReference ref on bib.BibliographyReferenceFk = ref.ReferenceID
where (bib.BibliographyIsDeleted = 0 or bib.BibliographyIsDeleted is null)
	and (bib.bibliographyreferencefk is null or bib.bibliographyreferencefk not in ('EB3B0740-917C-441D-8AE0-3185E7003ED9')) -- Dawson 2008, 


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
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.TaxonConcept con
	inner join tblBibliographyRelationship bibr on con.taxonconceptid = bibr.BibliographyRelationshipBibliographyFromFk
		and bibr.BibliographyRelationshipTypeFk = 8
	inner join tblbibliography b on b.BibliographyGuid = bibr.BibliographyRelationshipBibliographyFromFk
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.TaxonConcept cto on cto.taxonconceptid = bibr.BibliographyRelationshipBibliographyToFk
		
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
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.TaxonConcept con
	inner join tblBibliographyRelationship bibr on con.Taxonconceptid = bibr.BibliographyRelationshipBibliographyFromFk
		and bibr.BibliographyRelationshipTypeFk = 7
	inner join tblbibliography b on b.BibliographyGuid = bibr.BibliographyRelationshipBibliographyFromFk
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.TaxonConcept cto on cto.taxonconceptid = bibr.BibliographyRelationshipBibliographyToFk

--add missing concepts as name based concepts
--first delete any taxon concepts that arent complete - ie dont have a parent relationship defined

--delete tc
--from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.TaxonConcept tc
--inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.Name n on n.nameid = tc.nameid
--left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.taxonconcept pc on pc.taxonconceptid = tc.parentconceptid
--where pc.taxonConceptId is null and n.rank <> 'kingdom'

declare @accToId uniqueidentifier
if (not exists(select publicationid from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.publication where citation = 'NZFLORA'))
begin
	set @accToId = NEWID()
	insert [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.publication(PublicationID, Citation, [Type]) select @accToId, 'NZFLORA', 'Electronic source'
	insert [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.publicationTitle(PublicationID, Title, Level, Type) select @accToId, 'NZFLORA', 1, 'full'
end
else
begin
	select @accToId = publicationid from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.publication where citation = 'NZFLORA'
end

insert [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.TaxonNameUse 
	(TaxonNameUseid, CreatedDate, ModifiedDate, NameId, AccordingToId, AcceptedNameId, ParentNameId)
select newid(), n.nameaddeddate, n.nameupdateddate, n.NameGuid, @accToId, NameCurrentFk, NameParentFk
from tblName n
inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.Name n1 on n1.nameid = n.nameguid
--left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.TaxonConcept con on con.nameid = n.NameGuid 
left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.TaxonNameUse nbc on nbc.nameid = n.NameGuid 
where nbc.TaxonNameUseid is null
	and NameParentFk <> '19F62517-BD63-42E3-97DA-E043A1644B1B'
	and not exists(select taxonconceptid from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.taxonconcept 
					where nameid = n1.nameid and parentconceptid is not null and parentconceptinuse = 1)
	and (namereferencefk is null or namereferencefk not in ('EB3B0740-917C-441D-8AE0-3185E7003ED9'))
			
--null out accepted concepts if they have been provided by another 
update nbc
set acceptednameid = null
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.TaxonNameUse nbc
inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.taxonconcept tc on tc.nameid = nbc.nameid 
	and acceptedconceptid is not null and acceptedconceptinuse = 1
	

--accepted names defined by name uses when parent has been defined by a concept
insert [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.TaxonNameUse 
	(TaxonNameUseid, CreatedDate, ModifiedDate, NameId, AccordingToId, AcceptedNameId, ParentNameId)
select newid(), n.nameaddeddate, n.nameupdateddate, n.NameGuid, @accToId, NameCurrentFk, null
from tblName n
inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.Name n1 on n1.nameid = n.nameguid
--left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.TaxonConcept con on con.nameid = n.NameGuid 
left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.TaxonNameUse nbc on nbc.nameid = n.NameGuid 
where nbc.TaxonNameUseid is null
	and not exists(select taxonconceptid from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.taxonconcept 
					where nameid = n1.nameid and acceptedconceptid is not null and acceptedconceptinuse = 1)


/*  other relationships..

insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.plant_name.ConceptRelationship(
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

grant execute on dbo.[PNDCache15] to dbi_user

go

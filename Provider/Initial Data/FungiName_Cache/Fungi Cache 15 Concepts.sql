IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FungiCache15]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[FungiCache15]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[FungiCache15]
as

set concat_null_yields_null off

delete [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.TaxonConcept
delete [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.TaxonNameUse


insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.TaxonConcept(taxonconceptid,
	CreatedDate, ModifiedDate, Name, NameId, AccordingTo, AccordingToId, [Rank])
select distinct bib.BibliographyGuid, BibliographyAddedDate, BibliographyUpdatedDate,
	bib.BibliographyName, bib.BibliographyNameFk, 
	ref.ReferenceGenCitation,
	bib.BibliographyReferenceFk, null
	from tblBibliography bib
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Name n on n.nameid = bib.BibliographyNameFk	
	left join tblReference ref on bib.BibliographyReferenceFk = ref.ReferenceID
	left join tblBibliographyRelationship br1 on br1.BibliographyRelationshipBibliographyFromFk = bib.BibliographyGuid
	left join tblBibliographyRelationship br2 on br2.BibliographyRelationshipBibliographyToFk = bib.BibliographyGuid
where (bib.BibliographyIsDeleted = 0 or bib.BibliographyIsDeleted is null)
	--and (br2.BibliographyRelationshipPk is not null or br1.BibliographyRelationshipPk is not null) --harvest all concepts
	--and (bib.bibliographyreferencefk is null or bib.bibliographyreferencefk not in ('3E34-DE710734-4556-93A9-773CBD5A9A52')) --dictionary of fungi 10


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
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.TaxonConcept con
	inner join tblBibliographyRelationship bibr on con.taxonconceptid = bibr.BibliographyRelationshipBibliographyFromFk
		and bibr.BibliographyRelationshipTypeFk = 8
	inner join tblbibliography b on b.BibliographyGuid = bibr.BibliographyRelationshipBibliographyFromFk
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.TaxonConcept cto on cto.taxonconceptid = bibr.BibliographyRelationshipBibliographyToFk
		
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
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.TaxonConcept con
	inner join tblBibliographyRelationship bibr on con.Taxonconceptid = bibr.BibliographyRelationshipBibliographyFromFk
		and bibr.BibliographyRelationshipTypeFk = 7
	inner join tblbibliography b on b.BibliographyGuid = bibr.BibliographyRelationshipBibliographyFromFk
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.TaxonConcept cto on cto.taxonconceptid = bibr.BibliographyRelationshipBibliographyToFk

--add missing concepts as name based concepts
--first delete any taxon concepts that arent complete - ie dont have a parent relationship defined

--delete tc
--from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.TaxonConcept tc
--inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Name n on n.nameid = tc.nameid
--left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.taxonconcept pc on pc.taxonconceptid = tc.parentconceptid
--where pc.taxonConceptId is null and n.rank <> 'kingdom'

declare @accToId uniqueidentifier
if (not exists(select publicationid from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.publication where citation = 'NZFUNGI'))
begin
	set @accToId = NEWID()
	insert [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.publication(PublicationID, Citation, [Type]) select @accToId, 'NZFUNGI', 'Electronic source'
	insert [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.publicationTitle(PublicationID, Title, Level, Type) select @accToId, 'NZFUNGI', 1, 'full'
end
else
begin
	select @accToId = publicationid from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.publication where citation = 'NZFUNGI'
end

insert [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.TaxonNameUse 
	(TaxonNameUseid, CreatedDate, ModifiedDate, NameId, AccordingToId, AcceptedNameId, ParentNameId)
select newid(), n.nameaddeddate, n.nameupdateddate, n.NameGuid, @accToId, NameCurrentFk, 
	case when NameParentFk = '7DEAC4FE-BE39-4F83-BDEE-F3DC686BAFEF' or nameparentfk = '1CB1CEE0-36B9-11D5-9548-00D0592D548C' 
		then null
		else NameParentFk
	end
from tblName n
inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Name n1 on n1.nameid = n.nameguid
--left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.TaxonConcept con on con.nameid = n.NameGuid 
left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.TaxonNameUse nbc on nbc.nameid = n.NameGuid 
where nbc.TaxonNameUseid is null
	and not exists(select taxonconceptid from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.taxonconcept 
					where nameid = n1.nameid and parentconceptid is not null and parentconceptinuse = 1)
	--and (namereferencefk is null or namereferencefk not in ('DE710734-3E34-4556-93A9-773CBD5A9A52')) --dictionary of fungi 10		

--null out accepted concepts if they have been provided by another 
update nbc
set acceptednameid = null
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.TaxonNameUse nbc
inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.taxonconcept tc on tc.nameid = nbc.nameid 
	and acceptedconceptid is not null and acceptedconceptinuse = 1

--accepted names defined by name uses when parent has been defined by a concept
insert [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.TaxonNameUse 
	(TaxonNameUseid, CreatedDate, ModifiedDate, NameId, AccordingToId, AcceptedNameId, ParentNameId)
select newid(), n.nameaddeddate, n.nameupdateddate, n.NameGuid, @accToId, NameCurrentFk, null
from tblName n
inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.Name n1 on n1.nameid = n.nameguid
--left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.TaxonConcept con on con.nameid = n.NameGuid 
left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.TaxonNameUse nbc on nbc.nameid = n.NameGuid 
where nbc.TaxonNameUseid is null
	and not exists(select taxonconceptid from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.taxonconcept 
					where nameid = n1.nameid and acceptedconceptid is not null and acceptedconceptinuse = 1)


/*  other relationships....

insert into [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.fungi_name.ConceptRelationship(
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

grant execute on dbo.[FungiCache15] to dbi_user

go
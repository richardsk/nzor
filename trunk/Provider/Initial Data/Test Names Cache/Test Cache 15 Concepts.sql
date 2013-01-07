IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestCache15]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TestCache15]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[TestCache15]
as

set concat_null_yields_null off

delete [devserver02\sql2005].Name_Cache_Test.test_name.TaxonConcept
delete [devserver02\sql2005].Name_Cache_Test.test_name.TaxonNameUse
delete [devserver02\sql2005].Name_Cache_Test.test_name_2.TaxonConcept
delete [devserver02\sql2005].Name_Cache_Test.test_name_2.TaxonNameUse



--------------------------------------
--Plant name concepts

insert into [devserver02\sql2005].Name_Cache_Test.test_name.TaxonConcept(taxonconceptid,
	CreatedDate, ModifiedDate, Name, NameId, AccordingTo, AccordingToId, [Rank])
select bib.BibliographyGuid, BibliographyAddedDate, BibliographyUpdatedDate,
	bib.BibliographyName, bib.BibliographyNameFk, 
	ref.ReferenceGenCitation,
	bib.BibliographyReferenceFk, null
	from tblBibliography bib
	inner join [devserver02\sql2005].Name_Cache_Test.test_name.Name n on n.nameid = bib.BibliographyNameFk
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
from [devserver02\sql2005].Name_Cache_Test.test_name.TaxonConcept con
	inner join tblBibliographyRelationship bibr on con.taxonconceptid = bibr.BibliographyRelationshipBibliographyFromFk
		and bibr.BibliographyRelationshipTypeFk = 8
	inner join tblbibliography b on b.BibliographyGuid = bibr.BibliographyRelationshipBibliographyFromFk
	inner join [devserver02\sql2005].Name_Cache_Test.test_name.TaxonConcept cto on cto.taxonconceptid = bibr.BibliographyRelationshipBibliographyToFk
		
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
from [devserver02\sql2005].Name_Cache_Test.test_name.TaxonConcept con
	inner join tblBibliographyRelationship bibr on con.Taxonconceptid = bibr.BibliographyRelationshipBibliographyFromFk
		and bibr.BibliographyRelationshipTypeFk = 7
	inner join tblbibliography b on b.BibliographyGuid = bibr.BibliographyRelationshipBibliographyFromFk
	inner join [devserver02\sql2005].Name_Cache_Test.test_name.TaxonConcept cto on cto.taxonconceptid = bibr.BibliographyRelationshipBibliographyToFk

--add missing concepts as name based concepts
--first delete any taxon concepts that arent complete - ie dont have a parent relationship defined

declare @accToId uniqueidentifier
if (not exists(select publicationid from [devserver02\sql2005].Name_Cache_Test.test_name.publication where citation = 'NZFLORA'))
begin
	set @accToId = NEWID()
	insert [devserver02\sql2005].Name_Cache_Test.test_name.publication(PublicationID, Citation) select @accToId, 'NZFLORA'
	insert [devserver02\sql2005].Name_Cache_Test.test_name.publicationTitle(PublicationID, Title, Level, Type) select @accToId, 'NZFLORA', 1, 'full'
end
else
begin
	select @accToId = publicationid from [devserver02\sql2005].Name_Cache_Test.test_name.publication where citation = 'NZFLORA'
end

insert [devserver02\sql2005].Name_Cache_Test.test_name.TaxonNameUse 
	(taxonnameuseid, CreatedDate, ModifiedDate, NameId, AccordingToId, AcceptedNameId, ParentNameId)
select newid(), n.nameaddeddate, n.nameupdateddate, n.NameGuid, @accToId, NameCurrentFk, NameParentFk
from tblName n
inner join [devserver02\sql2005].Name_Cache_Test.test_name.Name n1 on n1.nameid = n.nameguid
--left join [devserver02\sql2005].Name_Cache_Test.test_name.TaxonConcept con on con.nameid = n.NameGuid 
left join [devserver02\sql2005].Name_Cache_Test.test_name.TaxonNameUse nbc on nbc.nameid = n.NameGuid 
where nbc.taxonnameuseid is null
	and NameParentFk <> '19F62517-BD63-42E3-97DA-E043A1644B1B'
	and not exists(select taxonconceptid from [devserver02\sql2005].Name_Cache_Test.test_name.taxonconcept 
					where nameid = n1.nameid and parentconceptid is not null and parentconceptinuse = 1)
		
	

---------------------------------------------
--Fungi name concepts

insert into [devserver02\sql2005].Name_Cache_Test.test_name_2.TaxonConcept(taxonconceptid,
	CreatedDate, ModifiedDate, Name, NameId, AccordingTo, AccordingToId, [Rank])
select bib.BibliographyGuid, BibliographyAddedDate, BibliographyUpdatedDate,
	bib.BibliographyName, bib.BibliographyNameFk, 
	ref.ReferenceGenCitation,
	bib.BibliographyReferenceFk, null
	from funginamesfromprod.dbo.tblBibliography bib
	inner join [devserver02\sql2005].Name_Cache_Test.test_name_2.Name n on n.nameid = bib.BibliographyNameFk
		left join funginamesfromprod.dbo.tblReference ref on bib.BibliographyReferenceFk = ref.ReferenceID
where (bib.BibliographyIsDeleted = 0 or bib.BibliographyIsDeleted is null)


update con
	set AcceptedConceptId  = bibr.BibliographyRelationshipBibliographyToFk,
	AcceptedConceptInUse = 
		case when (select count(BibliographyRelationshipPk)  
			from funginamesfromprod.dbo.tblBibliographyRelationship pbr
			inner join funginamesfromprod.dbo.tblBibliography pb on pb.BibliographyGuid = pbr.BibliographyRelationshipBibliographyFromFk 
			where pb.BibliographyNameFk = b.BibliographyNameFk and pbr.BibliographyRelationshipTypeFk = 8
				and isnull(pb.BibliographyIsDeleted,0) = 0 and pbr.BibliographyRelationshipIsActive = 1) = 1
			then 
				case when (select top 1 BibliographyGuid from funginamesfromprod.dbo.tblBibliographyRelationship pbr
							inner join funginamesfromprod.dbo.tblBibliography pb on pb.BibliographyGuid = pbr.BibliographyRelationshipBibliographyFromFk
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
from [devserver02\sql2005].Name_Cache_Test.test_name_2.TaxonConcept con
	inner join funginamesfromprod.dbo.tblBibliographyRelationship bibr on con.taxonconceptid = bibr.BibliographyRelationshipBibliographyFromFk
		and bibr.BibliographyRelationshipTypeFk = 8
	inner join funginamesfromprod.dbo.tblbibliography b on b.BibliographyGuid = bibr.BibliographyRelationshipBibliographyFromFk
	inner join [devserver02\sql2005].Name_Cache_Test.test_name_2.TaxonConcept cto on cto.taxonconceptid = bibr.BibliographyRelationshipBibliographyToFk
		
update con
	set ParentconceptId  = bibr.BibliographyRelationshipBibliographyToFk,
	ParentConceptInUse = 
		case when (select count(BibliographyRelationshipPk)  --only one parent concept for this name
			from funginamesfromprod.dbo.tblBibliographyRelationship pbr
			inner join funginamesfromprod.dbo.tblBibliography pb on pb.BibliographyGuid = pbr.BibliographyRelationshipBibliographyFromFk 
			where pb.BibliographyNameFk = b.BibliographyNameFk and pbr.BibliographyRelationshipTypeFk = 7 
				and isnull(pb.BibliographyIsDeleted,0) = 0 and pbr.BibliographyRelationshipIsActive = 1) = 1			
			then 
				case when (select top 1 BibliographyGuid from funginamesfromprod.dbo.tblBibliographyRelationship pbr
							inner join funginamesfromprod.dbo.tblBibliography pb on pb.BibliographyGuid = pbr.BibliographyRelationshipBibliographyFromFk
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
from [devserver02\sql2005].Name_Cache_Test.test_name_2.TaxonConcept con
	inner join funginamesfromprod.dbo.tblBibliographyRelationship bibr on con.Taxonconceptid = bibr.BibliographyRelationshipBibliographyFromFk
		and bibr.BibliographyRelationshipTypeFk = 7
	inner join funginamesfromprod.dbo.tblbibliography b on b.BibliographyGuid = bibr.BibliographyRelationshipBibliographyFromFk
	inner join [devserver02\sql2005].Name_Cache_Test.test_name_2.TaxonConcept cto on cto.taxonconceptid = bibr.BibliographyRelationshipBibliographyToFk

--add missing concepts as name based concepts
--first delete any taxon concepts that arent complete - ie dont have a parent relationship defined

if (not exists(select publicationid from [devserver02\sql2005].Name_Cache_Test.test_name_2.publication where citation = 'NZFLORA'))
begin
	set @accToId = NEWID()
	insert [devserver02\sql2005].Name_Cache_Test.test_name_2.publication(PublicationID, Citation) select @accToId, 'NZFLORA'
	insert [devserver02\sql2005].Name_Cache_Test.test_name_2.publicationTitle(PublicationID, Title, Level, Type) select @accToId, 'NZFLORA', 1, 'full'
end
else
begin
	select @accToId = publicationid from [devserver02\sql2005].Name_Cache_Test.test_name_2.publication where citation = 'NZFLORA'
end

insert [devserver02\sql2005].Name_Cache_Test.test_name_2.TaxonNameUse 
	(TaxonNameUseid, CreatedDate, ModifiedDate, NameId, AccordingToId, AcceptedNameId, ParentNameId)
select newid(), n.nameaddeddate, n.nameupdateddate, n.NameGuid, @accToId, NameCurrentFk, NameParentFk
from funginamesfromprod.dbo.tblName n
inner join [devserver02\sql2005].Name_Cache_Test.test_name_2.Name n1 on n1.nameid = n.nameguid
--left join [devserver02\sql2005].Name_Cache_Test.test_name_2.TaxonConcept con on con.nameid = n.NameGuid 
left join [devserver02\sql2005].Name_Cache_Test.test_name_2.TaxonNameUse nbc on nbc.nameid = n.NameGuid 
where nbc.TaxonNameUseid is null
	and NameParentFk <> '19F62517-BD63-42E3-97DA-E043A1644B1B'
	and not exists(select taxonconceptid from [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonconcept 
					where nameid = n1.nameid and parentconceptid is not null and parentconceptinuse = 1)
		
	

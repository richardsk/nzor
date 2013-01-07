IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NZACCache6]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[NZACCache6]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[NZACCache6]
as
--test transfer is ok

set concat_null_yields_null off

--test there are no names where the parent does not exist
if (exists(select * from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.name n 
			inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.TaxonNameUse nbc on nbc.nameid = n.nameid
			left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.name n2 on n2.nameid = nbc.parentnameid
			where nbc.parentnameid is not null and n2.nameid is null and n.rank <> 'kingdom'))
begin
	declare @pnames nvarchar(max), @pmsg nvarchar(max)
	set @pnames = char(13) 
	select @pnames = @pnames + n.NameFull + char(13) from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Name n
		inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.TaxonNameUse nbc on nbc.nameid = n.nameid
		left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.name n2 on n2.nameid = nbc.parentnameid
		where nbc.parentnameid is not null and n2.nameid is null and n.rank <> 'kingdom'
	set @pmsg = 'NZACName_Cache ERROR : There are names that exist where the parent name is absent.  ' + @pnames
	print(@pmsg)
	
	--FOR NOW delete these names
	--TODO - remove these and add raiserror back in
	declare @done bit
	set @done = 0
	while (@done = 0) 
	begin
		--delete annotations first
		delete a
		from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.annotation a
			inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Name n on n.nameid = a.nameid
			inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonnameuse nbc on nbc.nameid = n.nameid
			left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.name n2 on n2.nameid = nbc.parentnameid
		where nbc.parentnameid is not null and n2.nameid is null and n.rank <> 'kingdom'
		
		delete n
		from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Name n
			inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.TaxonNameUse nbc on nbc.nameid = n.nameid
			left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.name n2 on n2.nameid = nbc.parentnameid
			where nbc.parentnameid is not null and n2.nameid is null and n.rank <> 'kingdom'
		
		if (@@ROWCOUNT = 0) set @done = 1
	end 
		
	delete nbc
	from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.TaxonNameUse nbc
	left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Name n on n.nameid = nbc.nameid
	where n.nameid is null
		
	delete nbc
	from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.TaxonNameUse nbc
	left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Name n on n.nameid = nbc.parentnameid
	where n.nameid is null and nbc.parentnameid is not null
	
	delete nbc
	from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.TaxonNameUse nbc
	left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Name n on n.nameid = nbc.acceptednameid
	where n.nameid is null and nbc.acceptednameid is not null
	
	--raiserror(@pmsg, 1, 1)
end

--same for taxonconcepts
if (exists(select * from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.name n 
			inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept tc on tc.nameid = n.nameid
			left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept ptc on ptc.taxonconceptid = tc.parentconceptid
			where tc.parentconceptid is not null and ptc.taxonconceptid is null and n.rank <> 'kingdom'))
begin	
	set @done = 0
	while (@done = 0) 
	begin
		--delete annotations first
		delete a
		from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.annotation a
			inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Name n on n.nameid = a.nameid
			inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept tc on tc.nameid = n.nameid
			left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept ptc on ptc.taxonconceptid = tc.parentconceptid
		where tc.parentconceptid is not null and ptc.taxonconceptid is null and n.rank <> 'kingdom'
		
		delete n
		from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Name n
			inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept tc on tc.nameid = n.nameid
			left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept ptc on ptc.taxonconceptid = tc.parentconceptid
			where tc.parentconceptid is not null and ptc.taxonconceptid is null and n.rank <> 'kingdom'
		
		if (@@ROWCOUNT = 0) set @done = 1
	end 

	--delete annotations first
	delete a
	from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.annotation a
	inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept tc on tc.taxonconceptid = a.conceptid
	left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Name n on n.nameid = tc.nameid
	where n.nameid is null 
	
	delete tc
	from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept tc
	left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Name n on n.nameid = tc.nameid
	where n.nameid is null 
	
	
	--raiserror(@pmsg, 1, 1)
end

delete tc
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept tc
left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.name n on n.nameid = tc.nameid
where n.nameid is null
		
		
--test there are no names where the preferred name does not exist
if (exists(select * from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.name n 
			inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.TaxonNameUse nbc on nbc.nameid = n.nameid
			left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.name n2 on n2.nameid = nbc.acceptednameid
			where nbc.acceptednameid is not null and n2.nameid is null))
begin
	declare @names nvarchar(max), @msg nvarchar(max)
	set @names = char(13) + CHAR(10)
	select @names = @names + n.NameFull + char(13) + CHAR(10) from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Name n
		inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.TaxonNameUse nbc on nbc.nameid = n.nameid
		left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.name n2 on n2.nameid = nbc.acceptednameid
		where nbc.acceptednameid is not null and n2.nameid is null
	set @msg = 'NZACName_Cache ERROR : There are names that exist where the accepted name is absent.  ' + @names
	print @msg
	
	--FOR NOW set accepted name to NULL
	--TODO - remove these and add raiserror back in
	update nbc set nbc.AcceptedNameId = null
	from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.Name n
		inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.TaxonNameUse nbc on nbc.nameid = n.nameid
		left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.name n2 on n2.nameid = nbc.acceptednameid
		where nbc.acceptednameid is not null and n2.nameid is null 
		
	update tc set tc.AcceptedconceptId = null
	from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept tc
		left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept ptc on ptc.taxonconceptid = tc.acceptedconceptid
		where tc.acceptedconceptid is not null and ptc.taxonconceptid is null 
		
	--raiserror(@msg, 1, 1)
end
		
	
if (exists(
		select distinct c.parentconceptid from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept c 
		left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept c2 on c2.taxonconceptid = c.parentconceptid
		left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.TaxonNameUse c3 on c3.TaxonNameUseid = c.parentconceptid
		inner join tblbibliography b on b.bibliographyguid = c.parentconceptid
		where c2.taxonconceptid is null and c3.TaxonNameUseid is null and c.parentconceptid is not null))
begin
	--delete annotations first
	delete a
	from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.annotation a
		inner join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept c on c.taxonconceptid = a.conceptid
		left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept c2 on c2.taxonconceptid = c.parentconceptid
		left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.TaxonNameUse c3 on c3.TaxonNameUseid = c.parentconceptid
	where c2.taxonconceptid is null and c3.TaxonNameUseid is null and c.parentconceptid is not null
	
	delete c from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept c 
		left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept c2 on c2.taxonconceptid = c.parentconceptid
		left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.TaxonNameUse c3 on c3.TaxonNameUseid = c.parentconceptid
	where c2.taxonconceptid is null and c3.TaxonNameUseid is null and c.parentconceptid is not null
		
	--TODO put raise error back in
	--raiserror('TaxonConcepts exist where the ParentConceptId is neither in the TaxonConcept table or the TaxonNameUse table',
	--	10, 1)	
end

--delete orphaned vernacular uses
delete vu
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.vernacularuse vu
left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.name n on n.nameid = vu.taxonnameid
where n.nameid is null

delete ca
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.conceptapplication ca
left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.vernacularconcept vc on vc.vernacularconceptid = ca.fromconceptid
where vc.vernacularconceptid is null

delete ca
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.conceptapplication ca
left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept tc on tc.taxonconceptid = ca.toconceptid
left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonnameuse nu on nu.taxonnameuseid = ca.toconceptid
left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.name n on n.nameid = ca.tonameid
where tc.taxonconceptid is null and nu.taxonnameuseid is null and n.nameid is null


update tc
set tc.acceptedconceptid = null, acceptedconceptinuse = 0
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept tc
left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonconcept ac on ac.taxonconceptid = tc.acceptedconceptid
left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.taxonnameuse tu on tu.taxonnameuseid = tc.acceptedconceptid
where tc.acceptedconceptid is not null and ac.taxonconceptid is null and tu.taxonnameuseid is null

delete v
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.vernacular v
left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.vernacularuse vu on vu.vernacularid = v.vernacularid
left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.vernacularconcept vc on vc.nameid = v.vernacularid
where vu.vernacularuseid is null and vc.vernacularconceptid is null

delete vc
from [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.vernacularconcept vc
left join [NAME_CACHE.DATABASES.LANDCARERESEARCH.CO.NZ].name_cache.nzac_name.conceptapplication ca on ca.fromconceptid = vc.vernacularconceptid
where ca.conceptapplicationid is null

go

grant execute on dbo.[NZACCache6] to dbi_user

go

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestCache6]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TestCache6]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[TestCache6]
as
--test transfer is ok

set concat_null_yields_null off


---------------------------
--Plant names

--test there are no names where the parent does not exist
if (exists(select * from [devserver02\sql2005].Name_Cache_Test.test_name.name n 
			inner join [devserver02\sql2005].Name_Cache_Test.test_name.taxonnameuse nbc on nbc.nameid = n.nameid
			left join [devserver02\sql2005].Name_Cache_Test.test_name.name n2 on n2.nameid = nbc.parentnameid
			where nbc.parentnameid is not null and n2.nameid is null and n.rank <> 'kingdom'))
begin
	declare @pnames nvarchar(max), @pmsg nvarchar(max)
	set @pnames = char(13) 
	select @pnames = @pnames + n.NameFull + char(13) from [devserver02\sql2005].Name_Cache_Test.test_name.Name n
		inner join [devserver02\sql2005].Name_Cache_Test.test_name.taxonnameuse nbc on nbc.nameid = n.nameid
		left join [devserver02\sql2005].Name_Cache_Test.test_name.name n2 on n2.nameid = nbc.parentnameid
		where nbc.parentnameid is not null and n2.nameid is null and n.rank <> 'kingdom'
	set @pmsg = 'PlantName_Cache ERROR : There are names that exist where the parent name is absent.  ' + @pnames
	print(@pmsg)
	
	--FOR NOW delete these names
	--TODO - remove these and add raiserror back in
	declare @done bit
	set @done = 0
	while (@done = 0) 
	begin
		delete n
		from [devserver02\sql2005].Name_Cache_Test.test_name.Name n
			inner join [devserver02\sql2005].Name_Cache_Test.test_name.taxonnameuse nbc on nbc.nameid = n.nameid
			left join [devserver02\sql2005].Name_Cache_Test.test_name.name n2 on n2.nameid = nbc.parentnameid
			where nbc.parentnameid is not null and n2.nameid is null and n.rank <> 'kingdom'
		
		if (@@ROWCOUNT = 0) set @done = 1
	end 
		
	delete nbc
	from [devserver02\sql2005].Name_Cache_Test.test_name.taxonnameuse nbc
	left join [devserver02\sql2005].Name_Cache_Test.test_name.Name n on n.nameid = nbc.nameid
	where n.nameid is null
		
	delete nbc
	from [devserver02\sql2005].Name_Cache_Test.test_name.taxonnameuse nbc
	left join [devserver02\sql2005].Name_Cache_Test.test_name.Name n on n.nameid = nbc.parentnameid
	where n.nameid is null and nbc.parentnameid is not null
	
	delete nbc
	from [devserver02\sql2005].Name_Cache_Test.test_name.taxonnameuse nbc
	left join [devserver02\sql2005].Name_Cache_Test.test_name.Name n on n.nameid = nbc.acceptednameid
	where n.nameid is null and nbc.acceptednameid is not null
	
	delete tc
	from [devserver02\sql2005].Name_Cache_Test.test_name.TaxonConcept tc
	left join [devserver02\sql2005].Name_Cache_Test.test_name.Name n on n.nameid = tc.nameid
	where n.nameid is null
	
	delete tc
	from [devserver02\sql2005].Name_Cache_Test.test_name.TaxonConcept tc
	left join [devserver02\sql2005].Name_Cache_Test.test_name.TaxonConcept ptc on tc.parentconceptid = ptc.taxonconceptid
	where tc.parentconceptid is not null and ptc.taxonconceptid is null
	
	delete tc
	from [devserver02\sql2005].Name_Cache_Test.test_name.TaxonConcept tc
	left join [devserver02\sql2005].Name_Cache_Test.test_name.TaxonConcept atc on tc.acceptedconceptid = atc.taxonconceptid
	where tc.acceptedconceptid is not null and atc.taxonconceptid is null
	
	--raiserror(@pmsg, 1, 1)
end

--same for taxonconcepts
if (exists(select * from [devserver02\sql2005].Name_Cache_Test.test_name.name n 
			inner join [devserver02\sql2005].Name_Cache_Test.test_name.taxonconcept tc on tc.nameid = n.nameid
			left join [devserver02\sql2005].Name_Cache_Test.test_name.taxonconcept ptc on ptc.taxonconceptid = tc.parentconceptid
			where tc.parentconceptid is not null and ptc.taxonconceptid is null and n.rank <> 'kingdom'))
begin	
	set @done = 0
	while (@done = 0) 
	begin
		delete n
		from [devserver02\sql2005].Name_Cache_Test.test_name.Name n
			inner join [devserver02\sql2005].Name_Cache_Test.test_name.taxonconcept tc on tc.nameid = n.nameid
			left join [devserver02\sql2005].Name_Cache_Test.test_name.taxonconcept ptc on ptc.taxonconceptid = tc.parentconceptid
			where tc.parentconceptid is not null and ptc.taxonconceptid is null and n.rank <> 'kingdom'
		
		if (@@ROWCOUNT = 0) set @done = 1
	end 

	delete tc
	from [devserver02\sql2005].Name_Cache_Test.test_name.taxonconcept tc
	left join [devserver02\sql2005].Name_Cache_Test.test_name.Name n on n.nameid = tc.nameid
	where n.nameid is null 
	
	
	--raiserror(@pmsg, 1, 1)
end
		
		
--test there are no names where the preferred name does not exist
if (exists(select * from [devserver02\sql2005].Name_Cache_Test.test_name.name n 
			inner join [devserver02\sql2005].Name_Cache_Test.test_name.taxonnameuse nbc on nbc.nameid = n.nameid
			left join [devserver02\sql2005].Name_Cache_Test.test_name.name n2 on n2.nameid = nbc.acceptednameid
			where nbc.acceptednameid is not null and n2.nameid is null))
begin
	declare @names nvarchar(max), @msg nvarchar(max)
	set @names = char(13) + CHAR(10)
	select @names = @names + n.NameFull + char(13) + CHAR(10) from [devserver02\sql2005].Name_Cache_Test.test_name.Name n
		inner join [devserver02\sql2005].Name_Cache_Test.test_name.taxonnameuse nbc on nbc.nameid = n.nameid
		left join [devserver02\sql2005].Name_Cache_Test.test_name.name n2 on n2.nameid = nbc.acceptednameid
		where nbc.acceptednameid is not null and n2.nameid is null
	set @msg = 'PlantName_Cache ERROR : There are names that exist where the accepted name is absent.  ' + @names
	print @msg
	
	--FOR NOW set accepted name to NULL
	--TODO - remove these and add raiserror back in
	update nbc set nbc.AcceptedNameId = null
	from [devserver02\sql2005].Name_Cache_Test.test_name.Name n
		inner join [devserver02\sql2005].Name_Cache_Test.test_name.taxonnameuse nbc on nbc.nameid = n.nameid
		left join [devserver02\sql2005].Name_Cache_Test.test_name.name n2 on n2.nameid = nbc.acceptednameid
		where nbc.acceptednameid is not null and n2.nameid is null 
		
	update tc set tc.AcceptedconceptId = null
	from [devserver02\sql2005].Name_Cache_Test.test_name.taxonconcept tc
		left join [devserver02\sql2005].Name_Cache_Test.test_name.taxonconcept ptc on ptc.taxonconceptid = tc.acceptedconceptid
		where tc.acceptedconceptid is not null and ptc.taxonconceptid is null 
		
	--raiserror(@msg, 1, 1)
end
		
	
if (exists(
		select distinct c.parentconceptid from [devserver02\sql2005].Name_Cache_Test.test_name.taxonconcept c 
		left join [devserver02\sql2005].Name_Cache_Test.test_name.taxonconcept c2 on c2.taxonconceptid = c.parentconceptid
		left join [devserver02\sql2005].Name_Cache_Test.test_name.taxonnameuse c3 on c3.taxonnameuseid = c.parentconceptid
		inner join tblbibliography b on b.bibliographyguid = c.parentconceptid
		where c2.taxonconceptid is null and c3.taxonnameuseid is null and c.parentconceptid is not null))
begin
	raiserror('TaxonConcepts exist where the ParentConceptId is neither in the TaxonConcept table or the TaxonNameUse table',
		10, 1)
end


---------------------------
--Fungi names

--test there are no names where the parent does not exist
if (exists(select * from [devserver02\sql2005].Name_Cache_Test.test_name_2.name n 
			inner join [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonnameuse nbc on nbc.nameid = n.nameid
			left join [devserver02\sql2005].Name_Cache_Test.test_name_2.name n2 on n2.nameid = nbc.parentnameid
			where nbc.parentnameid is not null and n2.nameid is null and n.rank <> 'kingdom'))
begin
	
	set @pnames = char(13) 
	select @pnames = @pnames + n.NameFull + char(13) from [devserver02\sql2005].Name_Cache_Test.test_name_2.Name n
		inner join [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonnameuse nbc on nbc.nameid = n.nameid
		left join [devserver02\sql2005].Name_Cache_Test.test_name_2.name n2 on n2.nameid = nbc.parentnameid
		where nbc.parentnameid is not null and n2.nameid is null and n.rank <> 'kingdom'
	set @pmsg = 'PlantName_Cache ERROR : There are names that exist where the parent name is absent.  ' + @pnames
	print(@pmsg)
	
	--FOR NOW delete these names
	--TODO - remove these and add raiserror back in
	
	set @done = 0
	while (@done = 0) 
	begin
		delete n
		from [devserver02\sql2005].Name_Cache_Test.test_name_2.Name n
			inner join [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonnameuse nbc on nbc.nameid = n.nameid
			left join [devserver02\sql2005].Name_Cache_Test.test_name_2.name n2 on n2.nameid = nbc.parentnameid
			where nbc.parentnameid is not null and n2.nameid is null and n.rank <> 'kingdom'
		
		if (@@ROWCOUNT = 0) set @done = 1
	end 
		
	delete nbc
	from [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonnameuse nbc
	left join [devserver02\sql2005].Name_Cache_Test.test_name_2.Name n on n.nameid = nbc.nameid
	where n.nameid is null
		
	delete nbc
	from [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonnameuse nbc
	left join [devserver02\sql2005].Name_Cache_Test.test_name_2.Name n on n.nameid = nbc.parentnameid
	where n.nameid is null and nbc.parentnameid is not null
	
	delete nbc
	from [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonnameuse nbc
	left join [devserver02\sql2005].Name_Cache_Test.test_name_2.Name n on n.nameid = nbc.acceptednameid
	where n.nameid is null and nbc.acceptednameid is not null
	
	delete tc
	from [devserver02\sql2005].Name_Cache_Test.test_name_2.TaxonConcept tc
	left join [devserver02\sql2005].Name_Cache_Test.test_name_2.Name n on n.nameid = tc.nameid
	where n.nameid is null
	
	delete tc
	from [devserver02\sql2005].Name_Cache_Test.test_name_2.TaxonConcept tc
	left join [devserver02\sql2005].Name_Cache_Test.test_name_2.TaxonConcept ptc on tc.parentconceptid = ptc.taxonconceptid
	where tc.parentconceptid is not null and ptc.taxonconceptid is null
	
	delete tc
	from [devserver02\sql2005].Name_Cache_Test.test_name_2.TaxonConcept tc
	left join [devserver02\sql2005].Name_Cache_Test.test_name_2.TaxonConcept atc on tc.acceptedconceptid = atc.taxonconceptid
	where tc.acceptedconceptid is not null and atc.taxonconceptid is null
	
	--raiserror(@pmsg, 1, 1)
end

--same for taxonconcepts
if (exists(select * from [devserver02\sql2005].Name_Cache_Test.test_name_2.name n 
			inner join [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonconcept tc on tc.nameid = n.nameid
			left join [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonconcept ptc on ptc.taxonconceptid = tc.parentconceptid
			where tc.parentconceptid is not null and ptc.taxonconceptid is null and n.rank <> 'kingdom'))
begin	
	set @done = 0
	while (@done = 0) 
	begin
		delete n
		from [devserver02\sql2005].Name_Cache_Test.test_name_2.Name n
			inner join [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonconcept tc on tc.nameid = n.nameid
			left join [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonconcept ptc on ptc.taxonconceptid = tc.parentconceptid
			where tc.parentconceptid is not null and ptc.taxonconceptid is null and n.rank <> 'kingdom'
		
		if (@@ROWCOUNT = 0) set @done = 1
	end 

	delete tc
	from [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonconcept tc
	left join [devserver02\sql2005].Name_Cache_Test.test_name_2.Name n on n.nameid = tc.nameid
	where n.nameid is null 
	
	
	--raiserror(@pmsg, 1, 1)
end
		
		
--test there are no names where the preferred name does not exist
if (exists(select * from [devserver02\sql2005].Name_Cache_Test.test_name_2.name n 
			inner join [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonnameuse nbc on nbc.nameid = n.nameid
			left join [devserver02\sql2005].Name_Cache_Test.test_name_2.name n2 on n2.nameid = nbc.acceptednameid
			where nbc.acceptednameid is not null and n2.nameid is null))
begin
	
	set @names = char(13) + CHAR(10)
	select @names = @names + n.NameFull + char(13) + CHAR(10) from [devserver02\sql2005].Name_Cache_Test.test_name_2.Name n
		inner join [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonnameuse nbc on nbc.nameid = n.nameid
		left join [devserver02\sql2005].Name_Cache_Test.test_name_2.name n2 on n2.nameid = nbc.acceptednameid
		where nbc.acceptednameid is not null and n2.nameid is null
	set @msg = 'PlantName_Cache ERROR : There are names that exist where the accepted name is absent.  ' + @names
	print @msg
	
	--FOR NOW set accepted name to NULL
	--TODO - remove these and add raiserror back in
	update nbc set nbc.AcceptedNameId = null
	from [devserver02\sql2005].Name_Cache_Test.test_name_2.Name n
		inner join [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonnameuse nbc on nbc.nameid = n.nameid
		left join [devserver02\sql2005].Name_Cache_Test.test_name_2.name n2 on n2.nameid = nbc.acceptednameid
		where nbc.acceptednameid is not null and n2.nameid is null 
		
	update tc set tc.AcceptedconceptId = null
	from [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonconcept tc
		left join [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonconcept ptc on ptc.taxonconceptid = tc.acceptedconceptid
		where tc.acceptedconceptid is not null and ptc.taxonconceptid is null 
		
	--raiserror(@msg, 1, 1)
end
		
	
if (exists(
		select distinct c.parentconceptid from [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonconcept c 
		left join [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonconcept c2 on c2.taxonconceptid = c.parentconceptid
		left join [devserver02\sql2005].Name_Cache_Test.test_name_2.taxonnameuse c3 on c3.taxonnameuseid = c.parentconceptid
		inner join funginamesfromprod.dbo.tblbibliography b on b.bibliographyguid = c.parentconceptid
		where c2.taxonconceptid is null and c3.taxonnameuseid is null and c.parentconceptid is not null))
begin
	raiserror('TaxonConcepts exist where the ParentConceptId is neither in the TaxonConcept table or the TaxonNameUse table',
		10, 1)
end

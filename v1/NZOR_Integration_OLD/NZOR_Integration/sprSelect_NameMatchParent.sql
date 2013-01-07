 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_NameParentMatch')
	BEGIN
		DROP  procedure  sprSelect_NameParentMatch
	END

GO

CREATE procedure sprSelect_NameParentMatch
	@nameGuid uniqueidentifier,
	@parentFk uniqueidentifier output
as

	declare @pos int, @nm nvarchar(300)
	declare @rankPk int, @fullName nvarchar(300), @canonical nvarchar(300), @rankSort int, @rnk nvarchar(100)
	declare @highNameId uniqueidentifier, @genNameId uniqueidentifier, @cpn int, @cnt int
	
	--find parent based on rank and full name
	-- if multiple matches then we cant choose between them so fail, parent fk = '00000000-0000-0000-0000-000000000000'	
	
	select @rankSort = RankSort, @rnk = RankName, @rankPk = NameRankFk, @canonical = NameCanonical, @fullname = NameFull
	from tblName
	inner join tblRank on RankPk = NameRankFk
	where NameGuid = @nameGuid
	
	select @highNameId = ProviderImportHigherNameId, 
		@genNameId = ProviderImportGenusNameId		
	from tblProviderImport
	inner join tblProvider on providerpk = providerimportproviderfk
	where ProviderName = 'SYSTEM'
		
		
	if (@rankSort < 3000) --family and higher, use default name id
	begin
		--get majority default Id from all prov names for this name
		select distinct ProviderImportHigherNameId as Id,
			COUNT(pnpk) CountPNames
		into #tmp
		from tblProviderimport
			inner join tblProviderName pn on ProviderImportPk = pn.PNProviderImportFk and ProviderImportHigherNameId is not null
		where pn.PNNameFk = @nameGuid
		group by ProviderImportHigherNameId

		--get count of majority values
		select @cpn = Countpnames, @cnt = COUNT(ID)
		from #tmp
		where countpnames = (select top 1 countpnames from #tmp order by countpnames desc)
		group by countpnames
		
		--if there is only 1 def name with the max prov name records, then use it
		if (@cnt = 1) 
		begin
			select @parentFk = id from #tmp where countpnames = @cpn
		end
		else if (@cnt > 1)
		begin
			set @parentFk = '00000000-0000-0000-0000-000000000000'
		end
	
		drop table #tmp
	end
	else if (@rankSort = 3000) --genus, use default name id
	begin	
		--get majority default Id from all prov names for this name
		select distinct ProviderImportGenusNameId as Id,
			COUNT(pnpk) CountPNames
		into #tmp2
		from tblProviderimport
			inner join tblProviderName pn on ProviderImportPk = pn.PNProviderImportFk and ProviderImportGenusNameId is not null
		where pn.PNNameFk = @nameGuid
		group by ProviderImportGenusNameId

		--get count of majority values
		select @cpn = Countpnames, @cnt = COUNT(ID)
		from #tmp2
		where countpnames = (select top 1 countpnames from #tmp2 order by countpnames desc)
		group by countpnames
		
		--if there is only 1 def name with the max prov name records, then use it
		if (@cnt = 1) 
		begin
			select @parentFk = id from #tmp2 where countpnames = @cpn
		end
		else if (@cnt > 1)
		begin
			set @parentFk = '00000000-0000-0000-0000-000000000000'
		end
	
		drop table #tmp2
	end
	else if (@rankSort < 4200) --infra generic
	begin
		if (charindex(' subgen', @fullName) <> 0 or 
			charindex(' sect', @fullName) <> 0 or
			charindex(' subsect', @fullName) <> 0 or
			charindex(' ser', @fullName) <> 0)
		begin
			set @pos = charindex(' ', @fullName)
			set @nm = substring(@fullName, 0, @pos)
			
			select @cnt = count(*) from tblName where NameCanonical = @nm and NameRankFk = 8 
			if (@cnt = 1)
			begin
				select @parentFk = NameGuid
				from tblName
				where NameCanonical = @nm and NameRankFk = 8
			end 			
			if (@cnt > 1)
			begin
				select @cnt = count(*) from tblName where NameCanonical = @nm and NameRankFk = 8 and isnull(NameInvalid, 0) = 0 and isnull(NameIllegitimate, 0) = 0
				if (@cnt = 1)
				begin
					select @parentFk = NameGuid
					from tblName
					where NameCanonical = @nm and NameRankFk = 8 and isnull(NameInvalid, 0) = 0 and isnull(NameIllegitimate, 0) = 0
				end
				
				if (@cnt > 1) set @parentFk = '00000000-0000-0000-0000-000000000000'
			end 
		end	
	end
	else if (@rankSort = 4200) --species
	begin
		set @pos = charindex(' ', @fullName)
		set @nm = substring(@fullName, 0, @pos)
		
		select @cnt = count(*) from tblName where NameCanonical = @nm and NameRankFk = 8 
		if (@cnt = 1)
		begin
			select @parentFk = NameGuid
			from tblName
			where NameCanonical = @nm and NameRankFk = 8		
		end
		if (@cnt > 1)
		begin
			select @cnt = count(*) from tblName where NameCanonical = @nm and NameRankFk = 8 and isnull(NameInvalid, 0) = 0 and isnull(NameIllegitimate, 0) = 0
			if (@cnt = 1)
			begin
				select @parentFk = NameGuid
				from tblName
				where NameCanonical = @nm and NameRankFk = 8 and isnull(NameInvalid, 0) = 0 and isnull(NameIllegitimate, 0) = 0
			end
			
			if (@cnt > 1) set @parentFk = '00000000-0000-0000-0000-000000000000'
		end 
	end
	else if (@rankSort = 4400) --subspecies
	begin
		set @pos = charindex(' subsp', @fullName)
		set @nm = substring(@fullName, 0, @pos)
		
		select @cnt = count(*) from tblName where NameFull = @nm and NameRankFk = 24 
		if (@cnt = 1)
		begin
			select @parentFk = NameGuid
			from tblName
			where NameFull = @nm and NameRankFk = 24		
		end
		
		if (@cnt = 0 and @parentfk is null)
		begin
			select @cnt = count(*) from tblName where NameFull like @nm + ' %' and NameRankFk = 24 
			if (@cnt = 1)
			begin
				select @parentFk = NameGuid
				from tblName
				where NameFull like @nm + ' %' and NameRankFk = 24
			end
			if (@cnt > 1)
			begin
				select @cnt = count(*) from tblName where NameFull like @nm + ' %' and NameRankFk = 24 and isnull(NameInvalid, 0) = 0 and isnull(NameIllegitimate, 0) = 0
				if (@cnt = 1)
				begin
					select @parentFk = NameGuid
					from tblName
					where NameFull like @nm + ' %' and NameRankFk = 24 and isnull(NameInvalid, 0) = 0 and isnull(NameIllegitimate, 0) = 0
				end
				
				if (@cnt > 1) set @parentFk = '00000000-0000-0000-0000-000000000000'
			end 
		end
		
		if (@cnt > 1)
		begin
			select @cnt = count(*) from tblName where NameFull = @nm and NameRankFk = 24 and isnull(NameInvalid, 0) = 0 and isnull(NameIllegitimate, 0) = 0
			if (@cnt = 1)
			begin
				select @parentFk = NameGuid
				from tblName
				where NameFull = @nm and NameRankFk = 24 and isnull(NameInvalid, 0) = 0 and isnull(NameIllegitimate, 0) = 0
			end
			
			if (@cnt > 1) set @parentFk = '00000000-0000-0000-0000-000000000000'
		end 
	end
	else if (@rankSort = 4600) --variety
	begin
		if (charindex(' subsp', @fullName) <> 0)
		begin		
			set @pos = charindex(' var', @fullName)
			set @nm = substring(@fullName, 0, @pos)
			
			select @cnt = count(*) from tblName where NameFull = @nm and NameRankFk = 35
			if (@cnt = 1)
			begin
				select @parentFk = NameGuid
				from tblName
				where NameFull = @nm and NameRankFk = 35
			end
			
			if (@cnt = 0 and @parentfk is null)
			begin		
				select @cnt = count(*) from tblName where NameFull like @nm + ' %' and NameRankFk = 35 
				if (@cnt = 1)
				begin
					select @parentFk = NameGuid
					from tblName
					where NameFull like @nm + ' %' and NameRankFk = 35
				end
				if (@cnt > 1)
				begin
					select @cnt = count(*) from tblName where NameFull like @nm + ' %' and NameRankFk = 35 and isnull(NameInvalid, 0) = 0 and isnull(NameIllegitimate, 0) = 0
					if (@cnt = 1)
					begin
						select @parentFk = NameGuid
						from tblName
						where NameFull like @nm + ' %' and NameRankFk = 35 and isnull(NameInvalid, 0) = 0 and isnull(NameIllegitimate, 0) = 0
					end
					
					if (@cnt > 1) set @parentFk = '00000000-0000-0000-0000-000000000000'
				end 
			end
			
			if (@cnt > 1)
			begin
				select @cnt = count(*) from tblName where NameFull = @nm and NameRankFk = 35 and isnull(NameInvalid, 0) = 0 and isnull(NameIllegitimate, 0) = 0
				if (@cnt = 1)
				begin
					select @parentFk = NameGuid
					from tblName
					where NameFull = @nm and NameRankFk = 35 and isnull(NameInvalid, 0) = 0 and isnull(NameIllegitimate, 0) = 0
				end
				
				if (@cnt > 1) set @parentFk = '00000000-0000-0000-0000-000000000000'
			end 
		end
		else
		begin
			set @pos = charindex(' var', @fullName)
			set @nm = substring(@fullName, 0, @pos)
			
			select @cnt = count(*) from tblName where NameFull = @nm and NameRankFk = 24 
			if (@cnt = 1)
			begin
				select @parentFk = NameGuid
				from tblName
				where NameFull = @nm and NameRankFk = 24		
			end
			
			if (@cnt = 0 and @parentfk is null)
			begin		
				select @cnt = count(*) from tblName where NameFull like @nm + ' %' and NameRankFk = 24 
				if (@cnt = 1)
				begin
					select @parentFk = NameGuid
					from tblName
					where NameFull like @nm + ' %' and NameRankFk = 24
				end
				if (@cnt > 1)
				begin
					select @cnt = count(*) from tblName where NameFull like @nm + ' %' and NameRankFk = 24 and isnull(NameInvalid, 0) = 0 and isnull(NameIllegitimate, 0) = 0
					if (@cnt = 1)
					begin
						select @parentFk = NameGuid
						from tblName
						where NameFull like @nm + ' %' and NameRankFk = 24 and isnull(NameInvalid, 0) = 0 and isnull(NameIllegitimate, 0) = 0
					end
					
					if (@cnt > 1) set @parentFk = '00000000-0000-0000-0000-000000000000'
				end 
			end
			
			if (@cnt > 1)
			begin
				select @cnt = count(*) from tblName where NameFull = @nm and NameRankFk = 24 and isnull(NameInvalid, 0) = 0 and isnull(NameIllegitimate, 0) = 0
				if (@cnt = 1)
				begin
					select @parentFk = NameGuid
					from tblName
					where NameFull = @nm and NameRankFk = 24 and isnull(NameInvalid, 0) = 0 and isnull(NameIllegitimate, 0) = 0
				end
				
				if (@cnt > 1) set @parentFk = '00000000-0000-0000-0000-000000000000'
			end 
		end
	end
	
	--if failed, set to unknown name
	if (@parentFk is null or @parentFk = '00000000-0000-0000-0000-000000000000')
	begin
		select @parentFk = nameguid from tblName where NameFull = 'Unknown'
	end
	

GO


GRANT EXEC ON sprSelect_NameParentMatch TO PUBLIC

GO



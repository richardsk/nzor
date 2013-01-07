IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_NamesWithProvNameAuthors')
	BEGIN
		DROP  Procedure  sprSelect_NamesWithProvNameAuthors
	END

GO

CREATE Procedure sprSelect_NamesWithProvNameAuthors
	@providerNamePk int,
	@threshold int
AS

	if ((select count(*) from tblProviderNameAuthors where PNAProviderNameFk = @providerNamePk) = 0)
	begin
		--succeed (no authors)
		return
	end

	
	declare @combAuthors nvarchar(100), @basAuthors nvarchar(100)
	select @combAuthors = rtrim(ltrim(lower(PNACombinationAuthors))),
		@basAuthors = rtrim(ltrim(lower(PNABasionymAuthors))) 
	from tblProviderNameAuthors 
	where PNAProviderNameFk = @providerNamePk
		
	declare @correctCombAuth nvarchar(100), @correctBasAuth nvarchar(100), @pos int, @endPos int
	declare @authPk int
	
	set @correctCombAuth = ''
	if (@combAuthors is not null)
	begin		
		set @pos = 1
		set @endPos = charindex(' ', @combAuthors)
		while(@endPos <> 0)
		begin
			select @authPk = CorrectAuthorFk
			from tblAuthors 
			where AuthorPk = cast(substring(@combAuthors, @pos, @endPos - @pos) as int)
			if (@authPk is not null) set @correctCombAuth = @correctCombAuth + cast(@authPk as nvarchar(10)) + ' '
			
			set @pos = @endPos
			set @endPos = charindex(' ', @combAuthors, @pos + 1)
		end

		--add last one		
		select @authPk = CorrectAuthorFk
		from tblAuthors 
		where AuthorPk = cast(substring(@combAuthors, @pos, len(@combAuthors)) as int)
		if (@authPk is not null) set @correctCombAuth = @correctCombAuth + cast(@authPk as nvarchar(10)) 
		
		set @correctCombAuth = rtrim(@correctCombAuth)
	end
	
	set @correctBasAuth = ''
	if (@basAuthors is not null)
	begin				
		set @pos = 1
		set @endPos = charindex(' ', @basAuthors)
		while(@endPos <> 0)
		begin
			select @authPk = CorrectAuthorFk
			from tblAuthors 
			where AuthorPk = cast(substring(@basAuthors, @pos, @endPos - @pos) as int)
			if (@authPk is not null) set @correctBasAuth = @correctBasAuth + cast(@authPk as nvarchar(10)) + ' '
			
			set @pos = @endPos
			set @endPos = charindex(' ', @basAuthors, @pos + 1)
		end

		--add last one		
		select @authPk = CorrectAuthorFk
		from tblAuthors 
		where AuthorPk = cast(substring(@basAuthors, @pos, len(@basAuthors)) as int)
		if (@authPk is not null) set @correctBasAuth = @correctBasAuth + cast(@authPk as nvarchar(10)) 
		
		set @correctBasAuth = rtrim(@correctBasAuth)
	end
		
	print(@correctCombAuth)
	print(@correctBasAuth)
	
	
	--if there are no matching names with the specified authors, then 
	-- check the provider name authors for matches
	delete m
	from tmpMatchResults m
	where not exists( select * 
		from tblprovidername pn
		left join tblProviderNameAuthors pna on pna.PNAProviderNameFk = pn.PNPk
		where pn.pnnamefk = m.MatchResultRecordId 
			and isnull(PNACombinationAuthors,'') = @correctCombAuth
			and isnull(PNABasionymAuthors,'') = @correctBasAuth ) 
		

GO


GRANT EXEC ON sprSelect_NamesWithProvNameAuthors TO PUBLIC

GO



IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_NamesWithCombAuthors')
	BEGIN
		DROP  Procedure  sprSelect_NamesWithCombAuthors
	END

GO

CREATE Procedure sprSelect_NamesWithCombAuthors
	@providerNamePk int,
	@threshold int
AS

	if ((select count(*) from tblProviderNameAuthors where PNAProviderNameFk = @providerNamePk) = 0)
	begin
		--succeed (no authors)
		return
	end

	
	declare @combAuthors nvarchar(100)
	select @combAuthors = rtrim(ltrim(lower(PNACombinationAuthors)))
	from tblProviderNameAuthors 
	where PNAProviderNameFk = @providerNamePk
		
	declare @correctCombAuth nvarchar(100), @pos int, @endPos int
	declare @authPk int
	
	if (@combAuthors is not null)
	begin
		set @correctCombAuth = ''
		
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
	
	print(@correctCombAuth)
	
	if ((select count(*) from tmpMatchResults) = 0) --assume this is the first match
	begin
		insert tmpMatchResults
		select NameGuid, 100
		from tblName
		left join tblNameAuthors on NameAuthorsNameFk = Nameguid
		where isnull(NameAuthorsCombinationAuthors,'') = isnull(@correctCombAuth,'')
	end
	else
	begin
		delete m
		from tmpMatchResults m
		inner join tblName n on n.NameGuid = m.MatchResultRecordId
		left join tblNameAuthors na on na.NameAuthorsNameFk = n.Nameguid
		where isnull(NameAuthorsCombinationAuthors,'') <> isnull(@correctCombAuth,'')
	end
	
GO


GRANT EXEC ON sprSelect_NamesWithCombAuthors TO PUBLIC

GO



 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_NamesWithPartialAuthorsText')
	BEGIN
		DROP  Procedure  sprSelect_NamesWithPartialAuthorsText
	END

GO

CREATE Procedure sprSelect_NamesWithPartialAuthorsText
	@providerNamePk int,
	@threshold int
AS

	declare @nameAuthors nvarchar(1000)
	select @nameAuthors = lower(PNNameAuthors) from tblProviderName where PNPk = @providerNamePk
	
	if (@nameAuthors is null or len(@nameAuthors) = 0)
	begin
		--succeed
		return
	end

	if ((select count(*) from tmpMatchResults) = 0) --assume this is the first match
	begin
		insert tmpMatchResults
		select NameGuid, dbo.fnPartialAuthorTextMatch(lower(NameAuthors), @nameAuthors)
		from tblName 
		where dbo.fnPartialAuthorTextMatch(lower(NameAuthors), @nameAuthors) >= @threshold
	end
	else
	begin
		declare @lvs table(nid uniqueidentifier, lv int)
		insert @lvs
		select NameGuid, dbo.fnPartialAuthorTextMatch(lower(NameAuthors), @nameAuthors) 
		from tmpmatchresults m
		inner join tblname on nameguid = m.matchresultrecordid

		delete m
		from tmpmatchresults m
		inner join @lvs l on l.nid = m.matchresultrecordid
		where l.lv < @threshold

	end

	
GO


GRANT EXEC ON sprSelect_NamesWithPartialAuthorsText TO PUBLIC

GO


 
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_NamesWithPartialCombAuthors')
	BEGIN
		DROP  Procedure  sprSelect_NamesWithPartialCombAuthors
	END

GO

CREATE Procedure sprSelect_NamesWithPartialCombAuthors
	@providerNamePk int,
	@threshold int
AS

	declare @nameAuthors nvarchar(1000)
	select @nameAuthors = lower(PNCombinationAuthors) from tblProviderName where PNPk = @providerNamePk
	
	if (@nameAuthors is null or len(@nameAuthors) = 0)
	begin
		--succeed
		return
	end

	if ((select count(*) from tmpMatchResults) = 0) --assume this is the first match
	begin
		insert tmpMatchResults
		select NameGuid, dbo.fnPartialAuthorMatch(lower(NameCombinationAuthors), @nameAuthors)
		from tblName 
		where dbo.fnPartialAuthorMatch(lower(NameCombinationAuthors), @nameAuthors) >= @threshold
	end
	else
	begin
		declare @lvs table(nid uniqueidentifier, lv int)
		insert @lvs
		select NameGuid, dbo.fnPartialAuthorMatch(lower(NameCombinationAuthors), @nameAuthors) 
		from tmpmatchresults m
		inner join tblname on nameguid = m.matchresultrecordid

		delete m
		from tmpmatchresults m
		inner join @lvs l on l.nid = m.matchresultrecordid
		where l.lv < @threshold

	end

	
GO


GRANT EXEC ON sprSelect_NamesWithPartialCombAuthors TO PUBLIC

GO



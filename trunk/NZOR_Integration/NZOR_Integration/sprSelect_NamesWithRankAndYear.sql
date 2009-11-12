IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_NamesWithRankAndYear')
	BEGIN
		DROP  Procedure  sprSelect_NamesWithRankAndYear
	END

GO

CREATE Procedure sprSelect_NamesWithRankAndYear
	@providerNamePk int,
	@threshold int 
AS

	declare @nameRankFk int, @nameYear nvarchar(20)
	select @nameRankFk = PNNameRankFk, @nameYear = PNYear from tblProviderName
	where PNPk = @providerNamePk
	
	if (@nameRankFk is null or @nameYear is null)
	begin
		--fail
		delete tmpMatchResults
		return
	end
	

	if ((select count(*) from tmpMatchResults) = 0) --assume this is the first match
	begin
		insert tmpMatchResults
		select NameGuid, 100
		from tblName 
		where NameRankFk = @nameRankFk and NameYear = @nameYear
	end
	else
	begin
		delete mr
		from tmpMatchResults mr
		inner join tblName n on n.NameGuid = mr.MatchResultRecordId
		where n.NameRankFk <> @nameRankFk or n.NameYear <> @nameYear
	end


GO


GRANT EXEC ON sprSelect_NamesWithRankAndYear TO PUBLIC

GO



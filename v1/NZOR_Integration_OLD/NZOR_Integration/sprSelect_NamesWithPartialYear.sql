IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_NamesWithPartialYear')
	BEGIN
		DROP  Procedure  sprSelect_NamesWithPartialYear
	END

GO

CREATE Procedure sprSelect_NamesWithPartialYear
@providerNamePk int,
	@threshold int
AS

	declare @nameYear nvarchar(20)
	select @nameYear = lower(PNYear) from tblProviderName where PNPk = @providerNamePk
	
	
	if (@nameYear is null)
	begin
		--fail
		delete tmpMatchResults
		return
	end
	

	if ((select count(*) from tmpMatchResults) = 0) --assume this is the first match
	begin
		insert tmpMatchResults
		select NameGuid, dbo.fnPartialYearMatch(lower(NameYear), @nameYear)
		from tblName 
		where dbo.fnPartialYearMatch(lower(NameYear), @nameYear) >= @threshold
	end
	else
	begin
		delete mr
		from tmpMatchResults mr
		inner join tblName n on n.NameGuid = mr.MatchResultRecordId
		where dbo.fnPartialYearMatch(lower(NameYear), @nameYear) < @threshold
	end

	
GO


GRANT EXEC ON sprSelect_NamesWithPartialYear TO PUBLIC

GO



IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_NamesWithRank')
	BEGIN
		DROP  Procedure  sprSelect_NamesWithRank
	END

GO

CREATE Procedure sprSelect_NamesWithRank
	@provNameId uniqueidentifier,
	@threshold int --ignored - must match rank id exaclty
AS

	declare @RankId uniqueidentifier
	select @rankId = TaxonRankId
	from prov.Name
	where NameID = @provNameId
	
	if (@rankId is null)
	
	begin
		--fail
		delete tmpMatchResults
		return
	end
	

	if ((select count(*) from tmpMatchResults) = 0) --assume this is the first match
	begin
		insert tmpMatchResults
		select ConsensusNameId, NameID, 100
		from prov.Name
		where consensusnameid is not null and TaxonRankId = @rankId
	end
	else
	begin
		delete mr
		from tmpMatchResults mr
		inner join prov.Name n on n.NameID = mr.ProviderRecordID
		where n.TaxonRankId <> @rankID
	end
	
GO


GRANT EXEC ON sprSelect_NamesWithRank TO PUBLIC

GO



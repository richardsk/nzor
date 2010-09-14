IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_NamesWithAuthors')
	BEGIN
		DROP  Procedure  sprSelect_NamesWithAuthors
	END

GO

CREATE Procedure sprSelect_NamesWithAuthors
	@provNameId uniqueidentifier,
	@threshold int
AS

	
	declare @nameAuth nvarchar(4000)
	select @nameAuth = lower(Value) 
	from prov.NameProperty 
	where NameID = @provNameId and NameClassPropertyID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
	
	
	if (@nameAuth is null or len(@nameAuth) = 0)
	begin
		--pass
		return
	end
	

	if ((select count(*) from tmpMatchResults) = 0) --assume this is the first match
	begin
		insert tmpMatchResults
		select distinct ConsensusNameId, n.NameID, 100
		from prov.NameProperty np
		inner join prov.Name n on n.nameid = np.nameid
		where Value = @nameAuth and NameClassPropertyID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
	end
	else
	begin
		delete mr
		from tmpMatchResults mr
		inner join prov.NameProperty n on n.NameID = mr.ProviderRecordID and NameClassPropertyID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
		where len(Value) > 0 and isnull(Value, @nameAuth) <> @nameAuth	
	end
		

GO


GRANT EXEC ON sprSelect_NamesWithAuthors TO PUBLIC

GO



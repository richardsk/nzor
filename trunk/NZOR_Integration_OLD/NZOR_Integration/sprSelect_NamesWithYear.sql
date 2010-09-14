IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_NamesWithYear')
	BEGIN
		DROP  Procedure  sprSelect_NamesWithYear
	END

GO

CREATE Procedure sprSelect_NamesWithYear
	@provNameId uniqueidentifier,
	@threshold int
AS

	declare @nameYear nvarchar(20)
	select @nameYear = lower(Value) 
	from prov.NameProperty 
	where NameID = @provNameId and NameClassPropertyID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'
	
	
	if (@nameYear is null or len(@nameYear) = 0)
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
		where Value = @nameYear and NameClassPropertyID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'
	end
	else
	begin
		delete mr
		from tmpMatchResults mr
		inner join prov.NameProperty n on n.NameID = mr.ProviderRecordID and NameClassPropertyID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'
		where len(Value) > 0 and isnull(Value, @nameYear) <> @nameYear	
	end

	
GO


GRANT EXEC ON sprSelect_NamesWithYear TO PUBLIC

GO



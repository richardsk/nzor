IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_NamesWithExactCanonical')
	BEGIN
		DROP  Procedure  sprSelect_NamesWithExactCanonical
	END

GO

CREATE Procedure sprSelect_NamesWithExactCanonical
	@provNameId uniqueidentifier,
	@threshold int
AS

	declare @nameCanonical nvarchar(max)
	select @nameCanonical = ltrim(rtrim(lower(Value))) 
	from prov.NameProperty 
	where NameID = @provNameId and NameClassPropertyID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
	
	if (@namecanonical is null or len(@nameCanonical) = 0)
	begin
		--fail
		delete tmpMatchResults
		return
	end

	if ((select count(*) from tmpMatchResults) = 0) --assume this is the first match
	begin
		insert tmpMatchResults
		select ConsensusNameId, n.NameID, dbo.fnLevenshteinPercentage(lower(Value), @nameCanonical)
		from prov.NameProperty np 
		inner join prov.name n on n.nameid = np.nameid
		where consensusnameid is not null and
			ltrim(rtrim(np.value)) = @nameCanonical and NameClassPropertyID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
	end
	else
	begin
	
		delete m
		from tmpmatchresults m
		inner join prov.NameProperty on NameID = ProviderRecordID
		where Value is null or ltrim(rtrim(Value)) <> @nameCanonical and NameClassPropertyID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
		
	end

GO


GRANT EXEC ON sprSelect_NamesWithExactCanonical TO PUBLIC

GO



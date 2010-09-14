IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_NamesWithCanonical')
	BEGIN
		DROP  Procedure  sprSelect_NamesWithCanonical
	END

GO

CREATE Procedure sprSelect_NamesWithCanonical
	@providerNamePk int,
	@threshold int
AS

	declare @nameCanonical nvarchar(300)
	select @nameCanonical = lower(PNNameCanonical) from tblProviderName where PNPk = @providerNamePk
		
	if (@namecanonical is null)
	begin
		--fail
		delete tmpMatchResults
		return
	end

	declare @lenDiff int
	set @lenDiff = ceiling((len(@nameCanonical)*10/100))
	
	if ((select count(*) from tmpMatchResults) = 0) --assume this is the first match
	begin
		insert tmpMatchResults
		select NameGuid, dbo.fnLevenshteinPercentage(lower(NameCanonical), @nameCanonical)
		from tblName 
		where dbo.fnLevenshteinPercentage(lower(NameCanonical), @nameCanonical) >= @threshold
	end
	else
	begin
		declare @lvs table(nid uniqueidentifier, lv int)
		insert @lvs
		select NameGuid, null 
		from tmpmatchresults
		inner join tblName on NameGuid = MatchResultRecordId
		where namecanonical is null or abs(len(NameCanonical) - len(@nameCanonical)) <= @lenDiff
		
		update l
		set lv = dbo.fnLevenshteinPercentage(lower(NameCanonical), @nameCanonical) 
		from @lvs l
		inner join tblname n on n.nameguid = l.nid
		

		delete m
		from tmpmatchresults m
		left join @lvs l on l.nid = m.matchresultrecordid
		where l.lv < @threshold or l.nid is null
		
	end

GO


GRANT EXEC ON sprSelect_NamesWithCanonical TO PUBLIC

GO



IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_NamesWithPartialCanonical')
	BEGIN
		DROP  Procedure  sprSelect_NamesWithPartialCanonical
	END

GO

CREATE Procedure sprSelect_NamesWithPartialCanonical
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

	declare @lvs table(nid uniqueidentifier, lv int)
	declare @leftStr nvarchar(300), @rightStr nvarchar(300), @endsStr nvarchar(300)
	
	set @leftStr = @nameCanonical
	if (len(@nameCanonical) > 4) set @leftStr = left(@nameCanonical, len(@namecanonical) - 3) + '%'
	set @rightStr = @nameCanonical
	if (len(@nameCanonical) > 4) set @rightStr = '%' + right(@nameCanonical, len(@namecanonical) - 3) 
	set @endsStr = @nameCanonical
	if (len(@namecanonical) > 7) set @endsStr = left(@namecanonical, 3) + '%' + right(@nameCanonical, 3)
	
	
	if ((select count(*) from tmpMatchResults) = 0) --assume this is the first match
	begin		
				
		insert tmpMatchResults
		select NameGuid, null
		from tblName 
		where namecanonical like @leftStr or namecanonical like @rightStr or namecanonical like @endsStr
		
		insert @lvs
		select NameGuid, dbo.fnLevenshteinPercentage(lower(NameCanonical), @nameCanonical) 
		from tmpmatchresults m
		inner join tblname on nameguid = m.matchresultrecordid

		delete m
		from tmpmatchresults m
		inner join @lvs l on l.nid = m.matchresultrecordid
		where l.lv < @threshold
		
		update m
		set MatchResultScore = lv.lv
		from tmpMatchResults m
		inner join @lvs lv on lv.nid = m.MatchResultRecordId
	end
	else
	begin
		delete m
		from tmpmatchresults m
		inner join tblName on NameGuid = MatchResultRecordId
		where namecanonical is null or 
		(namecanonical not like @leftStr and namecanonical not like @rightStr and namecanonical not like @endsStr)
	
		insert @lvs
		select NameGuid, dbo.fnLevenshteinPercentage(lower(NameCanonical), @nameCanonical) 
		from tmpmatchresults m
		inner join tblname on nameguid = m.matchresultrecordid

		delete m
		from tmpmatchresults m
		inner join @lvs l on l.nid = m.matchresultrecordid
		where l.lv < @threshold
		
	end

GO


GRANT EXEC ON sprSelect_NamesWithPartialCanonical TO PUBLIC

GO



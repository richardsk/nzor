IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_NameMatches')
	BEGIN
		DROP  Procedure  sprSelect_NameMatches
	END

GO

CREATE Procedure sprSelect_NameMatches
	@providerNameId uniqueidentifier
AS

	delete tmpMatchResults
	
	--get match rule set for rank of this name
	declare @matchId nvarchar(50)
	select @matchId = matchrulesetid	
	from taxonrank tr
	inner join prov.name pn on pn.taxonrankid = tr.taxonrankid
	where pn.nameid = @providerNameId
		
	--execute each match function until either no records in tmpMatchResults or end of match set
		
	declare @matchFuncs table(MatchPk int, MatchFunction nvarchar(1000), Threshold int, PassFk int, FailFk int)
	
	insert into @matchFuncs
	select MatchPk, MatchFunction, MatchThreshold, MatchPassFk, MatchFailFk
	from Match
	where MatchRuleSet = @matchId
	order by MatchFirst desc
	
	declare @finished bit, @func nvarchar(1000), @pk int, @passFk int, @failFk int, @thHold int, @count int
	set @finished = 0
	select top 1 @pk = MatchPk from @matchFuncs
	
	create table #tmpRes(consRecId uniqueidentifier, recId uniqueidentifier, matchScore int)
	
	--todo update prov.Name set MatchPAth = '' where NameID = @providerNameID
			
	while (@finished = 0)
	begin	
	
		select @pk = MatchPk, @func = MatchFunction, @thHold = Threshold, @passFk = PassFk, @failFk = FailFk
		from @matchFuncs 
		where MatchPk = @pk
	
		update tmpIntegration
		set SPTrace = isnull(SPTrace + ', ', '') + @func 
		where RecordId = @providerNameID
	
		--tmp store current results
		delete #tmpRes
		insert #tmpRes select * from tmpMatchResults
			
		--build up stored proc call
		declare @spCall nvarchar(4000)
		set @spCall = 'exec ' + isnull(@func,'') + ' ''' + cast(@providerNameID as nvarchar(50)) + ''', ' + isnull(cast(@thHold as varchar(10)),'') 
		
		print(@spCall)
		exec(@spCall)						
				
		select @count = count(*) from tmpMatchResults
		
		if (@count = 0) 
		begin
			-- if failed then restore previous results, then continue to the failFk match row
			set @pk = @failFk
			if (@pk is not null)
			begin
				insert tmpMatchResults
				select * from #tmpRes
			end
		end
		else set @pk = @passFk
		
		if (@pk is null) set @finished = 1
	end
	
	--todo update prov.Name
	--set MatchPAth = (select SPTrace from tmpIntegration where RecordId = @providerNameID)
	--where NameID = @providerNameID
	
	
	--return remaining matching names
	select *
	from cons.Name n
	inner join tmpMatchResults r on r.ConsensusRecordID = n.NameID

	select avg(MatchScore) as MatchScore from tmpMatchResults

GO


GRANT EXEC ON sprSelect_NameMatches TO PUBLIC

GO



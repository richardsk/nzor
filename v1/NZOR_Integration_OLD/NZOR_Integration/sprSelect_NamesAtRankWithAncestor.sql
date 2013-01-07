IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_NamesAtRankWithAncestor')
	BEGIN
		DROP  Procedure  sprSelect_NamesAtRankWithAncestor
	END

GO

CREATE Procedure sprSelect_NamesAtRankWithAncestor
	@provNameId uniqueidentifier,
	@threshold int
AS
	--updates any current matched names so that they are the same rank as the prov name we are matching
	-- this is done by getting all child names of the "ancestor" of the provider name, then
	-- recursing down the tree from each name until we get to a name with the appropriate rank
	--This procedure doesnt really fail, as it only adds extra names to the matched list
	
	set nocount on
	
	declare @rank uniqueidentifier, @ancestorRank uniqueidentifier, @pnRank uniqueidentifier
	declare @ancestor uniqueidentifier
	declare @names table(rowId int identity, nameId uniqueidentifier, consNameId uniqueidentifier, rankId uniqueidentifier)
    declare @currentName uniqueidentifier, @rankName nvarchar(50), @selId uniqueidentifier
	
	select @pnRank = taxonrankid, @rank = taxonrankid
	from prov.name 
	where nameid = @provNameId
	
	select @ancestorRank = ancestorrankid from taxonrank where taxonrankid = @rank
	
	--print(@ancestorRank)
	
	if (@ancestorRank is not null)
	begin	
		--get ancestor
		select top 1 @currentName = ProviderRecordID from tmpmatchresults
		
		if (@currentName is null) --previous step failed, ie no names, so get consensus parent name record
		begin
			declare @name1Id nvarchar(300), @provPk uniqueidentifier

			select @name1Id = providerrecordid, @provPk = ProviderId 
			from prov.Name 
			where NameId = @provNameId
	
			select @currentName = ConsensusNameToId
			from vwProviderConcepts pc
			inner join prov.Name pn on pn.NameID = pc.NameToID
			where pc.providerrecordid = @name1Id and RelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155'
			
		end
			    	   			    	   
		while (@currentName is not null and @rank is not null)
		begin
			if (@rank = @ancestorRank)
			begin
				set @selId = @currentName
				set @currentName = null --to exit while loop
			end
			else
			begin
			
				select @currentName = NameToID 
				from vwProviderConcepts
				where relationshiptypeid = '6A11B466-1907-446F-9229-D604579AA155'
					and NameID = @currentName
	            
				select @rank = taxonrankid
				from prov.name
				where NameID = @currentName
				
				print(@rank)
			end        
		end
	    
	    print(@selId)
	    
	    if (@selId is not null)
	    begin
				
			insert @names
			select consensusnameid, nameid, taxonrankid
			from vwProviderConcepts
			where consensusnameid is not null and
				NameToID = @selId and relationshiptypeid = '6A11B466-1907-446F-9229-D604579AA155'
			
			--iterate through all names until they are all of the correct rank
			while (exists(select * from @names where rankId <> @pnrank))
			begin
				declare @pos int, @last int, @n uniqueidentifier, @r uniqueidentifier
				select @pos = min(rowId) from @names
				select @last = max(rowId) from @names
				
				--print (@pos)
				--print(@last)
				
				while (@pos <= @last)
				begin
					if (exists(select * from @names where rowid = @pos and rankId <> @pnrank))
					begin
						select @n = NameId, @r = rankId from @names where rowId = @pos
						
						if (@r <> @pnrank)
						begin
							insert @names
							select consensusnameid, nameid, taxonrankId
							from vwProviderConcepts 
							where consensusnameid is not null and
								nameToID = @n and relationshiptypeid = '6A11B466-1907-446F-9229-D604579AA155'
							
							delete @names where nameid = @n
							delete tmpmatchresults where ProviderRecordId = @n
						end
					end
					
					set @pos = @pos + 1
					select @last = max(rowId) from @names
				end
							
			end
			
			if ((select count(*) from @names) > 0)
			begin
				insert tmpMatchResults
				select n.consNameId, n.NameId, 100
				from @names n
				where not exists(select * from tmpmatchresults where providerrecordid = n.nameId)
			end
		end
	end

GO


GRANT EXEC ON sprSelect_NamesAtRankWithAncestor TO PUBLIC

GO



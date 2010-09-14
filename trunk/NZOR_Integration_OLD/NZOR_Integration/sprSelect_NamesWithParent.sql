IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_NamesWithParent')
	BEGIN
		DROP  Procedure  sprSelect_NamesWithParent
	END

GO

CREATE Procedure sprSelect_NamesWithParent
	@providerNameID uniqueidentifier,
	@threshold int
AS

	declare @name1Id nvarchar(300), @parentFk uniqueidentifier, @provPk uniqueidentifier
	
	select @name1Id = providerrecordid, @provPk = ProviderId 
	from prov.name
	where nameid = @providerNameID
	
	--assumes parent names in the taxon hierarchy have been added
	-- joins to the provider concept records to obtain parent concept, then parent name guid/fk
	select @parentFk = ConsensusNameToID
	from vwProviderConcepts
	where providerrecordid = @name1Id and providerid = @provPk 
		and relationshiptypeid = '6A11B466-1907-446F-9229-D604579AA155'
	
	if (@parentFk is not null)
	begin
		if ((select count(*) from tmpMatchResults) = 0) --assume this is the first match
		begin
			insert tmpMatchResults
			select ConsensusNameID, NameID, 100
			from vwProviderConcepts
			where consensusnameid is not null and
				ConsensusNameToID = @parentFk and relationshiptypeid = '6A11B466-1907-446F-9229-D604579AA155'
		end
		else
		begin
			delete mr
			from tmpMatchResults mr
			left join vwProviderConcepts c on c.nameid = mr.ProviderRecordID and relationshiptypeid = '6A11B466-1907-446F-9229-D604579AA155'
			where ConsensusNameToID <> @parentFk or consensusnametoid is null
		end
	end
	else
	begin		
		delete tmpMatchResults
	end
		
	
GO


GRANT EXEC ON sprSelect_NamesWithParent TO PUBLIC

GO



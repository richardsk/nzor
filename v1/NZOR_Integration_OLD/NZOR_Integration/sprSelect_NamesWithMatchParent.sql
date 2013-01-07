 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_NamesWithMatchParent')
	BEGIN
		DROP  Procedure  sprSelect_NamesWithMatchParent
	END

GO

CREATE Procedure sprSelect_NamesWithMatchParent
	@providerNamePk int,
	@threshold int
AS

	--only do this if there is no provider parent concept
	--if there is a parent prov concept, then it may be possible that there are no children
	-- of the parent name and hence the previous step failed
	declare @name1Id nvarchar(300), @parentFk uniqueidentifier, @provPk int
	
	select @name1Id = PNNameId, @provPk = ProviderPk from vwProviderName where PNPk = @providerNamePk
	
	--assumes parent names in the taxon hierarchy have been added
	-- joins to the provider concept records to obtain parent concept, then parent name guid/fk
	select @parentFk = PNNameFk
	from vwProviderConceptRelationship pcr
	inner join vwProviderName pn on pn.PNNameId = pcr.PCName2Id and pn.ProviderPk = pcr.ProviderPk
	where PCName1Id = @name1Id and PCRRelationshipFk = 6 and pcr.ProviderPk = @provPk --parent type
	
	if (@parentFk is null)
	begin
			
		select @parentFk = dbo.fnGetProviderNameParentMatch( @providerNamePk )
		
		if (@parentFk is not null and @parentFk <> '00000000-0000-0000-0000-000000000000')
		begin
			if ((select count(*) from tmpMatchResults) = 0) --assume this is the first match
			begin
				insert tmpMatchResults
				select n.NameGuid, 100
				from tblName n
				where n.NameParentFk = @parentFk
			end
			else
			begin
				delete mr
				from tmpMatchResults mr
				inner join tblName n on n.NameGuid = mr.MatchResultRecordId
				where n.NameParentFk is null or n.NameParentFk <> @parentFk
			end
		end
		else
		begin		
			delete tmpMatchResults
		end
		
	end
	else
	begin		
		delete tmpMatchResults
	end
		
	
GO


GRANT EXEC ON sprSelect_NamesWithMatchParent TO PUBLIC

GO



IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprUpdate_ProvFlatNameData_Omissions')
	BEGIN
		DROP  Procedure  sprUpdate_ProvFlatNameData_Omissions
	END

GO

CREATE Procedure sprUpdate_ProvFlatNameData_Omissions
AS

	DECLARE @NameId uniqueidentifier

	-- iterate through name table
	DECLARE names_cursor   CURSOR FORWARD_ONLY FOR
	SELECT  pn.NameID FROM prov.Name pn
	inner join dbo.taxonrank tr on tr.taxonrankid = pn.taxonrankid
	left join provider.FlatName fn on fn.seednameid = pn.nameid	
	where fn.nameid is null
	ORDER BY tr.sortorder

	OPEN names_cursor
	-- Perform the first fetch.
	FETCH NEXT FROM names_cursor INTO @NameId

	-- Check @@FETCH_STATUS to see if there are any more rows to fetch.
	WHILE @@FETCH_STATUS = 0
		BEGIN
 
		INSERT provider.FlatName(ParentNameID, NameID, Canonical, TaxonRankID, RankName, SortOrder, Depth, SeedNameID)
		EXEC sprSelect_ProvFlatNameToRoot @NameId

		-- This is executed as long as the previous fetch succeeds.
		FETCH NEXT FROM names_cursor INTO @NameId
		END

	CLOSE names_cursor
	DEALLOCATE names_cursor

GO


GRANT EXEC ON sprUpdate_ProvFlatNameData_Omissions TO PUBLIC

GO



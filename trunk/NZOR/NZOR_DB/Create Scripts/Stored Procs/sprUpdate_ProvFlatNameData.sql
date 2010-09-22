IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprUpdate_ProvFlatNameData')
	BEGIN
		DROP  Procedure  sprUpdate_ProvFlatNameData
	END

GO

CREATE Procedure sprUpdate_ProvFlatNameData
AS

	-- clear table
	truncate table prov.FlatName

	DECLARE @NameId uniqueidentifier

	-- iterate through name table
	DECLARE names_cursor   CURSOR FORWARD_ONLY FOR
	SELECT  NameID FROM prov.Name pn
	inner join dbo.taxonrank tr on tr.taxonrankid = pn.taxonrankid
	ORDER BY tr.sortorder

	OPEN names_cursor
	-- Perform the first fetch.
	FETCH NEXT FROM names_cursor INTO @NameId

	-- Check @@FETCH_STATUS to see if there are any more rows to fetch.
	WHILE @@FETCH_STATUS = 0
		BEGIN
 
		INSERT prov.FlatName
		EXEC sprSelect_ProvFlatNameToRoot @NameId

		-- This is executed as long as the previous fetch succeeds.
		FETCH NEXT FROM names_cursor INTO @NameId
		END

	CLOSE names_cursor
	DEALLOCATE names_cursor

GO


GRANT EXEC ON sprUpdate_ProvFlatNameData TO PUBLIC

GO



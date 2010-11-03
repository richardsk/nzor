IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprUpdate_FlatNameData')
	BEGIN
		DROP  Procedure  sprUpdate_FlatNameData
	END

GO

CREATE Procedure sprUpdate_FlatNameData
AS

	-- clear table
	truncate table cons.FlatName

	DECLARE @NameId uniqueidentifier

	-- iterate through name table
	DECLARE names_cursor   CURSOR FORWARD_ONLY FOR
	SELECT  c.NameID 
	FROM cons.Name c
	inner join dbo.taxonrank tr on tr.TaxonRankID = c.TaxonRankID
	ORDER BY tr.SortOrder

	OPEN names_cursor
	-- Perform the first fetch.
	FETCH NEXT FROM names_cursor INTO @NameId

	-- Check @@FETCH_STATUS to see if there are any more rows to fetch.
	WHILE @@FETCH_STATUS = 0
		BEGIN
 
		INSERT cons.FlatName
		EXEC sprSelect_FlatNameToRoot @NameId

		-- This is executed as long as the previous fetch succeeds.
		FETCH NEXT FROM names_cursor INTO @NameId
		END

	CLOSE names_cursor
	DEALLOCATE names_cursor

GO


GRANT EXEC ON sprUpdate_FlatNameData TO PUBLIC

GO



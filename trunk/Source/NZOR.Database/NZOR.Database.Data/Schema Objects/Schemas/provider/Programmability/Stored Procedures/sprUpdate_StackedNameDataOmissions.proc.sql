CREATE PROCEDURE [provider].[sprUpdate_StackedNameDataOmissions]
	
AS

DECLARE @NameId uniqueidentifier

-- iterate through name table
DECLARE names_cursor   CURSOR FORWARD_ONLY FOR
SELECT  pn.NameID FROM provider.Name pn
inner join dbo.taxonrank tr on tr.taxonrankid = pn.taxonrankid
left join provider.StackedName fn on fn.seednameid = pn.nameid	
inner join dbo.NameClass nc on nc.NameClassID = pn.NameClassID
where fn.nameid is null and nc.HasClassification = 1 
ORDER BY tr.sortorder

OPEN names_cursor
-- Perform the first fetch.
FETCH NEXT FROM names_cursor INTO @NameId

-- Check @@FETCH_STATUS to see if there are any more rows to fetch.
WHILE @@FETCH_STATUS = 0
	BEGIN
 
	INSERT provider.StackedName(StackedNameID, SeedNameID, AccordingToID, NameID, TaxonRankID, CanonicalName, RankName, SortOrder, Depth)
	EXEC provider.sprSelect_StackedNameToRoot @NameId

	-- This is executed as long as the previous fetch succeeds.
	FETCH NEXT FROM names_cursor INTO @NameId
	END

CLOSE names_cursor
DEALLOCATE names_cursor

RETURN @@ERROR
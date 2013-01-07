IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_FlatNameToRoot')
	BEGIN
		DROP  Procedure  sprSelect_FlatNameToRoot
	END

GO

CREATE Procedure sprSelect_FlatNameToRoot
	@StartNameId uniqueidentifier
AS

	DECLARE @tableResults	TABLE(NameId uniqueidentifier, NameLevel int )
	DECLARE @ParentId		uniqueidentifier
	DECLARE @NameId		uniqueidentifier
	DECLARE @intCount		int
	DECLARE @GrandParent	uniqueidentifier
	
	DECLARE @intLevel		int
	
	
	SELECT @ParentId = NameToID, @NameId = n.NameId
	FROM  consensus.Name n
	inner join vwConsensusConcepts cc on cc.nameid = n.nameid and ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' -- child
	WHERE n.NameId = @StartNameId
	
	SET @intLevel = 0
	
 	SELECT @intCount = COUNT(*) FROM @tableResults WHERE NameId = @ParentId
	
	WHILE @intCount < 1 
	AND (NOT @NameId IS NULL)
	BEGIN
		
		INSERT INTO @tableResults VALUES(@NameId, @intLevel)
		
		SELECT top 1 @GrandParent = NameToID 
		FROM  consensus.Name n
		inner join vwConsensusConcepts cc on cc.nameid = n.nameid and ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' -- child
		WHERE n.NameId = @ParentId
		
		SET @NameId = @ParentId
		SET @ParentId = @GrandParent
		
		SET @intLevel = @intLevel + 1
		
		SELECT @intCount = COUNT(*) FROM @tableResults WHERE NameId = @NameId
	END
	
	
	
	SELECT newid() as FlatNameID,
		cc.NameToID AS ParentNameID,
		cast(n.NameId as varchar(38)) AS NameID, 
		(select top 1 Value from consensus.NameProperty where NameID = n.NameID and NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750') AS Canonical, 
		tr.TaxonRankID AS TaxonRankID, 
		tr.Name AS RankName, 
		res.NameLevel AS Depth,  
		cast(@StartNameId as varchar(38)) as SeedNameID
	FROM  @tableResults res
	JOIN consensus.Name n ON n.NameId = res.NameId
	inner join vwConsensusConcepts cc on cc.nameid = n.nameid and ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' -- child
	LEFT JOIN dbo.TaxonRank tr ON n.TaxonRankID = tr.TaxonRankID	
	ORDER BY res.NameLevel DESC

GO


GRANT EXEC ON sprSelect_FlatNameToRoot TO PUBLIC

GO



IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'sprSelect_ProvFlatNameToRoot')
	BEGIN
		DROP  Procedure  sprSelect_ProvFlatNameToRoot
	END

GO

CREATE Procedure sprSelect_ProvFlatNameToRoot
	@StartNameId uniqueidentifier
AS

	DECLARE @tableResults	TABLE(NameId uniqueidentifier, NameLevel int )
	DECLARE @ParentId		uniqueidentifier
	DECLARE @NameId		uniqueidentifier
	DECLARE @intCount		int
	DECLARE @GrandParent	uniqueidentifier
	
	DECLARE @intLevel		int
	
	
	SELECT @ParentId = NameToID, @NameId = n.NameId
	FROM  prov.Name n
	inner join vwProviderConcepts cc on cc.nameid = n.nameid and RelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' -- child
	WHERE n.NameId = @StartNameId
	
	SET @intLevel = 0
	
 	SELECT @intCount = COUNT(*) FROM @tableResults WHERE NameId = @ParentId
	
	WHILE @intCount < 1 
	AND (NOT @NameId IS NULL)
	BEGIN
		
		INSERT INTO @tableResults VALUES(@NameId, @intLevel)
		
		SELECT @GrandParent = NameToID 
		FROM  prov.Name n
		inner join vwProviderConcepts cc on cc.nameid = n.nameid and RelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' -- child
		WHERE n.NameId = @ParentId
		
		SET @NameId = @ParentId
		SET @ParentId = @GrandParent
		
		SET @intLevel = @intLevel + 1
		
		SELECT @intCount = COUNT(*) FROM @tableResults WHERE NameId = @NameId
	END
	
	
	
	SELECT newid() as FlatNameID,
		cc.NameToID AS ParentNameID,
		cast(n.NameId as varchar(38)) AS NameID, 
		(select Value from prov.NameProperty where NameID = n.NameID and NameClassPropertyID = '1F64E93C-7EE8-40D7-8681-52B56060D750') AS Canonical, 
		tr.TaxonRankID AS TaxonRankID, 
		tr.Name AS RankName, 
		res.NameLevel AS Depth,  
		cast(@StartNameId as varchar(38)) as SeedNameID
	FROM  @tableResults res
	JOIN prov.Name n ON n.NameId = res.NameId
	inner join vwProviderConcepts cc on cc.nameid = n.nameid and RelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' -- child
	LEFT JOIN dbo.TaxonRank tr ON n.TaxonRankID = tr.TaxonRankID	
	ORDER BY res.NameLevel DESC

GO


GRANT EXEC ON sprSelect_ProvFlatNameToRoot TO PUBLIC

GO



CREATE NONCLUSTERED INDEX [idxCanonicalRankDepth]
    ON consensus.[StackedName]([CanonicalName],[RankName],[Depth])
	INCLUDE ([SeedNameID])



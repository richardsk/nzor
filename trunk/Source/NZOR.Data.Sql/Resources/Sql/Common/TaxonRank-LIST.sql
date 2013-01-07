SELECT 
	[TaxonRankID], 
	[Name], 
	[DisplayName],
	[KnownAbbreviations], 
	[SortOrder],
	[MatchRuleSetId],
	[IncludeInFullName],
	[ShowRank],
	[GoverningCode]
FROM 
	[TaxonRank] 
ORDER BY 
	[SortOrder]
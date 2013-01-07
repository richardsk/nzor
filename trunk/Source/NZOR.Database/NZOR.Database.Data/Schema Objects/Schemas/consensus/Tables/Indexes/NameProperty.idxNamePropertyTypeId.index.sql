CREATE INDEX 
	[idxNamePropertyTypeId]
ON 
	[consensus].[NameProperty]
	(
	NamePropertyTypeId,
	NameId
	)
INCLUDE
	(
	Value
	)
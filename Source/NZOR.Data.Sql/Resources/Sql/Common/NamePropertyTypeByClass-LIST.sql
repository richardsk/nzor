SELECT 
	NamePropertyTypeID, 
	NameClassID,
	Name,
	MinOccurrences,
	MaxOccurrences,
	GoverningCode
FROM 
	NamePropertyType
WHERE NameClassID = @nameClassID
ORDER BY 
	Name
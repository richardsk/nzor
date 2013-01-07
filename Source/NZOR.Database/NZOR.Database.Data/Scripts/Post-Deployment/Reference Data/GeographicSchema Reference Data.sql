PRINT 'Starting GeographicSchema Reference Data'

GO

SET NOCOUNT ON

DECLARE @GeographicSchema TABLE
	(
	GeographicSchemaID UNIQUEIDENTIFIER NOT NULL, 

	Name NVARCHAR(50) NOT NULL,
	Description NVARCHAR(MAX) NOT NULL
	)

INSERT INTO
	@GeographicSchema
VALUES 
	('2C158A02-311E-47A1-BFC7-19C27D162A0B' ,N'Land District' ,N'Land Districts'),
	('FE5C4980-5EF4-4FCF-837A-68CA565875D6' ,N'Ecological Districts' ,N'Ecological Districts'),
	('1FB78A56-6061-4E10-9696-906AB41D5BE0' ,N'World Geographic' ,N'Schema for Recording Plant Distributions (TDWG)	World Geographic Schema for Recording Plant Distributions'),
	('39257942-37EC-4BF2-8DE0-CAF60D66493E' ,N'ISO Country' ,N'ISO Country Schema'),
	('840B34EE-7AAA-4964-A3B5-D7BEFB462FEB' ,N'Specialist Regions' ,N'Mixed singleton polygons used by specialist groups.')

MERGE 
    dbo.GeographicSchema AS Target
USING 
    @GeographicSchema AS Source 
ON 
    (Target.GeographicSchemaID = Source.GeographicSchemaID)
WHEN MATCHED 
    THEN UPDATE
        SET   
            Target.Name = Source.Name,
            Target.Description = Source.Description

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (GeographicSchemaID, Name, Description)
    VALUES      
		(GeographicSchemaID, Name, Description)

WHEN NOT MATCHED BY SOURCE 
    THEN DELETE
; 

GO

PRINT 'Finished GeographicSchema Reference Data'

GO
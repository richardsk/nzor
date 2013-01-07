PRINT 'Starting ReferencePropertyType Reference Data'

GO

SET NOCOUNT ON

DECLARE @ReferencePropertyType TABLE
	(
	ReferencePropertyTypeID UNIQUEIDENTIFIER NOT NULL, 

	Name NVARCHAR(50) NOT NULL,
	Description NVARCHAR(MAX) NOT NULL
	)

INSERT INTO
	@ReferencePropertyType
VALUES 
	('38b41b0a-e58c-4923-8a9a-031b32ad16f2', N'Title', N''),
	('4e562f80-2d41-444f-a273-08fc4dd91eba', N'Start Page', N''),
	('dcefe1dd-5326-4fde-8ee1-0db6c41e2561', N'Editor', N''),
	('037d1485-c6c1-47d2-9619-234f743e9ff6', N'Author', N''),
	('36f7fcc8-e11c-4eda-bbf3-237c5cf9e6a2', N'Keyword', N''),
	('7f835876-b459-4023-90e4-6c22646fbe07', N'Citation', N''),
	('40f8cd14-20c9-4ecb-aff2-8b7bd0731e23', N'Volume', N''),
	('a4e5a88f-99ee-4b1c-967a-91727b40761f', N'Edition', N''),
	('8ac724b2-8715-422d-9b09-96cd2a04f25e', N'PlaceOfPublication', N''),
	('6e716610-0dc2-495a-b3a0-99ca3ba5fc8b', N'End Page', N''),
	('6fd6c4f9-cba1-4bf0-9d2b-a2a278263259', N'Link', N''),
	('8d5027bd-39f1-408d-acef-a728599a216f', N'Date', N''),
	('f65dc475-dd3f-4462-b34e-b5a0fc21f221', N'Page', N''),
	('80afb964-8d62-4b00-a347-b7a16dd98ee5', N'Publisher', N''),
	('8600068c-ede7-4f3a-a8fb-c8a9b59e8fe3', N'ParentReferenceID', N''),
	('cf641443-f11f-4906-8ff5-ccd856db77de', N'Issue', N''),
	('79dfad1f-ddb4-4eeb-aa63-f7a544243a69', N'Identifer', N'')

MERGE 
    dbo.ReferencePropertyType AS Target
USING 
    @ReferencePropertyType AS Source 
ON 
    (Target.ReferencePropertyTypeID = Source.ReferencePropertyTypeID)
WHEN MATCHED 
    THEN UPDATE
        SET   
            Target.Name = Source.Name,
            Target.Description = Source.Description

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (ReferencePropertyTypeID, Name, Description)
    VALUES      
		(ReferencePropertyTypeID, Name, Description)

WHEN NOT MATCHED BY SOURCE 
    THEN DELETE
; 

GO

PRINT 'Finished ReferencePropertyType Reference Data'

GO
PRINT 'Starting NamePropertyLookUp Reference Data'

GO

SET NOCOUNT ON

DECLARE @NamePropertyLookUp TABLE
	(
	NamePropertyLookUpID UNIQUEIDENTIFIER NOT NULL,  

	NameClassPropertyID UNIQUEIDENTIFIER NOT NULL, 
	ParentNamePropertyLookUpID UNIQUEIDENTIFIER NULL, 
	
	Value NVARCHAR(1000) NULL, 
	AlternativeStrings XML NULL, 
	SortOrder INT NULL
	)

INSERT INTO
	@NamePropertyLookUp
VALUES 
	('11AFE31D-6444-4FF9-96B4-1475114C1598', '6C98DA78-903D-4A2D-98E2-F9E23029EE28', NULL, N'Validated Secondary Source', NULL, NULL),
	('F9DF8A18-02DB-4238-B4E0-B0CD1248CC6F', '6C98DA78-903D-4A2D-98E2-F9E23029EE28', NULL, N'Unvalidated', NULL, NULL),
	('97829D73-6F2F-4426-B00E-D039B4918ED9', '6C98DA78-903D-4A2D-98E2-F9E23029EE28', NULL, N'Validated Primary Source', NULL, NULL)

MERGE 
    dbo.NamePropertyLookUp AS Target
USING 
    @NamePropertyLookUp AS Source 
ON 
    (Target.NamePropertyLookUpID = Source.NamePropertyLookUpID)
WHEN MATCHED 
    THEN UPDATE
        SET   
			Target.NameClassPropertyID = Source.NameClassPropertyID,
			Target.ParentNamePropertyLookUpID = Source.ParentNamePropertyLookUpID,

            Target.Value = Source.Value,
            Target.AlternativeStrings = Source.AlternativeStrings,
            Target.SortOrder = Source.SortOrder

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (NamePropertyLookUpID, NameClassPropertyID, ParentNamePropertyLookUpID, Value, AlternativeStrings, SortOrder)
    VALUES      
        (NamePropertyLookUpID, NameClassPropertyID, ParentNamePropertyLookUpID, Value, AlternativeStrings, SortOrder)

WHEN NOT MATCHED BY SOURCE 
    THEN DELETE
; 

GO

PRINT 'Finished NamePropertyLookUp Reference Data'

GO
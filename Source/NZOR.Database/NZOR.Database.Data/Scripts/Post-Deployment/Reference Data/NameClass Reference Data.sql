PRINT 'Starting NameClass Reference Data'

GO

SET NOCOUNT ON

DECLARE @NameClass TABLE
	(
	NameClassID UNIQUEIDENTIFIER NOT NULL, 

	Name NVARCHAR(50) NOT NULL,
	Description NVARCHAR(MAX) NOT NULL,
	HasClassification bit null
	)

INSERT INTO
	@NameClass
VALUES 
	('a5233111-61a0-4ae6-9c2b-5e8e71f1473a', N'Scientific Name', N'A scientific name published according to one of the codes of nomenclature', 1),
	('3d3a13b8-c673-459c-b98d-8a5b08e3ca44', N'Trade Name', N'A trade name', 0),
	('05bcc19c-27e8-492c-8add-ec5f73325bc5', N'Vernacular Name', N'A vernacular name', 0)

MERGE 
    dbo.NameClass AS Target
USING 
    @NameClass AS Source 
ON 
    (Target.NameClassID = Source.NameClassID)
WHEN MATCHED 
    THEN UPDATE
        SET   
            Target.Name = Source.Name,
            Target.Description = Source.Description

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (NameClassID, Name, Description)
    VALUES      
		(NameClassID, Name, Description)

WHEN NOT MATCHED BY SOURCE 
    THEN DELETE
; 

GO

PRINT 'Finished NameClass Reference Data'

GO
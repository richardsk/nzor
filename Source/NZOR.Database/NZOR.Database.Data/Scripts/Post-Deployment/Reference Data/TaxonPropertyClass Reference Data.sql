PRINT 'Starting TaxonPropertyClass Reference Data'

GO

SET NOCOUNT ON

DECLARE @TaxonPropertyClass TABLE
	(
	TaxonPropertyClassID UNIQUEIDENTIFIER NOT NULL, 

	Name NVARCHAR(50) NOT NULL,
	Description NVARCHAR(MAX) NOT NULL
	)

INSERT INTO
	@TaxonPropertyClass
VALUES 
	('11EEACAE-C29D-47A0-A5F5-1E862707BDA8', N'Management Status', N'Management Status of taxon with the specified region'),
	('CFA152D5-831C-4A4E-BA4F-50F9F18E7B70', N'Biostatus', N'Biostatus of taxon with the specified region, including origin and occurrence')

MERGE 
    dbo.TaxonPropertyClass AS Target
USING 
    @TaxonPropertyClass AS Source 
ON 
    (Target.TaxonPropertyClassID = Source.TaxonPropertyClassID)
WHEN MATCHED 
    THEN UPDATE
        SET   
            Target.Name = Source.Name,
            Target.Description = Source.Description

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (TaxonPropertyClassID, Name, Description)
    VALUES      
		(TaxonPropertyClassID, Name, Description)

WHEN NOT MATCHED BY SOURCE 
    THEN DELETE
; 

GO

PRINT 'Finished TaxonPropertyClass Reference Data'

GO
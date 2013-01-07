PRINT 'Starting TaxonPropertyType Reference Data'

GO

SET NOCOUNT ON

DECLARE @TaxonPropertyType TABLE
	(
	TaxonPropertyTypeID UNIQUEIDENTIFIER NOT NULL, 

	TaxonPropertyClassID UNIQUEIDENTIFIER NOT NULL, 

	Name NVARCHAR(50) NOT NULL,
	Description NVARCHAR(MAX) NULL
	)

INSERT INTO
	@TaxonPropertyType
VALUES 
	('7C42B4C0-BC62-4776-9C06-1C78C7215ACF', '11EEACAE-C29D-47A0-A5F5-1E862707BDA8', N'GeoRegion', NULL),
	('C8CA8106-B421-4138-97CF-326B4A0E4C72', '11EEACAE-C29D-47A0-A5F5-1E862707BDA8', N'Status', NULL),
	('ACE60424-87E1-4ECE-863D-462E5D5D7692', '11EEACAE-C29D-47A0-A5F5-1E862707BDA8', N'Outcome', NULL),
	('1A9DF2D0-049E-413B-A6BF-4E0647867B15', 'CFA152D5-831C-4A4E-BA4F-50F9F18E7B70', N'GeoSchema', NULL),
	('D955AD6E-4678-4AC9-B752-6A94F1C07080', 'CFA152D5-831C-4A4E-BA4F-50F9F18E7B70', N'Origin', N'the origin of the taxon within the specified region'),
	('BABCDC8B-E40B-43A8-B6F6-88C97B9197A0', 'CFA152D5-831C-4A4E-BA4F-50F9F18E7B70', N'GeoRegion', NULL),
	('9BB63B14-0208-4070-A575-94F90DFD47B0', 'CFA152D5-831C-4A4E-BA4F-50F9F18E7B70', N'Occurrence', N'the occurrence of the taxon within the specified region'),
	('EC773BB7-F6B1-46C7-A9AC-99F91B9CF409', '11EEACAE-C29D-47A0-A5F5-1E862707BDA8', N'GeoSchema', NULL),
	('10B3F77E-1A0B-48E5-AD62-BAF86BA0D02D', '11EEACAE-C29D-47A0-A5F5-1E862707BDA8', N'Biome', NULL),
	('9E726761-12E2-450A-87FB-C3AA68851CE5', '11EEACAE-C29D-47A0-A5F5-1E862707BDA8', N'Action Status', NULL),
	('7CDB8D51-55FB-4F1D-8897-C93DE6F1001D', 'CFA152D5-831C-4A4E-BA4F-50F9F18E7B70', N'Environmental Context', N'the environmemal context to which the information applies for the taxon'),
	('1DFDDEF8-B7E9-498F-9D51-E16665907F90', '11EEACAE-C29D-47A0-A5F5-1E862707BDA8', N'Action', NULL),
	('9233FE71-4C27-4CE0-86B9-F1B2B392658E', 'CFA152D5-831C-4A4E-BA4F-50F9F18E7B70', N'Biome', N'the biome to which the information applies for the taxon')

MERGE 
    dbo.TaxonPropertyType AS Target
USING 
    @TaxonPropertyType AS Source 
ON 
    (Target.TaxonPropertyTypeID = Source.TaxonPropertyTypeID)
WHEN MATCHED 
    THEN UPDATE
        SET   
			Target.TaxonPropertyClassID = Source.TaxonPropertyClassID,

            Target.Name = Source.Name,
            Target.Description = Source.Description

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (TaxonPropertyTypeID, TaxonPropertyClassID, Name, Description)
    VALUES      
		(TaxonPropertyTypeID, TaxonPropertyClassID, Name, Description)

WHEN NOT MATCHED BY SOURCE 
    THEN DELETE
; 

GO

PRINT 'Finished TaxonPropertyType Reference Data'

GO
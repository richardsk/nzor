PRINT 'Starting ConceptApplicationType Reference Data'

GO

SET NOCOUNT ON

DECLARE @ConceptApplicationType TABLE
	(
	ConceptApplicationTypeID UNIQUEIDENTIFIER NOT NULL, 

	Name NVARCHAR(150) NOT NULL
	)

INSERT INTO
	@ConceptApplicationType
VALUES 
	('FD250DDD-5B41-4E45-8DED-0910BAA7E581', 'is vernacular for'),
	('DE10EFC6-8F2A-4B8A-B473-0E181AF9C49C', 'is trade name for')

MERGE 
    dbo.ConceptApplicationType AS Target
USING 
    @ConceptApplicationType AS Source 
ON 
    (Target.ConceptApplicationTypeID = Source.ConceptApplicationTypeID)
WHEN MATCHED 
    THEN UPDATE
        SET   
            Target.Name = Source.Name

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (ConceptApplicationTypeID, Name)
    VALUES      
		(ConceptApplicationTypeID, Name)

WHEN NOT MATCHED BY SOURCE 
    THEN DELETE
; 

GO

PRINT 'Finished ConceptApplicationType Reference Data'

GO
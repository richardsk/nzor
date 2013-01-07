PRINT 'Starting ConceptRelationshipType Reference Data'

GO

SET NOCOUNT ON

DECLARE @ConceptRelationshipType TABLE
	(
	ConceptRelationshipTypeID UNIQUEIDENTIFIER NOT NULL, 
	
	Relationship NVARCHAR(50) NOT NULL, 
	MinOccurrences INT NULL, 
	MaxOccurrences INT NULL
	)

INSERT INTO
	@ConceptRelationshipType
VALUES 
	('348913DD-D273-41F6-8664-46F0AD536A11', N'is vernacular for', 0, NULL),
	('0CA79AB3-E213-4F51-88B9-4CE01F735A1D', N'is synonym of', 0, 1),
	('6A11B466-1907-446F-9229-D604579AA155', N'is child of', 0, 1),
	('E768C620-5D77-443D-B81D-0A6E70A91F5A', N'is parent of', NULL, NULL),
	('FF28D24C-2CA9-469C-B107-60CBE8750FA8', N'has vernacular of', NULL, NULL),
	('0018A60A-3C1D-4EFA-B4B3-534B40ACD079', N'is anamorph of', NULL, NULL),
	('1EAA046F-61A6-4F1E-B1A7-0E1D2CE78BBF', N'is teleomorph of', NULL, NULL),
	('1559AB9A-A3A6-4008-BFAD-DC9E74E7E5B8', N'is hybrid parent for', NULL, NULL),
	('AC997AF6-6D0B-487E-AE32-28B6C17E83D3', N'is hybrid child of', NULL, NULL)

MERGE 
    dbo.ConceptRelationshipType AS Target
USING 
    @ConceptRelationshipType AS Source 
ON 
    (Target.ConceptRelationshipTypeID = Source.ConceptRelationshipTypeID)
WHEN MATCHED 
    THEN UPDATE
        SET   
            Target.Relationship = Source.Relationship,
            Target.MinOccurrences = Source.MinOccurrences,
            Target.MaxOccurrences = Source.MaxOccurrences

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (ConceptRelationshipTypeID, Relationship, MinOccurrences, MaxOccurrences)
    VALUES      
        (ConceptRelationshipTypeID, Relationship, MinOccurrences, MaxOccurrences)

WHEN NOT MATCHED BY SOURCE 
    THEN DELETE
; 

GO

PRINT 'Finished ConceptRelationshipType Reference Data'

GO
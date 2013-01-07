PRINT 'Starting Consensus Concept Reference Data'

GO

SET NOCOUNT ON

DECLARE @Concept TABLE
	(
	ConceptID UNIQUEIDENTIFIER NOT NULL, 
	NameID UNIQUEIDENTIFIER NOT NULL, 
	accordingtoreferenceid UNIQUEIDENTIFIER NULL, 
	Orthography nvarchar(1000) null,
	TaxonRank nvarchar(100) null,
	HigherClassification nvarchar(max) null,
	AddedDate datetime null,
	ModifiedDate datetime null
	)

INSERT INTO
	@Concept
VALUES 
	('87F73532-0EA2-40BB-960A-AC03C60F26F5', '7C087DE1-FD0C-4997-8874-06D61D7CB244', NULL, null, null, null, GETDATE(), null)

MERGE 
    consensus.Concept AS Target
USING 
    @Concept AS Source 
ON 
    (Target.ConceptID = Source.ConceptID)

WHEN MATCHED 
    THEN UPDATE
        SET   
            Target.nameid = source.nameid,
			target.accordingtoreferenceid = source.accordingtoreferenceid,
			target.orthography = source.orthography,
			target.taxonrank = source.taxonrank,
			target.higherclassification = source.higherclassification,
			target.addeddate = source.addeddate,
			target.modifieddate = source.modifieddate

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (ConceptID, NameId, AccordingToReferenceID, Orthography, TaxonRank, HigherClassification, AddedDate, ModifiedDate)
    VALUES      
        (ConceptID, NameId, AccordingToReferenceID, Orthography, TaxonRank, HigherClassification, AddedDate, ModifiedDate)

; 

GO

PRINT 'Finished Consensus Concept Reference Data'

GO
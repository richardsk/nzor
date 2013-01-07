PRINT 'Starting Consensus Name Data'

SET NOCOUNT ON

DECLARE @Name TABLE
	(
	NameID UNIQUEIDENTIFIER NOT NULL,
	TaxonRankID UNIQUEIDENTIFIER NOT NULL,
	NameClassID UNIQUEIDENTIFIER NOT NULL,
	FullName nvarchar(500) not null,
	GoverningCode nvarchar(5) null,
	AddedDate datetime null,
	ModifiedDate datetime null
	)

INSERT INTO
	@Name
values ('7C087DE1-FD0C-4997-8874-06D61D7CB244', '057D6434-A12A-460D-B705-4510603FAE4F', 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A', 'ROOT', null,  GETDATE(), null)
	

MERGE 
    consensus.Name AS Target
USING 
    @Name AS Source 
ON 
    (Target.NameID = Source.NameID)
WHEN MATCHED 
    THEN UPDATE
        SET   
            Target.taxonrankid = source.taxonrankid,
            Target.nameclassid = source.nameclassid,
            Target.FullName = source.fullname,
			Target.GoverningCode = source.governingcode,
			target.addeddate = source.addeddate,
			target.modifieddate = source.modifieddate

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (nameid, taxonrankid, nameclassid, fullname, governingcode, addeddate, modifieddate)
    VALUES      
        (nameid, taxonrankid, nameclassid, fullname, governingcode, addeddate, modifieddate)

; 

GO

PRINT 'Finished Consensus Name Data'

GO
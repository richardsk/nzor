PRINT 'Starting Consensus Name Property Data'

SET NOCOUNT ON

DECLARE @NameProp TABLE
	(
	NamePropertyID uniqueidentifier not null,
	NameID UNIQUEIDENTIFIER NOT NULL,
	NamePropertyTypeID UNIQUEIDENTIFIER NOT NULL,
	RelatedID UNIQUEIDENTIFIER NULL,
	Sequence int null,
	Value nvarchar(max) null
	)

INSERT INTO
	@NameProp
values ('7743190F-5804-49C9-816A-093AD58EDBDF', '7C087DE1-FD0C-4997-8874-06D61D7CB244', '1F64E93C-7EE8-40D7-8681-52B56060D750', null, null, 'ROOT')

MERGE 
    consensus.NameProperty AS Target
USING 
    @NameProp AS Source 
ON 
    (Target.NamePropertyID = Source.NamePropertyID)
WHEN MATCHED 
    THEN UPDATE
        SET   
			target.nameid = source.nameid,
			target.namepropertytypeid = source.namepropertytypeid,
			target.relatedid = source.relatedid,
			target.sequence = source.sequence,
			target.value = source.value

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (namepropertyid, nameid, namepropertytypeid, relatedid, sequence, value)
    VALUES      
        (namepropertyid, nameid, namepropertytypeid, relatedid, sequence, value)

; 

GO

PRINT 'Finished Consensus Name Property Data'

GO
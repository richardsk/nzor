PRINT 'Starting [admin].DataType Data'

SET NOCOUNT ON

DECLARE @DataType TABLE
	(
	DataTypeID uniqueidentifier not null,
	Name nvarchar(50) not null
	)

INSERT INTO
	@DataType
values ('ECBC40CA-C241-4FBD-8A33-F3A9B32644AA','NZOR_OAI')
	
MERGE 
    [admin].DataType AS Target
USING 
    @DataType AS Source 
ON 
    (Target.DataTypeId = Source.DataTypeId)
WHEN MATCHED 
    THEN UPDATE
        SET   
			target.name = source.name

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (DataTypeID, Name)
    VALUES      
        (DataTypeID, Name)

WHEN NOT MATCHED BY SOURCE 
    THEN DELETE
; 

GO

PRINT 'Finished [admin].DataType Data'

go


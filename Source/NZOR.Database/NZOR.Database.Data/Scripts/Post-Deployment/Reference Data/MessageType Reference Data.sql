SET NOCOUNT ON

DECLARE @msgType TABLE
	(
	MessageTypeID UNIQUEIDENTIFIER NOT NULL, 
	Name NVARCHAR(100) NOT NULL
	)

INSERT INTO
	@msgType
VALUES 
	('16B63F25-DC67-4096-9FFF-F8C57199518C', 'Debug'),
	('BA7526D0-EE32-48E9-8832-8F3830422282', 'Provider'),
	('87CB9A9C-81F5-42B0-ACAD-23DD6CD03FB3', 'Harvest'),
	('4049E82F-DCAD-44A5-8C7D-8D9E4A10D890', 'Integration'),
	('E76C8215-4E7A-4025-BE05-0AE1AB162C4A', 'DataQuality')

MERGE 
    [admin].MessageType AS Target
USING 
    @msgType AS Source 
ON 
    (Target.MessageTypeID = Source.MessageTypeID)
WHEN MATCHED 
    THEN UPDATE
        SET   
            Target.Name = Source.Name

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (MessageTypeID, Name)
    VALUES      
		(MessageTypeID, Name)

WHEN NOT MATCHED BY SOURCE 
    THEN DELETE
; 

go
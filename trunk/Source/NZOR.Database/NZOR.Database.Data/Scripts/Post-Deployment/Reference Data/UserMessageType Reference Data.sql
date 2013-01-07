SET NOCOUNT ON

DECLARE @userMsg TABLE
	(
	UserMessageTypeID UNIQUEIDENTIFIER NOT NULL, 
	UserID UNIQUEIDENTIFIER NOT NULL, 
	MessageTypeID UNIQUEIDENTIFIER NOT NULL
	)

INSERT INTO
	@userMsg
VALUES 
	('EEC7A9CB-9979-4B89-BA43-907A167FCB76', 'AD21A955-AB30-4208-A149-9DC5EEC958AF', '16B63F25-DC67-4096-9FFF-F8C57199518C')

MERGE 
    [admin].[UserMessageType] AS Target
USING 
    @userMsg AS Source 
ON 
    (Target.UserMessageTypeID = Source.UserMessageTypeID)
WHEN MATCHED 
    THEN UPDATE
        SET   
            Target.UserID = Source.UserID,
			Target.MessageTypeID = Source.MessageTypeID

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (UserMessageTypeID, UserID, MessageTypeID)
    VALUES      
		(UserMessageTypeID, UserID, MessageTypeID)

WHEN NOT MATCHED BY SOURCE 
    THEN DELETE
; 

go
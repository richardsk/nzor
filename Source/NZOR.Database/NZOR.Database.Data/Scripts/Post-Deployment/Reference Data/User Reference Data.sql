SET NOCOUNT ON

DECLARE @user TABLE
	(
	UserID UNIQUEIDENTIFIER NOT NULL, 
	Name NVARCHAR(250) NOT NULL,
	Email nvarchar(1000),
	Password varbinary(500),
	Organisation nvarchar(250),
	APIKey nvarchar(250),
	Status nvarchar(100),
	AddedDate datetime,
	ModifiedDate datetime
	)

INSERT INTO
	@user
VALUES 
	('AD21A955-AB30-4208-A149-9DC5EEC958AF', 'Kevin Richards', 'richardsk@landcareresearch.co.nz', null, 'Landcare Research', '123', 'Registered', GETDATE(), GETDATE()),
	('95DC6F05-4BA8-43EC-A757-61C5F4341FC5', 'Jerry Cooper', 'cooperj@landcareresearch.co.nz', null, 'Landcare Research', '95DC6F05-4BA8-43EC-A757-61C5F4341FC5', 'Registered', GETDATE(), GETDATE()),
	('78810620-DFC1-4C4A-A1C4-266BA7082747', 'MPI', 'cooperj@landcareresearch.co.nz', null, 'Ministry of Primary Industries', '5ADC27E8-0AB5-4AEE-89F9-F3EEDE0B92AD', 'Registered', GETDATE(), GETDATE())

MERGE 
    [admin].[User] AS Target
USING 
    @user AS Source 
ON 
    (Target.UserID = Source.UserID)
WHEN MATCHED 
    THEN UPDATE
        SET   
            Target.Name = Source.Name,
			Target.Email = Source.Email,
			Target.Password = Source.Password,
			Target.Organisation = Source.Organisation,
			Target.APIKey = Source.APIKey,
			Target.Status = Source.Status,
			Target.AddedDate = Source.AddedDate,
			Target.ModifiedDate = Source.ModifiedDate

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (UserID, Name, Email, Password, Organisation, APIKey, Status, AddedDate, ModifiedDate)
    VALUES      
		(UserID, Name, Email, Password, Organisation, APIKey, Status, AddedDate, ModifiedDate)

WHEN NOT MATCHED BY SOURCE 
    THEN DELETE
; 

go
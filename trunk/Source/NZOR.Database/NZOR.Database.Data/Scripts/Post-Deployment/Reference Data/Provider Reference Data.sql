PRINT 'Starting [admin].Provider Data'

SET NOCOUNT ON

DECLARE @Provider TABLE
	(
	ProviderID UNIQUEIDENTIFIER NOT NULL,
	Name nvarchar(500) not null,
	Code nvarchar(150) not null,
	Url nvarchar(500) null,
	ContactEmail nvarchar(500) null,
	Disclaimer nvarchar(max) null,
	Attribution nvarchar(max) null,
	Licensing nvarchar(max) null,
	PublicStatement nvarchar(max) null,
	AddedDate datetime null,
	AddedBy nvarchar(150) null
	)

INSERT INTO
	@Provider
values (N'd0b0b9fb-bda0-43a6-9bb2-07b6f3db2c26', N'NZOR Test', N'NZOR_Test', null, 'richardsk@landcareresearch.co.nz', null, null, null, null, GETDATE(), 'System'),
	(N'e4e30dc8-c708-445f-86c3-235e0028f129', N'New Zealand Fungi Names Database', N'NZFUNGI', 'http://nzfungi.landcareresearch.co.nz', 'cooperj@landcareresearch.co.nz', '', '', '', '', getdate(), 'System'),
	(N'e5afbf6d-d7f7-4f01-b107-70e3a8bfcd91', N'NIWA', N'NIWA', '', null, null, null, null, null, GETDATE(), 'System'),
	(N'b8e1ef06-1f7d-43ce-bf30-71735e600a96', N'New Zealand Plant Names Database', N'NZFLORA', 'http://nzflora.landcareresearch.co.nz', 'cooperj@landcareresearch.co.nz', '', '', '', '', GETDATE(), 'System'),
	(N'91cbf580-e513-4ca4-89af-724253a393ac', N'New Zealand Inventory of Biodiversity', N'NZIB', null, 'Dennis.Gordon@niwa.co.nz', null, null, null, null, GETDATE(), 'System'),
	(N'1D977BFC-E67E-4E70-B4D1-F3AB62572C57', N'New Zealand Arthropod Collection Names Database', N'NZAC', null, 'CrosbyT@landcareresearch.co.nz', null, null, null, null, GETDATE(), 'System'),
	(N'5D639BAD-112F-4FBE-B474-0DF2BE67356A', N'NZOR System', N'NZOR_SYSTEM', null, null, null, null, null, null, GETDATE(), 'System'),
	(N'22B420A6-5C18-4CBC-9560-E48B2B24EDFF', N'NZOR Hosted Names', N'NZOR_Hosted', null, 'cooperj@landcareresearch.co.nz', null, null, null, null, GETDATE(), 'System'),
	(N'13CE8407-7352-4936-A310-53F0B1ED3A80', N'Global Cache', N'Global_Cache', null, 'cooperj@landcareresearch.co.nz', null, null, null, null, GETDATE(), 'System')

	
MERGE 
    [admin].Provider AS Target
USING 
    @Provider AS Source 
ON 
    (Target.ProviderID = Source.ProviderID)
WHEN MATCHED 
    THEN UPDATE
        SET   
            Target.name = source.name,
            Target.code = source.code,
			Target.url = source.url,
			Target.ContactEmail = Source.ContactEmail,
			target.disclaimer = source.disclaimer,
			target.attribution = source.attribution,
			target.licensing = source.licensing,
			target.publicstatement = source.publicstatement,
			target.addeddate = source.addeddate,
			target.addedby = source.addedby

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (providerid, name, code, url, contactEmail, disclaimer, attribution, licensing, publicstatement, addeddate, addedby)
    VALUES      
        (providerid, name, code, url, contactEmail, disclaimer, attribution, licensing, publicstatement, addeddate, addedby)

WHEN NOT MATCHED BY SOURCE 
    THEN DELETE
; 

GO

PRINT 'Finished [admin].Provider Data'

go
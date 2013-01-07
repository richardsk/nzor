PRINT 'Starting [admin].DataSource Data'

SET NOCOUNT ON

DECLARE @DataSource TABLE
	(
	DataSourceID UNIQUEIDENTIFIER NOT NULL,
	ProviderID UNIQUEIDENTIFIER NOT NULL,
	Name nvarchar(250) not null,
	Code nvarchar(150) not null,
	Description nvarchar(max) null,
	AddedDate datetime not null,
	AddedBy nvarchar(150) not null,
	ModifiedDate datetime null,
	ModifiedBy nvarchar(150) null
	)

INSERT INTO
	@DataSource
values (N'175d49cd-0785-4008-bb56-04df3e46de13', N'e4e30dc8-c708-445f-86c3-235e0028f129', 'New Zealand Fungi Names', N'NZFUNGI', '', GETDATE(), 'System', null, null),
	(N'2dd748d7-0cf4-4a74-8e01-3464f688603b', N'd0b0b9fb-bda0-43a6-9bb2-07b6f3db2c26', 'NZOR Test Dataset', N'NZOR_Test', '', GETDATE(), 'System', null, null),
	(N'c93f3e15-92da-4e93-9de0-416f937cc8e5', N'91cbf580-e513-4ca4-89af-724253a393ac', 'New Zealand Inventory of Biodiversity', 'NZIB', '', GETDATE(), 'System', null, null),
	(N'f710b2d6-b492-4104-845b-49990afb1abb', N'd0b0b9fb-bda0-43a6-9bb2-07b6f3db2c26', 'NZOR Test Dataset 2', N'NZOR_Test_2', '', GETDATE(), 'System', null, null),
	(N'f6235951-ca30-4449-87f3-9159beebfb24', N'b8e1ef06-1f7d-43ce-bf30-71735e600a96', 'New Zealand Plant Names', N'NZFLORA', '', GETDATE(), 'System', null, null),
	(N'DC793129-7C07-4B4C-B496-B0D9AAE4620F', N'1D977BFC-E67E-4E70-B4D1-F3AB62572C57', 'New Zealand Arthropod Collection Names Database', N'NZAC', '', GETDATE(), 'System', null, null),	
	(N'E208AB9C-FF25-448C-9338-80A2F2F7D48C', N'5D639BAD-112F-4FBE-B474-0DF2BE67356A', 'NZOR System', N'NZOR_SYSTEM', '', GETDATE(), 'System', null, null),	
	(N'144D59E3-C85D-4173-B25A-41EE6008B6C6', N'22B420A6-5C18-4CBC-9560-E48B2B24EDFF', N'NZOR Hosted', 'NZOR_Hosted', '', GETDATE(), 'System', null, null),
	(N'3EF3A2A7-5D65-46A3-A806-EA440FF5EC88', N'13CE8407-7352-4936-A310-53F0B1ED3A80', N'Catalogue of Life', 'CoL', '', GETDATE(), 'System', null, null)
	
MERGE 
    [admin].DataSource AS Target
USING 
    @DataSource AS Source 
ON 
    (Target.DataSourceID = Source.DataSourceID)
WHEN MATCHED 
    THEN UPDATE
        SET   
            Target.providerid = source.providerid,
			target.name = source.name,
            Target.code = source.code,
			target.description = source.description,
			target.addeddate = source.addeddate,
			target.addedby = source.addedby,
			target.modifieddate = source.modifieddate,
			target.modifiedby = source.modifiedby

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (datasourceid, providerid, name, code, description, addeddate, addedby, modifieddate, modifiedby)
    VALUES      
        (datasourceid, providerid, name, code, description, addeddate, addedby, modifieddate, modifiedby)

WHEN NOT MATCHED BY SOURCE 
    THEN DELETE
; 

GO

PRINT 'Finished [admin].DataSource Data'

go
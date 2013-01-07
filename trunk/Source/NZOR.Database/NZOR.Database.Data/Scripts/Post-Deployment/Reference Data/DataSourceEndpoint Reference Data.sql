PRINT 'Starting [admin].DataSourceEndpoint Data'

SET NOCOUNT ON

DECLARE @DataSourceEndpoint TABLE
	(
	DataSourceEndpointID UNIQUEIDENTIFIER NOT NULL,
	DataSourceID UNIQUEIDENTIFIER NOT NULL,
	DataTypeID uniqueidentifier not null,
	Name nvarchar(50) not null,
	Description nvarchar(500) null,
	Url nvarchar(500) null,
	LastHarvestDate datetime not null
	)

INSERT INTO
	@DataSourceEndpoint
values ('7079BDFB-6652-487E-851E-97433EEBEC84','f6235951-ca30-4449-87f3-9159beebfb24','ECBC40CA-C241-4FBD-8A33-F3A9B32644AA','NZFLORA',NULL,'http://data.harvesting.landcareresearch.co.nz/plantnamesoai/OAIPMHService.svc/plant_names','2011-04-15 09:01:09.480'),
	('B8BBC9F6-5C67-4D08-A7BF-D975C8DF0AD0','175d49cd-0785-4008-bb56-04df3e46de13','ECBC40CA-C241-4FBD-8A33-F3A9B32644AA','NZFUNGI',NULL,'http://data.harvesting.landcareresearch.co.nz/funginamesoai/OAIPMHService.svc/fungi_names','2011-04-15 09:01:09.480'),
	('BBEE01E4-11D5-4569-8797-3161D226658A','c93f3e15-92da-4e93-9de0-416f937cc8e5','ECBC40CA-C241-4FBD-8A33-F3A9B32644AA','NZIB',NULL,'http://data.harvesting.landcareresearch.co.nz/nzibnamesoai/OAIPMHService.svc/nzib_names','2011-04-15 09:01:09.480'),
	('941A9527-BB76-4497-B12D-FBF3689CFFF3','DC793129-7C07-4B4C-B496-B0D9AAE4620F','ECBC40CA-C241-4FBD-8A33-F3A9B32644AA','NZAC',NULL,'http://data.harvesting.landcareresearch.co.nz/nzacnamesoai/OAIPMHService.svc/nzac_names','2011-04-15 09:01:09.480'),
	('DB5E531B-27F6-4CBD-BEE6-B279527FDECD','144D59E3-C85D-4173-B25A-41EE6008B6C6','ECBC40CA-C241-4FBD-8A33-F3A9B32644AA','NZOR_Hosted',NULL,'http://data.harvesting.landcareresearch.co.nz/hostednamesoai/OAIPMHService.svc/hosted_names','2011-04-15 09:01:09.480')
	--('46C37CEB-BCC4-4E70-8A3B-4B917AAF3BE5','2DD748D7-0CF4-4A74-8E01-3464F688603B','ECBC40CA-C241-4FBD-8A33-F3A9B32644AA','NZOR_Test',NULL,'http://202.27.243.4/testnamesoai/OAIPMHService.svc/test_names','2001-04-15 09:01:09.480'),
	--('12A1D5FA-EE05-4D5B-9506-4FC9BE61F582','F710B2D6-B492-4104-845B-49990AFB1ABB','ECBC40CA-C241-4FBD-8A33-F3A9B32644AA','NZOR_Test_2',NULL,'http://202.27.243.4/testnamesoai/OAIPMHService.svc/test_names_2','2001-04-15 09:01:09.480')
	
MERGE 
    [admin].DataSourceEndpoint AS Target
USING 
    @DataSourceEndpoint AS Source 
ON 
    (Target.DataSourceEndpointID = Source.DataSourceEndpointID)
WHEN MATCHED 
    THEN UPDATE
        SET   
            Target.datasourceid = source.datasourceid,
			target.datatypeid = source.datatypeid,
			target.name = source.name,
            target.description = source.description,
			target.url = source.url,
			target.Lastharvestdate = source.lastharvestdate

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (DataSourceEndpointID, DataSourceID, DataTypeID, Name, Description, Url, LastHarvestDate)
    VALUES      
        (DataSourceEndpointID, DataSourceID, DataTypeID, Name, Description, Url, LastHarvestDate)
;

GO

PRINT 'Finished [admin].DataSourceEndpoint Data'

go


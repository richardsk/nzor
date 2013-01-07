CREATE TABLE [admin].[DataSourceEndpoint] (
    [DataSourceEndpointID] UNIQUEIDENTIFIER NOT NULL,
    [DataSourceID]         UNIQUEIDENTIFIER NOT NULL,
    [DataTypeID]           UNIQUEIDENTIFIER NOT NULL,
    [Name]                 NVARCHAR (50)    NULL,
    [Description]          NVARCHAR (500)   NULL,
    [Url]                  NVARCHAR (500)   NULL,
	[LastHarvestDate]	   datetime null
);


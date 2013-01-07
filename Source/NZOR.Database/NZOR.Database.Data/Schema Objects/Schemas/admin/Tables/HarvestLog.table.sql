CREATE TABLE [admin].[HarvestLog] (
    [HarvestLogID]       UNIQUEIDENTIFIER NOT NULL,
    [ProviderEndpointID] UNIQUEIDENTIFIER NOT NULL,
    [Date]               DATETIME         NOT NULL,
    [RecordCount]        INT              NULL,
    [Result]             NVARCHAR (500)   NULL
);


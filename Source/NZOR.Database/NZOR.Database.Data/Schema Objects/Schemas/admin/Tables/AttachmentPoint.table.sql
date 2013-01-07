CREATE TABLE [admin].[AttachmentPoint] (
    [AttachmentPointID] UNIQUEIDENTIFIER NOT NULL,
    [ProviderID]        UNIQUEIDENTIFIER NOT NULL,
    [DataSourceID]      UNIQUEIDENTIFIER NOT NULL,
    [ProviderRecordID]  NVARCHAR (1000)  NOT NULL,
    [ConsensusNameID]   UNIQUEIDENTIFIER NOT NULL,
    [FullName]          NVARCHAR (500)   NULL,
    [AddedDate]         DATETIME         NOT NULL,
    [AddedBy]           NVARCHAR (150)   NOT NULL,
    [ModifiedDate]      DATETIME         NULL,
    [ModifiedBy]        NVARCHAR (150)   NULL
);


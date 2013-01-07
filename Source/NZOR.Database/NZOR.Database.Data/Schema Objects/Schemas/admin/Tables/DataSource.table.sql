CREATE TABLE 
	[admin].DataSource
	(
    [DataSourceID] UNIQUEIDENTIFIER NOT NULL,
    [ProviderID]   UNIQUEIDENTIFIER NOT NULL,
    [Name]         NVARCHAR (250)   NOT NULL,
	[Code]		   nvarchar(150) not null,
    [Description]  NVARCHAR (MAX)   NULL,
    [AddedDate]    DATETIME         NOT NULL,
    [AddedBy]      NVARCHAR (150)   NOT NULL,
    [ModifiedDate] DATETIME         NULL,
    [ModifiedBy]   NVARCHAR (150)   NULL
	)

CREATE TABLE 
	[admin].[Provider]
	(
    [ProviderID]             UNIQUEIDENTIFIER NOT NULL,
    [Name]                   NVARCHAR (500)   NOT NULL,
	[Code]					 nvarchar(150) not null,
    [Url]                    NVARCHAR (500)   NULL,
	[ContactEmail]			nvarchar(500) null,
    [Disclaimer]             NVARCHAR (MAX)   NULL,
    [Attribution]            NVARCHAR (MAX)   NULL,
    [Licensing]              NVARCHAR (MAX)   NULL,
    [PublicStatement]        NVARCHAR (MAX)   NULL,
    [AddedDate]              DATETIME         NULL,
    [AddedBy]                NVARCHAR (150)   NULL,
    [ModifiedDate]           DATETIME         NULL,
    [ModifiedBy]             NVARCHAR (150)   NULL
	)

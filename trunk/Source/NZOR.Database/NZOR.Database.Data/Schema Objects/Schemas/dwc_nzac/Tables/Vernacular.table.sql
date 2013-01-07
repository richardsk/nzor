CREATE TABLE dwc_nzac.[Vernacular] (
    [TaxonID]         NVARCHAR (1000)  NULL,
    [VernacularName]  NVARCHAR (1000)  NULL,
    [Source]          NVARCHAR (2000)  NULL,
    [Language]        NVARCHAR (1000)  NULL,
    [Temporal]        NVARCHAR (1000)  NULL,
    [LocationID]      NVARCHAR (1000)  NULL,
    [Locality]        NVARCHAR (1000)  NULL,
    [CountryCode]     NVARCHAR (100)   NULL,
    [Sex]             NVARCHAR (100)   NULL,
    [LifeStage]       NVARCHAR (100)   NULL,
    [IsPlural]        BIT              NULL,
    [IsPreferredName] BIT              NULL,
    [OrganismPart]    NVARCHAR (1000)  NULL,
    [TaxonRemarks]    NVARCHAR (2000)  NULL
);


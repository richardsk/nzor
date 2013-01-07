CREATE TABLE dwc_nzib.[Distribution] (
    [TaxonID]            nvarchar (1000)  NULL,
    [LocationID]         NVARCHAR (1000)  NULL,
    [Locality]           NVARCHAR (1000)  NULL,
    [CountryCode]        NVARCHAR (100)   NULL,
    [LifeStage]          NVARCHAR (100)   NULL,
    [OccurrenceStatus]   NVARCHAR (100)   NULL,
    [ThreatStatus]       NVARCHAR (100)   NULL,
    [EstablishmentMeans] NVARCHAR (100)   NULL,
    [AppendixCITES]      NVARCHAR (100)   NULL,
    [EventDate]          NVARCHAR (100)   NULL,
    [StartDayOfYear]     NVARCHAR (100)   NULL,
    [EndDayOfYear]       NVARCHAR (100)   NULL,
    [Source]             NVARCHAR (1000)  NULL,
    [OccurrenceRemarks]  NVARCHAR (2000)  NULL
);


CREATE TABLE [admin].[ScheduledTask] (
    [ScheduledTaskID]     UNIQUEIDENTIFIER NOT NULL,
	[Name] nvarchar(100) not null,
    [RelatedID]    UNIQUEIDENTIFIER NOT NULL,
    [FrequencyDays]         INT              not NULL,
    [PreferredStartTimeGMT] NVARCHAR (50)    NULL,
	[LastRun] datetime null,
	[LastRunOutcome] nvarchar(500) null,
	[Status] nvarchar(1000) null
);


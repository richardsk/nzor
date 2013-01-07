PRINT 'Starting [admin].ScheduledTask Data'

SET NOCOUNT ON

DECLARE @ScheduledTask TABLE
	(
	ScheduledTaskId uniqueidentifier not null,
	Name nvarchar(100) not null,
	RelatedID uniqueidentifier not null,
	FrequencyDays int not null,
	PreferredStartTimeGMT nvarchar(50) null,
	LastRun datetime null,
	LastRunOutcome nvarchar(500) null,
	[Status] nvarchar(1000) null
	)

INSERT INTO
	@ScheduledTask
values ('B76EC3E9-BD81-469F-82FA-7938A5FC2DA0', 'NZOR SYSTEM RUN', '9D334ED3-82F0-4B6E-AEB4-7EB806163142', 3, null, null, null, 'Not Started'),
	('D739EC2D-ECEE-4255-9F8F-86B52871C8E3', 'NZFLORA Harvest', '7079BDFB-6652-487E-851E-97433EEBEC84', 2, null, null, null, 'Not Started'),
	('FE5E6AA0-543C-41FD-AFE9-14D8FE77752E', 'NZUNGI Harvest', 'B8BBC9F6-5C67-4D08-A7BF-D975C8DF0AD0', 2, null, null, null, 'Not Started'),
	('5226525B-3F4E-4949-A7C6-A953635EDA84', 'NZIB Harvest', 'BBEE01E4-11D5-4569-8797-3161D226658A', 2, null, null, null, 'Not Started'),
	('B7A9761E-DB57-4CE5-B24C-2529EC21CBAC', 'NZAC Harvest', '941A9527-BB76-4497-B12D-FBF3689CFFF3', 2, null, null, null, 'Not Started'),
	('B7D4AB1F-2933-4B32-B303-14818CBBD7CE', 'NZOR Hosted Harvest', 'DB5E531B-27F6-4CBD-BEE6-B279527FDECD', 2, null, null, null, 'Not Started')
		
MERGE 
    [admin].ScheduledTask AS Target
USING 
    @ScheduledTask AS Source 
ON 
    (Target.ScheduledTaskId = Source.ScheduledTaskId)
WHEN MATCHED 
    THEN UPDATE
        SET   
			target.name = source.name,
			target.relatedid = source.relatedid,
			target.frequencydays = source.frequencydays,
			target.preferredstarttimegmt = source.preferredstarttimegmt,
			target.lastrun = source.lastrun,
			target.lastrunoutcome = source.lastrunoutcome,
			target.[status] = source.[status]

WHEN NOT MATCHED BY TARGET 
    THEN INSERT      
        (ScheduledTaskId, Name, relatedid, frequencydays, preferredstarttimegmt, lastrun, lastrunoutcome, [status])
    VALUES      
        (ScheduledTaskId, Name, relatedid, frequencydays, preferredstarttimegmt, lastrun, lastrunoutcome, [status])
;

GO

PRINT 'Finished [admin].ScheduledTask Data'

go


update [admin].ScheduledTask
set Name = @name,
	RelatedId = @relatedId,
	FrequencyDays = @frequencyDays,
	PreferredStartTimeGMT = @preferredStartTimeGMT,
	LastRun = @lastRun,
	LastRunOutcome = @lastRunOutcome,
	[Status] = @status
where ScheduledTaskId = @scheduledTaskId